using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Convo.Commands
{
    internal class DisplayContext : ConvoCommand
    {
        public DisplayContext()
        {
            Id = "DisplayContextAction";
            RequireAuthentication = true;
            Command = "me";
            Description = "Display chat context";
        }

        public override Task<ConvoResponse?> HandleCommand(IConvoContext context, ConvoMessage command)
        {
            return Task.FromResult<ConvoResponse?>(new ConvoResponse
            {
                Text = $"{JsonSerializer.Serialize(context)}"
            });
        }

        public override Task<ConvoResponse?> HandleReply(IConvoContext context, ConvoMessage reply)
        {
            return Task.FromResult<ConvoResponse?>(new ConvoResponse
            {
                Text = ""
            });
        }
    }
}
