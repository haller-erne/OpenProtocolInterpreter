using System.Collections.Generic;

namespace OpenProtocolInterpreter.RexrothJob
{
    /// <summary>
    /// Activate job
    /// <para>
    ///     Enable or disable the job function of the tightening channel.
    ///     Requires the JobEnable signal to be applied to the OP module in the PLC assignment table.
    /// </para>
    /// <para>Message sent by: Integrator</para>
    /// <para>
    ///     Answer: <see cref="Communication.Mid0005"/> Command accepted or
    ///     <see cref="Communication.Mid0004"/> Command error, request timeout or PLC signal not assigned
    /// </para>
    /// </summary>
    public class Mid0570 : Mid, IRexrothJob, IIntegrator, IAcceptableCommand, IDeclinableCommand
    {
        public const int MID = 570;

        public IEnumerable<Error> DocumentedPossibleErrors => new Error[] { Error.ControllerInternalRequestTimeout };

        /// <summary>
        /// <para>Job Deactivated = false (0)</para>
        /// <para>Job Activated = true (1)</para>
        /// </summary>
        [BooleanDataFieldDefinition(revision: 1, field: 1, Index = 20)]
        public bool JobStatus { get; set; }

        public Mid0570() : this(DEFAULT_REVISION) { }

        public Mid0570(Header header) : base(header) { }

        public Mid0570(int revision) : this(new Header() { Mid = MID, Revision = revision }) { }
    }
}