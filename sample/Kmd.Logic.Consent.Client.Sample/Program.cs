using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Consent.Client.Models;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Kmd.Logic.Consent.Client.Sample
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            InitLogger();

            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddUserSecrets(typeof(Program).Assembly)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build()
                    .Get<AppConfiguration>();

                await Run(config).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                Log.Fatal(ex, "Caught a fatal unhandled exception");
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void InitLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static async Task Run(AppConfiguration configuration)
        {
            var validator = new ConfigurationValidator(configuration);
            if (!validator.Validate())
            {
                return;
            }

            using (var httpClient = new HttpClient())
            using (var tokenProviderFactory = new LogicTokenProviderFactory(configuration.TokenProvider))
            {
                var consentClient = new ConsentClient(httpClient, tokenProviderFactory, configuration.Consent);

                var groups = await consentClient.GetAllConsentGroupsAsync().ConfigureAwait(false);

                if (configuration.Consent.ConsentGroupId == Guid.Empty)
                {
                    if (groups != null && groups.Count > 0)
                    {
                        if (groups.Count > 1)
                        {
                            Log.Error("There is more than one consent group defined for this subscription");
                            return;
                        }
                        else
                        {
                            configuration.Consent.ConsentGroupId = groups[0].Id.Value;
                        }
                    }
                    else
                    {
                        var memberTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(configuration.Consent.ConsentMember);

                        var scopes = string.IsNullOrEmpty(configuration.ConsentScope)
                                        ? null
                                        : new List<string> { configuration.ConsentScope };

                        var newGroup = await consentClient.CreateConsentGroup(
                            $"{memberTitle} Sample Group",
                            new List<ConsentGroupMemberRequest>
                            {
                                new ConsentGroupMemberRequest
                                {
                                    Key = configuration.Consent.ConsentMember,
                                    Name = memberTitle,
                                    SubscriptionId = configuration.Consent.SubscriptionId,
                                    Roles = new ConsentRolesRequestResponse
                                    {
                                        CanRead = true,
                                        CanWrite = true,
                                        CanDelete = true,
                                    },
                                },
                            },
                            configuration.ConsentKeyFormat,
                            scopes).ConfigureAwait(false);

                        Log.Information("Created consent group {Name} with id {Id}", newGroup.Name, newGroup.Id);

                        configuration.Consent.ConsentGroupId = newGroup.Id.Value;
                    }
                }

                var ownedGroup = groups.FirstOrDefault(x => x.Id == configuration.Consent.ConsentGroupId);
                if (ownedGroup != null)
                {
                    Log.Information(
                        "Consent Group {Id}, {Name} is managed by subscription {SubscriptionId}",
                        ownedGroup.Id,
                        ownedGroup.Name,
                        configuration.Consent.SubscriptionId);

                    var groupDetails = await consentClient.GetConsentGroupAsync(configuration.Consent.ConsentGroupId).ConfigureAwait(false);

                    Log.Information("Consent group data: {@Details}", groupDetails);
                }

                Log.Information("Fetching consent for {Key}", configuration.ConsentKey);

                var details = await consentClient.GetConsentAsync(configuration.ConsentKey, configuration.ConsentScope).ConfigureAwait(false);

                Log.Information("Consent data: {@Details}", details);

                if (details == null)
                {
                    await consentClient.SaveConsentAsync(configuration.ConsentKey).ConfigureAwait(false);

                    Log.Information("Created new consent for key {Key}", configuration.ConsentKey);
                }

                if (ownedGroup != null)
                {
                    var consentReview = await consentClient.ReviewConsentAsync(configuration.ConsentKey).ConfigureAwait(false);

                    if (consentReview != null)
                    {
                        Log.Information("Found consent definition {@Definition}", consentReview);
                    }
                    else
                    {
                        Log.Error("Unable to find consent details for key {Key}", configuration.ConsentKey);
                    }
                }

                var deleted = await consentClient.DeleteConsent(configuration.ConsentKey).ConfigureAwait(false);
                if (deleted)
                {
                    Log.Information("Consent for key {Key} was revoked", configuration.ConsentKey);
                }
                else
                {
                    Log.Error("Unable to revoke consent details for key {Key}", configuration.ConsentKey);
                }

                if (ownedGroup != null)
                {
                    var consentReview = await consentClient.ReviewConsentAsync(configuration.ConsentKey).ConfigureAwait(false);

                    if (consentReview != null)
                    {
                        Log.Error("Unexpectedly found consent definition {@Definition}", consentReview);
                    }
                    else
                    {
                        Log.Information("Consent details for key {Key} not longer exist, as expected", configuration.ConsentKey);
                    }
                }
            }
        }
    }
}