using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Convo.Abstractions.Actions
{
    internal class DisplayContextAction : ConvoAction
    {
        public DisplayContextAction()
        {
            Id = "DisplayContextAction";
            RequireAuthentication = true;
            Command = "me";
            Description = "Display chat context";
        }

        public override Task<ConvoResponse?> HandleCommand(ConvoContext context, Dictionary<string, string> data, ConvoMessage command)
        {
            return Task.FromResult<ConvoResponse?>(new ConvoResponse
            {
                Text = $"<pre>{JsonSerializer.Serialize(context)}</pre>"
            });
        }

        public override Task<ConvoResponse?> HandleReply(ConvoContext context, Dictionary<string, string> data, ConvoMessage reply)
        {
            return Task.FromResult<ConvoResponse?>(new ConvoResponse
            {
                Text = ""
            });
        }
    }
}
