using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Webinar.Dynamo.Repository.Enumerators;
using Webinar.Dynamo.Repository.Factories;
using Webinar.Dynamo.Repository.Helpers;
using Webinar.Dynamo.Repository.ValueObject;

namespace Webinar.Dynamo.Repository
{
    public partial class DynamoRepository<TEntity> : IDynamoRepository<TEntity> where TEntity : class
    {
        protected readonly IConfiguration Configuration;
        protected AmazonDynamoDBClient Client;
        private readonly Type typeTEntity;
        protected List<string> IndexNames;

        protected DynamoRepository(IConfiguration configuration)
        {
            typeTEntity = typeof(TEntity);
            Configuration = configuration;
            IndexNames = new List<string>();
            GetClient();
        }

        private void GetClient()
        {
            if (Client == null)
            {
                var serviceUrl = Configuration["AWS:Dynamo:ServiceURL"];

                if (!string.IsNullOrEmpty(serviceUrl))
                {
                    var config = new AmazonDynamoDBConfig
                    {
                        ServiceURL = serviceUrl
                    };

                    Client = new AmazonDynamoDBClient(config);
                }
                else
                {
                    Client = new AmazonDynamoDBClient();
                }
            }
        }

        private DynamoDBContext GetContext()
        {
            string prefix = Configuration["AWS:Dynamo:TableNamePrefix"];
            if (prefix == null)
            {
                Exception ex = new Exception("Define AWS:Dynamo:TableNamePrefix in your Configuration file");
                throw ex;
            }

            AWSConfigsDynamoDB.Context.TableNamePrefix = prefix;
            return new DynamoDBContext(Client);
        }

        public BatchWrite<TEntity> GetBatchWrite(TEntity entity, DynamoDbBatchOperator batchOperator)
        {
            using DynamoDBContext context = GetContext();
            BatchWrite<TEntity> batch = context.CreateBatchWrite<TEntity>();
            switch (batchOperator)
            {
                case DynamoDbBatchOperator.Delete:
                    batch.AddDeleteItem(entity);
                    break;
                case DynamoDbBatchOperator.Put:
                    batch.AddPutItem(entity);
                    break;
            }
            return batch;
        }

        public BatchWrite<TEntity> GetBatchWrite(List<TEntity> entities, DynamoDbBatchOperator batchOperator)
        {
            using DynamoDBContext context = GetContext();
            BatchWrite<TEntity> batch = context.CreateBatchWrite<TEntity>();
            switch (batchOperator)
            {
                case DynamoDbBatchOperator.Delete:
                    batch.AddDeleteItems(entities);
                    break;
                case DynamoDbBatchOperator.Put:
                    batch.AddPutItems(entities);
                    break;
            }
            return batch;
        }

        public bool ExecuteTransaction(List<BatchWrite> batchWrites)
        {
            using DynamoDBContext context = GetContext();
            var multi = context.CreateMultiTableBatchWrite(batchWrites.ToArray());
            var task = multi.ExecuteAsync();
            task.Wait();
            return task.IsCompletedSuccessfully;
        }

        public bool Add(TEntity obj)
        {
            using DynamoDBContext context = GetContext();
            var task = context.SaveAsync(obj);
            task.Wait();
            return task.IsCompletedSuccessfully;
        }

        public bool Add(List<TEntity> lstObj)
        {
            using DynamoDBContext context = GetContext();
            var batch = context.CreateBatchWrite<TEntity>();
            batch.AddPutItems(lstObj);
            var task = batch.ExecuteAsync();
            task.Wait();
            return task.IsCompletedSuccessfully;
        }

        public List<TEntity> Get(List<object> keys)
        {
            using DynamoDBContext context = GetContext();
            var batch = context.CreateBatchGet<TEntity>();
            keys.ForEach(item => batch.AddKey(item));
            batch.ExecuteAsync().Wait();
            return batch.Results;
        }

        public List<TEntity> Get(List<TEntity> keys)
        {
            using DynamoDBContext context = GetContext();
            var batch = context.CreateBatchGet<TEntity>();
            keys.ForEach(item => batch.AddKey(item));
            batch.ExecuteAsync().Wait();
            return batch.Results;
        }

        public List<TEntity> Get(Dictionary<object, object> keys)
        {
            using DynamoDBContext context = GetContext();
            var batch = context.CreateBatchGet<TEntity>();
            foreach (var keyPairs in keys)
            {
                batch.AddKey(keyPairs.Key, keyPairs.Value);
            }

            batch.ExecuteAsync().Wait();
            return batch.Results;
        }

        public TEntity Get(long id, int range)
        {
            using DynamoDBContext context = GetContext();
            return context.LoadAsync<TEntity>(id, range).Result;
        }

        public TEntity Get(TEntity entity)
        {
            using DynamoDBContext context = GetContext();
            return context.LoadAsync(entity).Result;
        }

        public List<TEntity> GetAll()
        {
            List<TEntity> partialResultDynamo = new List<TEntity>();
            List<TEntity> totalResultDynamo = new List<TEntity>();

            using DynamoDBContext context = GetContext();
            //Teniendo en cuenta la restricción de Dynamo en cuanto al límite de 1MB de los registros devueltos por cada consulta:
            AsyncSearch<TEntity> query = context.ScanAsync<TEntity>(null);
            do
            {
                partialResultDynamo.Clear();
                partialResultDynamo = query.GetNextSetAsync().Result.ToList();
                if (partialResultDynamo.Count > 0)//Si en la porción consultada obtuvo registros que cumplen con los filtros
                {
                    totalResultDynamo.AddRange(partialResultDynamo); //Añádalos.
                }
            } while (!query.IsDone);

            return totalResultDynamo;
        }

        public bool Update(TEntity obj)
        {
            return Add(obj);
        }

        public bool Update(List<TEntity> lstObj)
        {
            return Add(lstObj);
        }

        public bool Remove(TEntity obj)
        {
            using DynamoDBContext context = GetContext();
            var task = context.DeleteAsync(obj);
            task.Wait();
            return task.IsCompletedSuccessfully;
        }

        public bool Remove(List<TEntity> lstObj)
        {
            using DynamoDBContext context = GetContext();
            var batch = context.CreateBatchWrite<TEntity>();
            batch.AddDeleteItems(lstObj);
            var task = batch.ExecuteAsync();
            task.Wait();
            return task.IsCompletedSuccessfully;
        }

        protected FilterResponse<TEntity> GetAllByFilters(FilterRequest request)
        {
            if (request.NewLimit.HasValue && request.CurrentPage.HasValue)
            {
                return GetAllByFiltersChangeLimit(request);
            }
            else
            {
                return GetAllByFiltersCurrentLimit(request);
            }
        }

        private FilterResponse<TEntity> GetAllByFiltersChangeLimit(FilterRequest request)
        {
            int lastElementPos = request.Limit * request.CurrentPage.Value;
            int loops = (int)Math.Floor((decimal)(lastElementPos / request.NewLimit.Value));
            loops = loops == 0 ? 1 : loops;

            FilterResponse<TEntity> resultList = new FilterResponse<TEntity>();
            List<string> paginationTokens = new List<string> { "{}" };

            for (int i = 0; i < loops; i++)
            {
                resultList = GetAllByFilters(request);
                if (!resultList.PaginationToken.Equals("{}"))
                {
                    paginationTokens.Add(resultList.PaginationToken);
                }
                else if (resultList.PaginationToken.Equals("{}"))
                {
                    break;
                }
            }

            return new FilterResponse<TEntity>
            {
                Elements = resultList.Elements,
                Total = resultList.Total,
                PaginationToken = resultList.PaginationToken,
                PaginationTokens = paginationTokens
            };
        }

        private FilterResponse<TEntity> GetAllByFiltersCurrentLimit(FilterRequest request)
        {
            string paginationToken;
            int total;

            using DynamoDBContext context = GetContext();
            var table = context.GetTargetTable<TEntity>();

            Task<int> taskTotalElements = GetTotalRecords(table, (FilterRequest)request.Clone());

            List<TEntity> totalElements = GetElements(request, table, context, out Search search, out TEntity lastElementGeneral);

            #region GeneratePaginationToken Y TotalResults
            if (request.Limit > 0)
            {
                TEntity lastElementResult = totalElements.LastOrDefault();

                if (search.PaginationToken == "{}" && lastElementGeneral != lastElementResult) //Si encuentra {} es porque tomó un 
                                                                                               //rango de búsqueda tan grande, que logró terminar la búsqueda (search.IsDone==true) y
                                                                                               //por lo tanto, no genera paginationToken.
                {
                    //Se hace una búsqueda con límite 1 para obtener el formato correcto de paginationToken
                    var searchForPagTokenFormat = GetFormatPaginationToken(table, request);
                    paginationToken = GeneratePaginationToken(searchForPagTokenFormat.PaginationToken, lastElementResult);
                }
                else if (search.PaginationToken != "{}")
                {
                    paginationToken = GeneratePaginationToken(search.PaginationToken, lastElementResult);
                }
                else
                {
                    paginationToken = search.PaginationToken;
                }

                total = taskTotalElements != null && taskTotalElements.Result > 0 ? taskTotalElements.Result : search.Count;
            }
            else
            {
                paginationToken = search.PaginationToken;
                total = totalElements.Count;
            }
            #endregion

            return new FilterResponse<TEntity>
            {
                Elements = totalElements,
                PaginationToken = paginationToken,
                Total = total
            };
        }

        private Task<int> GetTotalRecords(Table table, FilterRequest request)
        {
            return Task.Run(() =>
            {
                if (DefineGetTotalRecords(request.Conditions.Select(c => c.AtributeName).ToList()) && request.PaginationToken.Equals("{}") && request.Limit > 0)
                {
                    return GetTotalElements(table, request);
                }
                else
                {
                    return 0;
                }
            });
        }

        private Search GetSearch(Table table, FilterRequest request)
        {
            ISearchFactory SearchFactory = new SearchFactory(table, request);
            return SearchFactory.CreateSearch();
        }

        private List<TEntity> GetElements(FilterRequest request, Table table, DynamoDBContext context, out Search search, out TEntity lastElement)
        {
            var requestElement = (FilterRequest)request.Clone();
            requestElement.GetTotal = false;
            requestElement.Limit = 0;
            search = GetSearch(table, requestElement);

            List<TEntity> totalElements = new List<TEntity>();
            int missingItem = request.Limit; //Si quiero que traiga un máximo (limite) de 100 registros que cumplen con el filtro
                                             //y apenas lleva 30 registros, entonces faltan 70 registros (misingItems).

            do
            {
                List<Document> items = search.GetNextSetAsync().GetAwaiter().GetResult();
                List<TEntity> partialElements = context.FromDocuments<TEntity>(items).ToList();

                if (partialElements.Any())
                {
                    if (request.Limit == 0)
                    {
                        totalElements.AddRange(partialElements);
                    }
                    else
                    {
                        if (partialElements.Count <= missingItem)
                        {
                            totalElements.AddRange(partialElements);
                            missingItem -= partialElements.Count;
                        }
                        else
                        {
                            totalElements.AddRange(partialElements.Take(missingItem));
                        }
                    }
                }

                lastElement = partialElements.LastOrDefault();

            } while (!search.IsDone && (request.Limit == 0 || totalElements.Count < request.Limit));

            return totalElements;
        }

        private string GeneratePaginationToken(string paginationToken, TEntity lastElement)
        {
            Dictionary<string, Dictionary<string, string>> pagToken = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(paginationToken);
            Dictionary<string, Dictionary<string, string>> newTokenObject = new Dictionary<string, Dictionary<string, string>>();

            foreach (var property in pagToken)
            {
                PropertyInfo key = lastElement.GetType().GetProperty(property.Key);
                string valueToSet;

                if (key.PropertyType == typeof(bool))
                {
                    valueToSet = $"{((bool)key.GetValue(lastElement) ? 1 : 0)}";
                }
                else if (key.PropertyType == typeof(DateTime))
                {
                    DateTime date = (DateTime)key.GetValue(lastElement);
                    valueToSet = date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                }
                else
                {
                    valueToSet = key.GetValue(lastElement).ToString();
                }

                newTokenObject.Add(property.Key, new Dictionary<string, string> { { property.Value.First().Key, valueToSet } });
            }

            paginationToken = JsonConvert.SerializeObject(newTokenObject); //Del último registro de la consulta serializa el nuevo PaginationToken construido
                                                                           //para poder saber en una próxima consulta hacia adelante, a partir de cual registro continuar buscando (esto evita volver a recorrer toda la tabla buscando).
            return paginationToken;
        }

        private bool DefineGetTotalRecords(List<string> filedsFilters)
        {
            bool getTotalRecords = filedsFilters.Any();
            try
            {
                var properties = typeTEntity.GetProperties().ToList();

                foreach (var field in filedsFilters)
                {
                    var property = properties.Find(p => p.Name.Equals(field));
                    var propertyAtributes = property.GetCustomAttributes().ToList().OfType<SearchTotalRecordsAttribute>();

                    if (propertyAtributes.Any())
                    {
                        foreach (SearchTotalRecordsAttribute attr in propertyAtributes.OfType<SearchTotalRecordsAttribute>())
                        {
                            if (!attr.GetTotal)
                            {
                                return attr.GetTotal;
                            }

                            getTotalRecords &= attr.GetTotal;
                        }
                    }
                    else
                    {
                        getTotalRecords = false;
                    }
                }
            }
            catch (Exception)
            {
                getTotalRecords &= true;
            }

            return getTotalRecords;
        }

        private int GetTotalElements(Table table, FilterRequest request)
        {
            try
            {
                request.GetTotal = true;
                request.Limit = 0;

                using DynamoDBContext context = GetContext();

                Search search = GetSearch(table, request);
                do { search.GetNextSetAsync().GetAwaiter().GetResult(); } while (!search.IsDone);
                return search.Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private Search GetFormatPaginationToken(Table table, FilterRequest request)
        {
            var requestOnlyElement = (FilterRequest)request.Clone();
            requestOnlyElement.GetTotal = false;
            requestOnlyElement.Limit = 1;
            Search searchForPagTokenFormat = GetSearch(table, requestOnlyElement);
            searchForPagTokenFormat.GetNextSetAsync().GetAwaiter().GetResult();
            return searchForPagTokenFormat;
        }
    }
}
