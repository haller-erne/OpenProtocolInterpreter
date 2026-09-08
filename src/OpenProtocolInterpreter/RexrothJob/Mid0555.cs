namespace OpenProtocolInterpreter.RexrothJob
{
    /// <summary>
    /// Rexroth job result data upload
    /// <para>
    ///     The controller reports a job result including the job result number and result value.
    /// </para>
    /// <para>Message sent by: Controller</para>
    /// <para>Answer: <see cref="Mid0556"/> Rexroth job result acknowledge</para>
    /// </summary>
    public class Mid0555 : Mid, IRexrothJob, IController, IAcknowledgeable<Mid0556>
    {
        public const int MID = 555;

        /// <summary>
        /// Job result number (3 digits, 000-999).
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 1, Index = 20, Size = 3)]
        public int JobResultNumber { get; set; }

        /// <summary>
        /// Job result value (single digit).
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 2, Index = 25, Size = 1)]
        public int JobResultValue { get; set; }

        public Mid0555() : this(DEFAULT_REVISION) { }

        public Mid0555(Header header) : base(header) { }

        public Mid0555(int revision) : this(new Header() { Mid = MID, Revision = revision }) { }
    }
}