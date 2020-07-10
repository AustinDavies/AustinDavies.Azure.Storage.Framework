using Microsoft.Azure.Cosmos.Table;

namespace AustinDavies.Azure.Storage.Framework
{
    public class AzureStorageAccount : IAzureStorageAccount
	{
		private CloudStorageAccount cloudStorageAccount { get; set; }
		private CloudTableClient cloudTableClient { get; set; }

		public AzureStorageAccount(string connectionString)
		{
			SetConnectionString(connectionString);
		}

		public IAzureTable<DynamicTableEntity> GetTable(string tableName)
		{
			return new AzureStorageTable<DynamicTableEntity>(GetNewTableReference(tableName));
		}

		IAzureTable<TEntity> IAzureStorageAccount.GetTable<TEntity>(string tableName)
		{
			return new AzureStorageTable<TEntity>(GetNewTableReference(tableName));
		}

		private CloudTable GetNewTableReference(string tableName)
		{
			return cloudTableClient.GetTableReference(tableName);
		}

		private void SetConnectionString(string connectionString)
		{
			cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
			cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
		}

	}
}
