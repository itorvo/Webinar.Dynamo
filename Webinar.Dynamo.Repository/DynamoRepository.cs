using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Webinar.Dynamo.Repository.Enumerators;
using Webinar.Dynamo.Repository.Filters;
using Webinar.Dynamo.Repository.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Webinar.Dynamo.Repository
{
    public class DynamoRepository<TEntity> : IDynamoRepository<TEntity> where TEntity : class
    {
        protected readonly IConfiguration Configuration;
        protected AmazonDynamoDBClient Client;
        private readonly Type typeTEntity;

        public DynamoRepository(IConfiguration configuration)
        {
            typeTEntity = typeof(TEntity);
            Configuration = configuration;
        }

        public DynamoRepository()
        {
            typeTEntity = typeof(TEntity);
        }

        private void GetClient()
        {
            if (Client == null)
            {
                var serviceUrl = Configuration["AWS:Dynamo:ServiceURL"];
                var config = new AmazonDynamoDBConfig();

                if (!string.IsNullOrEmpty(serviceUrl))
                {
                    config.ServiceURL = serviceUrl;
                }

                Client = new AmazonDynamoDBClient(config);
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

            GetClient();
            AWSConfigsDynamoDB.Context.TableNamePrefix = prefix;
            return new DynamoDBContext(Client);
        }

        public BatchWrite<TEntity> GetBatchWrite(TEntity entity, DynamoDbBatchOperator batchOperator)
        {
            using (DynamoDBContext context = GetContext())
            {
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
        }

        public BatchWrite<TEntity> GetBatchWrite(List<TEntity> entities, DynamoDbBatchOperator batchOperator)
        {
            using (DynamoDBContext context = GetContext())
            {
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
        }

        public bool ExecuteTransaction(List<BatchWrite> batchWrites)
        {
            using (DynamoDBContext context = GetContext())
            {
                var multi = context.CreateMultiTableBatchWrite(batchWrites.ToArray());
                var task = multi.ExecuteAsync();
                task.Wait();
                return task.IsCompletedSuccessfully;
            }
        }

        public bool Add(TEntity obj)
        {
            using (DynamoDBContext context = GetContext())
            {
                var task = context.SaveAsync(obj);
                task.Wait();
                return task.IsCompletedSuccessfully;
            }
        }

        public bool Add(List<TEntity> lstObj)
        {
            using (DynamoDBContext context = GetContext())
            {
                var batch = context.CreateBatchWrite<TEntity>();
                batch.AddPutItems(lstObj);
                var task = batch.ExecuteAsync();
                task.Wait();
                return task.IsCompletedSuccessfully;
            }
        }

        public List<TEntity> Get(List<object> keys)
        {
            using (DynamoDBContext context = GetContext())
            {
                var batch = context.CreateBatchGet<TEntity>();
                keys.ForEach(item => batch.AddKey(item));
                batch.ExecuteAsync().Wait();
                return batch.Results;
            }
        }

        public List<TEntity> Get(List<TEntity> keys)
        {
            using (DynamoDBContext context = GetContext())
            {
                var batch = context.CreateBatchGet<TEntity>();
                keys.ForEach(item => batch.AddKey(item));
                batch.ExecuteAsync().Wait();
                return batch.Results;
            }
        }

        public List<TEntity> Get(Dictionary<object, object> keys)
        {
            using (DynamoDBContext context = GetContext())
            {
                var batch = context.CreateBatchGet<TEntity>();
                foreach (var keyPairs in keys)
                    batch.AddKey(keyPairs.Key, keyPairs.Value);

                batch.ExecuteAsync().Wait();
                return batch.Results;
            }
        }

        public TEntity Get(long id, int range)
        {
            TEntity entity;
            using (DynamoDBContext context = GetContext())
            {
                entity = context.LoadAsync<TEntity>(id, range).Result;
            }
            return entity;
        }

        public TEntity Get(TEntity entity)
        {
            using (DynamoDBContext context = GetContext())
            {
                return context.LoadAsync(entity).Result;
            }
        }

        public List<TEntity> GetAll()
        {
            List<TEntity> partialResultDynamo = new List<TEntity>();
            List<TEntity> totalResultDynamo = new List<TEntity>();

            using (DynamoDBContext context = GetContext())
            {
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
            }

            return totalResultDynamo;
        }

        public bool Update(TEntity obj)
        {
            using (DynamoDBContext context = GetContext())
            {
                var task = context.SaveAsync(obj);
                task.Wait();
                return task.IsCompletedSuccessfully;
            }
        }

        public bool Update(List<TEntity> lstObj)
        {
            using (DynamoDBContext context = GetContext())
            {
                var batch = context.CreateBatchWrite<TEntity>();
                batch.AddPutItems(lstObj);
                var task = batch.ExecuteAsync();
                task.Wait();
                return task.IsCompletedSuccessfully;
            }
        }

        public bool Remove(TEntity obj)
        {
            using (DynamoDBContext context = GetContext())
            {
                var task = context.DeleteAsync(obj);
                task.Wait();
                return task.IsCompletedSuccessfully;
            }
        }

        public bool Remove(List<TEntity> lstObj)
        {
            using (DynamoDBContext context = GetContext())
            {
                var batch = context.CreateBatchWrite<TEntity>();
                batch.AddDeleteItems(lstObj);
                var task = batch.ExecuteAsync();
                task.Wait();
                return task.IsCompletedSuccessfully;
            }
        }

        protected List<TEntity> GetAllByFilters(FilterQuery condition, string indexName = null, List<string> attributesToGet = null, bool backwardSearch = true,
            int limit = 0)
        {
            List<FilterQuery> conditions = new List<FilterQuery>();
            if (condition != null)
            {
                conditions.Add(condition);
            }

            return GetAllByFilters(conditions: conditions, indexName, attributesToGet, backwardSearch, limit);
        }

        protected List<TEntity> GetAllByFilters(List<FilterQuery> conditions, string indexName = null, List<string> attributesToGet = null, bool backwardSearch = true,
            int limit = 0)
        {
            string paginationToken = "{}";
            int total = 0;

            return GetAllByFilters(conditions, ref paginationToken, ref total, attributesToGet, backwardSearch: backwardSearch, indexName: indexName, limit: limit);
        }

        protected List<TEntity> GetAllByFilters(FilterQuery condition, ref string paginationToken, ref int total, List<string> attributesToGet = null, int limit = 0,
            string indexName = null, bool backwardSearch = true)
        {
            return GetAllByFilters(new List<FilterQuery> { condition }, ref paginationToken,
            ref total, attributesToGet, limit, indexName, backwardSearch);
        }

        protected List<TEntity> GetAllByFilters(List<FilterQuery> conditions, ref string paginationToken, ref int total, List<string> attributesToGet = null,
            int limit = 0, string indexName = null, bool backwardSearch = true)
        {
            //backwardSearch es usado para ordenar por sort key en orden ascendente o descendente.
            List<TEntity> partialElements;
            List<TEntity> totalElements = new List<TEntity>();

            using (DynamoDBContext context = GetContext())
            {
                var table = context.GetTargetTable<TEntity>();
                Task<int> taskTotalElements = null;

                if (DefineGetTotalRecords(conditions.Select(c => c.AtributeName).ToList()) && paginationToken.Equals("{}") && limit > 0)
                {
                    var pagiToken = paginationToken;
                    taskTotalElements = new Task<int>(() => GetTotalElements(table, indexName, pagiToken, conditions));
                    taskTotalElements.Start();
                }

                Search search = GetSearch(table, conditions, paginationToken, indexName, backwardSearch, attributesToGet, false);

                //Teniendo en cuenta la restricción de Dynamo en cuanto al límite de 1MB de los registros devueltos por cada consulta:

                int missingItem = limit - totalElements.Count; //Si quiero que traiga un máximo (limite) de 100 registros que cumplen con el filtro
                                                               //y apenas lleva 30 registros, entonces faltan 70 registros (misingItems).
                #region Obtener Data
                do
                {
                    List<Document> items = search.GetNextSetAsync().GetAwaiter().GetResult();
                    partialElements = context.FromDocuments<TEntity>(items).ToList();

                    if (partialElements.Any())
                    {
                        if (limit == 0)
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
                } while (!search.IsDone && (limit == 0 || totalElements.Count < limit));
                #endregion

                #region GeneratePaginationToken Y TotalResults
                if (limit > 0)
                {
                    TEntity lastElement = totalElements.LastOrDefault();

                    if (search.PaginationToken == "{}" && partialElements.LastOrDefault() != lastElement) //Si encuentra {} es porque tomó un 
                                                                                                          //rango de búsqueda tan grande, que logró terminar la búsqueda (search.IsDone==true) y
                                                                                                          //por lo tanto, no genera paginationToken.
                    {
                        //Se hace una búsqueda con límite 1 para obtener el formato correcto de paginationToken
                        var searchForPagTokenFormat = GetSearch(table, conditions, paginationToken, indexName, backwardSearch, attributesToGet, limit: 1);
                        searchForPagTokenFormat.GetNextSetAsync().GetAwaiter().GetResult();
                        paginationToken = GeneratePaginationToken(searchForPagTokenFormat.PaginationToken, lastElement);

                    }
                    else if (search.PaginationToken != "{}")
                    {
                        paginationToken = GeneratePaginationToken(search.PaginationToken, lastElement);
                    }
                    else
                    {
                        paginationToken = search.PaginationToken;
                    }

                    total = taskTotalElements != null ? taskTotalElements.Result : search.Count;
                }
                else
                {
                    paginationToken = search.PaginationToken;
                    total = totalElements.Count;
                }
                #endregion
            }
            return totalElements;
        }

        protected List<TEntity> GetAllByFilters(List<FilterQuery> conditions, ref List<string> paginationTokens, int newLimit, int currentPage, out int count,
            string indexName = null, int limit = 0, bool backwardSearch = true, List<string> attributesToGet = null)
        {
            int lastElementPos = limit * currentPage;
            int loops = (int)Math.Floor((decimal)(lastElementPos / newLimit));
            loops = loops == 0 ? 1 : loops;
            string pagToken = null;
            List<TEntity> resultList = null;
            paginationTokens.Add("{}");
            count = 0;
            for (int i = 0; i < loops; i++)
            {
                resultList = GetAllByFilters(conditions, ref pagToken, ref count, limit: newLimit, indexName: indexName, backwardSearch: backwardSearch, attributesToGet: attributesToGet);
                if (pagToken != "{}")
                {
                    paginationTokens.Add(pagToken);
                }
                else if (pagToken == "{}")
                {
                    break;
                }
            }
            return resultList;
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
            bool getTotalRecords = true;
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

        private int GetTotalElements(Table table, string indexName, string paginationToken, List<FilterQuery> valuesAtribute)
        {
            try
            {
                int total = 0;

                using (DynamoDBContext context = GetContext())
                {
                    Search search = GetSearch(table, valuesAtribute, paginationToken, indexName, getTotal: true);

                    do
                    {
                        search.GetNextSetAsync().GetAwaiter().GetResult();
                    } while (!search.IsDone);
                    total = search.Count;
                }

                return total;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private Search GetSearch(Table table, List<FilterQuery> conditions, string paginationToken = "{}", string indexName = null,
            bool backwardSearch = true, List<string> attributesToGet = null, bool getTotal = false, int? limit = null)
        {
            var queries = conditions.Where(f => f.TypeCondition == DynamoDbTypeCondition.Query).ToList();
            var scans = conditions.Where(f => f.TypeCondition == DynamoDbTypeCondition.Scan).ToList();

            if (queries.Any())
            {
                var query = GetQueryOperationConfig(queries, scans, paginationToken, indexName, backwardSearch, attributesToGet, getTotal, limit);
                return table.Query(query);
            }
            else
            {
                var scan = GetScanOperationConfig(scans, paginationToken, indexName, attributesToGet, getTotal, limit);
                return table.Scan(scan);
            }
        }

        private QueryOperationConfig GetQueryOperationConfig(List<FilterQuery> queries, List<FilterQuery> scans, string paginationToken = "{}",
            string indexName = null, bool backwardSearch = true, List<string> attributesToGet = null, bool getTotal = false, int? limit = null)
        {
            QueryFilter queryFilter = new QueryFilter();

            SetQueryCondition(queries, queryFilter);
            SetScanCondition(scans, queryFilter);

            var query = new QueryOperationConfig
            {
                Filter = queryFilter,
                BackwardSearch = backwardSearch,
                CollectResults = true,
                PaginationToken = !string.IsNullOrEmpty(paginationToken) && !paginationToken.Equals("{}") ? paginationToken : null,
                IndexName = !string.IsNullOrEmpty(indexName) ? indexName : null,
                Limit = limit != null ? limit.Value : int.MaxValue
            };

            if (getTotal)
            {
                attributesToGet = queryFilter.ToConditions().Keys.Distinct().ToList();
            }

            if (attributesToGet != null && attributesToGet.Any())
            {
                query.AttributesToGet = attributesToGet;
                query.Select = SelectValues.SpecificAttributes;
            }

            return query;
        }

        private ScanOperationConfig GetScanOperationConfig(List<FilterQuery> scans, string paginationToken = "{}", string indexName = null,
            List<string> attributesToGet = null, bool getTotal = false, int? limit = null)
        {
            ScanFilter scanFilter = new ScanFilter();

            SetScanCondition(scans, scanFilter);

            var scan = new ScanOperationConfig
            {
                Filter = scanFilter,
                CollectResults = true,
                PaginationToken = !string.IsNullOrEmpty(paginationToken) && !paginationToken.Equals("{}") ? paginationToken : null,
                IndexName = !string.IsNullOrEmpty(indexName) ? indexName : null,
                Limit = limit != null ? limit.Value : int.MaxValue
            };

            if (getTotal)
            {
                attributesToGet = scanFilter.ToConditions().Keys.Distinct().ToList();
            }

            if (attributesToGet != null && attributesToGet.Any())
            {
                scan.AttributesToGet = attributesToGet;
                scan.Select = SelectValues.SpecificAttributes;
            }

            return scan;
        }

        private void SetScanCondition(List<FilterQuery> conditions, ScanFilter scanFilter)
        {
            foreach (FilterQuery condition in conditions)
            {
                Type attributeClass;

                switch ((ScanOperator)condition.Operator)
                {
                    case ScanOperator.Between:
                        attributeClass = condition.ValueAtribute.GetType();
                        if (attributeClass.Equals(typeof(sbyte)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (byte)condition.ValueAtribute, (byte)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(int)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (int)condition.ValueAtribute, (int)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(long)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (long)condition.ValueAtribute, (long)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(double)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (double)condition.ValueAtribute, (double)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(DateTime)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (DateTime)condition.ValueAtribute, (DateTime)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(string)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (string)condition.ValueAtribute, (string)condition.ValueAtributeFinal);
                        break;
                    case ScanOperator.In:
                        attributeClass = condition.ValueAtribute.GetType();

                        if (attributeClass.Equals(typeof(List<string>)))
                        {
                            var listValues = ((List<string>)condition.ValueAtribute).Select(v => new AttributeValue() { S = v }).ToList();
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, listValues);
                        }
                        else if (attributeClass.Equals(typeof(List<long>)))
                        {
                            var listValues = ((List<long>)condition.ValueAtribute).Select(v => new AttributeValue() { N = $"{v}" }).ToList();
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, listValues);
                        }
                        else if (attributeClass.Equals(typeof(List<int>)))
                        {
                            var listValues = ((List<int>)condition.ValueAtribute).Select(v => new AttributeValue() { N = $"{v}" }).ToList();
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, listValues);
                        }
                        else
                        {
                            throw new NotImplementedException("No se ha definido el manejo de condicion");
                        }
                        break;
                    case ScanOperator.IsNotNull:
                    case ScanOperator.IsNull:
                        ComparisonOperator @operator = (ScanOperator)condition.Operator == ScanOperator.IsNotNull ? ComparisonOperator.NOT_NULL : ComparisonOperator.NULL;
                        scanFilter.AddCondition(condition.AtributeName, new Condition { ComparisonOperator = @operator });
                        break;
                    default:
                        attributeClass = condition.ValueAtribute.GetType();
                        if (attributeClass.Equals(typeof(sbyte)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (byte)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(int)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (int)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(long)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (long)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(double)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (double)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(DateTime)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (DateTime)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(string)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (string)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(bool)))
                            scanFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (bool)condition.ValueAtribute);
                        break;
                }
            }
        }

        private void SetScanCondition(List<FilterQuery> conditions, QueryFilter queryFilter)
        {
            foreach (FilterQuery condition in conditions)
            {
                Type attributeClass;

                switch ((ScanOperator)condition.Operator)
                {
                    case ScanOperator.Between:
                        attributeClass = condition.ValueAtribute.GetType();

                        if (attributeClass.Equals(typeof(sbyte)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (byte)condition.ValueAtribute, (byte)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(int)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (int)condition.ValueAtribute, (int)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(long)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (long)condition.ValueAtribute, (long)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(double)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (double)condition.ValueAtribute, (double)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(DateTime)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (DateTime)condition.ValueAtribute, (DateTime)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(string)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (string)condition.ValueAtribute, (string)condition.ValueAtributeFinal);
                        break;
                    case ScanOperator.In:
                        attributeClass = condition.ValueAtribute.GetType();

                        if (attributeClass.Equals(typeof(List<string>)))
                        {
                            var listValues = ((List<string>)condition.ValueAtribute).Select(v => new AttributeValue() { S = v }).ToList();
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, listValues);
                        }
                        else if (attributeClass.Equals(typeof(List<long>)))
                        {
                            var listValues = ((List<long>)condition.ValueAtribute).Select(v => new AttributeValue() { N = $"{v}" }).ToList();
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, listValues);
                        }
                        else if (attributeClass.Equals(typeof(List<int>)))
                        {
                            var listValues = ((List<int>)condition.ValueAtribute).Select(v => new AttributeValue() { N = $"{v}" }).ToList();
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, listValues);
                        }
                        else
                        {
                            throw new NotImplementedException("No se ha definido el manejo de condicion");
                        }
                        break;
                    case ScanOperator.IsNotNull:
                    case ScanOperator.IsNull:
                        ComparisonOperator @operator = (ScanOperator)condition.Operator == ScanOperator.IsNotNull ? ComparisonOperator.NOT_NULL : ComparisonOperator.NULL;
                        queryFilter.AddCondition(condition.AtributeName, new Condition { ComparisonOperator = @operator });
                        break;
                    default:
                        attributeClass = condition.ValueAtribute.GetType();
                        if (attributeClass.Equals(typeof(sbyte)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (byte)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(int)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (int)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(long)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (long)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(double)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (double)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(DateTime)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (DateTime)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(string)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (string)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(bool)))
                            queryFilter.AddCondition(condition.AtributeName, (ScanOperator)condition.Operator, (bool)condition.ValueAtribute);
                        break;
                }
            }
        }

        private void SetQueryCondition(List<FilterQuery> conditions, QueryFilter queryFilter)
        {
            foreach (FilterQuery condition in conditions)
            {
                Type attributeClass = condition.ValueAtribute.GetType();

                switch ((QueryOperator)condition.Operator)
                {
                    case QueryOperator.BeginsWith:
                        if (attributeClass.Equals(typeof(sbyte)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (byte)condition.ValueAtribute, (byte)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(byte)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (byte)condition.ValueAtribute, (DateTime)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(DateTime)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (DateTime)condition.ValueAtribute, (DateTime)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(int)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (int)condition.ValueAtribute, (int)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(long)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (long)condition.ValueAtribute, (long)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(double)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (double)condition.ValueAtribute, (double)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(string)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (string)condition.ValueAtribute, (string)condition.ValueAtributeFinal);
                        else if (attributeClass.Equals(typeof(bool)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (bool)condition.ValueAtribute, (bool)condition.ValueAtributeFinal);
                        break;

                    default:
                        if (attributeClass.Equals(typeof(sbyte)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (byte)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(byte)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (byte)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(int)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (int)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(long)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (long)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(double)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (double)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(DateTime)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (DateTime)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(string)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (string)condition.ValueAtribute);
                        else if (attributeClass.Equals(typeof(bool)))
                            queryFilter.AddCondition(condition.AtributeName, (QueryOperator)condition.Operator, (bool)condition.ValueAtribute);
                        break;
                }
            }
        }
    }
}
