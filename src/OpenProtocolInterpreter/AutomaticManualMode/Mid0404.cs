namespace OpenProtocolInterpreter.AutomaticManualMode
{
    /// <summary>
    /// Select automatic/manual mode
    /// <para>
    ///     The operating mode is changed. This is a Rexroth/NEXO vendor extension (FW ≥1300).
    /// </para>
    /// <para>Message sent by: Integrator</para>
    /// <para>
    ///     Answer: <see cref="Communication.Mid0005"/> Command accepted
    /// </para>
    /// </summary>
    public class Mid0404 : Mid, IAutomaticManualMode, IIntegrator, IAcceptableCommand
    {
        public const int MID = 404;

        /// <summary>
        /// <para>Automatic Mode = false (0)</para>
        /// <para>Manual Mode = true (1)</para>
        /// </summary>
        [BooleanDataFieldDefinition(revision: 1, field: 1, Index = 20, HasPrefix = false)]
        public bool ManualAutomaticMode { get; set; }

        public Mid0404() : this(DEFAULT_REVISION) { }

        public Mid0404(Header header) : base(header)
        {
        }

        public Mid0404(int revision) : this(new Header() { Mid = MID, Revision = revision })
        {
        }
    }
}