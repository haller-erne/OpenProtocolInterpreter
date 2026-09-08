namespace OpenProtocolInterpreter.SocketTray
{
    /// <summary>
    /// Socket tray selection
    /// <para>
    ///     Send socket position selections to the controller.
    ///     Each of the 8 socket positions is represented by a single digit value.
    /// </para>
    /// <para>Message sent by: Integrator</para>
    /// <para>Answer: None</para>
    /// </summary>
    public class Mid0524 : Mid, ISocketTray, IIntegrator
    {
        public const int MID = 524;

        [Int32DataFieldDefinition(revision: 1, field: 1, Index = 20, Size = 1)]
        public int Socket1 { get; set; }

        [Int32DataFieldDefinition(revision: 1, field: 2, Index = 23, Size = 1)]
        public int Socket2 { get; set; }

        [Int32DataFieldDefinition(revision: 1, field: 3, Index = 26, Size = 1)]
        public int Socket3 { get; set; }

        [Int32DataFieldDefinition(revision: 1, field: 4, Index = 29, Size = 1)]
        public int Socket4 { get; set; }

        [Int32DataFieldDefinition(revision: 1, field: 5, Index = 32, Size = 1)]
        public int Socket5 { get; set; }

        [Int32DataFieldDefinition(revision: 1, field: 6, Index = 35, Size = 1)]
        public int Socket6 { get; set; }

        [Int32DataFieldDefinition(revision: 1, field: 7, Index = 38, Size = 1)]
        public int Socket7 { get; set; }

        [Int32DataFieldDefinition(revision: 1, field: 8, Index = 41, Size = 1)]
        public int Socket8 { get; set; }

        public Mid0524() : this(DEFAULT_REVISION) { }

        public Mid0524(Header header) : base(header) { }

        public Mid0524(int revision) : this(new Header() { Mid = MID, Revision = revision }) { }
    }
}