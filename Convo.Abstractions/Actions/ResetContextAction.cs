using System.Collections.Generic;
using System.Threading.Tasks;

namespace Convo.Abstractions.Actions
{
    internal class ResetContextAction : IConvoAction
    {
        public string Id { get; private set; } = "ResetContextAction";

        public bool RequireAuthentication { get { return false; } }

        public string Command { get { return "reset"; } }
        public string Description { get { return "Reset chat context"; } }

        public Task<ConvoResponse?> HandleCommand(ConvoContext context, Dictionary<string, string> data, ConvoMessage command)
        {
            context.IsAuthenticated = false;
            context.ExpectingReply = false;
            context.ExpectingReplyActionId = null;
            context.RedirectActionId = null;
            context.Protocol = command.Protocol;

            data.Clear();

            return Task.FromResult<ConvoResponse?>(new ConvoResponse
            {
                Text = $"Chat context has been reset"
            });
        }

        public Task<ConvoResponse?> HandleReply(ConvoContext context, Dictionary<string, string> data, ConvoMessage reply)
        {
            return Task.FromResult<ConvoResponse?>(new ConvoResponse
            {
                Text = ""
            });
        }

        public Task<ConvoResponse?> HandleCallback(ConvoContext context, Dictionary<string, string> data, ConvoMessage callback)
        {
            return Task.FromResult<ConvoResponse?>(null);
        }
    }
}
