using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Convo.Commands
{
    internal class BasicAuthentication : ConvoCommand
    {
        private readonly string secret = "password";

        public BasicAuthentication()
        {
            string? envSecret = Environment.GetEnvironmentVariable("CONVO_BOT_SECRET");
            if (envSecret != null)
            {
                secret = envSecret;
            }

            Id = "AuthenticationAction";
            RequireAuthentication = false;
            Command = "auth";
            Description = "Built in authentication action";
        }

        public override Task<ConvoResponse?> HandleCommand(IConvoContext context, ConvoMessage command)
        {
            var response = new ConvoResponse
            {
                Text = $"Hello {command.Name}, please reply with a password"
            };

            if (command.Arguments.Length > 0)
            {
                if (command.Arguments[0].ToLowerInvariant().Equals(secret.ToLowerInvariant()))
                {
                    context.IsAuthenticated = true;

                    response = new ConvoResponse
                    {
                        Text = $"Thank you {command.Name} you are now authenticated"
                    };
                }
                else
                {
                    response = new ConvoResponse
                    {
                        Text = $"Sorry, the password is incorrect"
                    };
                }
            }
            else
            {
                context.ExpectingReplyActionId = Id;
            }

            response.DeleteMessageId = command.MessageId;

            return Task.FromResult<ConvoResponse?>(response);
        }

        public override Task<ConvoResponse?> HandleReply(IConvoContext context, ConvoMessage reply)
        {
            var response = new ConvoResponse
            {
                Text = $"Sorry, the password is incorrect"
            };


            if (reply.Text.ToLowerInvariant().Equals(secret.ToLowerInvariant()))
            {
                context.IsAuthenticated = true;

                response = new ConvoResponse
                {
                    Text = $"Thank you {reply.Name} you are now authenticated"
                };
            }
            else
            {
                context.ExpectingReplyActionId = Id;
            }

            response.DeleteMessageId = reply.MessageId;
            return Task.FromResult<ConvoResponse?>(response);
        }
    }
}
