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

Progress: 1 of 2 currently planned slices accepted; no active slice. Layer support
itself is not implemented.

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

### Slice 2 - Layers support

Stable plan ID: `layers-support`. Depends on accepted
`layers-current-design-evidence`. This approved outcome is unassigned; refine its
implementation boundaries and concrete proof before committing a coder brief.

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
