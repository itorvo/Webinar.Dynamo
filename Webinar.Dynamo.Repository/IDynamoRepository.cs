using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using Webinar.Dynamo.Repository.Enumerators;

namespace Webinar.Dynamo.Repository
{
    public interface IDynamoRepository<TEntity> where TEntity : class
    {
        bool Add(List<TEntity> lstObj);

        bool Add(TEntity obj);

        List<TEntity> Get(Dictionary<object, object> keys);

        List<TEntity> Get(List<object> keys);

        TEntity Get(long id, int range);

        TEntity Get(TEntity entity);

        List<TEntity> Get(List<TEntity> keys);

        List<TEntity> GetAll();

        bool Remove(List<TEntity> lstObj);

        bool Remove(TEntity obj);

        bool Update(List<TEntity> lstObj);

        bool Update(TEntity obj);

        BatchWrite<TEntity> GetBatchWrite(TEntity entity, DynamoDbBatchOperator batchOperator);

        BatchWrite<TEntity> GetBatchWrite(List<TEntity> entities, DynamoDbBatchOperator batchOperator);

        bool ExecuteTransaction(List<BatchWrite> batchWrites);
    }
}