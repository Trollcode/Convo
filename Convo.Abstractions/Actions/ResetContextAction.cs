﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Convo.Abstractions.Actions
{
    internal class ResetContextAction : ConvoAction
    {
        public ResetContextAction()
        {
            Id = "ResetContextAction";
            RequireAuthentication = false;
            Command = "reset";
            Description = "Reset chat context";
        }

        public override Task<ConvoResponse?> HandleCommand(ConvoContext context, Dictionary<string, string> data, ConvoMessage command)
        {
            context.IsAuthenticated = false;
            context.ExpectingReply = false;
            context.ExpectingReplyActionId = null;
            //context.RedirectActionId = null;
            context.Protocol = command.Protocol;

            data.Clear();

            return Task.FromResult<ConvoResponse?>(new ConvoResponse
            {
                Text = $"Chat context has been reset"
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
