using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convo.Telegram.Example
{
    public class TablestorageContextStorage : IConvoContextStorage
    {
        private readonly TableServiceClient tableServiceClient;
        private readonly TableClient stateTableClient;
        private readonly TableClient dataTableClient;

        private static string partitionName = "convocontext";

        public TablestorageContextStorage()
        {
            tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("CONVO_TABLE_STORAGE"));
            stateTableClient = tableServiceClient.GetTableClient("botstate");
            dataTableClient = tableServiceClient.GetTableClient("botdata");

            stateTableClient.CreateIfNotExists();
            dataTableClient.CreateIfNotExists();
        }

        public async Task<IConvoContext> CreateOrUpdateContext(IConvoContext ctx, Dictionary<string, string> data)
        {
            ContextEntity cet = new ContextEntity
            {
                PartitionKey = partitionName,
                RowKey = ctx.ChatId,
                ChatId = ctx.ChatId,
                Alias = ctx.Alias,
                Name = ctx.Name,
                IsAuthenticated = ctx.IsAuthenticated,
                ExpectingReplyActionId = ctx.ExpectingReplyActionId,
                //RedirectActionId = ctx.RedirectActionId,
                //Updated = DateTime.UtcNow,
                //Created = ctx.Created
            };

            await stateTableClient.UpsertEntityAsync(cet);

            Dictionary<string, string> existingData = await GetDataForChatContext(ctx);

            if (existingData.Any())
            {
                List<string> toBeDeleted = new List<string>();

                foreach (string existing in existingData.Values)
                {
                    if (!data.ContainsKey(existing))
                    {
                        toBeDeleted.Add(existing);
                    }
                }

                foreach (string keyToBeDeleted in toBeDeleted)
                {
                    await dataTableClient.DeleteEntityAsync(ctx.ChatId, keyToBeDeleted);
                }
            }

            if (data.Any())
            {
                foreach (var keyValue in data)
                {
                    await dataTableClient.UpsertEntityAsync(new DataEntity
                    {
                        PartitionKey = ctx.ChatId,
                        RowKey = keyValue.Key,
                        Value = keyValue.Value
                    });
                }
            }
            return cet;
        }

        public async Task<IConvoContext?> GetContextForConversationId(string conversationId)
        {
            try
            {
                Response<ContextEntity> ctx = await stateTableClient.GetEntityAsync<ContextEntity>(
                    rowKey: conversationId,
                    partitionKey: partitionName //NOTE: Can not have all conversations in same partition. Need some Primary key here.
                );
                return ctx.Value;
            }
            catch(RequestFailedException)
            {
                return null;
            }
        }

        public Task<Dictionary<string, string>> GetDataForChatContext(IConvoContext ctx)
        {
            Pageable<DataEntity> data = dataTableClient.Query<DataEntity>(x => x.PartitionKey == ctx.ChatId);

            return Task.FromResult(data.ToDictionary(x => x.RowKey, x => x.Value));
        }

        Task<IConvoContext?> IConvoContextStorage.GetContextForConversationId(string conversationId)
        {
            throw new NotImplementedException();
        }

        public Task<IConvoContext> CreateOrUpdateContext(IConvoContext ctx)
        {
            throw new NotImplementedException();
        }

        private class ContextEntity : IConvoContext, ITableEntity
        {
            public string RowKey { get; set; } = default!;
            public string PartitionKey { get; set; } = default!;
            public ETag ETag { get; set; } = default!;
            public DateTimeOffset? Timestamp { get; set; } = default!;
            public string ChatId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string Alias { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public bool IsAuthenticated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string? ExpectingReplyActionId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public Dictionary<string, object?> Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public void Reset(ConvoMessage message)
            {
                throw new NotImplementedException();
            }
        }

        private class DataEntity : ITableEntity
        {
            public string RowKey { get; set; } = default!;
            public string PartitionKey { get; set; } = default!;
            public ETag ETag { get; set; } = default!;
            public DateTimeOffset? Timestamp { get; set; } = default!;
            public string Value { get; set; } = default!;
        }
    }
}
