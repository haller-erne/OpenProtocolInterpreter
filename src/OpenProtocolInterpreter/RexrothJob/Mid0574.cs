namespace OpenProtocolInterpreter.RexrothJob
{
    /// <summary>
    /// Job manipulate (abort/increment/decrement)
    /// <para>
    ///     Manipulate the current job: abort, increment, or decrement.
    /// </para>
    /// <para>Message sent by: Integrator</para>
    /// <para>Answer: None</para>
    /// </summary>
    public class Mid0574 : Mid, IRexrothJob, IIntegrator
    {
        public const int MID = 574;

        /// <summary>
        /// Action code:
        /// <para>01 = Abort job</para>
        /// <para>02 = Increment job</para>
        /// <para>03 = Decrement job</para>
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 1, Index = 20, Size = 2)]
        public int ActionCode { get; set; }

        public Mid0574() : this(DEFAULT_REVISION) { }

        public Mid0574(Header header) : base(header) { }

        public Mid0574(int revision) : this(new Header() { Mid = MID, Revision = revision }) { }
    }
}