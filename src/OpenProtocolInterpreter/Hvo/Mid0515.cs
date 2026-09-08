using System;
using System.Linq;
using System.Text;

namespace OpenProtocolInterpreter.Hvo
{
    /// <summary>
    /// Set HVO signal
    /// <para>
    ///     Command to set the HVO (Hand-guided Visual Output) lamp signals.
    ///     Revision 1: Controls 4 individual lamp signals.
    ///     Revision 2: Controls a numbered light with a status value.
    /// </para>
    /// <para>Message sent by: Integrator</para>
    /// <para>Answer: None</para>
    /// </summary>
    public class Mid0515 : Mid, IHvo, IIntegrator
    {
        public const int MID = 515;

        /// <summary>
        /// Revision 1: Lamp 1 signal value (0-9).
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 1, Index = 20, Size = 1)]
        public int Lamp1 { get; set; }

        /// <summary>
        /// Revision 1: Lamp 2 signal value (0-9).
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 2, Index = 23, Size = 1)]
        public int Lamp2 { get; set; }

        /// <summary>
        /// Revision 1: Lamp 3 signal value (0-9).
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 3, Index = 26, Size = 1)]
        public int Lamp3 { get; set; }

        /// <summary>
        /// Revision 1: Lamp 4 signal value (0-9).
        /// </summary>
        [Int32DataFieldDefinition(revision: 1, field: 4, Index = 29, Size = 1)]
        public int Lamp4 { get; set; }

        /// <summary>
        /// Revision 2: Light number (1-999).
        /// </summary>
        [Int32DataFieldDefinition(revision: 2, field: 1, Index = 20, Size = 3)]
        public int LightNumber { get; set; }

        /// <summary>
        /// Revision 2: Light status value (1-999).
        /// </summary>
        [Int32DataFieldDefinition(revision: 2, field: 2, Index = 25, Size = 3)]
        public int LightStatus { get; set; }

        public Mid0515() : this(DEFAULT_REVISION) { }

        public Mid0515(int revision) : this(new Header()
        {
            Mid = MID,
            Revision = revision
        })
        {
        }

        public Mid0515(Header header) : base(header) { }

        /// <summary>
        /// Revision 2 is a replacement format, not additive. Only process the current revision's fields.
        /// </summary>
        protected override void ProcessDataFields(ReadOnlySpan<char> package)
        {
            ProcessDataFields(Header.StandardizedRevision, package);
        }

        public override string Pack()
        {
            if (!RevisionsByFields.Any())
                return BuildHeader();

            int revision = Header.StandardizedRevision;
            if (RevisionsByFields.TryGetValue(revision, out var dataFields))
            {
                Header.Length = Header.DefaultSize;
                foreach (var dataField in dataFields)
                    Header.Length += (dataField.HasPrefix ? 2 : 0) + dataField.Size;
            }

            var builder = new StringBuilder(Header.ToString());
            builder.Append(Pack(revision));
            return builder.ToString();
        }
    }
}