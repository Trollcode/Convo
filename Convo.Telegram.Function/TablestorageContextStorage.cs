using Azure;
using Azure.Data.Tables;
using Convo.Abstractions;
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

        public TablestorageContextStorage()
        {
            tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("CONVO_TABLE_STORAGE"));
            stateTableClient = tableServiceClient.GetTableClient("botstate");
            dataTableClient = tableServiceClient.GetTableClient("botdata");

            stateTableClient.CreateIfNotExists();
            dataTableClient.CreateIfNotExists();
        }

        public async Task<ConvoContext> CreateOrUpdateContext(ConvoContext ctx, Dictionary<string, string> data)
        {
            ContextEntity cet = new ContextEntity
            {
                PartitionKey = "convocontext",
                RowKey = ctx.Id,
                Id = ctx.Id,
                Protocol = ctx.Protocol,
                ProtocolAlias = ctx.ProtocolAlias,
                ProtocolName = ctx.ProtocolName,
                IsAuthenticated = ctx.IsAuthenticated,
                ExpectingReply = ctx.ExpectingReply,
                ExpectingReplyActionId = ctx.ExpectingReplyActionId,
                RedirectActionId = ctx.RedirectActionId,
                Updated = DateTime.UtcNow,
                Created = ctx.Created
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
                    await dataTableClient.DeleteEntityAsync(ctx.Id, keyToBeDeleted);
                }
            }

            if (data.Any())
            {
                foreach (var keyValue in data)
                {
                    await dataTableClient.UpsertEntityAsync(new DataEntity
                    {
                        PartitionKey = ctx.Id,
                        RowKey = keyValue.Key,
                        Value = keyValue.Value
                    });
                }
            }
            return cet;
        }

        public async Task<ConvoContext?> GetContextForConversationId(string conversationId)
        {
            try
            {
                Response<ContextEntity> ctx = await stateTableClient.GetEntityAsync<ContextEntity>(
                    rowKey: conversationId,
                    partitionKey: "convocontext" //NOTE: Can not have all conversations in same partition. Need some Primary key here.
                );
                return ctx.Value;
            }
            catch(RequestFailedException)
            {
                return null;
            }
        }

        public Task<Dictionary<string, string>> GetDataForChatContext(ConvoContext ctx)
        {
            Pageable<DataEntity> data = dataTableClient.Query<DataEntity>(x => x.PartitionKey == ctx.Id);

            return Task.FromResult(data.ToDictionary(x => x.RowKey, x => x.Value));
        }

        private class ContextEntity : ConvoContext, ITableEntity
        {
            public string RowKey { get; set; } = default!;
            public string PartitionKey { get; set; } = default!;
            public ETag ETag { get; set; } = default!;
            public DateTimeOffset? Timestamp { get; set; } = default!;
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
