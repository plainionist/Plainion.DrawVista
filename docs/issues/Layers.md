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

Progress: 1 of 3 currently planned slices accepted. The next coder assignment is
the current-main evidence prerequisite below. Layer support itself is not
implemented; slice 2 retains its stable ID and approved outcome.

### Accepted baseline

Slice 1, `layers-current-design-evidence`, was accepted without findings in
`00cd608ad9721c7975441a403d83feea82e4ce37` and is integrated into `main`.
The coder evidence commit is `1dee493984675435a6d0262f9d0810ca3f4a7e81`.
Assignment identity: `20261116T065946Z_000001_from_architect`.

Only the issue brief changed. Fresh design generation and bounded reads succeeded
independently in both worker checkouts without application changes, external tool
edits, or safeguard bypasses. The original exception did not reproduce; its cause
remains unverified. The accepted issue revision above retains the full diagnostic,
artifact references, model selectors, packet counts, confidence, and acceptance
evidence; the completed brief is retired here.

The accepted overview reports 91 places and 423 occurrences: total 514, shown 20,
omitted 494 on page 1. Reviewer generation `7742d8f0af9b495a9b02adbecfc20529`
and its source fingerprint
`EB7C608791906A2B8406565410D94930EE8ABD96768A81187FDF3F10FA3BFD87`
are historical references, not current-main structural authority.

### Current-main generation evidence

On 2026-11-16, generation at brief commit `2afa9d2` reproduced
`System.IO.InvalidDataException: Invalid authored contract.` in
`DesignReader.ValidatePart` at line 215, followed by three recursive frames at
line 230. The command was:

```powershell
& 'C:\ws\plainionist\BlaXquad\bin\squad.exe' discover map --project 'C:\ws\plainionist\Plainion.DrawVista'
```

Generation exited `-532462766`. Guarded reading exited 1 with
`MAP Missing`: the shared checkout has no `map\current.json`.
The full common exception stack is also retained in the accepted historical
issue revision. Current commands, output and input diagnostics are preserved in
`C:\Users\ssmosett\.copilot\session-state\70018c83-7287-4475-8bf1-7628dcb44d9e\files\layers-main-model-diagnostic.txt`.

Two distinct controlled comparisons failed with the same exception:

- Preserving the closed generated index and regenerating without it did not fix
  the failure. The old index had 36 declaration inputs and zero design inputs;
  the new index has zero of each. Cached design reuse is not necessary to
  reproduce the failure. Both historical worker indexes have 55 design inputs.
- Read-only input comparison found six main files differing from both successful
  worker inputs solely by CRLF versus LF, totaling 70 extra CR bytes. A backed-up
  LF-only comparison required every normalized file to match its committed Git
  blob exactly. Generation still failed; every original byte buffer was restored.
  Thus these newline differences are not a sufficient explanation.

The six original inputs are `src\Plainion.DrawVista.Tests\JsonExtensions.cs`,
`src\Plainion.DrawVista\GlobalConst.cs`,
`src\Plainion.DrawVista\IO\DrawIOModel.cs`,
`src\Plainion.DrawVista\IO\DrawingWorkbookFactory.cs`,
`src\Plainion.DrawVista\UseCases\Documents.cs`, and
`src\Plainion.DrawVista\UseCases\SvgExtensions.cs`.
Git now reports their original newline representations as modified after the
comparison; exclude them from planning commits and do not clean them for handoff.
Their original bytes remain backed up in the session's
`files\layers-source-input-before-lf` directory. The old generated index is
preserved as `files\layers-discovery-index-before-regeneration.sqlite3`.

No application implementation or external tool was changed, and no validation
safeguard was bypassed. The offending input and underlying cause remain
unverified. Worker success, cache replacement, and newline normalization do not
close this current-main prerequisite.

### Slice 1b - Restore current-main design evidence

Stable plan ID: `layers-current-main-design-evidence`. Depends on accepted
`layers-current-design-evidence`; blocks `layers-support`. This is a new,
bounded prerequisite, not a reopening of the accepted worker evidence slice.

- **Outcome:** Resolve the reproducible main-checkout generation prerequisite so
  a freshly generated, guarded-readable current-main model is available for
  Layers planning.
- **Start:** This issue, accepted baseline `00cd608ad9721c7975441a403d83feea82e4ce37`,
  the current-main diagnostic above, and the explicit CLI and selected checkout
  in its command. Historical worker input inventories are diagnostic comparison
  points only, not architectural authority. The exception does not identify its
  offending input; do not assume that another successful worker run resolves it.
- **Change:** Coder prepares their own clean, exclusively owned checkout once
  with `squad sync-to-base --merge <handed-off-commit>`. Identify the smallest
  failing input or input/environment distinction, using supported discovery,
  bounded diagnostics and a faithful private reproduction rather than another
  blind retry. Ordinary implementation inspection for diagnosing the failure
  belongs to coder, not architectural structural inference. Do not change
  application behavior merely to appease the generator.
- **Recovery ownership:** Architect executes any justified shared-checkout
  recovery; coder must not synchronize or edit shared source. Keep source and
  generated-state recovery reversible and preserve unrelated work. No external
  CLI edits, safeguard weakening, copied historical model, or unsupported
  publication are authorized. If a concrete external-tool correction is required,
  report the exact failing input, reproduction and required prerequisite to
  architect before attempting it.
- **Evidence:** Commit the decisive diagnosis and minimal recovery instructions
  in this issue, replacing superseded hypotheses. Preserve full failure output
  and reproduction references. Include any necessary repository-local correction
  and its directly affected documentation and proof; exclude Layers feature
  implementation. A diagnosis or approved experiment alone is not acceptance.
- **Preserve:** Every acceptance criterion, both stable existing plan IDs, the
  accepted worker evidence, current application behavior, and original pending
  source bytes. Keep Layers unavailable. Never stage the six original
  newline-representation differences merely to obtain a clean handoff.
- **Prove:** The explicit current-main generation command above exits 0 and
  publishes a complete immutable pair. Guarded `discover map read --project`
  succeeds for that selected checkout, with generation, source fingerprint,
  paths, confidence, and per-packet total/shown/omitted/NEXT retained. Read the
  overview and bounded import, page/storage, SVG/caption/link/search, viewer and
  toolbar elements needed by slice 2. Reviewer independently verifies this
  current-main evidence; successful private-checkout generation alone is
  insufficient. If application source changes are genuinely necessary, also
  require the smallest affected existing tests and build. Failed current-main
  proof returns to diagnosis/recovery, not feature assignment or acceptance.

### Slice 2 - Layers support

Stable plan ID: `layers-support`. Depends on accepted
`layers-current-design-evidence` and unresolved
`layers-current-main-design-evidence`. This approved outcome remains unassigned;
refine its implementation boundaries and concrete proof after the current-main
prerequisite, before committing its coder brief.

- **Outcome:** Fulfill all Layers acceptance criteria above, including both import
  formats, legacy-page compatibility, rendering and interaction, per-page
  browser-tab preferences, and search.
- **Start:** The accepted baseline, `README.md` for product intent and import naming,
  and `src\Plainion.DrawVista\WebUI\README.md` for frontend tooling. Authored
  Gherkin discovery completed with zero scenarios and zero feature files; this
  does not establish absence of other tests. The issue is behavioral authority.
- **Preparation:** Generate a fresh model in the shared main checkout using
  `C:\ws\plainionist\BlaXquad\bin\squad.exe`, then consume bounded overview and
  element packets with counts, confidence, and continuation pages retained.
  Historical candidate anchors are `DrawingWorkbookFactory`, `DrawIOModel`,
  `DrawIOApp`, `ProcessedDocument`, `IDocumentStore`, `SqliteDocumentStore`,
  `SvgProcessor`, `FullTextSearch`, `Program`, `SvgContainer`, `MainLayout`, and
  `SearchResults`. They do not establish resolved ownership, runtime calls,
  browser-session lifetime, or test coverage.
- **Unresolved technical decisions:** Layer extraction and shared framing;
  backward-compatible persistence/API contracts; composite SVG, captions and
  links; toolbar and tab-state ownership; and sufficient existing test entry
  points. Resolve these through current model and documentation evidence before
  assignment, without substituting implementation scans for architectural
  structure. Coder inspection and independent reviewer verification remain
  necessary.
- **Preserve:** The agreed scope and every acceptance criterion. Technical
  refinement or cohesive slicing must not change product approval, expose an
  unfinished path, or defer required compatibility and behavioral proof.
