using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using WebTask1.Config;
using WebTask1.Messaging;
using WebTask1.RabbitMQ;
using WebTask1.RabbitMQMessaging;
using WebTask1.Storages;

namespace WebTask1.Start
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<IConnection>(provider =>
            {
                ConnectionFactory factory = new ConnectionFactory
                {
                    UserName = Configuration["RabbitMQ:UserName"],
                    Password = Configuration["RabbitMQ:Password"],
                    VirtualHost = Configuration["RabbitMQ:VirtualHost"],
                    HostName = Configuration["RabbitMQ:HostName"]
                };

                return factory.CreateConnection();
            });

            services.AddSingleton<RabbitMQSend>();

            services.AddSingleton<TransactionStorage>();

            services.AddSingleton<IHostedService, RabbitMQReceiver>();
            services.AddSingleton<IHostedService, RabbitServer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();

            //app.ApplicationServices.GetRequiredService<RabbitServer>(); Maybe
        }
    }
}
