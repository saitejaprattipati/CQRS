//using Microsoft.Azure.ServiceBus;
using Author.Command.Persistence;
using Author.Command.Persistence.Author.Command.API.ArticleAggregate;
using Author.Command.Service;
using Author.Core.Services.EventBus;
using Author.Core.Services.EventBus.Azure;
using Author.Core.Services.EventBus.Interfaces;
using Author.Core.Services.EventBus.RabbitMQ;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Reflection;
using NJsonSchema;
using NSwag.AspNetCore;

namespace AuthorAdmin.Command.API
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
            services.Configure<AuthorConfigurationSettings>(Configuration);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors();
            ///   services.AddCorrelationId();
            services.AddMediatR(typeof(CreateArticleCommandHandler).GetTypeInfo().Assembly);
            services.AddTransient<IIntegrationEventPublisherServiceService, IntegrationEventPublisherService>();
            services.AddTransient<CreateArticleCommandHandler>();
            services.AddTransient<IArticleRepository, ArticleRepository>();
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Taxathand API";
                    document.Info.Description = "ASP.NET Core web API";
                    document.Info.TermsOfService = "None";
                };
            });

            services.AddEntityFrameworkSqlServer()
                  .AddDbContext<TaxatHand_StgContext>(options =>
                  {
                      options.UseSqlServer(Configuration["ConnectionString"],
                            sqlServerOptionsAction: sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                            });
                  },
                      ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                  );




            //  services.AddDbContext<TaxatHand_StgContext>(options => options.UseSqlServer(connection));
            AddEventing(services);
        }

        private void AddEventing(IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            var connection = Configuration.GetValue<string>("ServiceBusConnection");
            if (string.IsNullOrWhiteSpace(connection))
            {
                //    _logger.LogError("Error configuring eventing. Service Bus connection settings are missing");
            }
            else
            {
                //  _logger.LogInformation($"Retrieved Service Bus connection settings");
            }

            // Configure Service Bus Provider - RabbitMq or Azure Service Bus based on environment variables
            if (Configuration.GetValue<bool>("UseAzureServiceBus"))
            {
                //   _logger.LogInformation($"Configuring Azure service bus");

                services.AddSingleton<IServiceBusPersisterConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();
                    var serviceBusConnection = new ServiceBusConnectionStringBuilder(connection);

                    return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
                });

                services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
                {
                    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                    var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                    var subscriptionClientName = Configuration["SubscriptionClientName"];

                    return new EventBusServiceBus(serviceBusPersisterConnection, logger, eventBusSubcriptionsManager, subscriptionClientName, sp);
                });
            }
            else
            {
                //  _logger.LogInformation($"Configuring Rabbitmq");

                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var settings = sp.GetRequiredService<IOptions<AuthorConfigurationSettings>>().Value;
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = settings.ServiceBusConnection //"my-rabbit"
                    };
                    //if (!string.IsNullOrEmpty(settings.ServiceBusUserName))
                    //{
                    //    factory.UserName = settings.ServiceBusUserName;
                    //}

                    //if (!string.IsNullOrEmpty(settings.ServiceBusUserPassword))
                    //{
                    //    factory.Password = settings.ServiceBusUserPassword;
                    //}
                    var retryCount = 5;

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });

                services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                {
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    var retryCount = 5;

                    return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, eventBusSubcriptionsManager, sp, retryCount);
                });
            }

            //   _logger.LogInformation($"Finished configuring service bus");

            // Register Event Handlers
            //EX:


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUi3();
            app.UseMvc();
        }
    }
}
