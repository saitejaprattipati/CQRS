//using Microsoft.Azure.ServiceBus;
using Author.Command.Persistence;
using Author.Command.Persistence.DBContextAggregate;
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
//using NJsonSchema;
//using NSwag.AspNetCore;
using AuthorAdmin.Command.API.ExceptionMiddleware;
using Author.Core.Framework;
using Author.Core.Framework.Utilities;
using NetCore.AutoRegisterDi;
using AutoMapper;
using Author.Command.Service.Mapping;
using Author.Core.Services.BlobStorage;
using Author.Core.Services.BlobStorage.Interfaces;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Linq;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.ApplicationInsights.Extensibility;
using AuthorAdmin.Command.API.Telemetry;

namespace AuthorAdmin.Command.API
{
    public class Startup
    {
        private IHostingEnvironment CurrentEnvironment { get; set; }

        public Startup(IHostingEnvironment env,IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme).AddAzureADBearer(options => Configuration.Bind("AzureActiveDirectory", options));

            services.Configure<AuthorConfigurationSettings>(Configuration);
            ConfigureCSRFValidationByEnvironment(services);
            services.AddCors((options => { options.AddPolicy("FrontEnd", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials()); }));
            services.AddSingleton<ITelemetryInitializer, AppinsightsTelemetry>();
            //  services.AddMediatR(CreateUserCommandHandler);
            services.AddApplicationInsightsTelemetry();
            services.ConfigureTelemetryModule<QuickPulseTelemetryModule>((module, o) => module.AuthenticationApiKey = "66f5037c-21cc-4736-97ef-6d065ec25c12");

            services.AddMediatR(typeof(CreateArticleCommandHandler).GetTypeInfo().Assembly);
            services.AddTransient<IIntegrationEventPublisherServiceService, IntegrationEventPublisherService>();
            services.AddTransient<IIntegrationEventBlobService, IntegrationEventBlobService>();
            services.AddTransient<IUtilityService, UtilityService>();
            services.RegisterAssemblyPublicNonGenericClasses(
              Assembly.GetExecutingAssembly())
        .Where(c => c.Name.EndsWith("Persistence"))
        .AsPublicImplementedInterfaces();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Taxathand AuthorAdminAPI",
                    Description = "Taxathand AuthorAdminAPI",
                    TermsOfService = "None",
                    Contact = new Contact() { Name = "tax@hand Author", Email = "prteja@deloitte.com", Url = "" }
                });

                ////Locate the XML file being generated by ASP.NET...
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                //var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
            });
            services.ConfigureSwaggerGen(options =>
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                if (System.IO.File.Exists(basePath + "AuthorAdmin.Command.API.XML"))
                {
                    options.IncludeXmlComments(basePath + "AuthorAdmin.Command.API.XML");
                }
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

            services.Configure<JwtBearerOptions>(AzureADDefaults.JwtBearerAuthenticationScheme, options =>
            {
                // This is an Azure AD v2.0 Web API
                options.Authority += "/v2.0";

                // The valid audiences are both the Client ID (options.Audience) and api://{ClientID}
                options.TokenValidationParameters.ValidAudiences = new string[] { options.Audience, $"api://{options.Audience}" };

                // Instead of using the default validation (validating against a single tenant, as we do in line of business apps),
                // we inject our own multitenant validation logic (which even accepts both V1 and V2 tokens)
                options.TokenValidationParameters.IssuerValidator = AadIssuerValidator.ValidateAadIssuer;
            });
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            //  services.AddDbContext<TaxatHand_StgContext>(options => options.UseSqlServer(connection));
            AddEventing(services);
            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsTelemetry();
        }

        private void ConfigureCSRFValidationByEnvironment(IServiceCollection services)
        {
            if (CurrentEnvironment.EnvironmentName.Equals(EnvironmentName.Development))
            {
                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Latest).ConfigureApiBehaviorOptions(op =>
                    {
                        op.SuppressUseValidationProblemDetailsForInvalidModelStateResponses = true;
                    });
            }
            else
            {
                services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
                    .SetCompatibilityVersion(CompatibilityVersion.Latest).ConfigureApiBehaviorOptions(op =>
                    {
                        op.SuppressUseValidationProblemDetailsForInvalidModelStateResponses = true;
                    });

                // Angular's default header name for sending the XSRF token.
                services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            }
        }

        private void AddEventing(IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            var connection = Configuration.GetValue<string>("ServiceBusConnection");
            var connectionBlob = Configuration.GetValue<string>("BlobConnection");
            if (string.IsNullOrWhiteSpace(connection))
            {
                //    _logger.LogError("Error configuring eventing. Service Bus connection settings are missing");
            }
            else
            {
                //  _logger.LogInformation($"Retrieved Service Bus connection settings");
            }




            services.AddSingleton<IBlobConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<BlobConnection>>();    
                return new BlobConnection(connectionBlob);
            });
            services.AddSingleton<IEventStorage, EventsBlobStorage>(sp =>
            {
                var blobStoragePersisterConnection = sp.GetRequiredService<IBlobConnection>();
                var logger = sp.GetRequiredService<ILogger<EventsBlobStorage>>();

                return new EventsBlobStorage(blobStoragePersisterConnection, logger);
            });




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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IUtilityService utilityService, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddFile("Logs/AuthorAdmin-{Date}.txt");
            }
            else
            {
           //     app.UseHsts();
            }

          //  app.UseHttpsRedirection();
            //app.UseSwagger();
            //app.UseSwaggerUi3();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Taxathand AuthorAdminAPI");
            });

            app.ConfigureExceptionHandler(utilityService);
            app.UseMvc();
            app.UseCors("FrontEnd");
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
        }
    }
}
