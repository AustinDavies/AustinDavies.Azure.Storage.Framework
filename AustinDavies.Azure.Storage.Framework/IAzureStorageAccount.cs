using Microsoft.Azure.Cosmos.Table;

namespace AustinDavies.Azure.Storage.Framework
{
    public interface IAzureStorageAccount
	{
		IAzureTable<DynamicTableEntity> GetTable(string tableName);
		IAzureTable<TEntity> GetTable<TEntity>(string tableName) where TEntity : class, ITableEntity, new();
	}
}
