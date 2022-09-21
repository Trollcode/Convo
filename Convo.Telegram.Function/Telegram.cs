using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Convo.Telegram.Example
{
    public class Telegram
    {
        private readonly TelegramContextHandler telegram;

        public Telegram(TelegramContextHandler telegram)
        {
            this.telegram = telegram;
        }

        [FunctionName("Telegram-Message-Handler")]
        public async Task<IActionResult> Handler([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# Telegram-Message-Handler function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if(!string.IsNullOrWhiteSpace(requestBody))
            {
                try
                {
                    Update data = JsonConvert.DeserializeObject<Update>(requestBody);
                    if(data != null)
                    {
                        await telegram.HandleUpdate(data);
                    }
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "Error processing message");
                }
            }
            return new OkResult();
        }

        [FunctionName("Telegram-Message-Webhook")]
        public async Task<IActionResult> Webhook([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req, ILogger log)
        {
            string url = req.Query["url"];

            log.LogInformation($"C# HTTP trigger function processed a request. {url}");

            // TODO: Check if url
            await telegram.RegisterWebhook(url);

            return new OkObjectResult("");
        }
    }
}
