using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.Consent.Client.Models;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Rest;

namespace Kmd.Logic.Consent.Client
{
    /// <summary>
    /// Manage distributed consent shared between multiple member systems.
    /// </summary>
    /// <remarks>
    /// To access the consent service you:
    /// - Create a Logic subscription
    /// - Have a client credential issued for the Logic platform
    /// - Create a Consent Group, nominating one or more members with what permissions they have.
    /// </remarks>
    [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "HttpClient is not owned by this class.")]
    public sealed class ConsentClient
    {
        private readonly HttpClient httpClient;
        private readonly ConsentOptions options;
        private readonly LogicTokenProviderFactory tokenProviderFactory;

        private InternalClient internalClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use. The caller is expected to manage this resource and it will not be disposed.</param>
        /// <param name="tokenProviderFactory">The Logic access token provider factory.</param>
        /// <param name="options">The required configuration options.</param>
        public ConsentClient(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, ConsentOptions options)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.tokenProviderFactory = tokenProviderFactory ?? throw new ArgumentNullException(nameof(tokenProviderFactory));
        }

        /// <summary>
        /// Review the complete consent details for the nominated key.
        /// </summary>
        /// <param name="key">The consent key.</param>
        /// <param name="member">The member with review permissions.</param>
        /// <returns>The consent details or null if the key isn't known.</returns>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        /// <exception cref="ConsentValidationException">Invalid consent parameters.</exception>
        /// <exception cref="ConsentConfigurationException">Invalid consent configuration details.</exception>
        public async Task<ConsentInstance> ReviewConsentAsync(string key, string member = null)
        {
            var client = this.CreateClient();

            try
            {
                var response = await client.GetConsentWithHttpMessagesAsync(
                                    subscriptionId: this.options.SubscriptionId,
                                    consentGroupId: this.options.ConsentGroupId,
                                    key: key,
                                    member: member ?? this.options.ConsentMember).ConfigureAwait(false);

                switch (response.Response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        return response.Body as ConsentInstance;

                    case System.Net.HttpStatusCode.NotFound:
                        return null;

                    case System.Net.HttpStatusCode.BadRequest:
                        throw new ConsentValidationException(response.Body as IDictionary<string, IList<string>>);

                    default:
                        throw new ConsentConfigurationException("Invalid configuration provided to access consent service", response.Body as string);
                }
            }
            catch (ValidationException ex)
            {
                throw new ConsentValidationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Get the consent details for the nominated key.
        /// </summary>
        /// <param name="key">The consent key.</param>
        /// <param name="scope">The scope being requested.</param>
        /// <returns>The consent details or null if the key isn't known.</returns>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        /// <exception cref="ConsentValidationException">Invalid consent parameters.</exception>
        /// <exception cref="ConsentConfigurationException">Invalid consent configuration details.</exception>
        public Task<MemberConsentResponse> GetConsentAsync(string key, string scope = null)
        {
            return this.GetConsentForMemberAsync(key, this.options.ConsentMember, string.IsNullOrEmpty(scope) ? null : new[] { scope });
        }

        /// <summary>
        /// Get the consent details for the nominated key.
        /// </summary>
        /// <param name="key">The consent key.</param>
        /// <param name="scopes">The scopes being requested. Consent must be granted for at least one of the nominated scopes.</param>
        /// <returns>The consent details or null if the key isn't known.</returns>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        /// <exception cref="ConsentValidationException">Invalid consent parameters.</exception>
        /// <exception cref="ConsentConfigurationException">Invalid consent configuration details.</exception>
        public Task<MemberConsentResponse> GetConsentAsync(string key, ICollection<string> scopes)
        {
            return this.GetConsentForMemberAsync(key, this.options.ConsentMember, scopes);
        }

        /// <summary>
        /// Get the consent details for the nominated key.
        /// </summary>
        /// <param name="key">The consent key.</param>
        /// <param name="member">The consent member we are requesting on behalf of.</param>
        /// <param name="scope">The scope being requested.</param>
        /// <returns>The consent details or null if the key isn't known.</returns>
        /// <exception cref="ValidationException">Missing key.</exception>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        /// <exception cref="ConsentValidationException">Invalid consent parameters.</exception>
        /// <exception cref="ConsentConfigurationException">Invalid consent configuration details.</exception>
        public Task<MemberConsentResponse> GetConsentForMemberAsync(string key, string member, string scope = null)
        {
            return this.GetConsentForMemberAsync(key, member, string.IsNullOrEmpty(scope) ? null : new[] { scope });
        }

        /// <summary>
        /// Get the consent details for the nominated key.
        /// </summary>
        /// <param name="key">The consent key.</param>
        /// <param name="member">The consent member we are requesting on behalf of.</param>
        /// <param name="scopes">The scopes being requested. Consent must be granted for at least one of the nominated scopes.</param>
        /// <returns>The consent details or null if the key isn't known.</returns>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        /// <exception cref="ConsentValidationException">Invalid consent parameters.</exception>
        /// <exception cref="ConsentConfigurationException">Invalid consent configuration details.</exception>
        public async Task<MemberConsentResponse> GetConsentForMemberAsync(string key, string member, ICollection<string> scopes = null)
        {
            var client = this.CreateClient();

            try
            {
                string scopesList = null;
                if (scopes != null && scopes.Count > 0)
                {
                    scopesList = string.Join(" ", scopes);
                }

                var response = await client.GetMemberConsentWithHttpMessagesAsync(
                                    subscriptionId: this.options.SubscriptionId,
                                    consentGroupId: this.options.ConsentGroupId,
                                    key: key,
                                    member: member,
                                    scopes: scopesList).ConfigureAwait(false);

                switch (response.Response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        return response.Body;

                    case System.Net.HttpStatusCode.NotFound:
                        return null;

                    default:
                        throw new ConsentConfigurationException("Invalid configuration provided to access consent service");
                }
            }
            catch (ValidationException ex)
            {
                throw new ConsentValidationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Set the consent details for the nominated key.
        /// </summary>
        /// <param name="key">The consent key.</param>
        /// <param name="scopes">The consent scopes being granted. If null, all scopes are granted.</param>
        /// <param name="authorizedMembers">The members authorized to access this consent. If null, all members are authorized.</param>
        /// <returns>The new consent details.</returns>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        /// <exception cref="ConsentValidationException">Invalid consent request.</exception>
        /// <exception cref="ConsentConfigurationException">Invalid consent configuration details.</exception>
        public Task SaveConsentAsync(string key, IList<string> scopes = null, IList<string> authorizedMembers = null)
        {
            return this.SaveConsentAsync(key, this.options.ConsentMember, scopes, authorizedMembers);
        }

        /// <summary>
        /// Set the consent details for the nominated key.
        /// </summary>
        /// <param name="key">The consent key.</param>
        /// <param name="member">The consent member we are updating on behalf of.</param>
        /// <param name="scopes">The consent scopes being granted. If null, all scopes are granted.</param>
        /// <param name="authorizedMembers">The members authorized to access this consent. If null, all members are authorized.</param>
        /// <returns>The new consent details.</returns>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        /// <exception cref="ConsentValidationException">Invalid consent request.</exception>
        /// <exception cref="ConsentConfigurationException">Invalid consent configuration details.</exception>
        public async Task<ConsentInstance> SaveConsentAsync(string key, string member, IList<string> scopes = null, IList<string> authorizedMembers = null)
        {
            var client = this.CreateClient();

            try
            {
                var request = new ConsentRequest
                {
                    Member = member,
                    Scopes = scopes,
                    AuthorizedMembers = authorizedMembers,
                };

                var response = await client.SaveConsentWithHttpMessagesAsync(
                                    subscriptionId: this.options.SubscriptionId,
                                    consentGroupId: this.options.ConsentGroupId,
                                    key: key,
                                    request: request).ConfigureAwait(false);

                switch (response.Response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                    case System.Net.HttpStatusCode.Created:
                        return (ConsentInstance)response.Body;

                    case System.Net.HttpStatusCode.BadRequest:
                        throw new ConsentValidationException(response.Body as IDictionary<string, IList<string>>);

                    default:
                        throw new ConsentConfigurationException("Invalid configuration provided to access consent service", response.Body as string);
                }
            }
            catch (ValidationException ex)
            {
                throw new ConsentValidationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Delete (revoke) all consent for a given key.
        /// </summary>
        /// <param name="key">The consent key.</param>
        /// <param name="member">The consent member we are updating on behalf of.</param>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        /// <exception cref="ConsentValidationException">Invalid consent request.</exception>
        /// <exception cref="ConsentConfigurationException">Invalid consent configuration details.</exception>
        /// <returns>True if consent was revoked, false if there was no prior consent given for the key.</returns>
        public async Task<bool> DeleteConsent(string key, string member = null)
        {
            var client = this.CreateClient();

            try
            {
                var response = await client.DeleteConsentWithHttpMessagesAsync(
                    subscriptionId: this.options.SubscriptionId,
                    consentGroupId: this.options.ConsentGroupId,
                    key: key,
                    member: member ?? this.options.ConsentMember).ConfigureAwait(false);

                switch (response.Response.StatusCode)
                {
                    case System.Net.HttpStatusCode.NoContent:
                        return true;

                    case System.Net.HttpStatusCode.NotFound:
                        return false;

                    default:
                        throw new ConsentConfigurationException("Invalid configuration provided to access consent service");
                }
            }
            catch (ValidationException ex)
            {
                throw new ConsentValidationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Get the consent groups for the Logic subscription.
        /// </summary>
        /// <remarks>
        /// Only consent groups managed by this subscription will be reported, not groups you are a member of.
        /// </remarks>
        /// <returns>The list of consent groups.</returns>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        public async Task<IList<ConsentGroupListResponse>> GetAllConsentGroupsAsync()
        {
            var client = this.CreateClient();

            return await client.GetAllConsentGroupsAsync(subscriptionId: this.options.SubscriptionId).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new consent group, nominating who may participate.
        /// </summary>
        /// <param name="consentGroupId">The consent group to access.</param>
        /// <returns>The consent group details.</returns>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        /// <exception cref="ConsentConfigurationException">Invalid consent configuration details.</exception>
        public async Task<ConsentGroupResponse> GetConsentGroupAsync(Guid consentGroupId)
        {
            var client = this.CreateClient();

            var response = await client.GetConsentGroupWithHttpMessagesAsync(
                subscriptionId: this.options.SubscriptionId,
                consentGroupId: consentGroupId).ConfigureAwait(false);

            switch (response.Response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    return response.Body;

                case System.Net.HttpStatusCode.NotFound:
                    return null;

                default:
                    throw new ConsentConfigurationException("Invalid configuration provided to access consent service");
            }
        }

        /// <summary>
        /// Creates a new consent group, nominating who may participate.
        /// </summary>
        /// <param name="name">The name of the consent group.</param>
        /// <param name="members">The members of the group and what they are authorized to do.</param>
        /// <param name="keyFormat">A regular expression the consent key must conform to.</param>
        /// <param name="scopes">The consent scopes.</param>
        /// <returns>The created consent group.</returns>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        /// <exception cref="ConsentValidationException">Invalid request details.</exception>
        /// <exception cref="ConsentConfigurationException">Invalid consent configuration details.</exception>
        public async Task<ConsentGroupResponse> CreateConsentGroup(string name, IList<ConsentGroupMemberRequest> members, string keyFormat = null, IList<string> scopes = null)
        {
            var client = this.CreateClient();

            var request = new ConsentGroupRequest
            {
                Name = name,
                KeyFormat = keyFormat,
                Members = members,
                Scopes = scopes,
            };

            var response = await client.CreateConsentGroupWithHttpMessagesAsync(
                subscriptionId: this.options.SubscriptionId,
                request: request).ConfigureAwait(false);

            switch (response.Response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                case System.Net.HttpStatusCode.Created:
                    return (ConsentGroupResponse)response.Body;

                case System.Net.HttpStatusCode.BadRequest:
                    throw new ConsentValidationException(response.Body as IDictionary<string, IList<string>>);

                default:
                    throw new ConsentConfigurationException("Invalid configuration provided to access consent service", response.Body as string);
            }
        }

        /// <summary>
        /// Updates the consent group, nominating who may participate.
        /// </summary>
        /// <param name="consentGroupId">The consent group to update.</param>
        /// <param name="name">The name of the consent group.</param>
        /// <param name="members">The members of the group and what they are authorized to do.</param>
        /// <param name="keyFormat">A regular expression the consent key must conform to.</param>
        /// <param name="scopes">The consent scopes.</param>
        /// <returns>The created consent group.</returns>
        /// <exception cref="SerializationException">Unable process the service response.</exception>
        /// <exception cref="LogicTokenProviderException">Unable to issue an authorization token.</exception>
        /// <exception cref="KeyNotFoundException">The consent group does not exist.</exception>
        /// <exception cref="ConsentValidationException">Invalid request details.</exception>
        /// <exception cref="ConsentConfigurationException">Invalid consent configuration details.</exception>
        public async Task<ConsentGroupResponse> UpdateConsentGroup(Guid consentGroupId, string name, IList<ConsentGroupMemberRequest> members, string keyFormat = null, IList<string> scopes = null)
        {
            var client = this.CreateClient();

            var request = new ConsentGroupRequest
            {
                Name = name,
                KeyFormat = keyFormat,
                Members = members,
                Scopes = scopes,
            };

            var response = await client.UpdateConsentGroupWithHttpMessagesAsync(
                subscriptionId: this.options.SubscriptionId,
                consentGroupId: consentGroupId,
                request: request).ConfigureAwait(false);

            switch (response.Response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                case System.Net.HttpStatusCode.Created:
                    return (ConsentGroupResponse)response.Body;

                case System.Net.HttpStatusCode.NotFound:
                    throw new KeyNotFoundException($"The consent group {name} does not exist");

                case System.Net.HttpStatusCode.BadRequest:
                    throw new ConsentValidationException(response.Body as IDictionary<string, IList<string>>);

                case System.Net.HttpStatusCode.Unauthorized:
                    throw new ConsentConfigurationException($"You do not have permission to manage the consent group {name}");

                default:
                    throw new ConsentConfigurationException("Invalid configuration provided to access consent service", response.Body as string);
            }
        }

        private InternalClient CreateClient()
        {
            if (this.internalClient != null)
            {
                return this.internalClient;
            }

            var tokenProvider = this.tokenProviderFactory.GetProvider(this.httpClient);

            this.internalClient = new InternalClient(new TokenCredentials(tokenProvider))
            {
                BaseUri = this.options.ConsentServiceUri,
            };

            return this.internalClient;
        }
    }
}