using Convo.Abstractions;
using Convo.Telegram.Example;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Convo.Telegram.Example
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddServerlessTelegramConvo();
            builder.Services.AddTransient<IConvoContextStorage, TablestorageContextStorage>();
        }
    }
}
