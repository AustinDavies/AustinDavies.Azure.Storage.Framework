using AustinDavies.Azure.Storage.Framework.Enums;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AustinDavies.Azure.Storage.Framework.Util
{
    public static class BatchHelpers
	{
		private const int MAX_BATCH_SIZE = 100;

		/// <summary>
		/// Converts a collection of table entities to a grouped collection of batch operations.
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="tableEntities"></param>
		/// <returns></returns>
		public static List<TableBatchOperation> BuildExecutableBatch<T>(AzureBatchOperationOption operation, List<T> tableEntities)
			where T : ITableEntity
		{
			tableEntities = tableEntities.OrderBy(a => a.PartitionKey).ToList();
			List<TableBatchOperation> batches = new List<TableBatchOperation>();
			TableBatchOperation batch = new TableBatchOperation();
			string currentPartitionKey = null;
			for (int i = 0; i < tableEntities.Count; i++)
			{
				var tableEntity = tableEntities[i];
				if (tableEntity.PartitionKey == null) { throw new Exception("PartitionKey cannot be null"); }
				if (tableEntity.RowKey == null) { throw new Exception("RowKey cannot be null"); }
				if (string.IsNullOrEmpty(currentPartitionKey)) { currentPartitionKey = tableEntity.PartitionKey; }
				bool hasReachedBatchSizeLimit = batch.Count != 0 && (batch.Count % MAX_BATCH_SIZE) == 0,
					 currentBatchHasDifferentPartKey = (!currentPartitionKey.Equals(tableEntity.PartitionKey));
				if (hasReachedBatchSizeLimit || currentBatchHasDifferentPartKey)
				{
					batches.Add(batch);
					batch = new TableBatchOperation();
				}
				switch (operation)
				{
					case AzureBatchOperationOption.INSERT:
						batch.Insert(tableEntity);
						break;
					case AzureBatchOperationOption.REPLACE:
						batch.Replace(tableEntity);
						break;
					case AzureBatchOperationOption.INSERT_OR_MERGE:
						batch.InsertOrMerge(tableEntity);
						break;
					case AzureBatchOperationOption.INSERT_OR_REPLACE:
						batch.InsertOrReplace(tableEntity);
						break;
					case AzureBatchOperationOption.MERGE:
						batch.Merge(tableEntity);
						break;
					default:
						batch = null;
						break;
				}
				currentPartitionKey = tableEntity.PartitionKey;
			}
			if (batch.Count > 0) { batches.Add(batch); }
			return batches;
		}
	}
}
