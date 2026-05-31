# Figma Design

## Purpose

This document defines how to use the TacticalHeroes Figma file when
implementing UI.

Figma is the source of truth for visual layout, assets, typography, component
states, spacing, and screen composition.

Use the Figma plugin/MCP for general Figma access, metadata, screenshots,
assets, and design context. This document only records project-specific design
agreements and implementation rules.

Figma file:

```text
https://www.figma.com/design/EcA1f6Oy0drKGaIywbaGTQ/TacticalHeroes
```

## File Structure

The Figma file is organized by pages for different UI scenarios and includes
Assets used by the project.

Do not hardcode the list of pages, frames, or nodes in this document. The file
changes over time.

For each UI task, inspect the current pages, frames, components, variants,
Text Styles, and Assets through the Figma plugin/MCP.

## Unity Mapping

Do not create one Unity scene for every Figma page by default.

Use a UI Toolkit screen when the design is mostly UI. Use a Unity scene only
when the flow needs authored Unity objects, cameras, lighting, physics, or
scene-local systems. See `docs/architecture/navigation.md` and
`docs/unity/scenes.md`.

Figma pages and frames are design scenarios. Map them to screens, components,
or scene-backed flows according to runtime ownership.

## Implementation Workflow

Before changing UI:

1. Open the relevant Figma page or frame.
2. Inspect page/frame structure, component states, and Text Styles.
3. Export or reuse the required assets at sufficient quality.
4. Implement UXML, USS, and C# using `docs/unity/ui-toolkit.md`.
5. Compare against Figma at the target reference size.
6. Verify responsive behavior on compact phone, tablet, and desktop ratios.

## Pixel Matching

When implementing a designed screen, match Figma as closely as possible:

* Layout, spacing, alignment, and visual hierarchy.
* Backgrounds, overlays, gradients, borders, shadows, and opacity.
* Typography, letter spacing, text shadows, and line height.
* Component states such as default, hover, selected, pressed, focused, disabled.

Use exact dimensions from the reference frame as the base design. Adapt only
where required for target platform aspect ratios, safe areas, or input methods.

## Assets

Assets used by the UI belong under `Assets/` or feature-owned packages,
depending on ownership.

Import visual assets from Figma or source files at high enough resolution for
the largest target screen. Do not upscale low-resolution exports.

Preserve alpha, aspect ratio, and source detail. Avoid lossy recompression for
small UI elements, icons, borders, ornaments, and textural button assets.

Use sliced or scalable assets only when the source design supports it. Do not
stretch decorative frames, borders, or ornaments in a way that changes their
shape.

## Typography

Use Figma Text Styles as the source of truth for UI text.

Known style groups include:

```text
Typography / Display
Typography / Controls
Typography / Body
Typography / Documentation
```

Observed styles:

| Style                                  | Font                          | Size | Notes          |
| -------------------------------------- | ----------------------------- | ---- | -------------- |
| Typography/Display/Main Title          | Cinzel Bold                   | 46   | Main menu logo |
| Typography/Controls/Large Button Label | Cormorant Garamond SemiBold   | 28   | Menu buttons   |

Fonts referenced by Figma must be available in Unity and mapped through USS
classes or shared typography styles.

## UI States

Repeat all designed states in USS and component code.

For buttons and interactive controls, inspect Figma layers for:

* Base texture.
* Tint or disabled overlays.
* Hover and selected glow.
* Pressed visual response.
* Focus state for keyboard and gamepad navigation.

Prefer USS pseudo-states and stable classes such as `selected`, `disabled`, or
`focused`. Keep state names aligned with the ViewModel state where relevant.

## Transitions

Screen, modal, overlay, and scene-backed transitions should feel smooth and
intentional. Use the approved tweening tool from `docs/technology-stack.md`.

Transitions must not hide loading failures, block cancellation, or bypass
Router, ScreenHost, or SceneLoader ownership.

## Review Checklist

Before finishing a UI change, verify:

1. The relevant Figma page or frame was checked.
2. Layout is close to pixel-perfect at the reference size.
3. Figma Text Styles and asset states are represented.
4. Assets are imported at sufficient quality.
5. Responsive behavior matches target platforms.
6. Transitions are smooth and owned by navigation or screen code.
