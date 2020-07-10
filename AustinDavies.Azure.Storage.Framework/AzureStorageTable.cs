using AustinDavies.Azure.Storage.Framework.Enums;
using AustinDavies.Azure.Storage.Framework.Util;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AustinDavies.Azure.Storage.Framework
{
    public class AzureStorageTable<TEntity> : IAzureTable<TEntity> where TEntity : class, ITableEntity, new()
	{
		private readonly CloudTable _storageTable;
		public string TableName { get => _storageTable?.Name; }

		public AzureStorageTable(CloudTable table)
		{
			_storageTable = table;
		}

		public async Task<List<TEntity>> QueryAsync(string query, CancellationToken cancellationToken = default)
		{
			return (await _storageTable
				.ExecuteQuerySegmentedAsync(
					new TableQuery<TEntity>()
						.Where(query), new TableContinuationToken())).ToList();
		}

		public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
			return _storageTable.CreateQuery<TEntity>().AsQueryable().Where(predicate);
        }


		public Task<TableResult> DeleteEntryAsync(ITableEntity entity, CancellationToken cancellationToken = default)
		{
			return _storageTable.ExecuteAsync(TableOperation.Delete(entity));
		}

		public async Task ExecuteBatchListAsync(AzureBatchOperationOption option, List<ITableEntity> tableEntities, CancellationToken cancellationToken = default)
		{
			var batches = BatchHelpers.BuildExecutableBatch(option, tableEntities);
			for (int i = 0; i < batches.Count; i++)
			{
				await _storageTable.ExecuteBatchAsync(batches[i]);
			}
		}

		public Task ExecuteBatchAsync(AzureBatchOperationOption option, ITableEntity tableEntity, CancellationToken cancellationToken = default)
		{
			return ExecuteBatchListAsync(option, new List<ITableEntity> { tableEntity }, cancellationToken);
		}
	}
}
