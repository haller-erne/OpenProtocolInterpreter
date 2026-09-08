using System.Collections.Generic;

namespace OpenProtocolInterpreter.Battery
{
    /// <summary>
    /// Battery level changes subscribe
    /// <para>
    ///     Subscribe to battery level change notifications. After subscription, the controller
    ///     sends <see cref="Mid0803"/> when the battery level changes by the specified threshold.
    /// </para>
    /// <para>Message sent by: Integrator</para>
    /// <para>
    ///     Answer: <see cref="Communication.Mid0005"/> Command accepted or
    ///     <see cref="Communication.Mid0004"/> Command error, subscription already exists
    /// </para>
    /// </summary>
    public class Mid0802 : Mid, IBattery, IIntegrator, ISubscription, IAcceptableCommand, IDeclinableCommand
    {
        public const int MID = 802;

        public IEnumerable<Error> DocumentedPossibleErrors => new Error[] { Error.SubscriptionAlreadyExists };

        /// <summary>
        /// Change threshold in percent of maximum capacity (00-99).
        /// When the capacity changes by this amount, a notification is sent.
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 1, Index = 20, Size = 2, HasPrefix = false)]
        public int ChangeLevel { get; set; }

        public Mid0802() : this(DEFAULT_REVISION) { }

        public Mid0802(Header header) : base(header) { }

        public Mid0802(int revision) : this(new Header() { Mid = MID, Revision = revision }) { }
    }
}