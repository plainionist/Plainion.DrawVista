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

The operator's decision on 2026-11-16 removes `discover map` from Layers work.
Do not generate, repair, or require a design model for implementation or acceptance.
Proceed with one end-to-end feature slice, including implementation, coverage,
and directly affected documentation. No discovery-tool prerequisite remains.

Progress: slice 2, `layers-support`, assignment
`20261116T100315Z_000021_from_architect`, is complete and accepted in code review.
The accepted implementation is `d9425121d8edca191bfbb3985042bb7ddcd66b08`.
Final actual-browser verification remains pending with the operator; issue
retirement belongs to the architect.

### Accepted baseline and withdrawn prerequisite

Slice 1, `layers-current-design-evidence`, remains accepted in
`00cd608ad9721c7975441a403d83feea82e4ce37`, with coder evidence
`1dee493984675435a6d0262f9d0810ca3f4a7e81` and assignment identity
`20261116T065946Z_000001_from_architect`. It changed documentation only;
it is not acceptance of Layers behavior. Historical models are not current-source
authority and need not be consumed.

Slice 1b, `layers-current-main-design-evidence`, assignment identity
`20261116T073037Z_000002_from_architect`, is withdrawn, not accepted.
The operator explicitly excludes **R1**, unavailable current-main model proof,
from Layers acceptance. The generator failure is not claimed fixed. **R2**
remains independently resolved in coder revision
`3726098ef880a30139cb311b0a62da0783edd787` and reviewer disposition
`dd64163993d4b0be02c5d045615c98f49a871d0e`.

Existing diagnosis and dispositions remain preserved in those commits and
`b3fb0b0c4a277dfaa25b99e948b6d36da37abdbf`; full diagnostic artifacts remain at
`C:\ws\plainionist\Plainion.DrawVista\.worktrees\coder\.blaxquad\diagnostics\layers-current-main-design-evidence`.
Do not repeat probes, inspect or repair BlaXquad implementation, recreate the
deleted `SquadDiscoverMap.md`, or issue further diagnostic-document corrections.
The old tool-build, model-publication, report-migration, and historical preparation
requests are superseded.

### Slice 2 - Layers support

Stable plan ID: `layers-support`. The accepted baseline is retained, but there is
no dependency on the withdrawn slice 1b. This is one cohesive importer-to-viewer
contract: do not split its producer, storage, search, or user-facing consumers into
partially working deliveries.

**Outcome:** Import both supported formats into a backward-compatible,
layer-aware SVG page and let users independently toggle its layers with correct
alignment, links, search, and browser-tab preferences.

**Start:** This issue is behavioral authority. `README.md` defines product intent
and existing import naming; `src\Plainion.DrawVista\WebUI\README.md` defines
frontend commands and supported dependencies.

Current focused navigation located the following implementation candidates;
these are reading anchors, not proof of runtime ownership. Coder verifies actual
contracts in source; reviewer independently verifies the implementation:

- Import: `src\Plainion.DrawVista\IO\DrawingWorkbookFactory.cs`,
  `DrawIOWorkbook.cs`, `DrawIOPngWorkbook.cs`, `DrawIOModel.cs`, and `DrawIOApp.cs`.
- Page processing and storage: `src\Plainion.DrawVista\UseCases\Documents.cs`,
  `SvgProcessor.cs`, `IDocumentStore.cs`, and `IO\SqliteDocumentStore.cs` and
  `IO\FileSystemDocumentStore.cs`; check the caching decorator and `Program.cs`
  where their existing contracts are affected.
- Captions and links: `src\Plainion.DrawVista\Adapters\SvgCaptionParser.cs`,
  `SvgHyperlinkFormatter.cs`, and `UseCases\FullTextSearch.cs`.
- Viewer: `src\Plainion.DrawVista\WebUI\src\components\SvgContainer.vue`,
  `SearchResults.vue`, `views\BrowserView.vue`, and `layouts\MainLayout.vue`.
- Existing regression anchors: `src\Plainion.DrawVista.Tests\SqliteDocumentStoreTests.cs`
  and `StartPageTests.cs`; use existing backend and frontend runners.

**Change:**

1. Read page-specific layer identity, name, source order, and saved visibility
   from the embedded draw.io model, including initially hidden content. Reuse both
   existing workbook paths and preserve their page naming and selection behavior.
   The importer owns immutable source layer metadata, not viewer preferences.
2. Produce one self-describing SVG per page: layer content belongs to distinct
   SVG groups, ordered as in draw.io, carrying importer-owned layer ID, name, and
   source-visibility metadata. Embed this metadata in the stored SVG so legacy
   pages and existing storage/API shapes need no new tables or layer endpoints.
   All groups share one fixed all-layer coordinate system and viewport. Include
   hidden content when establishing bounds, never retain a duplicate base image,
   and ensure merged SVG IDs, definitions, references, and links remain valid.
   Reuse the existing draw.io exporter. Exact grouping/export mechanics are
   implementation details: an isolated temporary model or per-layer exports are
   acceptable if they preserve source bytes and prove the common framing.
   Clean up temporary files and surface export failures through existing patterns.
3. Preserve the complete SVG and its layer metadata through caption parsing,
   generated-link processing, storage round trips, caching, and delivery. Index
   captions from every layer independent of source visibility. An SVG without
   importer-owned layer metadata follows the existing legacy rendering path.
4. Add the checkbox dropdown immediately right of Search, using existing Quasar
   and localization patterns. Keep it visible and disabled for legacy pages or
   pages containing only the default layer. The enabled list includes the default
   layer and follows source order. Toggle group visibility in the existing SVG;
   do not reload the page, re-export, replace the pan/zoom instance, or recompute
   framing. Hidden groups must not intercept clicks; all-off leaves no content.
5. Keep selections in `sessionStorage`, scoped by the existing stable page
   identity and layer IDs. Apply saved selections when available; otherwise use
   source visibility. Newly encountered layer IDs use their source visibility.
   Navigation and refresh restore only that page's choices. No preference writes
   reach the source files or server. Search navigation uses the same page-state
   path and must not reveal a matched layer automatically.
6. Include required regression fixtures, tests, and concise user documentation
   for toggling, per-tab persistence, and re-uploading legacy pages. Deliver actual
   feature code and its proof together; no further tool/evidence-only slice.

**Preserve:** Every acceptance criterion and read-only constraint above; both
import paths, existing stored pages, generated and external links, search result
navigation, and supported build/tooling behavior. No layer editing, source
write-back, backfill, external-tool work, or unrelated cleanup. Leave the shared
checkout's six pre-existing newline-representation differences untouched and
unstaged; they are not part of the plan commit.

**Prove:** Use a fixture with a default layer, another visible layer, an initially
hidden layer extending beyond the visible bounds, overlapping content to expose
stacking mistakes, captions, and both generated and external links. Include a
second page with different layer settings, plus a legacy stored SVG.

The operator reserves final actual-browser verification. Reviewer checks the
implementation against every requested behavior and independently runs the
available automated checks. Execution of the interactive browser scenarios below
is not a prerequisite for the implementation commit, qualifying reviewer handoff,
reviewer code acceptance, or local integration. Do not claim those scenarios
passed when they have not been executed. Edge headless/remote-debugging policy
must not be bypassed or turned into another team prerequisite.

Use these observable scenarios for automated regression coverage where available
and the operator's final browser verification:

1. Import the `.drawio` and embedded-model `.png` equivalents through their real
   workbook paths. Assert existing names and selected pages, layer names/order,
   saved visibility, hidden geometry, and unchanged input bytes. Reload stored
   pages through both supported stores and confirm SVG metadata and captions
   survive; legacy pages render unchanged without metadata.
2. In the viewer, check toolbar placement, disabled legacy/default-only states,
   source defaults, and independent checkbox changes including the default layer.
   Enable initially hidden outlying content and confirm it is aligned and
   unclipped; overlap demonstrates source stacking. Disable everything and
   confirm an empty, non-clickable canvas without duplicate content.
3. Establish non-default zoom and pan before toggling. Assert the same SVG
   viewport and pan/zoom transform afterward, not merely that checkboxes change.
   Generated and external links work while visible and cannot activate while
   their layer is disabled.
4. Choose different selections on the two pages, navigate away and back, and
   refresh. Assert independent restored choices. A fresh tab/session starts from
   source visibility, and server/source state is unchanged.
5. Search for a caption present only in the initially hidden layer, with that
   layer disabled. Assert it is indexed and navigates to the existing target page
   without changing remembered/source layer selections.

Add automated import/composition/store/caption and preference-state coverage in
existing runners. Keep the fixtures and browser verification entry points usable
for the operator; mocks and static checks must not be reported as executed browser
verification.
Run the smallest targeted backend selection covering the change and existing
link/store/navigation regressions, `pnpm check` from
`src\Plainion.DrawVista\WebUI`, and a Release build of
`src\Plainion.DrawVista\Plainion.DrawVista.csproj`.
Report exact commands, results, and the explicitly unexecuted browser scenarios
with the implementation handoff; do not wait for final operator verification or
cycle through issue-history-only commits.

**Execution:** Coder preparation against
`33df289321e2bcc44b6a66ac7062db6c9e4faed9` is already complete in merge
`19890e501f1ace35f99381f31e4ece47d6539f85`. Do not repeat preparation or
synchronize a dirty checkout for this plan-only amendment; read its committed
identity without checkout mutation. Preserve all task-owned code and unrelated
work. No shared-checkout mutation, historical preparation, or reopening of
slice 1b is authorized.

Coder's next action is to commit the actual feature implementation, fixtures,
tests, and user documentation and hand that implementation to reviewer under
the existing slice identity. Ordinary commit and handoff safeguards remain.
The architect's plan amendment does not itself activate review.

Reviewer prepares its own clean private checkout once with
`squad sync-to-base --merge <qualifying-implementation-commit>`, then reviews
feature code against this issue and independently checks the available regression
evidence. Do not require `discover map`, reopen excluded R1, or block code review
on operator-owned final browser verification. Reviewer acceptance and normal
local integration remain required; final browser verification belongs to the
operator and must not be claimed complete by the team.

### Reviewer disposition - slice 2

Reviewed implementation `ee5625145d77f057641e3a10fc6553617451792a` against
every acceptance criterion and reviewed the complete correction through
`d9425121d8edca191bfbb3985042bb7ddcd66b08`. Under proof-owner amendment
`494b944bf17b2eea474e9f4b4758642e04491227`, slice 2 is accepted with no
outstanding findings. R1 remains excluded; R2, R3, and R4 are resolved.
This is code-review acceptance, not a claim of completed operator browser proof.

Reviewed the retained coder output in
`C:\Users\ssmosett\.copilot\session-state\34c4b3e2-c5c8-48d3-b1d0-221e9f7456be\files\layers-support-checks.txt`:
the final nine layer tests, five frontend tests with lint/type checking, and
Release build passed; 26 unchanged impact regressions passed in the initial
selection. No tests were run by reviewer. All five actual-browser scenarios remain
unexecuted and operator-owned; the preserved Edge policy failure is not an
acceptance finding.

Correction evidence is retained in the same directory's
`layers-review-r3-r4-console.txt`, copied from the existing captured tool output,
with the original invocation/times in `layers-review-r3-r4-checks.txt`.
Both Node version probes report `v22.13.0`; `pnpm check` passes ESLint,
Vue/TypeScript checking, and all five tests with exit 0. The corrected source
matches the accepted implementation. Unchanged backend/build evidence is reused;
no validation rerun or browser launch was performed to recover the console output.

**R3 - Medium (resolved): frontend tests failed on the supported minimum Node version.**

- **Location:** `src\Plainion.DrawVista\WebUI\package.json:12,47` and
  `tests\layers.test.cjs:20,54,74`.
- **Violated behavior:** Preserve the documented Node 22.13+ toolchain and the
  working `pnpm check` runner.
- **Root cause:** The new tests import `svgLayers.ts` directly, but `node --test`
  does not enable TypeScript support on Node 22.13-22.17. Type stripping became
  enabled by default in Node 22.18, so the existing supported minimum fails these
  imports with `ERR_UNKNOWN_FILE_EXTENSION`. Passing checks on a newer runtime
  do not cover that compatibility requirement.
- **Required outcome:** Run the TypeScript-based tests compatibly across the
  existing supported Node range without raising its minimum. Provide focused
  minimum-version compatibility evidence with the corrected check command.
- **Resolution:** The test runner explicitly enables `--experimental-strip-types`;
  the supported minimum is unchanged and the retained Node 22.13.0 output proves
  the corrected `pnpm check` command passes.

**R4 - Medium (resolved): browser regressions waited for intentionally hidden content.**

- **Location:** `src\Plainion.DrawVista\WebUI\tests\browser\layers.test.cjs:67-71`,
  `416-423`, and `477-486`.
- **Violated behavior:** Keep the operator's browser verification entry points
  usable for remembered page choices and hidden-caption search.
- **Root cause:** `selectPage` uses Playwright's default visible-state
  `waitFor()` on the selected cell. The persistence scenario disables Background,
  then returns to Layers and waits for `background-box` to become visible.
  The hidden-caption search scenario makes the same visible-state wait after
  navigation. Correctly restored choices keep that cell hidden, so both scenarios
  time out before asserting the required behavior.
- **Required outcome:** Detect completion of navigation/mounting independently
  of the chosen layer's visibility, while retaining the assertions that saved
  hidden choices remain hidden. Correct these waits without executing prohibited
  launches or treating final operator browser verification as a team gate.
- **Resolution:** Page, SVG, and viewport readiness waits use DOM attachment,
  independent of selected-layer visibility; hidden-caption navigation also waits
  for its viewport and settles. The remembered-choice and hidden-content
  assertions remain unchanged. This correction was independently reviewed and
  linted, not reported as executed browser verification.
