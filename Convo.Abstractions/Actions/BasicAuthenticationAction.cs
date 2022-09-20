using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Convo.Abstractions.Actions
{
    internal class BasicAuthenticationAction : IConvoAction
    {
        private readonly string secret = "password";

        public BasicAuthenticationAction()
        {
            string? envSecret = Environment.GetEnvironmentVariable("CONVO_BOT_SECRET");
            if(envSecret != null) { 
                secret = envSecret; 
            }
        }
        
        public string Id { get; private set; } = "AuthenticationAction";

        public bool RequireAuthentication { get { return false; } }

        public string Command { get { return "auth"; } }
        public string Description { get { return "Built in authentication action"; } }

        public Task<ConvoResponse?> HandleCommand(ConvoContext context, Dictionary<string, string> data, ConvoMessage command)
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
                context.ExpectingReply = true;
                context.ExpectingReplyActionId = Id;
            }

            response.DeleteMessageId = command.MessageId;

            return Task.FromResult<ConvoResponse?>(response);
        }

        public Task<ConvoResponse?> HandleReply(ConvoContext context, Dictionary<string, string> data, ConvoMessage reply)
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
                context.ExpectingReply = true;
                context.ExpectingReplyActionId = Id;
            }

            response.DeleteMessageId = reply.MessageId;
            return Task.FromResult<ConvoResponse?>(response);
        }
        public Task<ConvoResponse?> HandleCallback(ConvoContext context, Dictionary<string, string> data, ConvoMessage callback)
        {
            return Task.FromResult<ConvoResponse?>(null);
        }
    }
}
