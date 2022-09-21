using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace Convo.Telegram.Example
{
    public class CommandUpdater
    {
        private readonly TelegramContextHandler telegram;

        public CommandUpdater(TelegramContextHandler telegram)
        {
            this.telegram = telegram;
        }

        [FunctionName("Telegram-Commands-Updater")]
        public async Task Run([TimerTrigger("0 0 8 * * *", RunOnStartup = true)]TimerInfo timer)
        {
            await telegram.RegisterCommands();
        }
    }
}
