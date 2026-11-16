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

Current structural evidence is unavailable: `squad discover map` failed before
publishing a current design pair. No model-selected implementation owners or
resolved relationships have been established. Restore this prerequisite before
planning the feature's implementation boundaries; do not substitute implementation
scans or an earlier model for current architectural evidence.

### Slice 1 - Restore current design evidence

Stable plan ID: `layers-current-design-evidence`.

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

The remaining Layers behavior is approved by the acceptance criteria above.
Its implementation slices, dependencies, candidate owners, and concrete behavioral
proof will be finalized after this prerequisite and before a feature assignment.

### Generation diagnostic

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
