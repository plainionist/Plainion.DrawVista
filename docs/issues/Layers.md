---
title: Layers
priority: 10
status: planning
---

## Goal

i want to support layers like draw.io app does

## Acceptance Criteria

- support .drawio and .png (with drawio model) images
- we need a UI element showing layers with checkboxes to enable and disable layers
  - should be a drop down box in the toolbar right to "search"
  - when no layers area available the UI element should be dimmed so that user can realize easily
    whether layers exist for this page or not
- layers should then be shown as overlays to existing SVG
- these overlays need to work with zoom and pan as it would be one image

## Constraints

