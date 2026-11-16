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
safeguard was bypassed. The failing generated input and valid-literal rejection
are now isolated below. Current-main recovery and publication remain blocked;
neither the historical worker evidence nor private diagnostic models close this
prerequisite.

#### Coder diagnosis - 2026-11-16

**Status: diagnosed, not accepted.** The generator rejects meaningful
whitespace-only JavaScript literal fragments. Main contains generated frontend
inputs absent from the coder checkout; an isolated copy of one of these inputs
reproduces the same failure without cached design state.

The verified failing input is
`src\Plainion.DrawVista\wwwroot\assets\runtime-core.esm-bundler-CyXb5kU7.js`:
64833 bytes, SHA256
`413ca7c7e3f0b212f964c9e74b3e7ebbec85e10e338d209dbb579bc757e38db3`.
Its `Ca` function contains a single-space template literal default.
The meaningful space is at line 1, UTF-16 column 60421,
zero-based byte/UTF-16 offset 60420. The exact signature is:

```javascript
function Ca(e=` `,t=0)
```

`git check-ignore -v` identifies `.gitignore:2`, `**/wwwroot/**`, for this
asset. The repository already excludes generated output from Git; changing
application source or adding another Git ignore rule is not the correction.
The earlier repository-root glob omitted ignored assets; querying the known
`wwwroot\assets` directory directly found ten JavaScript files.

All diagnosis writes were confined to the coder's checkout under
`C:\ws\plainionist\Plainion.DrawVista\.worktrees\coder\.blaxquad\diagnostics\layers-current-main-design-evidence`.
Main files were read and copied, never edited. Each copy was byte-verified;
the manifests record source paths, sizes, and SHA256 values.

| Private diagnostic                   | Inputs                                           | Result                                                                                                   |
| ------------------------------------ | ------------------------------------------------ | -------------------------------------------------------------------------------------------------------- |
| Cold DocumentBuilder copy            | 1 current-main source file                       | Generation exits 0; overview total/shown/omitted = 10/10/0, NEXT none                                    |
| Cold historical comparison inventory | 55 paths, all copied from current-main bytes     | Generation exits 0; 83 places, 423 occurrences; overview page 1 total/shown/omitted = 506/20/486, NEXT 2 |
| Cold generated assets copy           | 10 current-main JavaScript assets                | Same `Invalid authored contract` exception                                                               |
| Bisection first group                | 5 generated assets                               | Generation exits 0                                                                                       |
| Bisection second group               | 3 other generated assets                         | Generation exits 0                                                                                       |
| Isolated Vue runtime copy            | 1 generated asset, identified above              | Same exception and recursive validation stack; no publication                                            |
| Single-space quoted default          | 1 minimal JavaScript file                        | `node --check` exits 0; generation fails                                                                 |
| Letter quoted default                | Same syntax, only space replaced with `x`        | `node --check` and generation exit 0                                                                     |
| Single-space template default        | 1 minimal JavaScript file                        | `node --check` exits 0; generation fails                                                                 |
| Letter template default              | Same syntax, only space replaced with `x`        | `node --check` and generation exit 0                                                                     |
| In-place template control            | Same project and file path; only byte 29 changes | `x` exits 0; restoring the space reproduces the exception; newline bytes unchanged                       |

The 55-path comparison is deliberately incomplete as a checkout model: it
omits project metadata and the additional generated assets. Its successful
generation is input-diagnosis evidence, not current-main structural authority.
One other generated asset was not tested separately; identifying
one independently failing input was sufficient for this diagnosis.

Minimal failing template input:

```javascript
function sample(separator = ` `) {}
```

The quoted form, `function sample(separator = ' ') {}`, also fails.
For each form, changing only the space to `x` makes generation succeed.
Run each input in its own new private source directory without generated state:

```powershell
node --check '<private-fixture>\sample.js'
& 'C:\ws\plainionist\BlaXquad\bin\squad.exe' discover map --project '<private-fixture>'
```

The failing generation exits `-532462766` (Node records its unsigned
representation `3762504530`) with the same common exception stack retained
above. Both passing controls were consumed with bounded `map read sample`
packets: total 2, shown 2, omitted 0, NEXT none, relationships 0.
Their authored contracts expose
`formal_parameters -> assignment_pattern -> string/template_string -> string_fragment`,
with the non-space fragment represented as `Tokens: ["x"]`.
The supported CLI fails for both whitespace-literal inputs with
`Invalid authored contract`. The controlled one-byte changes isolate valid
literal text, not missing names, source newline differences, or an invalid
JavaScript program, as the rejected input distinction. These are CLI and
syntax observations, not resolved application relationships or ownership
evidence. The internal extraction/validation defect remains unverified within
this scope; the diagnosis does not rely on generator implementation claims.

The template control was also toggled in place at the identical project and
source-file path. The `x` input passed with zero cached design inputs beforehand.
Restoring only byte 29 from `x` to a space failed again, even with the previous
`x` input cached. Both JavaScript syntax checks passed; the 37-byte file length
and every CRLF byte were preserved. This separates the literal-content
distinction from different fixture paths or warm-cache success.

Full failure output and reproduction manifests are retained in the diagnostic
directory named above:

- `wwwroot-assets-generation.txt` and `wwwroot-assets-manifest.json`.
- `assets-bisect-03-generation.txt` and `generated-asset-bisection.json`
  identify the byte-exact single failing input.
- `space-default-generation.txt`, `letter-default-generation.txt`,
  `template-space-default-generation.txt`, and
  `template-letter-default-generation.txt` retain the syntax and generation
  results for all four minimal controls.
- `default-parameter-comparison.json` and `template-default-comparison.json`
  retain the controlled results and exact failing literal location.
- `same-root-letter-generation.txt`, `same-root-space-generation.txt`, and
  `same-root-literal-comparison.json` retain the in-place one-byte comparison.
- `all-current-main-inputs-generation.txt` and
  `all-current-main-inputs-manifest.json` retain the cold source comparison.

Experiment-only fixture directories and their generated models are removed
before handoff; the failure logs, manifests, minimal source above, and recorded
packet observations remain. No diagnostic model is committed or offered as
acceptance evidence.

**Required recovery ownership:**

1. Architect forwards the exact asset and minimal controls to the authorized
   generator owner. Resolve the supported CLI's rejection of valid
   whitespace-literal defaults without losing literal meaning or weakening
   structural validation.
2. Do not rewrite generated JavaScript, alter application behavior, relocate
   live shared assets merely to make discovery pass, or copy a worker model.
   No external CLI edit is authorized in this coder slice.
3. After an authorized supported correction is available, architect executes
   the explicit current-main generation command and the required guarded
   overview/element reads. Reviewer independently verifies that current-main
   evidence before slice 1b can be accepted or Layers can be assigned.

The read-only current-main `discover map read --project` check still exits 1
with `MAP Missing`; no current-main generation or shared-checkout recovery was
performed by coder. All six protected main inputs were checked against the
initial read-only manifest and remain byte-identical. Application tests,
frontend checks, builds, and the full suite were not run because application,
build, and test code are unchanged. JavaScript syntax checks and controlled
generator probes cover the diagnosed input boundary, not Layers behavior.

### Slice 1b - Restore current-main design evidence

Stable plan ID: `layers-current-main-design-evidence`. Depends on accepted
`layers-current-design-evidence`; blocks `layers-support`. This is a new,
bounded prerequisite, not a reopening of the accepted worker evidence slice.

- **Status:** Blocked; external generator recovery is pending. Not accepted.
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
  failing input or input/environment distinction using supported CLI behavior,
  diagnostics, DrawVista input evidence and a faithful private reproduction
  rather than another blind retry. Any implementation inspection must be limited
  to DrawVista-authored source and a concrete input question. Do not inspect
  BlaXquad implementation; exception source paths are diagnostic output, not
  reading anchors. Do not change application behavior merely to appease the
  generator.
- **Recovery ownership:** Architect executes any justified shared-checkout
  recovery; coder must not synchronize or edit shared source. Keep source and
  generated-state recovery reversible and preserve unrelated work. No external
  CLI source inspection or edits, safeguard weakening, copied historical model,
  or unsupported publication are authorized. If supported diagnostics cannot
  resolve the failure, report the exact commands/output, established facts,
  remaining unknowns and required tool prerequisite to architect. Do not expand
  the investigation into BlaXquad sources.
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

#### Reviewer disposition - 2026-11-16

Reviewed `ed9d1cc0df9f20455b93bd35b1f5219e65362941` and the complete correction
delta through `3726098ef880a30139cb311b0a62da0783edd787`. The retained isolated-asset
failure, literal-comparison manifests, and same-path control logs support the
diagnosis and its external recovery boundary. Independently checked the named
main asset: its byte count, SHA256, and space at UTF-16 offset 60420 match the
brief. External-tool implementation was not independently inspected.
The handed-off change is issue-only; no application tests were run.

**R1 - High / blocking: current-main structural proof is unavailable.**

- **Location:** This issue's Slice 1b **Outcome** and **Prove**; selected checkout
  `C:\ws\plainionist\Plainion.DrawVista`.
- **Violated outcome:** No complete, freshly generated and guarded-readable
  current-main model is available. The reviewer's independent
  `discover map read --project 'C:\ws\plainionist\Plainion.DrawVista'` exited 1
  with `MAP Missing` for `.blaxquad\discovery\map\current.json`.
- **Root cause:** The recorded generated-asset projection fails; isolated valid
  whitespace-literal inputs reproduce that failure while one-byte letter
  controls pass. No authorized generator correction or current-main recovery
  has yet supplied the required publication. Private comparison models cannot
  substitute for the selected checkout's structural evidence.
- **Required outcome:** Architect coordinates the authorized generator-owner
  correction and shared-checkout recovery described above, then supplies fresh
  current-main generation and guarded overview/element evidence for independent
  reviewer verification. Preserve literal data, validation safeguards, pending
  main bytes, and the approved feature scope.

R1 blocks slice acceptance, not delivery of this diagnosis to the external
owner. Do not implement Layers
or edit the external CLI under this assignment. Slice 1b remains incomplete;
no accepted-result integration is authorized.

**R2 - Medium: remove implementation-derived diagnostic evidence (resolved).**

- **Location:** Coder diagnosis, the sentence asserting that
  `DesignReader.ValidatePart` rejects whitespace-only tokens at line 215.
- **Violated scope:** The coder's subsequent scope-correction handoff excludes
  external implementation inspection as authorized evidence. CLI exception
  locations and controlled input observations remain usable.
- **Root cause:** That sentence was derived from an earlier external source read,
  rather than the permitted DrawVista inputs and CLI output.
- **Required outcome:** Coder replaces the implementation-condition assertion
  with the observed invalid-contract failure and controlled whitespace/letter
  behavior. Preserve the decisive reproductions and recovery ownership without
  further external implementation inspection or edits.

Resolved in `3726098ef880a30139cb311b0a62da0783edd787`: the diagnosis now relies
on CLI failures and controlled inputs, explicitly leaves the internal defect
unverified, and limits investigation to the authorized DrawVista/CLI boundary.
The correction changes only this issue. R1 remains open; neither the corrected
diagnosis nor its private probes satisfies slice 1b's current-main model outcome.

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
