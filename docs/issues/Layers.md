---
title: Layers
priority: 10
status: planning
---

## Goal

Support viewing and toggling draw.io layers while browsing diagrams.

## Acceptance Criteria

### Import and compatibility

- Support `.drawio` files and `.png` images with an embedded draw.io model.
- Preserve existing page naming and page selection behavior for both formats.
- Layers belong to individual diagram pages. Preserve their names, stacking order,
  and visibility saved in draw.io.
- Include initially hidden layers so that they can be enabled in the viewer.
- Already-uploaded pages continue to render unchanged. Layer support for these pages
  requires re-upload; automatic recovery or backfill is not required.

### Layer control and viewer state

- Show a dropdown with a checkbox for each layer in the toolbar immediately to the
  right of Search.
- When enabled, the control lists all layers, including the default layer, in draw.io
  stacking order. Every listed layer can be independently enabled or disabled.
- Keep the control visible but dimmed and disabled when the current page has no layer
  metadata or only the default layer, with no additional layers.
- Opening a page for the first time in a browser tab uses its source visibility.
- Remember checkbox selections separately for each page within the current browser
  tab, including navigation away and back and browser refresh.
- Reset selections when the tab/session ends. Selections are viewer preferences,
  not changes to the source diagram or shared server-side state.

### Rendering and interaction

- Composite layers with the existing SVG as one image, without duplicating content
  or leaving disabled-layer content visible in the base image.
- Preserve draw.io stacking order and use a shared coordinate system so that layers
  remain aligned even when their individual content bounds differ.
- Initially hidden content must not be clipped when its layer is enabled.
- All layers share zoom and pan. Toggling a layer preserves the current zoom, pan,
  and framing, without shifting the diagram.
- Allow all layers to be disabled; the diagram canvas is then empty.
- Existing generated diagram links and external links continue to work in visible
  layers. Disabled-layer content is neither visible nor clickable.

### Search

- Index captions from all layers, including initially hidden layers.
- Checkbox selections do not filter the global search index.
- Search results retain the existing page-navigation behavior. Opening a result
  uses the page's remembered or source visibility and does not automatically enable
  layers containing matches.

## Constraints

- Preserve existing functionality and compatibility with already-stored pages.
- Read-only layer support: creating, deleting, renaming, or reordering layers and
  writing visibility changes back to draw.io files are out of scope.

## Plan

No Layers implementation has been accepted. The planning baseline is
`e9c9eb7dbb85b358a8453185fd0b3c6bfbf89802` on `main`.

`README.md` supplies product intent and the existing import naming rules.
`src\Plainion.DrawVista\WebUI\README.md` supplies frontend tooling and validation
requirements. `squad discover specs --list` completed with zero authored scenarios
and zero feature files; the acceptance criteria above remain the behavioral
authority. This does not establish the absence of other tests.

Initial structural evidence was unavailable: `squad discover map` failed before
publishing a current design pair. Fresh generation and bounded reads now succeed
in both assigned checkouts, as recorded below. Slice 1 is complete.
The model-selected candidates do not establish implementation
ownership or resolved calls; implementation boundaries remain unplanned.

### Slice 1 - Restore current design evidence

Stable plan ID: `layers-current-design-evidence`.

- **Status:** Complete; accepted on 2026-11-16.
- **Outcome:** Produce a complete, freshly generated and readable design model of
  the selected checkout so that the Layers implementation can be planned against
  current structure and contracts.
- **Start:** This issue, the two README files named above, the baseline commit, and
  the generation diagnostic below. The selected executable is
  `C:\ws\plainionist\BlaXquad\bin\squad.exe`; generation was invoked from
  `C:\ws\plainionist\Plainion.DrawVista`. The failing contract and root cause are
  not yet identified.
- **Change:** Diagnose the generation failure with bounded, current-source
  diagnostics and use a supported recovery if available. This slice does not
  authorize edits outside the coder's assigned checkout, application-behavior
  changes to accommodate the generator, or bypassing tool safeguards. If recovery
  requires an external tool-owner change, return the precise failing input,
  reproduction, and required correction to the architect rather than claiming
  completion. On success, update this brief with the generation identity, artifact
  references, and bounded model-selected candidates for import/export, page
  persistence and API contracts, caption/link processing, and viewer composition,
  toolbar, and browser-tab state. Retain total/shown/omitted counts and confidence;
  distinguish candidates from resolved ownership or calls.
- **Preserve:** All acceptance criteria and existing application behavior. No
  Layers implementation is authorized by this prerequisite. Do not commit the
  generated model artifacts or treat a cache or older publication as current
  evidence.
- **Prove:** `squad discover map` succeeds on current source, followed by a valid
  `squad discover map read` overview and the relevant bounded element reads.
  Record the selected worktree and immutable JSON/HTML pair. Reviewer verification
  must independently generate and read a current model in the reviewer's checkout.
  A diagnosed but unresolved generator failure does not satisfy this outcome.

#### Coder evidence - 2026-11-16

The initial failure did not reproduce with the selected executable on the assigned
checkout at `769b0ded549e14656d3f188f762d2e95698d029f`. No recovery, application
source change, external tool edit, or safeguard bypass was needed. The cause of
the historical exception remains unverified; its diagnostic is retained below.

- Executable: `C:\ws\plainionist\BlaXquad\bin\squad.exe`.
- Selected worktree: `C:\ws\plainionist\Plainion.DrawVista\.worktrees\coder`.
- Generation: `8e60ad27edc04adfb3c443e19f962b53`.
- Source fingerprint: `EB7C608791906A2B8406565410D94930EE8ABD96768A81187FDF3F10FA3BFD87`.
- Immutable JSON: `C:\ws\plainionist\Plainion.DrawVista\.worktrees\coder\.blaxquad\discovery\map\generations\8e60ad27edc04adfb3c443e19f962b53\model.json`.
- Immutable HTML: `C:\ws\plainionist\Plainion.DrawVista\.worktrees\coder\.blaxquad\discovery\map\generations\8e60ad27edc04adfb3c443e19f962b53\map.html`.

Generation succeeded with exit 0:

```powershell
& 'C:\ws\plainionist\BlaXquad\bin\squad.exe' discover map --project 'C:\ws\plainionist\Plainion.DrawVista\.worktrees\coder'
```

All bounded reads below succeeded with exit 0 against that same generation and
fingerprint. Commands used the same executable and `--project` value, with
`discover map read`, the listed selector (omitted for overview), and `--page`
for pages after 1. The overview reports 91 places and 423 occurrences.
Each packet contains at most 20 items and 16384 UTF-8 bytes. Counts are per
packet, not cumulative: omitted items include evidence on other pages, even
when those pages were also read. Unread pages remain available in the complete
artifacts and must not be treated as absent.

| Element                     | Read selector                                                        | Total | Packets: page = shown / omitted      | Next unread page |
| --------------------------- | -------------------------------------------------------------------- | ----- | ------------------------------------ | ---------------- |
| Overview                    | none                                                                 | 514   | 1 = 20 / 494                         | 2                |
| DrawingWorkbookFactory      | `93723CFC1EA28FD6F5B7F9B4A8BFEA5874C6505EF840F06807CBB431007FF2F4:0` | 6     | 1 = 6 / 0                            | none             |
| DrawIOModel                 | `79D080091FF1A788186B4BFB918E203E293E3C311716DA305C583ABEDF7CD9AE:0` | 14    | 1 = 10 / 4; 2 = 3 / 11               | 3                |
| DrawIOApp                   | `E5E31BB22DACE8D749D0D1D4763845A3545D84A87D9601FBA3FF834C18FF5AA8:0` | 15    | 1 = 12 / 3                           | 2                |
| IDocumentStore              | `02BF4E85E002547056B35D978D2720CC12A7FD24E997426F966A3512D0A7A837:0` | 22    | 1 = 20 / 2; 2 = 2 / 20               | none             |
| IDocumentStore.GetPage      | `120CBED59E39A4154FB944F07E4671779EB69F628ACFF78212C7850B750DD511:0` | 23    | 1 = 6 / 17                           | 2                |
| ProcessedDocument           | `ProcessedDocument`                                                  | 29    | 1 = 11 / 18                          | 2                |
| SqliteDocumentStore         | `831F97A473831A801A78FF5AB2EE684002A1659D4F5A3DF1198C708EF2B92976:0` | 22    | 1 = 7 / 15                           | 2                |
| Program                     | `src\Plainion.DrawVista\Program.cs`                                  | 76    | 1 = 7 / 69                           | 2                |
| SvgProcessor                | `D033A84F259ADB87B509D35EFA656E85B85000EFE66BD9FEFD7D563475BCE835:0` | 27    | 1 = 14 / 13; 2 = 2 / 25              | 3                |
| FullTextSearch              | `AF8D95F2D8CE016B653A07A40BCE9182E38C4BD556F5BEEB80C273BDEEDE9953:0` | 13    | 1 = 9 / 4                            | 2                |
| SearchResults               | `E19F16A28D1B85D523E0D5CED65CFF263969CDF77FCDAAE3B61343A569F62569:0` | 54    | 1 = 20 / 34                          | 2                |
| MainLayout                  | `AEB43F99490A0B19E5116D1872D7B79FC190F205E9B139A0267D7CC698CFE2A5:0` | 41    | 1 = 20 / 21; 2 = 20 / 21; 3 = 1 / 40 | none             |
| Routes file                 | `src\Plainion.DrawVista\WebUI\src\router\routes.ts`                  | 7     | 1 = 7 / 0                            | none             |
| routes value                | `5D11F3A012EB5D1A59F68CA7B60E38668459C7A2C0A343D286267520EA53F5FF:0` | 10    | 1 = 10 / 0                           | none             |
| SvgContainer                | `SvgContainer`                                                       | 77    | 1 = 20 / 57                          | 2                |
| SvgContainer props contract | `2F795BA2A782A19F1F492E4666E4D2DFBEBFC4700028546F513B959F5923B913:0` | 2     | 1 = 2 / 0                            | none             |

Model-selected navigation candidates follow; paths are relative to
`src\Plainion.DrawVista`. They are not accepted implementation owners.

- **Import/export:** `IO\DrawingWorkbookFactory.cs` declares `TryCreate`;
  `IO\DrawIOModel.cs` declares `GetPageNames` and `WriteTo`.
  Its relationships select `IO\DrawIOWorkbook.cs` and
  `IO\DrawIOPngWorkbook.cs` as model-return/parameter candidates.
  `IO\DrawIOApp.cs` declares `ExtractSvg` and `SaveModelOnDemand`, with a
  `DrawIOModel` constructor parameter. Relevant relationships have
  `candidate` confidence; primitive/external types remain `unresolved`.
- **Page persistence and API contracts:** `UseCases\IDocumentStore.cs`
  declares page retrieval, saving, deletion, enumeration, clearing, and
  `DocumentsChanged`. `GetPage(string pageName)` returns `ProcessedDocument`;
  `UseCases\Documents.cs` authors
  `ProcessedDocument(string Name, string Content, IReadOnlyCollection<string> Captions)`.
  Store relationships select `IO\SqliteDocumentStore.cs`,
  `IO\FileSystemDocumentStore.cs`, and
  `Adapters\DocumentStoreCachingDecorator.cs` with `candidate` confidence.
  `Program.cs` is a composition/API entry candidate selected by the factory's
  construction relationship, not proof of endpoint routes or response behavior.
  Its first packet has `ambiguous` and `unresolved` call relationships.
- **Captions, links, and search:** `UseCases\SvgProcessor.cs` authors
  `ISvgCaptionParser`, `ISvgHyperlinkFormatter`, and `IDocumentStore`
  constructor parameters, and declares `Process`, `AddLinks`,
  `RemoveGeneratedLink`, and `ApplyStyleToExistingLinks`.
  `UseCases\FullTextSearch.cs` authors an `IDocumentStore` parameter and two
  `Search` methods; relevant C# relationships have `candidate` confidence.
  `WebUI\src\components\SearchResults.vue` declares search retrieval and
  clearing functions. Its module/lifecycle/value references have `literal`,
  `import-binding`, and `lexical-binding` confidence, not navigation proof.
- **Viewer composition, toolbar, and tab-state candidates:**
  `WebUI\src\components\SvgContainer.vue` declares `svg`, `panZoom`,
  `resizeObserver`, `updateSvg`, `setSvg`, `sizeSvg`, `fitImage`, and
  `TransformToLinks`; its props contract authors `page: String`.
  Its `vue-svg-pan-zoom` module reference has `literal` confidence.
  `WebUI\src\layouts\MainLayout.vue` declares `currentPage`, `route`, and
  `router`, with `q-toolbar` and `router-view` references marked `unresolved`.
  `WebUI\src\router\routes.ts` authors `routes: RouteRecordRaw[]`;
  route exports/references have `lexical-binding` or `import-binding`
  confidence, while dynamic import calls remain `unresolved`.
  These are candidates for wiring page-specific viewer preferences; no
  browser-tab persistence owner or session lifetime has been resolved.

The viewer anchor was narrowed with
`squad discover find svg-pan-zoom --names-only --project 'C:\ws\plainionist\Plainion.DrawVista\.worktrees\coder'`:
1 selected of 1 candidate file, 0 selection omissions, and 0 analysis failures.
Its literal/name-association evidence was followed by the model reads above,
not an implementation scan.

Only this issue brief changes. Generated artifacts remain local and are not
committed. Application tests, frontend checks, builds, and the full suite were
not run because application, build, and test code are unchanged; generation
and guarded bounded reads are the required proof for this prerequisite.
Before acceptance, the reviewer must independently generate and read the
current model in their own checkout, retaining its own identities, counts,
confidence, and artifact paths rather than reusing this publication.

#### Reviewer acceptance - 2026-11-16

Accepted coder commit `1dee493984675435a6d0262f9d0810ca3f4a7e81` with no
findings. The complete delta from the planning baseline changes only this issue;
application behavior, acceptance criteria, and implementation scope are unchanged.
No generated artifacts are committed. The historical generator exception remains
unexplained, but it did not reproduce during independent current-source generation.

- Executable: `C:\ws\plainionist\BlaXquad\bin\squad.exe`.
- Selected worktree: `C:\ws\plainionist\Plainion.DrawVista\.worktrees\reviewer`.
- Source revision: `1dee493984675435a6d0262f9d0810ca3f4a7e81`.
- Generation: `7742d8f0af9b495a9b02adbecfc20529`.
- Source fingerprint: `EB7C608791906A2B8406565410D94930EE8ABD96768A81187FDF3F10FA3BFD87`.
- Immutable JSON: `C:\ws\plainionist\Plainion.DrawVista\.worktrees\reviewer\.blaxquad\discovery\map\generations\7742d8f0af9b495a9b02adbecfc20529\model.json`.
- Immutable HTML: `C:\ws\plainionist\Plainion.DrawVista\.worktrees\reviewer\.blaxquad\discovery\map\generations\7742d8f0af9b495a9b02adbecfc20529\map.html`.

Independent `discover map --project` generation and all `discover map read`
packets below exited 0 using that executable and selected worktree. Reads used
the corresponding selectors in the coder table and `--page` after page 1.
The guarded overview confirms 91 places and 423 occurrences. Counts below are
the reviewer's per-packet total/shown/omitted evidence, not cumulative coverage.

| Element                     | Total | Packets: page = shown / omitted      | Next unread page |
| --------------------------- | ----- | ------------------------------------ | ---------------- |
| Overview                    | 514   | 1 = 20 / 494                         | 2                |
| DrawingWorkbookFactory      | 6     | 1 = 6 / 0                            | none             |
| DrawIOModel                 | 14    | 1 = 10 / 4; 2 = 3 / 11               | 3                |
| DrawIOApp                   | 15    | 1 = 12 / 3                           | 2                |
| IDocumentStore              | 22    | 1 = 20 / 2                           | 2                |
| IDocumentStore.GetPage      | 23    | 1 = 6 / 17                           | 2                |
| ProcessedDocument           | 29    | 1 = 11 / 18                          | 2                |
| Program                     | 76    | 1 = 7 / 69                           | 2                |
| SvgProcessor                | 27    | 1 = 14 / 13                          | 2                |
| FullTextSearch              | 13    | 1 = 9 / 4                            | 2                |
| SearchResults               | 54    | 1 = 20 / 34                          | 2                |
| MainLayout                  | 41    | 1 = 20 / 21; 2 = 20 / 21; 3 = 1 / 40 | none             |
| routes value                | 10    | 1 = 10 / 0                           | none             |
| SvgContainer                | 77    | 1 = 20 / 57                          | 2                |
| SvgContainer props contract | 2     | 1 = 2 / 0                            | none             |

These bounded reads independently support the recorded import/export,
page-contract and store, caption/link/search, and viewer/layout/router navigation
candidates. C# relationships remain `candidate`, `ambiguous`, or `unresolved`;
frontend references retain `literal`, `import-binding`, `lexical-binding`, and
`unresolved` confidence. Authored contracts are not execution or ownership proof.
Unread evidence remains available in the complete immutable pair; it is not
absent. No application tests or builds were run by the reviewer.

This acceptance completes only the current-design-evidence prerequisite.
Feature slicing, implementation ownership, and behavioral proof remain with the
architect; the Layers issue stays in planning.

The remaining Layers behavior is approved by the acceptance criteria above.
Its implementation slices, dependencies, candidate owners, and concrete behavioral
proof will be finalized after this prerequisite and before a feature assignment.

### Initial generation diagnostic

Command: `squad discover map`

Exit code: `-532462766`

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
