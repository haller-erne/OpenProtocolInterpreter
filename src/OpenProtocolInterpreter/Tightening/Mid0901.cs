using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenProtocolInterpreter.Tightening
{
    /// <summary>
    /// Trace plot parameters message
    /// <para>
    ///     This MID 0901 response contains all trace plotting parameters necessary for drawing
    ///     the limit figures in relation to the trace curve. The plotting parameters sent depend
    ///     on the Trace types subscribed for.
    /// </para>
    /// <para>Message sent by: Controller</para>
    /// <para>Answer: <see cref="Communication.Mid0005"/> Command accepted</para>
    /// <para>
    ///     Revisions (Open Protocol Specification R 2.21.1, section 5.8.10 / Tables 146-148):
    ///     Rev 1 = RDI + Time stamp + Number of PIDs + Variable data fields.
    ///     Rev 2 = Rev 1 + Request MID after Number of PIDs.
    ///     Rev 3 = Rev 2 + Object ID / Object type / Reference object ID / Trace Type after Request MID.
    ///     Every revision registers its FULL wire layout.
    /// </para>
    /// </summary>
    public class Mid0901 : Mid, ITightening, IController
    {
        public const int MID = 901;

        /// <summary>
        /// The Result Data Identifier is a unique ID for each operation result within the system. (10 bytes)
        /// </summary>
        [StringDataFieldDefinition(revision: 1, field: 1, Index = 20, Size = 10, HasPrefix = false)]
        [StringDataFieldDefinition(revision: 2, field: 1, Index = 20, Size = 10, HasPrefix = false)]
        [StringDataFieldDefinition(revision: 3, field: 1, Index = 20, Size = 10, HasPrefix = false)]
        public string ResultDataIdentifier { get; set; }

        /// <summary>
        /// Time stamp for each operation sent to the control station (YYYY-MM-DD:HH:MM:SS, 19 bytes).
        /// </summary>
        [TimestampDataFieldDefinition(revision: 1, field: 2, Index = 30, HasPrefix = false)]
        [TimestampDataFieldDefinition(revision: 2, field: 2, Index = 30, HasPrefix = false)]
        [TimestampDataFieldDefinition(revision: 3, field: 2, Index = 30, HasPrefix = false)]
        public DateTime TimeStamp { get; set; }

        /// <summary>Number of PID's (parameter / variable data fields) in the telegram.</summary>
        [Int32DataFieldDefinition(revision: 1, field: 3, Index = 49, Size = 3, HasPrefix = false)]
        [Int32DataFieldDefinition(revision: 2, field: 3, Index = 49, Size = 3, HasPrefix = false)]
        [Int32DataFieldDefinition(revision: 3, field: 3, Index = 49, Size = 3, HasPrefix = false)]
        public int NumberOfPIDs { get; set; }

        /// <summary>The MID of the request that this message is a response to (typically 0008 or 0006). (Revision 2+)</summary>
        [Int32DataFieldDefinition(revision: 2, field: 4, Index = 52, Size = 4, HasPrefix = false)]
        [Int32DataFieldDefinition(revision: 3, field: 4, Index = 52, Size = 4, HasPrefix = false)]
        public int RequestMid { get; set; }

        /// <summary>The user defined object ID. (Revision 3)</summary>
        [Int32DataFieldDefinition(revision: 3, field: 5, Index = 56, Size = 4, HasPrefix = false)]
        public int ObjectId { get; set; }

        /// <summary>Type of the object (0 = Unknown, 1 = Dual Reading, 2 = Tightening Production, ...). (Revision 3)</summary>
        [Int32DataFieldDefinition(revision: 3, field: 6, Index = 60, Size = 1, HasPrefix = false)]
        public ObjectType ObjectType { get; set; }

        /// <summary>Link to related Object ID. (Revision 3)</summary>
        [Int32DataFieldDefinition(revision: 3, field: 7, Index = 61, Size = 4, HasPrefix = false)]
        public int ReferenceObjectId { get; set; }

        /// <summary>Type of the trace curve (1 = Angle, 2 = Torque, 3 = Current, 4 = Gradient, 5 = Stroke, 6 = Force). (Revision 3)</summary>
        [Int32DataFieldDefinition(revision: 3, field: 8, Index = 65, Size = 2, HasPrefix = false)]
        public int TraceType { get; set; }

        /// <summary>Variable / parameter data fields (plotting PID records).</summary>
        [VariableDataFieldCollectionDefinition(revision: 1, field: 9, Index = 52, HasPrefix = false)]
        [VariableDataFieldCollectionDefinition(revision: 2, field: 10, Index = 56, HasPrefix = false)]
        [VariableDataFieldCollectionDefinition(revision: 3, field: 14, Index = 67, HasPrefix = false)]
        public List<VariableDataField> VariableDataFields { get; set; } = new List<VariableDataField>();

        public Mid0901() : this(DEFAULT_REVISION)
        {
        }

        public Mid0901(Header header) : base(header)
        {
        }

        public Mid0901(int revision) : this(new Header()
        {
            Mid = MID,
            Revision = revision
        })
        {
        }

        /// <summary>
        /// Sets the Volatile field size and packs ONLY the current revision's field list, in wire order.
        /// </summary>
        public override string Pack()
        {
            var revision = Header.StandardizedRevision;
            NumberOfPIDs = VariableDataFields?.Count ?? 0;

            int volatileFieldId = revision == 1 ? 9 : revision == 2 ? 10 : 14;
            GetField(revision, volatileFieldId).Size = VariableDataFields?.Sum(x => x.TotalSize) ?? 0;

            var builder = new StringBuilder(BuildHeader());
            builder.Append(Pack(revision));
            return builder.ToString();
        }

        /// <summary>
        /// Sum ONLY the active revision's full field layout before rendering the length prefix.
        /// The base implementation sums every revision 1..N, which is wrong for full per-revision layouts.
        /// </summary>
        protected override string BuildHeader()
        {
            Header.Length = Header.DefaultSize;
            if (RevisionsByFields.TryGetValue(Header.StandardizedRevision, out var activeFields))
            {
                foreach (var field in activeFields)
                    Header.Length += (field.HasPrefix ? 2 : 0) + field.Size;
            }

            return Header.ToString();
        }

        /// <summary>
        /// Parses fixed lead fields for the active revision, then sizes the trailing Variable
        /// data fields section from the remaining declared body length.
        /// </summary>
        protected override void ProcessDataFields(ReadOnlySpan<char> package)
        {
            var revision = Header.StandardizedRevision;
            base.ProcessDataFields(revision, package);
        }

        protected override void ProcessDataField(DataField dataField, ReadOnlySpan<char> package)
        {
            int volatileFieldId = Header.StandardizedRevision == 1 ? 9 : Header.StandardizedRevision == 2 ? 10 : 14;
            if (dataField.Field == volatileFieldId)
            {
                dataField.Size = Header.Length - dataField.Index;
            }
            base.ProcessDataField(dataField, package);
        }
    }
}