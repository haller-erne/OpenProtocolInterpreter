using System.Collections.Generic;

namespace OpenProtocolInterpreter.RexrothJob
{
    /// <summary>
    /// Select job number
    /// <para>
    ///     Select a job by its number on the tightening channel.
    ///     Requires JobEnable, JobStart, and Job0-7 signals to be applied to the OP module.
    /// </para>
    /// <para>Message sent by: Integrator</para>
    /// <para>
    ///     Answer: <see cref="Communication.Mid0005"/> Command accepted or
    ///     <see cref="Communication.Mid0004"/> Command error, request timeout or PLC signal not assigned
    /// </para>
    /// </summary>
    public class Mid0573 : Mid, IRexrothJob, IIntegrator, IAcceptableCommand, IDeclinableCommand
    {
        public const int MID = 573;

        public IEnumerable<Error> DocumentedPossibleErrors => new Error[] { Error.ControllerInternalRequestTimeout };

        /// <summary>
        /// Job number to select (000-999).
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 1, Index = 20, Size = 3, HasPrefix = false)]
        public int JobNumber { get; set; }

        public Mid0573() : this(DEFAULT_REVISION) { }

        public Mid0573(Header header) : base(header) { }

        public Mid0573(int revision) : this(new Header() { Mid = MID, Revision = revision }) { }
    }
}