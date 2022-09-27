using Convo.Abstractions.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Convo.Abstractions
{
    public abstract class ConvoActionHandler
    {
        private readonly IConvoContextStorage storage;

        private readonly Dictionary<string, ConvoAction> actions = new Dictionary<string, ConvoAction>();

        private ConvoAction authAction = new BasicAuthenticationAction();

        public ConvoActionHandler(IConvoContextStorage storage)
        {
            this.storage = storage;

            RegisterOrUpdateChatAction(authAction);
            RegisterOrUpdateChatAction(new ResetContextAction());
            RegisterOrUpdateChatAction(new DisplayContextAction());
        }

        protected Dictionary<string, string> RegisteredActions
        {
            get
            {
                return actions.ToDictionary(x => x.Value.Command.Replace("/", ""), x => x.Value.Description);
            }
        }

        protected void RegisterOrUpdateChatAction(ConvoAction action)
        {
            if (actions.ContainsKey(action.Id))
            {
                actions[action.Id] = action;
            }
            else
            {
                actions.Add(action.Id, action);
            }
        }

        protected async Task<(ConvoContext, Dictionary<string, string>)> GetContextAndDataForMessage(ConvoMessage message)
        {
            ConvoContext? ctx = await storage.GetContextForConversationId(message.ConversationId);
            Dictionary<string, string> ctxData = new Dictionary<string, string>();

            if (ctx == null)
            {
                ctx = ConvoContext.FromMessage(message);
            }
            else
            {
                ctxData = await storage.GetDataForChatContext(ctx);
            }

            return (ctx, ctxData);
        }

        private async Task<ConvoResponse?> HandleReply(ConvoContext ctx, Dictionary<string, string> data, ConvoMessage message)
        {
            ConvoResponse? chatResponse = null;

            string replyActionId = ctx.ExpectingReplyActionId;

            ctx.ExpectingReply = false;
            ctx.ExpectingReplyActionId = null;

            if (actions.ContainsKey(replyActionId))
            {
                ConvoAction replyAction = actions[replyActionId];

                if (replyAction.RequireAuthentication && !ctx.IsAuthenticated)
                {
                    chatResponse = await authAction.HandleCommand(ctx, data, message);
                }
                else
                {
                    chatResponse = await replyAction.HandleReply(ctx, data, message);
                }
            }
            else
            {
                chatResponse = new ConvoResponse
                {
                    Text = $"An error happened to send reply to expecting action. No action with id {ctx.ExpectingReplyActionId} exists. Resetting context"
                };
                ConvoContext.ResetFromMessage(ctx, message);
                data.Clear();
            }

            return chatResponse;
        }

        private async Task<ConvoResponse?> HandleCommand(ConvoContext ctx, Dictionary<string, string> data, ConvoMessage message)
        {
            ConvoResponse? chatResponse = new ConvoResponse
            {
                Text = "Unknown command. Available commands are:\n\n"
                    + string.Join(
                        '\n',
                        actions.Select(x => $"{(x.Value.Command.StartsWith("/") ? x.Value.Command : "/" + x.Value.Command)} - {x.Value.Description}"))
            };

            if (!string.IsNullOrWhiteSpace(message.Command))
            {
                ConvoAction? action = actions.Values.FirstOrDefault(x => x.Command == message.Command);
                if (action != null)
                {
                    if (action.RequireAuthentication && !ctx.IsAuthenticated)
                    {
                        chatResponse = await authAction.HandleCommand(ctx, data, message);
                    }
                    else
                    {
                        chatResponse = await action.HandleCommand(ctx, data, message);
                    }
                }
            }
            else
            {
                // TODO: Create default action to handle and try to redirect message to correct place
            }

            return chatResponse;
        }

        protected virtual async Task<bool> HandleMessage(ConvoMessage message)
        {
            (ConvoContext, Dictionary<string, string>) info = await GetContextAndDataForMessage(message);

            ConvoContext ctx = info.Item1;
            Dictionary<string, string> ctxData = info.Item2;

            ConvoResponse? chatResponse = null;

            if (ctx.ExpectingReply && !string.IsNullOrWhiteSpace(ctx.ExpectingReplyActionId))
            {
                chatResponse = await HandleReply(ctx, ctxData, message);
            }
            else
            {
                chatResponse = await HandleCommand(ctx, ctxData, message);
            }

            
            //
            // Clean up context if some bad data were inserted
            //

            if(!ctx.ExpectingReply && !string.IsNullOrWhiteSpace(ctx.ExpectingReplyActionId))
            {
                ctx.ExpectingReplyActionId = null; // NOTE: Warn about this. Since this is user error
            }

            if(ctx.ExpectingReply && string.IsNullOrEmpty(ctx.ExpectingReplyActionId))
            {
                ctx.ExpectingReply = false; // NOTE: Warn about this. Since this is user error
            }


            await storage.CreateOrUpdateContext(ctx, ctxData);

            //
            // Send the response and perform update/deletion action on messages as requested
            //

            if(chatResponse != null)
            {
                if (!string.IsNullOrWhiteSpace(chatResponse.DeleteMessageId))
                {
                    await DeleteMessage(message.ConversationId, chatResponse);
                }

                if (string.IsNullOrWhiteSpace(chatResponse.Text))
                {
                    chatResponse.Text = "MISSING TEXT!";
                }

                if (!string.IsNullOrWhiteSpace(chatResponse.UpdateMessageId))
                {
                    await UpdateMessage(message.ConversationId, chatResponse);
                }
                else
                {
                    await SendResponse(message.ConversationId, chatResponse);
                }
            }

            return true;
        }

        public abstract Task RegisterCommands();

        protected abstract Task<bool> UpdateMessage(string conversationId, ConvoResponse response);
        protected abstract Task<bool> DeleteMessage(string conversationId, ConvoResponse response);
        protected abstract Task<bool> SendResponse(string conversationId, ConvoResponse response);

        protected abstract Task OnSendFailure(string conversationId, ConvoResponse response);

    }
}
