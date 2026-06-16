# Autonomous Development Roadmap

This roadmap defines safe future development phases for the Unity survivor MVP. Each phase should happen on its own branch and should preserve the current stable loop unless the user explicitly approves a larger change.

## Phase 1: Manual Bugfix And Balance Pass

Goal: Fix issues found during manual playtesting and tune early pacing.

Allowed changes: Small bug fixes, conservative stat tuning, UI text corrections, checklist updates, and devlog notes.

Not allowed changes: New weapons, new enemies, new bosses, external assets, save/load, major architecture rewrites, or broad visual redesigns.

Validation requirements: Hera status, console error check, refresh/compile, Play Mode start/stop, and a short manual checklist pass focused on the changed behavior.

Branch naming convention: `ai-bugfix-balance-pass-##`

Commit message convention: `Fix survivor MVP bugs and balance`

Manual playtest requirements: At least one 5-minute run, first-level-up timing, first-death timing, audio output check, pause/restart check, and console check after exiting Play Mode.

## Phase 2: Weapon Variety

Goal: Add one small new weapon or weapon behavior while preserving the existing auto-attack loop.

Allowed changes: One new placeholder weapon, simple upgrade hooks, pooled projectile reuse, UI text updates, and focused balance values.

Not allowed changes: Multiple simultaneous weapons, BGM, external assets, new enemy families, boss systems, or full weapon inventory UI.

Validation requirements: Existing weapon still works, new weapon fires, pooling remains stable, upgrades still compile, console has no errors, and Play Mode starts/stops.

Branch naming convention: `ai-weapon-pass-##`

Commit message convention: `Add survivor weapon variety`

Manual playtest requirements: Verify the weapon can hit Basic/Fast/Tank enemies, does not flood the screen, and remains readable at 1080p and 4K.

## Phase 3: Enemy And Wave Variety

Goal: Add limited enemy or wave variety without overwhelming the early game.

Allowed changes: One enemy definition or one simple wave timing rule, color/size placeholder visuals, stat tuning, and spawn-weight updates.

Not allowed changes: Bosses, complex pathfinding, large wave director rewrites, external art, or new arenas.

Validation requirements: Basic/Fast/Tank remain intact, new enemy or wave appears at the intended time, enemy pooling resets state, and console has no errors.

Branch naming convention: `ai-enemy-wave-pass-##`

Commit message convention: `Add survivor enemy wave variety`

Manual playtest requirements: Survive past the new unlock time and record whether the pressure feels fair.

## Phase 4: Boss Or Elite Enemy

Goal: Add a single simple elite or boss-style encounter.

Allowed changes: One elite/boss definition, larger HP bar behavior, clear placeholder visual, spawn timing, reward tuning, and checklist updates.

Not allowed changes: Multiple bosses, cutscenes, complex AI, new level loading, or full progression systems.

Validation requirements: Boss spawns once at the intended time, can be damaged and killed, drops reward correctly, and does not break pooling or game over.

Branch naming convention: `ai-boss-elite-pass-##`

Commit message convention: `Add survivor elite encounter`

Manual playtest requirements: Reach boss timing, test death before and during boss, verify restart cleans up boss state.

## Phase 5: Game Feel And Visual Polish

Goal: Improve clarity and feel using only placeholder or generated-in-code visuals.

Allowed changes: Color tuning, simple scale/flash effects, UI polish, damage/HP readability, animation timing, and camera feel tweaks.

Not allowed changes: External assets, audio asset imports, major UI rebuilds, new gameplay systems, or large balance rewrites.

Validation requirements: Existing gameplay loop remains intact, Play Mode starts/stops, console has no errors, and UI remains readable at target resolutions.

Branch naming convention: `ai-gamefeel-polish-pass-##`

Commit message convention: `Polish survivor game feel`

Manual playtest requirements: Check readability at 1280x720, 1920x1080, 2560x1440, 3840x2160, and ultrawide if available.

## Phase 6: Performance And Profiling Pass

Goal: Reduce runtime spikes and hot-path allocations.

Allowed changes: Pooling improvements, low-frequency stat updates, cheaper active counts, profiler notes, and minimal code comments explaining optimization choices.

Not allowed changes: Feature additions, broad refactors, DOTS conversion, external packages, or speculative optimization without measurement.

Validation requirements: Hera console clean, Play Mode start/stop, profiler or runtime observation notes, and confirmation that pooled objects reset correctly.

Branch naming convention: `ai-performance-pass-##`

Commit message convention: `Optimize survivor MVP performance`

Manual playtest requirements: Play past 3 minutes, observe enemy density, check for stutter during projectile hits, XP pickup, level-up, and restart.

## Phase 7: Build And Export Pass

Goal: Prepare a local playable build while keeping the editor project stable.

Allowed changes: Build settings review, scene inclusion, README build instructions, and local build validation.

Not allowed changes: Store packaging, platform-specific plugins, external assets, signing/notarization, or publishing.

Validation requirements: Unity console clean, Play Mode starts/stops before build, build completes, generated build output is not committed, and README documents how to build.

Branch naming convention: `ai-build-export-pass-##`

Commit message convention: `Prepare survivor MVP build`

Manual playtest requirements: Launch the build, test movement, pause, audio test tone, upgrade selection, game over, and restart.

## Phase 8: Portfolio Documentation Pass

Goal: Make the project easy to understand as an AI-generated Unity MVP.

Allowed changes: README structure, screenshots or GIF references if explicitly generated/approved, architecture notes, known limitations, and playtest notes.

Not allowed changes: Gameplay changes, external marketing assets, inflated claims, or claims that unverified device issues are fixed.

Validation requirements: Documentation accurately matches the project, links/paths work, and git status excludes generated folders.

Branch naming convention: `ai-portfolio-docs-pass-##`

Commit message convention: `Document survivor MVP portfolio`

Manual playtest requirements: Confirm docs match the latest controls, UI, audio test flow, and known limitations.
