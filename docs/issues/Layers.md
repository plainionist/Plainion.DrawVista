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

Progress: 1 of 3 currently planned slices accepted. The current-main prerequisite
is diagnosed but unaccepted and requires a corrected external tool build. Layer
support itself is not implemented; slice 2 retains its stable ID and approved
outcome.

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

The shared checkout cannot publish a current design model:
`squad discover map` fails with `Invalid authored contract`, and guarded reading
reports `MAP Missing`. Coder isolated a valid whitespace-literal case in the
ignored generated input
`src\Plainion.DrawVista\wwwroot\assets\runtime-core.esm-bundler-CyXb5kU7.js`,
absent in the original worker checkout. Its byte-exact cold single-file copy
reproduces the same exception: 64833 bytes, SHA256
`413ca7c7e3f0b212f964c9e74b3e7ebbec85e10e338d209dbb579bc757e38db3`.
The literal is at line 1, UTF-16 column 60421.

Minimal ``function sample(separator = ` `) {}`` passes `node --check` but fails
cold map generation. Replacing only the space with `x` passes generation.
Quoted-string whitespace controls reproduce the rejection. All 55 historical
comparison paths copied from current main bytes pass cold generation, but that
selected diagnostic set omits the failing generated asset and is not
current-main authority.

Decisive logs, manifests, asset bisection and four literal controls are retained
at `C:\ws\plainionist\Plainion.DrawVista\.worktrees\coder\.blaxquad\diagnostics\layers-current-main-design-evidence`.
Read `template-default-comparison.json`, `default-parameter-comparison.json`,
`generated-asset-bisection.json` and their named generation logs. The behaviour is
isolated; the tool's implementation mechanism and a successful main recovery
remain unverified.

Corrected coder diagnostic commit `3726098ef880a30139cb311b0a62da0783edd787`
supersedes `ed9d1cc0df9f20455b93bd35b1f5219e65362941`, removing the
implementation-derived validator claim. Only independent DrawVista input, syntax
and supported CLI observations are used as evidence. Its same-path control changes
only byte 29 of a 37-byte fixture: `x` passes, restoring the space fails, and
syntax checks and newline bytes remain unchanged. This also isolates the literal
distinction from fixture paths and warm-cache success. Temporary fixture models
were removed before handoff; logs and manifests remain. This diagnostic commit
contains no successful current-main publication and does not accept slice 1b.

Independent reviewer disposition `dd64163993d4b0be02c5d045615c98f49a871d0e`
confirms the corrected diagnostic evidence. **R1 - High/blocking** remains open:
current-main structural proof is unavailable; the independent guarded main read
exits 1 with `MAP Missing`. **R2 - Medium**, the implementation-derived evidence
finding, is resolved in `3726098ef880a30139cb311b0a62da0783edd787`.
That correction does not satisfy R1 or accept this slice.

The affected current-main command is:

```powershell
& 'C:\ws\plainionist\BlaXquad\bin\squad.exe' discover map --project 'C:\ws\plainionist\Plainion.DrawVista'
```

The former report is retained historically in
`6ed4dc75431d8417462441bced4f4c51b1c40562`; its removal in
`b8cfd061fe9ee917d366017bf9b437f083f861ae` is preserved. No report is recreated.
Original source bytes are intact; the six protected newline representations
remain unstaged and must be excluded from planning commits. Accepted worker and
diagnostic models do not close this current-main prerequisite.

The report-migration amendment at `6ed4dc7` is withdrawn. Worker synchronization
refusals left coder at `3726098ef880a30139cb311b0a62da0783edd787` and reviewer at
`dd64163993d4b0be02c5d045615c98f49a871d0e`, with restoration verified and
diagnostics preserved. Receiving or inspecting diagnostic-only briefs and
dispositions does not require checkout mutation. Do not retry these superseded
preparations or bypass their refusals with native recovery; necessary future
preparation must use the then-current authorized baseline.

### Slice 1b - Restore current-main design evidence

Stable plan ID: `layers-current-main-design-evidence`. Depends on accepted
`layers-current-design-evidence`; blocks `layers-support`. This is a new,
bounded prerequisite, not a reopening of the accepted worker evidence slice.

- **Outcome:** Resolve the reproducible main-checkout generation prerequisite so
  a freshly generated, guarded-readable current-main model is available for
  Layers planning.
- **Start:** This issue, accepted baseline `00cd608ad9721c7975441a403d83feea82e4ce37`,
  the decisive diagnostic references above, and the explicit CLI and selected
  checkout in the command. Current-main source selection includes the isolated
  generated asset; do not substitute the successful 55-input diagnostic model.
- **Change:** Supported input diagnosis is complete. The required external
  prerequisite is a corrected `squad` build from its owner, preserving meaningful
  whitespace string/template literal data and validation. This Layers assignment
  does not authorize tool-source inspection or repair. Do not repeat failed
  cache/newline experiments or rewrite, move or omit generated assets as a
  workaround. Coder preserves existing evidence; architect owns subsequent
  current-main generation and guarded reads when the corrected build is supplied.
  Any needed private-checkout preparation uses
  `squad sync-to-base --merge <handed-off-commit>` once from the owner's clean,
  exclusively owned checkout, preserving unrelated work.
- **Recovery ownership:** Architect executes any justified shared-checkout
  recovery; coder must not synchronize or edit shared source. Keep source and
  generated-state recovery reversible and preserve unrelated work. No external
  CLI source inspection or edits, safeguard weakening, copied historical model,
  or unsupported publication are authorized. No tool repair is assigned here.
  The deleted report remains deleted; retain the diagnosis in this brief and
  existing diagnostics instead.
- **Evidence:** Preserve existing commands/output, manifests, protected-input
  checks and literal controls. After a corrected build is supplied, record its
  identity and current-main generation/read evidence in this brief, without
  duplicating full logs or recreating the removed report. Route an existing
  qualifying evidence commit; no application implementation is required merely
  to deliver evidence. Diagnosis alone is not acceptance.
- **Preserve:** Every acceptance criterion, both stable existing plan IDs, the
  accepted worker evidence, current application behavior, and original pending
  source bytes. Keep Layers unavailable. Never stage the six original
  newline-representation differences merely to obtain a clean handoff.
- **Prove:** The corrected tool passes the four named literal controls with
  literal data retained. The explicit current-main generation command above exits
  0 and publishes a complete immutable pair. Guarded `discover map read --project`
  succeeds for that selected checkout, with generation, source fingerprint,
  paths, confidence, and per-packet total/shown/omitted/NEXT retained. Read the
  overview and bounded import, page/storage, SVG/caption/link/search, viewer and
  toolbar elements needed by slice 2. Reviewer independently verifies this
  current-main evidence; successful private-checkout generation alone is
  insufficient. Failed current-main proof leaves the tool prerequisite unresolved,
  not feature assignment or acceptance.

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
