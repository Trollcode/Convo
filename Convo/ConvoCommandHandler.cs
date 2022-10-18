using Convo.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Convo
{
    public abstract class ConvoCommandHandler
    {
        private readonly Dictionary<string, ConvoCommand> actions = new Dictionary<string, ConvoCommand>();

        private ConvoCommand authAction = new BasicAuthentication();

        public ConvoCommandHandler()
        {
            RegisterOrUpdateChatAction(authAction);
            RegisterOrUpdateChatAction(new ResetContext());
            RegisterOrUpdateChatAction(new DisplayContext());
        }

        protected Dictionary<string, string> RegisteredActions
        {
            get
            {
                return actions.ToDictionary(x => x.Value.Command.Replace("/", ""), x => x.Value.Description);
            }
        }

        protected void RegisterOrUpdateChatAction(ConvoCommand action)
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

        private async Task<ConvoResponse?> HandleReply(IConvoContext ctx, ConvoMessage message, string replyActionId)
        {
            ConvoResponse? chatResponse;
            if (!string.IsNullOrWhiteSpace(replyActionId) && actions.ContainsKey(replyActionId))
            {
                ConvoCommand replyAction = actions[replyActionId];

                if (replyAction.RequireAuthentication && !ctx.IsAuthenticated)
                {
                    chatResponse = await authAction.HandleCommand(ctx, message);
                }
                else
                {
                    chatResponse = await replyAction.HandleReply(ctx, message);
                }
            }
            else
            {
                chatResponse = new ConvoResponse
                {
                    Text = $"An error happened to send reply to expecting action. No action with id {ctx.ExpectingReplyActionId} exists. Resetting context"
                };

                ctx.Reset(message);
            }

            return chatResponse;
        }

        private async Task<ConvoResponse?> HandleCommand(IConvoContext ctx, ConvoMessage message)
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
                ConvoCommand? action = actions.Values.FirstOrDefault(x => x.Command == message.Command);
                if (action != null)
                {
                    if (action.RequireAuthentication && !ctx.IsAuthenticated)
                    {
                        chatResponse = await authAction.HandleCommand(ctx, message);
                    }
                    else
                    {
                        chatResponse = await action.HandleCommand(ctx, message);
                    }
                }
            }
            else
            {
                // TODO: Create default action to handle and try to redirect message to correct place
            }

            return chatResponse;
        }

        protected virtual async Task<bool> HandleMessage(IConvoContext ctx, ConvoMessage message)
        {
            ConvoResponse? chatResponse = null;

            string? replyActionId = ctx.ExpectingReplyActionId;

            ctx.ExpectingReplyActionId = null;

            if (!string.IsNullOrWhiteSpace(replyActionId))
            {
                chatResponse = await HandleReply(ctx, message, replyActionId);
            }
            else
            {
                chatResponse = await HandleCommand(ctx, message);
            }


            //
            // Send the response and perform update/deletion action on messages as requested
            //

            if (chatResponse != null)
            {
                if (!string.IsNullOrWhiteSpace(chatResponse.DeleteMessageId))
                {
                    await DeleteMessage(chatResponse, ctx);
                }

                if (!string.IsNullOrWhiteSpace(chatResponse.UpdateMessageId))
                {
                    if (!await UpdateMessage(chatResponse, ctx))
                    {
                        if (!string.IsNullOrEmpty(chatResponse.Text))
                        {
                            await SendResponse(chatResponse, ctx);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(chatResponse.Text))
                {
                    await SendResponse(chatResponse, ctx);
                }
            }

            return true;
        }

        public abstract Task RegisterCommands();

        protected abstract Task<bool> UpdateMessage(ConvoResponse response, IConvoContext ctx);
        protected abstract Task<bool> DeleteMessage(ConvoResponse response, IConvoContext ctx);
        protected abstract Task<bool> SendResponse(ConvoResponse response, IConvoContext ctx);

        protected abstract Task OnSendFailure(ConvoResponse response, IConvoContext ctx);
    }
}
