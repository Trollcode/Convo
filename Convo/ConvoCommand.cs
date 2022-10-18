using System.Collections.Generic;
using System.Threading.Tasks;

namespace Convo
{
    public abstract class ConvoCommand
    {
        public string Id { get; protected set; }
        public bool RequireAuthentication { get; protected set; }
        public string Command { get; protected set; }
        public string Description { get; protected set; }
        public abstract Task<ConvoResponse?> HandleCommand(IConvoContext context, ConvoMessage command);
        public abstract Task<ConvoResponse?> HandleReply(IConvoContext context, ConvoMessage reply);
    }
}