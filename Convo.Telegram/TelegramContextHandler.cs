using Convo.Abstractions;
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
    public class TelegramContextHandler : ConvoActionHandler
    {
        private readonly ILogger<TelegramContextHandler> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly IConvoContextStorage storage;

        private readonly TelegramBotClient telegramClient;

        private bool isInitialized = false;

        public TelegramContextHandler(ILoggerFactory loggerFactory, IConvoContextStorage storage) 
            : base(storage)
        {
            logger = loggerFactory.CreateLogger<TelegramContextHandler>();

            this.loggerFactory = loggerFactory;
            this.storage = storage;

            string? telegramKey = Environment.GetEnvironmentVariable("CONVO_TELEGRAM_KEY");
            if (telegramKey == null)
            {
                logger.LogError("Could not find a telegram key stored in environment variable CONVO_TELEGRAM_KEY.");
            }

            telegramClient = new TelegramBotClient(telegramKey);
        }

        public override async Task RegisterCommands()
        {
            if(!isInitialized)
            {
                await telegramClient.SetMyCommandsAsync(RegisteredActions.Select(x => new BotCommand
                {
                    Command = x.Key,
                    Description = x.Value
                }), BotCommandScope.AllPrivateChats());
                isInitialized = true;
            }
        }

        public async Task RegisterWebhook(string url)
        {
            WebhookInfo info = await telegramClient.GetWebhookInfoAsync();
            if(!info.Url.ToLowerInvariant().Equals(url.ToLowerInvariant()))
            {
                await telegramClient.SetWebhookAsync(url, dropPendingUpdates: true);
            }
        }

        public async Task HandleUpdate(Update update)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        await OnMessageReceived(update.Message);
                        //await BotOnMessageReceived(botClient, update.Message);
                        break;
                    case UpdateType.EditedMessage:
                        await OnMessageReceived(update.EditedMessage);
                        break;
                    case UpdateType.Unknown:
                    case UpdateType.InlineQuery:
                    case UpdateType.ChosenInlineResult:
                    case UpdateType.CallbackQuery:
                        await OnCallbackQueryReceived(update.CallbackQuery);
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


        private async Task OnCallbackQueryReceived(CallbackQuery? callbackQuery)
        {
            if (callbackQuery == null) return;
            if (callbackQuery.Message == null) return;

            await HandleMessage(new ConvoMessage
            {
                ConversationId = callbackQuery.Message.Chat.Id.ToString(),
                MessageId = callbackQuery.Message.MessageId.ToString(),
                Alias = callbackQuery.From.Username,
                Name = $"{callbackQuery.From.FirstName} {callbackQuery.From.LastName}",
                Text = callbackQuery.Data,
                Protocol = Protocol.TELEGRAM
            });
        }

        private async Task OnMessageReceived(Message? message)
        {
            if (message == null) return;
            if (message.Type != MessageType.Text) return;

            await HandleMessage(new ConvoMessage
            {
                ConversationId = message.Chat.Id.ToString(),
                MessageId = message.MessageId.ToString(),
                Alias = message.Chat.Username,
                Name = $"{message.Chat.FirstName} {message.Chat.LastName}",
                Text = message.Text,
                Protocol = Protocol.TELEGRAM
            });
        }

        protected override async Task<bool> HandleMessage(ConvoMessage message)
        {
            await RegisterCommands();
            return await base.HandleMessage(message);
        }

        protected override async Task<bool> UpdateMessage(string conversationId, ConvoResponse msg)
        {
            if(long.TryParse(conversationId, out long chatId) && int.TryParse(msg.UpdateMessageId, out int messageId))
            {
                await telegramClient.EditMessageTextAsync(new ChatId(chatId), messageId, msg.Text);
                return true;
            }
            return false;
        }

        protected override async Task<bool> DeleteMessage(string conversationId, ConvoResponse msg)
        {
            if (long.TryParse(conversationId, out long chatId) && int.TryParse(msg.DeleteMessageId, out int messageId))
            {
                await telegramClient.DeleteMessageAsync(new ChatId(chatId), messageId);
                return true;
            }
            return false;
        }

        protected override async Task<bool> SendResponse(string conversationId, ConvoResponse msg)
        {
            if (long.TryParse(conversationId, out long chatId))
            {
                await telegramClient.SendTextMessageAsync(new ChatId(chatId), msg.Text);
                return true;
            }
            return false;
        }

        protected override Task OnSendFailure(string conversationId, ConvoResponse response)
        {
            logger.LogError($"Error sending message to conversation {conversationId}");
            return Task.CompletedTask;
        }

    }
}
