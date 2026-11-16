---
title: squad discover map fails in shared checkout
priority: 10
status: planning
---

## Goal

Resolve the `squad discover map` failure observed in the shared Plainion.DrawVista
checkout, so supported current source produces usable design evidence.

## Acceptance Criteria

- Identify the offending input or input/environment distinction and document a
  decisive reproduction. Do not infer the cause from the exception alone.
- For supported current DrawVista inputs, generation in the affected shared
  checkout publishes a complete immutable JSON/HTML pair, and guarded bounded
  reads succeed with generation, fingerprint, confidence and packet counts
  retained. Historical worker success is not a substitute.
- Invalid or unsupported inputs produce an actionable diagnostic identifying the
  failing input and a controlled nonzero exit, rather than an unhandled exception.
  Preserve validation and any previous successful publication.
- A separately assigned tool fix includes regression coverage for the diagnosed
  case. Do not weaken validation or omit eligible source merely to make generation
  succeed.

## Constraints

- This is a dedicated tool-failure report, linked from [Layers](Layers.md).
  It does not assign BlaXquad implementation investigation or changes.
- Keep DrawVista application behavior, accepted work and original source bytes
  unchanged. Do not synchronize or clean the shared checkout to reproduce this.
- Do not copy historical models into the shared checkout or treat cached facts
  as current-source authority. Layers remains blocked on current-main evidence.

## Report - 2026-11-16

### Impact and environment

The shared checkout cannot publish a current design model. Consequently Layers
implementation boundaries cannot yet be planned against current-main structural
evidence. The exact offending input and underlying cause remain **unverified**.

- Operating system: Windows; commands were run from PowerShell.
- Selected checkout: `C:\ws\plainionist\Plainion.DrawVista`, branch `main`.
- Executable: `C:\ws\plainionist\BlaXquad\bin\squad.exe`.
- Failing source brief: `2afa9d2`; intervening issue commits changed documentation
  only. The initial Git status reported a clean checkout.
- Accepted worker evidence: coder commit
  `1dee493984675435a6d0262f9d0810ca3f4a7e81`, reviewer acceptance
  `00cd608ad9721c7975441a403d83feea82e4ce37`.
- Node.js `22.18.0` was used for diagnostic metadata and the reversible comparison,
  not as evidence of the map tool's version. The tool binary version was not
  captured during the failing runs.

### Generation command and complete failure output

```powershell
& 'C:\ws\plainionist\BlaXquad\bin\squad.exe' discover map --project 'C:\ws\plainionist\Plainion.DrawVista'
```

The original run, a cold-index run and an exact-committed LF-only comparison all
failed with this common exception stack. PowerShell reported exit `-532462766`.

```text
Unhandled exception. System.IO.InvalidDataException: Invalid authored contract.
   at squad.Discover.Design.DesignReader.ValidatePart(String path, DesignContractPart part, HashSet`1 validated, HashSet`1 active) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Design\DesignReader.cs:line 215
   at squad.Discover.Design.DesignReader.ValidatePart(String path, DesignContractPart part, HashSet`1 validated, HashSet`1 active) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Design\DesignReader.cs:line 230
   at squad.Discover.Design.DesignReader.ValidatePart(String path, DesignContractPart part, HashSet`1 validated, HashSet`1 active) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Design\DesignReader.cs:line 230
   at squad.Discover.Design.DesignReader.ValidatePart(String path, DesignContractPart part, HashSet`1 validated, HashSet`1 active) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Design\DesignReader.cs:line 230
   at squad.Discover.Design.DesignReader.Validate(String path, DesignFacts facts) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Design\DesignReader.cs:line 152
   at squad.Discover.Design.DesignPayload.Read(DesignPayloadData data) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Design\DesignPayload.cs:line 39
   at squad.Discover.Design.DesignPayload.Capture(DesignFacts facts) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Design\DesignPayload.cs:line 17
   at squad.Discover.Design.DesignReader.Project(String path) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Design\DesignReader.cs:line 40
   at squad.Discover.Navigation.SourceCatalog.Prepare(DiscoverySourceTree tree, IReadOnlyList`1 files, SourceSyntax syntax, DeclarationReader reader, IReadOnlyDictionary`2 inputs, IReadOnlyDictionary`2 savedDeclarations, IReadOnlyDictionary`2 savedUsages, DesignReader designReader, IReadOnlyDictionary`2 savedDesign) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Navigation\SourceCatalog.cs:line 108
   at squad.Discover.Design.DesignCapture.Read(DiscoverySourceTree tree, SourceSyntax syntax, DiscoveryIndex index) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Design\DesignCapture.cs:line 47
   at squad.Discover.Maps.MapGeneration.Capture(DiscoverySourceTree tree, MapAdmission admission, TextWriter notices) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Maps\MapGeneration.cs:line 14
   at squad.Discover.Maps.MapGeneration.Refresh(DiscoverySourceTree tree, MapAssets assets, TextWriter notices) in C:\ws\plainionist\BlaXquad\src\squad\Discover\Maps\MapGeneration.cs:line 28
   at squad.Discover.MapCommand.Execute(String[] arguments, DiscoveryTimings timings) in C:\ws\plainionist\BlaXquad\src\squad\Discover\MapCommand.cs:line 65
   at squad.Discover.DiscoverCommand.Dispatch(String[] arguments, DiscoveryTimings timings) in C:\ws\plainionist\BlaXquad\src\squad\Discover\DiscoverCommand.cs:line 123
   at squad.Discover.DiscoverCommand.Execute(String[] arguments) in C:\ws\plainionist\BlaXquad\src\squad\Discover\DiscoverCommand.cs:line 48
   at squad.Discover.DiscoverCommand.ExecuteAsync(String[] args) in C:\ws\plainionist\BlaXquad\src\squad\Discover\DiscoverCommand.cs:line 34
   at Program.<Main>$(String[] args) in C:\ws\plainionist\BlaXquad\src\squad\Program.cs:line 65
   at Program.<Main>(String[] args)
```

These paths and line numbers are captured diagnostic output, not instructions to
inspect BlaXquad source.

### Guarded read

```powershell
& 'C:\ws\plainionist\BlaXquad\bin\squad.exe' discover map read --project 'C:\ws\plainionist\Plainion.DrawVista'
```

Exit 1:

```text
MAP Missing: Could not find file 'C:\ws\plainionist\Plainion.DrawVista\.blaxquad\discovery\map\current.json'.
```

No current-main pair was published. This is an explicit evidence gap, not proof
of absent application structure.

### Controlled comparisons

**Cold generated index:** CLI help exposed no cache-rebuild command. The closed
generated index was preserved, with no WAL, SHM or journal sidecars present, and
the same generation command was run without an existing index. It failed with
the same exception. Read-only SQLite metadata, using
`node --experimental-sqlite -`, reported schema `user_version = 7`:

| Index                                | Declaration inputs | Design inputs |
| ------------------------------------ | ------------------ | ------------- |
| Main before cold comparison          | 36                 | 0             |
| Fresh main after failed cold capture | 0                  | 0             |
| Historical coder                     | 36                 | 55            |
| Historical reviewer                  | 36                 | 55            |

The old main index was 675840 bytes, modified
`2026-11-16T06:56:34.283Z`; the observed fresh index was 32768 bytes, modified
`2026-11-16T07:18:01.778Z`. Each stored checkout identity matched its root.
These are cache-entry counts, not eligible-input inventories or architectural
absence. Cached design reuse is not necessary to reproduce the failure.

**LF-only source comparison:** All 55 historical worker input paths were present
on main. Six differed from both workers only by CRLF versus LF, with 70 extra CR
bytes in total; normalized hashes and BOMs matched. The complete eligible input
set on main was not established.

| Input relative to `src`                           | Original bytes | LF bytes | Committed Git blob                         |
| ------------------------------------------------- | -------------- | -------- | ------------------------------------------ |
| `Plainion.DrawVista.Tests\JsonExtensions.cs`      | 205            | 196      | `ed0190faf4426d54db86ac5fdcfa23f6b1a1b237` |
| `Plainion.DrawVista\GlobalConst.cs`               | 187            | 182      | `db3aebab0916b3f2f28cc5f93e13c36d6d6f4c99` |
| `Plainion.DrawVista\IO\DrawIOModel.cs`            | 453            | 436      | `00a6575c1e99315ac946a6a52b81157f0ce47ae1` |
| `Plainion.DrawVista\IO\DrawingWorkbookFactory.cs` | 641            | 620      | `117c186fcedf07b49c3fa712fb125ca452ad8bd8` |
| `Plainion.DrawVista\UseCases\Documents.cs`        | 204            | 198      | `66c880d031fd5f8876c397c5bc1573831ed4301b` |
| `Plainion.DrawVista\UseCases\SvgExtensions.cs`    | 407            | 395      | `e745cb48d94e8959f78ceb5c28970e7c5fd0a871` |

The comparison backed up original bytes, required lossless UTF-8 and a clean
per-input Git status, replaced only physical CRLF with LF, and required
`git hash-object --stdin` of each normalized buffer to match its committed blob.
Generation still failed with the same exception. Node reported child exit
`3762504530`, the unsigned representation of `-532462766`; the comparison wrapper
then exited 1 with `LF-only generation failed`.

Every original byte buffer was restored and independently checked against its
backup. Git subsequently exposed the six original newline representations as
modified; they remain unstaged and must not be cleaned or committed for this
report. Configuration showed `core.autocrlf=false` and no tracked `.gitattributes`.
The six newline differences are not a sufficient explanation of the failure.

### Historical successful contrast

Independent generation and bounded reads succeeded in the two worker checkouts
without application changes, external tool edits or safeguard bypasses:

| Checkout                                                   | Generation                         |
| ---------------------------------------------------------- | ---------------------------------- |
| `C:\ws\plainionist\Plainion.DrawVista\.worktrees\coder`    | `8e60ad27edc04adfb3c443e19f962b53` |
| `C:\ws\plainionist\Plainion.DrawVista\.worktrees\reviewer` | `7742d8f0af9b495a9b02adbecfc20529` |

Both reported source fingerprint
`EB7C608791906A2B8406565410D94930EE8ABD96768A81187FDF3F10FA3BFD87`.
Their overview contained 91 places and 423 occurrences: total 514, shown 20,
omitted 494 on the first packet. Counts are per packet, not cumulative coverage.
Their complete immutable pairs remain below each checkout's
`.blaxquad\discovery\map\generations\<generation>` as `model.json` and `map.html`.
These are historical diagnostic references, not current-main authority.

### Preserved evidence and remaining unknowns

The full session log, including the comparison wrapper output and byte-restoration
check, is:
`C:\Users\ssmosett\.copilot\session-state\70018c83-7287-4475-8bf1-7628dcb44d9e\files\layers-main-model-diagnostic.txt`.
The same session's `files` directory retains
`layers-discovery-index-before-regeneration.sqlite3` and the six original byte
backups in `layers-source-input-before-lf`.

The accepted historical issue revision
`00cd608ad9721c7975441a403d83feea82e4ce37` retains the original exception, worker
artifact selectors, bounded-read counts and reviewer acceptance.

The exact failing file/contract, full main input selection, binary identity during
the historical runs, and underlying cause remain unknown. No main success or tool
fix has been established; neither failed comparison is a diagnosed recovery.
