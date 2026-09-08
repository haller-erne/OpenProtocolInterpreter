namespace OpenProtocolInterpreter.Wifi
{
    /// <summary>
    /// Reception quality change upload
    /// <para>
    ///     The controller reports the current WiFi reception quality when it changes.
    ///     Same data format as <see cref="Mid0806"/>.
    /// </para>
    /// <para>Message sent by: Controller</para>
    /// <para>Answer: None</para>
    /// </summary>
    public class Mid0808 : Mid, IWifi, IController
    {
        public const int MID = 808;

        /// <summary>
        /// Reception quality in dBm (typically -45 to -90), transmitted as 4 ASCII characters (e.g. "-080").
        /// </summary>
        [StringDataFieldDefinition(revision: 1, field: 1, Index = 20, Size = 4)]
        public string ReceptionQuality { get; set; }

        public Mid0808() : this(DEFAULT_REVISION) { }

        public Mid0808(Header header) : base(header) { }

        public Mid0808(int revision) : this(new Header() { Mid = MID, Revision = revision }) { }
    }
}