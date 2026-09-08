namespace OpenProtocolInterpreter.Battery
{
    /// <summary>
    /// Battery level changes upload
    /// <para>
    ///     The controller reports the current battery level when it changes.
    ///     Same data format as <see cref="Mid0801"/>.
    /// </para>
    /// <para>Message sent by: Controller</para>
    /// <para>Answer: None</para>
    /// </summary>
    public class Mid0803 : Mid, IBattery, IController
    {
        public const int MID = 803;

        /// <summary>
        /// Battery pack capacity in percent (000-100).
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 1, Index = 20, Size = 3)]
        public int Capacity { get; set; }

        /// <summary>
        /// State of the battery pack:
        /// <para>0 = Battery pack not inserted</para>
        /// <para>1 = Battery level critical (system shutdown)</para>
        /// <para>2 = Battery insufficient for tightening</para>
        /// <para>3 = Battery level okay</para>
        /// <para>4 = Battery reinserted (checking charge)</para>
        /// <para>5 = Battery warning level reached</para>
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 2, Index = 25, Size = 1)]
        public int State { get; set; }

        public Mid0803() : this(DEFAULT_REVISION) { }

        public Mid0803(Header header) : base(header) { }

        public Mid0803(int revision) : this(new Header() { Mid = MID, Revision = revision }) { }
    }
}