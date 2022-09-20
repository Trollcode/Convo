using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Convo.Abstractions
{
    public interface IConvoContextStorage
    {
        Task<Dictionary<string, string>> GetDataForChatContext(ConvoContext ctx);
        Task<ConvoContext> GetContextForConversationId(string conversationId);
        Task<ConvoContext> CreateOrUpdateContext(ConvoContext ctx, Dictionary<string, string> data);
    }
}
