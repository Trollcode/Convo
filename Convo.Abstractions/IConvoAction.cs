using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Convo.Abstractions
{
    public interface IConvoAction
    {
        string Id { get; }
        bool RequireAuthentication { get; }
        string Command { get; }
        string Description { get; }
        Task<ConvoResponse?> HandleCommand(ConvoContext context, Dictionary<string, string> data, ConvoMessage command);
        Task<ConvoResponse?> HandleReply(ConvoContext context, Dictionary<string, string> data, ConvoMessage reply);
        Task<ConvoResponse?> HandleCallback(ConvoContext context, Dictionary<string, string> data, ConvoMessage callback);
    }
}
