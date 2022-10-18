using Convo.Buttons;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Convo.Telegram
{
    public class TelegramContextHandler : ConvoCommandHandler
    {
        private readonly ILogger logger;
        TelegramBotClient telegramClient;
        public TelegramContextHandler(ILogger logger, IEnumerable<ConvoCommand> actions) : base()
        {
            this.logger = logger;
            telegramClient = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_KEY") ?? "");

            foreach (ConvoCommand action in actions)
            {
                RegisterOrUpdateChatAction(action);
            }
        }

        public async Task HandleMessage(Update update, IConvoContext ctx)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        await OnMessageReceived(update.Message, ctx);
                        //await BotOnMessageReceived(botClient, update.Message);
                        break;
                    case UpdateType.EditedMessage:
                        await OnMessageReceived(update.EditedMessage, ctx);
                        break;
                    case UpdateType.Unknown:
                    case UpdateType.InlineQuery:
                    case UpdateType.ChosenInlineResult:
                    case UpdateType.CallbackQuery:
                        await OnCallbackQueryReceived(update.CallbackQuery, ctx);
                        break;
                    case UpdateType.ChannelPost:
                    case UpdateType.EditedChannelPost:
                    case UpdateType.ShippingQuery:
                    case UpdateType.PreCheckoutQuery:
                    case UpdateType.Poll:
                    case UpdateType.PollAnswer:
                    case UpdateType.MyChatMember:
                    case UpdateType.ChatMember:
                    case UpdateType.ChatJoinRequest:
                    default:
                        logger.LogWarning($"Unhandeled Update Type: {update.Type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not process message");
            }
        }

        public override Task RegisterCommands()
        {
            throw new NotImplementedException();
        }


        private async Task OnCallbackQueryReceived(CallbackQuery? callbackQuery, IConvoContext ctx)
        {
            if (callbackQuery == null) return;
            if (callbackQuery.Message == null) return;
            if (string.IsNullOrWhiteSpace(callbackQuery.Data)) return;

            await HandleMessage(ctx, new ConvoMessage
            {
                MessageId = callbackQuery.Message.MessageId.ToString(),
                Alias = callbackQuery.From.Username,
                Name = $"{callbackQuery.From.FirstName} {callbackQuery.From.LastName}",
                Text = callbackQuery.Data
            });
        }

        private async Task OnMessageReceived(Message? message, IConvoContext ctx)
        {
            if (message == null) return;
            if (message.Type != MessageType.Text) return;
            if (string.IsNullOrWhiteSpace(message.Text)) return;

            await HandleMessage(ctx, new ConvoMessage
            {
                MessageId = message.MessageId.ToString(),
                Alias = message.Chat.Username,
                Name = $"{message.Chat.FirstName} {message.Chat.LastName}",
                Text = message.Text
            });
        }

        protected override async Task<bool> DeleteMessage(ConvoResponse msg, IConvoContext ctx)
        {
            try
            {
                if (long.TryParse(ctx.ChatId, out long chatId) && int.TryParse(msg.DeleteMessageId, out int messageId))
                {
                    await telegramClient.DeleteMessageAsync(new ChatId(chatId), messageId);
                    return true;
                }
                else
                {
                    logger.LogError($"Could not parse chat id {ctx.ChatId} or delete message id {msg.DeleteMessageId}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting message");
            }
            return false;
        }

        protected override Task OnSendFailure(ConvoResponse response, IConvoContext ctx)
        {
            logger.LogError($"Error sending message to conversation {ctx.ChatId}");
            return Task.CompletedTask;
        }

        protected override async Task<bool> SendResponse(ConvoResponse msg, IConvoContext ctx)
        {
            try
            {
                if (long.TryParse(ctx.ChatId, out long chatId))
                {
                    IReplyMarkup keyboard = new ReplyKeyboardRemove();// InlineKeyboardMarkup.Empty();

                    if (msg is ButtonResponse response)
                    {
                        if (response.ReplyButtons.Any())
                        {
                            keyboard = new InlineKeyboardMarkup(response.ReplyButtons
                                .Select(x => x.Select(k => InlineKeyboardButton.WithCallbackData(k.Text, k.Command)).ToArray()).ToArray());
                        }
                    }

                    await telegramClient.SendTextMessageAsync(new ChatId(chatId), msg.Text, replyMarkup: keyboard);

                    return true;
                }
                else
                {
                    logger.LogError($"Could not parse chat id {ctx.ChatId}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending message");
            }
            return false;
        }

        protected override async Task<bool> UpdateMessage(ConvoResponse msg, IConvoContext ctx)
        {
            try
            {
                if (long.TryParse(ctx.ChatId, out long chatId) && int.TryParse(msg.UpdateMessageId, out int messageId))
                {
                    IReplyMarkup keyboard = new ReplyKeyboardRemove();// InlineKeyboardMarkup.Empty();

                    if (msg is ButtonResponse response)
                    {
                        if (response.ReplyButtons.Any())
                        {
                            keyboard = new InlineKeyboardMarkup(response.ReplyButtons
                                .Select(x => x.Select(k => InlineKeyboardButton.WithCallbackData(k.Text, k.Command)).ToArray()).ToArray());
                        }
                    }

                    await telegramClient.EditMessageTextAsync(new ChatId(chatId), messageId, msg.Text, replyMarkup: keyboard as InlineKeyboardMarkup);
                    return true;
                }
                else
                {
                    logger.LogError($"Could not parse chat id {ctx.ChatId} or update message id {msg.UpdateMessageId}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating message");
            }
            return false;
        }

    }
}
