﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using WebTask1.Rabbit;

namespace WebTask1.Start
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<IConnection>(provider =>
            {
                ConnectionFactory factory = new ConnectionFactory
                {
                    UserName = "cdkxtrhe",
                    Password = "ALMkgEdwWx2M6ECRhqnnQPsTPgGDpz5U",
                    VirtualHost = "cdkxtrhe",
                    HostName = "sheep-01.rmq.cloudamqp.com"
                };

                return factory.CreateConnection();
            });
            services.AddSingleton<RabbitOperations>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
