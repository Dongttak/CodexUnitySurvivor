# AGENTS.md

This is a Unity 6 2D project controlled by an AI coding agent with hera-agent-unity.

## Mandatory Unity Verification

Before Unity-related work:
- Run `hera-agent-unity status`
- Run `hera-agent-unity list`
- Run `hera-agent-unity console --type error`
- Inspect the current scene before modifying scene objects.

After Unity-related work:
- Re-check Unity console errors.
- Report changed files.
- Fix compile errors before continuing.

## Safety Rules

Do not:
- Delete Assets, Packages, or ProjectSettings.
- Delete scenes or prefabs.
- Import external assets without permission.
- Make large destructive changes.
- Modify unrelated files.

## Development Goal

Create a Unity 2D top-down auto-battler survival MVP similar to Vampire Survivors.

Core loop:
move → survive → auto-attack → kill enemies → collect XP → level up → choose upgrade → survive longer → game over

Use placeholder visuals only.
Prioritize a simple playable MVP over polish.
