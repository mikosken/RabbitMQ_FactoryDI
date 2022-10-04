using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ_FactoryDI.MQFactory
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds the message queue factory to services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMessageQueues(this IServiceCollection services)
        {
            services.AddSingleton<IMessageQueueFactory, MessageQueueFactory>();
            return services;
        }
    }
}