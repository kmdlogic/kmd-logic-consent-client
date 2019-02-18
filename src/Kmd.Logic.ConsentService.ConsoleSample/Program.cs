using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kmd.Logic.ConsentService.ConsoleSample.Client;
using Kmd.Logic.ConsentService.ConsoleSample.Client.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using Serilog;

namespace Kmd.Logic.ConsentService.ConsoleSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build()
                    .Get<AppConfiguration>();

                await Run(config);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Caught a fatal unhandled exception");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static async Task Run(AppConfiguration config)
        {
            if (ValidateConfiguration(config) != ConfigValidity.Valid)
            {
                return;
            }

            Log.Information("Logic environment is {LogicEnvironmentName}", config.LogicEnvironmentName);
            var logicEnvironment = config.LogicEnvironments.FirstOrDefault(e => e.Name == config.LogicEnvironmentName);
            if (logicEnvironment == null)
            {
                Log.Error("No logic environment named {LogicEnvironmentName}", config.LogicEnvironmentName);
                return;
            }
            var subscriptionId = config.LogicAccount.SubscriptionId.Value;
            var client = new ConsentServiceClient(new TokenCredentials(new LogicTokenProvider(config)));
            client.BaseUri = logicEnvironment.ApiRootUri;

            var consentGroup = client.CreateConsentGroup(subscriptionId, new ConsentGroupRequest
            {
                Name = "TestGroup",
                KeyFormat = @"^\d{10}$",
                Scopes = new List<string> { "Scope1", "Scope2" },
                Members = new List<ConsentGroupMemberRequest>
                {
                    new ConsentGroupMemberRequest(
                        "TestMember",
                        "TestMember",
                        subscriptionId, 
                        "all"
                    )
                }
            });

            Log.Information("Created consent group with id {id}", consentGroup.Id);

            var key = "1234567890";
            var consent = client.CreateOrUpdateConsent(subscriptionId, consentGroup.Id.Value, key, new ConsentRequest
            {
                Member = "TestMember",
                Scopes = new List<string> { "Scope1" },
                AuthorizedMembers = new List<string>
                {
                    "TestMember"
                }
            });

            Log.Information("Created consent with id {id}", consent.Id);
        }


        enum ConfigValidity { Valid, Invalid, }

        private static ConfigValidity ValidateConfiguration(AppConfiguration config)
        {
            if (config.LogicAccount == null
                || string.IsNullOrWhiteSpace(config.LogicAccount?.ClientId)
                || string.IsNullOrWhiteSpace(config.LogicAccount?.ClientSecret)
                || config.LogicAccount?.SubscriptionId == null)
            {
                Log.Error("Please add your LogicAccount configuration to `appsettings.json`. You currently have {@LogicAccount}",
                    config.LogicAccount);
                return ConfigValidity.Invalid;
            }

            return ConfigValidity.Valid;
        }
    }
}
