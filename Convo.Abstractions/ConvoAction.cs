using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Convo.Abstractions
{
    public abstract class ConvoAction
    {
        public string Id { get; protected set; }
        public bool RequireAuthentication { get; protected set; }
        public string Command { get; protected set; }
        public string Description { get; protected set; }
        public abstract Task<ConvoResponse?> HandleCommand(ConvoContext context, Dictionary<string, string> data, ConvoMessage command);
        public abstract Task<ConvoResponse?> HandleReply(ConvoContext context, Dictionary<string, string> data, ConvoMessage reply);
    }
}
