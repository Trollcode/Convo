﻿using Microsoft.Extensions.DependencyInjection;


namespace Convo.Telegram
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddKafka(this IServiceCollection services)
        {

            return services;
        }
    }
}