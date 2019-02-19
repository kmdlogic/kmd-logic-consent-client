// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Kmd.Logic.ConsentService.ConsoleSample.Client
{
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for ConsentServiceClient.
    /// </summary>
    public static partial class ConsentServiceClientExtensions
    {
            /// <summary>
            /// Get the complete details of consent
            /// </summary>
            /// <remarks>
            /// The requesting member must have both Read and Write permissions to use this
            /// method.
            ///
            /// For a member to get consent details for operational use, use the route:
            /// subscriptions/{subscriptionId}/consents/{consentGroupId}/{key}/{member}
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription with access to the consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group being requested
            /// </param>
            /// <param name='key'>
            /// The consent key being requested
            /// </param>
            /// <param name='member'>
            /// The consent group member requesting access
            /// </param>
            public static object GetConsentDetails(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId, string key, string member)
            {
                return operations.GetConsentDetailsAsync(subscriptionId, consentGroupId, key, member).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get the complete details of consent
            /// </summary>
            /// <remarks>
            /// The requesting member must have both Read and Write permissions to use this
            /// method.
            ///
            /// For a member to get consent details for operational use, use the route:
            /// subscriptions/{subscriptionId}/consents/{consentGroupId}/{key}/{member}
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription with access to the consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group being requested
            /// </param>
            /// <param name='key'>
            /// The consent key being requested
            /// </param>
            /// <param name='member'>
            /// The consent group member requesting access
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> GetConsentDetailsAsync(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId, string key, string member, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetConsentDetailsWithHttpMessagesAsync(subscriptionId, consentGroupId, key, member, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Create or update the details of consent
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription with access to the consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group to update
            /// </param>
            /// <param name='key'>
            /// The consent key being updated
            /// </param>
            /// <param name='request'>
            /// The details of consent being granted
            /// </param>
            public static object CreateOrUpdateConsent(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId, string key, ConsentRequest request)
            {
                return operations.CreateOrUpdateConsentAsync(subscriptionId, consentGroupId, key, request).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Create or update the details of consent
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription with access to the consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group to update
            /// </param>
            /// <param name='key'>
            /// The consent key being updated
            /// </param>
            /// <param name='request'>
            /// The details of consent being granted
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> CreateOrUpdateConsentAsync(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId, string key, ConsentRequest request, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateOrUpdateConsentWithHttpMessagesAsync(subscriptionId, consentGroupId, key, request, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Revoke consent
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription with access to the consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group to revoked
            /// </param>
            /// <param name='key'>
            /// The consent key being revoked
            /// </param>
            /// <param name='member'>
            /// The consent group member revoking the consent
            /// </param>
            public static void RevokeConsent(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId, string key, string member)
            {
                operations.RevokeConsentAsync(subscriptionId, consentGroupId, key, member).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Revoke consent
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription with access to the consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group to revoked
            /// </param>
            /// <param name='key'>
            /// The consent key being revoked
            /// </param>
            /// <param name='member'>
            /// The consent group member revoking the consent
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task RevokeConsentAsync(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId, string key, string member, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.RevokeConsentWithHttpMessagesAsync(subscriptionId, consentGroupId, key, member, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Get the details of consent granted to the member
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription with access to the consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group to fetch
            /// </param>
            /// <param name='key'>
            /// The consent being requested
            /// </param>
            /// <param name='member'>
            /// The member retrieving consent
            /// </param>
            /// <param name='scopes'>
            /// An optional space separated list of scopes being requested. If specified,
            /// only these scopes are returned.
            /// </param>
            public static MemberConsentResponse GetMemberConsent(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId, string key, string member, string scopes = default(string))
            {
                return operations.GetMemberConsentAsync(subscriptionId, consentGroupId, key, member, scopes).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get the details of consent granted to the member
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription with access to the consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group to fetch
            /// </param>
            /// <param name='key'>
            /// The consent being requested
            /// </param>
            /// <param name='member'>
            /// The member retrieving consent
            /// </param>
            /// <param name='scopes'>
            /// An optional space separated list of scopes being requested. If specified,
            /// only these scopes are returned.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<MemberConsentResponse> GetMemberConsentAsync(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId, string key, string member, string scopes = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetMemberConsentWithHttpMessagesAsync(subscriptionId, consentGroupId, key, member, scopes, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get all consent groups managed by the subscription
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription which owns the consent group
            /// </param>
            public static IList<ConsentGroupListResponse> GetConsentGroups(this IConsentServiceClient operations, System.Guid subscriptionId)
            {
                return operations.GetConsentGroupsAsync(subscriptionId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get all consent groups managed by the subscription
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription which owns the consent group
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<ConsentGroupListResponse>> GetConsentGroupsAsync(this IConsentServiceClient operations, System.Guid subscriptionId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetConsentGroupsWithHttpMessagesAsync(subscriptionId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Create a new consent group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription responsible for managing this consent group
            /// </param>
            /// <param name='request'>
            /// The details of the consent group being created
            /// </param>
            public static object CreateConsentGroup(this IConsentServiceClient operations, System.Guid subscriptionId, ConsentGroupRequest request = default(ConsentGroupRequest))
            {
                return operations.CreateConsentGroupAsync(subscriptionId, request).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Create a new consent group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription responsible for managing this consent group
            /// </param>
            /// <param name='request'>
            /// The details of the consent group being created
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> CreateConsentGroupAsync(this IConsentServiceClient operations, System.Guid subscriptionId, ConsentGroupRequest request = default(ConsentGroupRequest), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateConsentGroupWithHttpMessagesAsync(subscriptionId, request, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get the details of consent group managed by the subscription
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription which owns the consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group to fetch
            /// </param>
            public static ConsentGroup GetConsentGroupDetail(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId)
            {
                return operations.GetConsentGroupDetailAsync(subscriptionId, consentGroupId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get the details of consent group managed by the subscription
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription which owns the consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group to fetch
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ConsentGroup> GetConsentGroupDetailAsync(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetConsentGroupDetailWithHttpMessagesAsync(subscriptionId, consentGroupId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Update an existing consent group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription responsible for managing this consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group to update
            /// </param>
            /// <param name='request'>
            /// The details of the consent group being updated
            /// </param>
            public static object UpdateConsentGroup(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId, ConsentGroupRequest request = default(ConsentGroupRequest))
            {
                return operations.UpdateConsentGroupAsync(subscriptionId, consentGroupId, request).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Update an existing consent group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='subscriptionId'>
            /// The subscription responsible for managing this consent group
            /// </param>
            /// <param name='consentGroupId'>
            /// The consent group to update
            /// </param>
            /// <param name='request'>
            /// The details of the consent group being updated
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> UpdateConsentGroupAsync(this IConsentServiceClient operations, System.Guid subscriptionId, System.Guid consentGroupId, ConsentGroupRequest request = default(ConsentGroupRequest), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UpdateConsentGroupWithHttpMessagesAsync(subscriptionId, consentGroupId, request, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}