using System;

namespace Kmd.Logic.Consent.Client
{
    /// <summary>
    /// Provide the configuration options for using the CPR service.
    /// </summary>
    public sealed class ConsentOptions
    {
        /// <summary>
        /// Gets or sets the Logic CPR service.
        /// </summary>
        /// <remarks>
        /// This option should not be overridden except for testing purposes.
        /// </remarks>
        public Uri ConsentServiceUri { get; set; } = new Uri("https://gateway.kmdlogic.io/consent/v1");

        /// <summary>
        /// Gets or sets the Logic Subscription.
        /// </summary>
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the Logic Consent Group identifier.
        /// </summary>
        public Guid ConsentGroupId { get; set; }

        /// <summary>
        /// Gets or sets the Logic Consent Member which we are acting on behalf of.
        /// </summary>
        public string ConsentMember { get; set; }
    }
}