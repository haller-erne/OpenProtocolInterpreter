# GitHub Copilot Instructions — OpenProtocolInterpreter

## Project Overview

**OpenProtocolInterpreter** is a zero-dependency .NET library that bidirectionally serializes/deserializes Atlas Copco Open Protocol MID messages (fixed-width ASCII) to typed C# objects and back.  
NuGet package: `OpenProtocolInterpreter` · Version: `6.1.1`  
Multi-targeted: `netstandard2.0; net6.0; net8.0; net10.0`

---

## Repository Layout

```
src/
  OpenProtocolInterpreter/   ← main library
    _internals/              ← infrastructure (MessagesTemplate, MidCompiledInstance)
    Enums/                   ← all shared enums (~50 files)
    {Category}/              ← one folder per MID domain (Alarm, Tightening, Job, …)
      I{Category}.cs         ← empty marker interface
      {Category}Messages.cs  ← internal MID registry for the category
      Mid{NNNN}.cs           ← one file per MID number
    MID.cs                   ← abstract base class
    Header.cs                ← 20-char header model
    DataField.cs             ← single field descriptor
    OpenProtocolConvert.cs   ← all type-conversion utilities
    MidInterpreter.cs        ← public entry point
    MidInterpreterMessagesExtensions.cs  ← fluent registration API
  MIDTesters.Core/           ← MSTest test project (net10.0)
sample/                      ← WinForms sample driver (not part of the library)
docs/                        ← vendor protocol specs (Atlas Copco, Desoutter)
```

---

## Core Abstractions

### Header (20 chars, always present)
| Position | Field | Width |
|---|---|---|
| 0–3 | Length | 4 |
| 4–7 | Mid | 4 |
| 8–10 | Revision | 3 |
| 11 | NoAckFlag | 1 |
| 12–13 | StationId | 2 |
| 14–15 | SpindleId | 2 |
| 16–17 | SequenceNumber | 2 |
| 18 | NumberOfMessages | 1 |
| 19 | MessageNumber | 1 |

### DataField
Describes one field within a MID. In the attribute-based pattern, fields are declared via `[XxxDataFieldDefinition]` attributes on properties (see MID Class Conventions below). The attribute system handles binding, parsing, and packing automatically. For MIDs that override `RegisterDatafields()` (rare — only when the attribute pattern cannot express the layout), use `DataField` factory statics:
- `DataField.String(field, index, size)` — space-padded
- `DataField.Number(field, index, size)` — zero-padded
- `DataField.Boolean(field, index)` — size 1
- `DataField.Timestamp(field, index)` — size 19, format `yyyy-MM-dd:HH:mm:ss`
- `DataField.Volatile(field, index)` — variable-length

### OpenProtocolConvert
The **only** permitted conversion utility. Never use `int.Parse`, `DateTime.Parse`, etc. in MID code.  
Key pairs: `ToBoolean`/`ToString(bool)`, `ToDateTime`/`ToString(DateTime)`, `ToDecimal`/`ToString(decimal)`, `ToInt32`, `ToInt64`, `TruncatedDecimalToString`/`ToTruncatedDecimal`, `TruncatePadded`, `GetBit`/`ToByte`.

---

## MID Class Conventions (mandatory)

Every concrete MID **must** follow this exact pattern (attribute-based, upstream v6.2.0+):

```csharp
// Namespace: OpenProtocolInterpreter.{Category}
public class Mid0071 : Mid, IAlarm, IController, IAcknowledgeable<Mid0072>
{
    public const int MID = 71; // always present

    // --- Properties decorated with DataFieldDefinition attributes ---
    [StringDataFieldDefinition(revision: 1, field: 1, Index = 20, Size = 4, PaddingOrientation = PaddingOrientation.LeftPadded)]
    [StringDataFieldDefinition(revision: 2, field: 1, Index = 20, Size = 5, PaddingOrientation = PaddingOrientation.LeftPadded)]
    [StringDataFieldDefinition(revision: 3, field: 1, Index = 20, Size = 5, PaddingOrientation = PaddingOrientation.LeftPadded)]
    public string ErrorCode { get; set; }

    [BooleanDataFieldDefinition(revision: 1, field: 2, Index = 26)]
    [BooleanDataFieldDefinition(revision: 2, field: 2, Index = 27)]
    [BooleanDataFieldDefinition(revision: 3, field: 2, Index = 27)]
    public bool ControllerReadyStatus { get; set; }

    [TimestampDataFieldDefinition(revision: 1, field: 4, Index = 32)]
    [TimestampDataFieldDefinition(revision: 2, field: 4, Index = 33)]
    [TimestampDataFieldDefinition(revision: 3, field: 4, Index = 33)]
    public DateTime Time { get; set; }

    // --- Constructors (all three required, enforced by tests) ---
    public Mid0071() : this(DEFAULT_REVISION) { }
    public Mid0071(Header header) : base(header) { }
    public Mid0071(int revision) : this(new Header() { Revision = revision, Mid = MID }) { }

    // --- Override Pack() only when volatile field sizes must be set before packing ---
    // --- Override ProcessDataFields() only when per-revision (non-additive) layout is used ---
}
```

**Attribute types:**
| Attribute | Property Type | Default Padding | Notes |
|---|---|---|---|
| `[StringDataFieldDefinition]` | `string` | Space, right | Set `PaddingOrientation` for left |
| `[Int32DataFieldDefinition]` | `int`, `enum` | Zero, left | Handles enum-backed int automatically |
| `[Int64DataFieldDefinition]` | `long` | Zero, left | |
| `[BooleanDataFieldDefinition]` | `bool` | Size=1 | |
| `[TimestampDataFieldDefinition]` | `DateTime` | Size=19 | Format `yyyy-MM-dd:HH:mm:ss` |
| `[DecimalDataFieldDefinition]` | `decimal` | Zero, left | |
| `[TruncatedDecimalDataFieldDefinition]` | `decimal` | Zero, left | Set `DecimalPoints` |
| `[VariableDataFieldCollectionDefinition]` | `List<VariableDataField>` | — | For variable-length PID lists |

**Rules:**
1. Class name: `Mid{NNNN}` with 4-digit zero-padded number.
2. Three required constructors (parameterless, `(Header)`, `(int revision)`) — validated by `DefaultMidTests<T>`.
3. `const int MID = N` must be present.
4. One attribute per (property, revision) pair — duplicate attributes for multi-revision properties.
5. `field: N` is the **1-based wire field number** (appears as prefix in the packed message).
6. `Index` is the **absolute offset** from position 0 (header occupies 0–19).
7. Use `HasPrefix = false` when the wire format has no field number prefix.
8. Properties use **auto-properties** `{ get; set; }` — the attribute system handles binding.
9. Apply the correct **category marker interface** (`IAlarm`, `ITightening`, etc.) and **role interface** (`IController` or `IIntegrator`).
10. Apply **behavior interfaces** as per the spec: `IAcknowledgeable<TAck>`, `IAnswerableBy<TAnswer>`, `IAcceptableCommand`, `IDeclinableCommand`, `ISubscription`, `IUnsubscription`.

**When to override:**
- `Pack()` — when volatile field sizes must be computed from list contents before packing (e.g., `Mid0902`, `Mid0901`)
- `ProcessDataFields()` — when revision layout is **replacement** (not additive), process only the active revision's fields
- `ProcessDataField(DataField, ReadOnlySpan<char>)` — when a specific field needs dynamic size adjustment during parsing (e.g., volatile trailing sections)
- `BuildHeader()` — when header length must sum only the active revision's fields (replacement layouts)
- `RegisterDatafields()` — **only** when the attribute pattern cannot express the layout (e.g., `Mid0900` with interleaved volatile/fixed fields whose indices depend on parsed data)

---

## Adding a New Category

1. Create folder `src/OpenProtocolInterpreter/{Category}/`.
2. Add `I{Category}.cs` — empty marker interface.
3. Add `{Category}Messages.cs` — `internal class {Category}Messages : MessagesTemplate` with:
   - MID-number dictionary of `MidCompiledInstance`
   - `IsAssignableTo(int mid)` range expression
   - Three constructors (no-arg, `InterpreterMode`, `IEnumerable<Type>`)
4. Add extension method `Use{Category}Messages(...)` to `MidInterpreterMessagesExtensions.cs`.

---

## Registering a New MID in an Existing Category

In `{Category}Messages.cs`, add to the dictionary:
```csharp
{ Mid{NNNN}.MID, new MidCompiledInstance(typeof(Mid{NNNN})) }
```
Extend `IsAssignableTo` if the MID number falls outside the current range expression.

---

## MidInterpreter Usage

```csharp
// Register all categories
var interpreter = new MidInterpreter().UseAllMessages();

// Or selectively, with role filtering
var interpreter = new MidInterpreter()
    .UseAlarmMessages(InterpreterMode.Controller)
    .UseTighteningMessages(InterpreterMode.Integrator);

// Parse
Mid parsed = interpreter.Parse(rawString);              // untyped
Mid0061 typed = interpreter.Parse<Mid0061>(rawString);  // typed
```

---

## Test Conventions

Tests live in `src/MIDTesters.Core/`. Framework: **MSTest** on `net10.0`.

Hierarchy:
```
MidTester (base, creates shared MidInterpreter)
  └── DefaultMidTests<TMid>  (enforces parameterless + Header constructors)
        └── TestMid{NNNN} : DefaultMidTests<Mid{NNNN}>
```

Per-MID test pattern:
```csharp
[TestClass, TestCategory("{Category}")]
public class TestMid0071 : DefaultMidTests<Mid0071>
{
    [TestMethod, TestCategory("Revision 1"), TestCategory("ASCII")]
    public void Mid0071Revision1()
    {
        string pack = @"00530071001         01E851021031042017-12-01:20:12:45";
        var mid = _midInterpreter.Parse<Mid0071>(pack);
        Assert.AreEqual("E851", mid.ErrorCode);
        // ... assert every field
        AssertEqualPackages(pack, mid); // mandatory round-trip check
    }

    [TestMethod, TestCategory("Revision 1"), TestCategory("ByteArray")]
    public void Mid0071ByteRevision1()
    {
        string pack = @"00530071001         01E851021031042017-12-01:20:12:45";
        var mid = _midInterpreter.Parse<Mid0071>(_midInterpreter.GetAsciiBytes(pack));
        Assert.AreEqual("E851", mid.ErrorCode);
        AssertEqualPackages(pack, mid);
    }
}
```

**Rules:**
- Each revision gets its own test method.
- Each revision gets both an ASCII and a ByteArray test method.
- Test fixtures must be real wire strings taken from the vendor protocol spec.
- Always call `AssertEqualPackages` to verify round-trip fidelity.
- Add `[TestCategory("{Category}")]` matching the domain folder name.

---

## Hard Constraints

- **Zero external NuGet dependencies** in `OpenProtocolInterpreter.csproj`. Do not add any.
- **`LangVersion: latest`** — modern C# features are allowed.
- All type conversions in MID code go through `OpenProtocolConvert`. Never call `int.Parse`, `double.Parse`, `DateTime.Parse`, `Convert.ToXxx`, etc. directly.
- Do not use `Reflection` or `dynamic` outside of `_internals/`.
- Do not change `Header` parsing logic — it is shared by all MIDs.
- Field index values are absolute offsets from position 0. The header consumes positions 0–19; the first data field always starts at ≥ 20.
