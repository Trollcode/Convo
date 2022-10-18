using System.Threading.Tasks;

namespace Convo
{
    public interface IConvoContextStorage
    {
        Task<IConvoContext?> GetContextForConversationId(string conversationId);
        Task<IConvoContext> CreateOrUpdateContext(IConvoContext ctx);
    }
}
