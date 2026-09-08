namespace OpenProtocolInterpreter.Battery
{
    /// <summary>
    /// Battery level response
    /// <para>
    ///     The controller transmits the current battery level including capacity percentage and state.
    /// </para>
    /// <para>Message sent by: Controller</para>
    /// <para>Answer: None</para>
    /// </summary>
    public class Mid0801 : Mid, IBattery, IController
    {
        public const int MID = 801;

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

        public Mid0801() : this(DEFAULT_REVISION) { }

        public Mid0801(Header header) : base(header) { }

        public Mid0801(int revision) : this(new Header() { Mid = MID, Revision = revision }) { }
    }
}