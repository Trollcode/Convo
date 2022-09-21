using Convo.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convo.Telegram.Example
{
    public class ConvoContextStorage : IConvoContextStorage
    {
        public Task<ConvoContext> CreateOrUpdateContext(ConvoContext ctx, Dictionary<string, string> data)
        {
            throw new NotImplementedException();
        }

        public Task<ConvoContext> GetContextForConversationId(string conversationId)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetDataForChatContext(ConvoContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
