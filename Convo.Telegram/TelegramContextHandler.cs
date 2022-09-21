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

namespace Convo.Telegram
{
    public class TelegramContextHandler : ConvoActionHandler
    {
        private readonly ILogger<TelegramContextHandler> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;
        private readonly IConvoContextStorage storage;

        private readonly TelegramBotClient telegramClient;

        public TelegramContextHandler(ILoggerFactory loggerFactory, IConfiguration configuration, IServiceProvider serviceProvider, IConvoContextStorage storage) 
            : base(storage)
        {
            logger = loggerFactory.CreateLogger<TelegramContextHandler>();

            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
            this.loggerFactory = loggerFactory;
            this.storage = storage;

            string? telegramKey = Environment.GetEnvironmentVariable("CONVO_TELEGRAM_KEY");
            if (telegramKey == null)
            {
                telegramKey = configuration["Convo:Telegram:Key"];
                if (telegramKey == null)
                {
                    logger.LogError("Could not find a telegram key stored in environment variable CONVO_TELEGRAM_KEY, or appsettings path Convo:Telegram:Key");
                }
            }

            //RegisterOrUpdateChatAction
            telegramClient = new TelegramBotClient(telegramKey);
        }

        public override async Task Initialize()
        {
            await telegramClient.SetMyCommandsAsync(RegisteredActions.Select(x => new BotCommand
            {
                Command = x.Key,
                Description = x.Value
            }), BotCommandScope.AllPrivateChats());
        }

        protected override Task<bool> UpdateMessage(ConvoResponse response)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> DeleteMessage(ConvoResponse response)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> SendResponse(ConvoResponse response)
        {
            throw new NotImplementedException();
        }

        protected override Task OnSendFailure(ConvoResponse response)
        {
            throw new NotImplementedException();
        }

    }
}
