using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Convo.Commands
{
    internal class ResetContext : ConvoCommand
    {
        public ResetContext()
        {
            Id = "ResetContextAction";
            RequireAuthentication = false;
            Command = "reset";
            Description = "Reset chat context";
        }

        public override Task<ConvoResponse?> HandleCommand(IConvoContext context, ConvoMessage command)
        {
            context.Reset(command);

            return Task.FromResult<ConvoResponse?>(new ConvoResponse
            {
                Text = $"Chat context has been reset"
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
