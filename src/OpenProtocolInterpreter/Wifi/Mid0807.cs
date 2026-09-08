using System.Collections.Generic;

namespace OpenProtocolInterpreter.Wifi
{
    /// <summary>
    /// Reception quality change subscribe
    /// <para>
    ///     Subscribe to reception quality change notifications. After subscription, the controller
    ///     sends <see cref="Mid0808"/> when the reception quality changes by the specified threshold.
    /// </para>
    /// <para>Message sent by: Integrator</para>
    /// <para>
    ///     Answer: <see cref="Communication.Mid0005"/> Command accepted or
    ///     <see cref="Communication.Mid0004"/> Command error, subscription already exists
    /// </para>
    /// </summary>
    public class Mid0807 : Mid, IWifi, IIntegrator, ISubscription, IAcceptableCommand, IDeclinableCommand
    {
        public const int MID = 807;

        public IEnumerable<Error> DocumentedPossibleErrors => new Error[] { Error.SubscriptionAlreadyExists };

        /// <summary>
        /// Change threshold in dBm (00-99). When the quality changes by this amount, a notification is sent.
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 1, Index = 20, Size = 2, HasPrefix = false)]
        public int ChangeLevel { get; set; }

        public Mid0807() : this(DEFAULT_REVISION) { }

        public Mid0807(Header header) : base(header) { }

        public Mid0807(int revision) : this(new Header() { Mid = MID, Revision = revision }) { }
    }
}