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
- Basic, Fast, and Tank enemies spawn over time, chase the player, and damage on contact.
- Automatic nearest-enemy projectile attacks.
- Projectile damage, enemy death, and XP orb drops.
- Floating damage numbers and small enemy HP bars for combat readability.
- XP collection and level-ups.
- Paused level-up upgrade choices:
  - Projectile damage
  - Fire rate
  - Movement speed
  - Max HP
  - Heal
  - Projectile size
  - XP magnet
  - Multi shot
- Level-up presents 3 random non-duplicate upgrade choices from the available pool.
- Enemy spawn interval ramps down over time.
- Runtime HUD for HP, level, XP, and survival time.
- Game over screen with restart.
- Conservative polish pass:
  - Clearer placeholder colors and sizing for player, enemies, projectiles, and XP.
  - Simple hit, death, projectile impact, and XP pickup pulse feedback.
  - Clearer level-up choice labels with short upgrade descriptions.
  - Gentler early spawn pacing for the first minute.
- Procedural placeholder SFX for shooting, enemy hits/deaths, XP pickup, level-up, player damage, and game over.
- Procedural placeholder SFX also play when an upgrade is selected.
- Simple runtime pooling for hot combat objects:
  - Projectiles
  - XP orbs
  - Feedback pulses
  - Floating damage numbers
  - Enemies

All visuals and sound effects are generated placeholders at runtime. No external art, audio, paid assets, or extra packages were added.

## Enemy Variety

- Basic Enemy: balanced HP, speed, damage, and XP.
- Fast Enemy: lower HP, faster movement, smaller orange visual, lower contact damage.
- Tank Enemy: higher HP, slower movement, larger purple visual, higher XP reward.

Enemy availability changes over time:

- `0:00` Basic only.
- `1:00` mostly Basic, some Fast.
- `2:00+` Basic, Fast, and Tank.

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
