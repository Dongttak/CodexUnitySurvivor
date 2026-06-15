# Codex Unity Survivor

A Unity 6 2D top-down auto-battler survival MVP built with placeholder visuals.

## How to Run

1. Open the project in Unity 6.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Press Play.

## Controls

- Move: `WASD` or arrow keys.
- Level-up choices: click a button or press `1`, `2`, or `3`.
- Restart after game over: click `Restart` or press `R`.

## What Was Built

- Player movement with 2D physics.
- Camera follow.
- Enemy spawning around the player.
- Enemies chase and damage the player on contact.
- Automatic nearest-enemy projectile attacks.
- Projectile damage, enemy death, and XP orb drops.
- XP collection and level-ups.
- Paused level-up upgrade choices:
  - Projectile damage
  - Fire rate
  - Movement speed
- Enemy spawn interval ramps down over time.
- Runtime HUD for HP, level, XP, and survival time.
- Game over screen with restart.
- Conservative polish pass:
  - Clearer placeholder colors and sizing for player, enemies, projectiles, and XP.
  - Simple hit, death, projectile impact, and XP pickup pulse feedback.
  - Clearer level-up choice labels with short upgrade descriptions.
  - Gentler early spawn pacing for the first minute.

All visuals are generated placeholder sprites at runtime. No external art, paid assets, or extra packages were added.

## Scene Setup

`SampleScene` contains:

- `GameManager`
- `Player`
- `EnemySpawner`
- `Main Camera`
- `Canvas`
- `EventSystem`
- `Global Light 2D`

The game should be playable without additional manual scene setup.
