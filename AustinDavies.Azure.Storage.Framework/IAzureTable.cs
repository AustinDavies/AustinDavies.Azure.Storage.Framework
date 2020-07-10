using AustinDavies.Azure.Storage.Framework.Enums;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AustinDavies.Azure.Storage.Framework
{
    public interface IAzureTable<TEntity> where TEntity : class, ITableEntity, new()
	{
		Task<List<TEntity>> QueryAsync(string query, CancellationToken cancellationToken = default);
		Task<TableResult> DeleteEntryAsync(ITableEntity entity, CancellationToken cancellationToken = default);
		Task ExecuteBatchListAsync(AzureBatchOperationOption option, List<ITableEntity> tableEntities, CancellationToken cancellationToken = default);
		Task ExecuteBatchAsync(AzureBatchOperationOption option, ITableEntity tableEntity, CancellationToken cancellationToken = default);
	}
}
