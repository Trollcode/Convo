﻿using Convo.Abstractions.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Convo.Abstractions
{
    public abstract class ConvoActionHandler
    {
        private readonly IConvoContextStorage storage;

        private readonly Dictionary<string, IConvoAction> actions = new Dictionary<string, IConvoAction>();

        private IConvoAction authAction = new BasicAuthenticationAction();

        public ConvoActionHandler(IConvoContextStorage storage)
        {
            this.storage = storage;

            RegisterOrUpdateChatAction(authAction);
            RegisterOrUpdateChatAction(new ResetContextAction());
        }

        protected Dictionary<string, string> RegisteredActions
        {
            get
            {
                return actions.ToDictionary(x => x.Value.Command.Replace("/", ""), x => x.Value.Description);
            }
        }

        protected void RegisterOrUpdateChatAction(IConvoAction action)
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
            ConvoContext ctx = await storage.GetContextForConversationId(message.ConversationId);
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

        protected virtual async Task<bool> HandleMessage(ConvoMessage message)
        {
            (ConvoContext, Dictionary<string, string>) info = await GetContextAndDataForMessage(message);

            ConvoContext ctx = info.Item1;
            Dictionary<string, string> ctxData = info.Item2;

            ConvoResponse? chatResponse = new ConvoResponse
            {
                Text = "Unknown command. Available commands are:\n\n"
                    + string.Join(
                        '\n',
                        actions.Select(x => $"{(x.Value.Command.StartsWith("/") ? x.Value.Command : "/" + x.Value.Command)} - {x.Value.Description}"))
            };



            if (ctx.ExpectingReply && !string.IsNullOrWhiteSpace(ctx.ExpectingReplyActionId))
            {
                if (actions.ContainsKey(ctx.ExpectingReplyActionId))
                {
                    string actionId = ctx.ExpectingReplyActionId;
                    ctx.ExpectingReply = false;
                    ctx.ExpectingReplyActionId = null;

                    chatResponse = await actions[actionId].HandleReply(ctx, ctxData, message);
                }
                else
                {
                    chatResponse = new ConvoResponse
                    {
                        Text = $"An error happened to send reply to expecting action. No action with id {ctx.ExpectingReplyActionId} exists. Resetting context"
                    };
                    ctx = ConvoContext.FromMessage(message);
                    ctxData.Clear();
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(message.Command))
                {
                    IConvoAction? tmp = actions.Values.FirstOrDefault(x => x.Command == message.Command);
                    if (tmp != null)
                    {
                        if (tmp.RequireAuthentication && !ctx.IsAuthenticated)
                        {
                            ctx.RedirectActionId = tmp.Id;
                            chatResponse = await authAction.HandleCommand(ctx, ctxData, message);
                        }
                        else if (ctx.IsAuthenticated && !string.IsNullOrWhiteSpace(ctx.RedirectActionId))
                        {
                            string actionId = ctx.RedirectActionId;
                            ctx.RedirectActionId = null;

                            chatResponse = await actions[actionId].HandleCommand(ctx, ctxData, message);
                        }
                        else
                        {
                            chatResponse = await tmp.HandleCommand(ctx, ctxData, message);
                        }
                    }
                }
            }

            await storage.CreateOrUpdateContext(ctx, ctxData);


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
