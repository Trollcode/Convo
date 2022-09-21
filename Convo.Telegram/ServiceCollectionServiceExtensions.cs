using Microsoft.Extensions.DependencyInjection;


namespace Convo.Telegram
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddServerlessTelegramConvo(this IServiceCollection services)
        {
            services.AddSingleton<TelegramContextHandler>();
            return services;
        }
    }
}
