# Codex Unity Survivor

A Unity 6 2D top-down auto-battler survival MVP built with placeholder visuals.

## How to Run

1. Open the project in Unity 6.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Press Play.

## Controls

- Move: `WASD` or arrow keys.
- Pause/resume: `Esc` or `P`.
- Toggle compact runtime stats: `Tab`.
- Play audio test tone: `T`.
- View full current player stats: pause with `Esc` or `P`.
- Level-up choices: click a button or press `1`, `2`, or `3`.
- Restart from pause: click `Restart`.
- Restart after game over: click `Restart` or press `R`.

## What Was Built

- Player movement with 2D physics.
- Camera follow.
- Enemy spawning around the player.
- Basic, Fast, and Tank enemies spawn over time, chase the player, and damage on contact.
- Automatic nearest-enemy projectile attacks through the starting `Basic Bolt` weapon.
- Unlockable weapon variety:
  - `Aura Pulse`: periodically damages nearby enemies with a visible area pulse.
  - `Orbit Blade`: creates a small orbiting blade that damages enemies it touches.
- Projectile damage, enemy death, and XP orb drops.
- Large high-contrast floating damage numbers and larger enemy HP bars for combat readability.
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
  - Unlock Aura Pulse
  - Unlock Orbit Blade
- Level-up presents 3 random non-duplicate upgrade choices from the available pool.
- Weapon unlock upgrades appear only while that weapon is still locked.
- Enemy spawn interval ramps down over time.
- Runtime HUD for HP, level, XP, and survival time.
- Readable HUD panels with HP text/bar, XP text/bar, level, timer, and a short start hint.
- Clearer level-up upgrade cards with names, descriptions, and `1`/`2`/`3` shortcuts.
- Game over screen with survival time, final level, restart instruction, and restart button.
- Pause menu with resume, restart, and current player stats.
- Compact runtime stats panel visible by default with `Tab` toggle.
- Conservative polish pass:
  - Clearer placeholder colors and sizing for player, enemies, projectiles, and XP.
  - Simple hit, death, projectile impact, and XP pickup pulse feedback.
  - Clearer level-up choice labels with short upgrade descriptions.
  - Gentler early spawn pacing for the first minute.
- Procedural placeholder SFX for shooting, enemy hits/deaths, XP pickup, level-up, player damage, and game over.
- Procedural placeholder SFX also play when an upgrade is selected.
- A short procedural audio test tone plays with `T` during gameplay or pause.
- Simple runtime pooling for hot combat objects:
  - Projectiles
  - XP orbs
  - Feedback pulses
  - Floating damage numbers
  - Enemies
- UI/UX readability pass:
  - Semi-transparent HUD panels keep stats readable without covering the center of combat.
  - Canvas uses `Scale With Screen Size`, `1920x1080` reference resolution, and a balanced width/height match.
  - HUD, timer, start hint, level-up, pause, and game-over text use explicit larger font sizes.
  - Responsive UI pass improves readability across common 16:9, 16:10, 4K, and ultrawide Game view sizes.
  - HUD anchors keep HP/XP/level at top-left, the timer at top-center, the start hint at bottom-center, and runtime stats on the right side.
  - Level-up, pause, and game-over panels are larger centered modals with stronger backing panels and more generous spacing.
  - Enemy HP bars stay hidden on full-health enemies and appear after damage.
  - Floating damage numbers use larger world-space text, a longer readable lifetime, and a dark shadow.
  - A start hint explains movement, pause, and upgrade selection for first-time players.
  - Pause menu stats show HP, level, XP, damage, fire rate, move speed, projectile size, multi-shot count, XP magnet range, survival time, and active enemies.
  - Runtime stats HUD shows key combat stats during gameplay: HP, damage, fire rate, move speed, projectile size, shot count, XP magnet range, weapon unlock state, level, and survival time.
  - Enemy HP bars are larger for high-resolution displays, with a dark border/backplate and high-contrast fill; damaged enemies show bars, and Tank enemies show bars even at full HP.

All visuals and sound effects are generated placeholders at runtime. No external art, audio, paid assets, or extra packages were added.

## Audio / AirPods Troubleshooting

This project uses Unity `AudioSource` playback with runtime-generated procedural `AudioClip` tones. The gameplay code does not call FMOD APIs directly and does not try to switch Unity or macOS output devices.

If AirPods or another Bluetooth output does not play sound in the Unity Editor:

1. Set the macOS output device to AirPods before starting Play Mode.
2. Set the macOS input device to the MacBook microphone or another non-AirPods microphone if possible.
3. Restart the Unity Editor after changing Bluetooth output devices.
4. Avoid disconnecting or reconnecting AirPods during Play Mode.
5. If an FMOD output-device error appears, restart the Unity Editor.
6. Try MacBook Speakers to distinguish project audio bugs from Unity Editor/device-output issues.

Press `T` in gameplay or pause to play a short test tone. If `T` and normal SFX work on MacBook Speakers but not AirPods, the issue is likely in the Unity Editor, macOS audio routing, or Bluetooth device state rather than this project's SFX code.

## Enemy Variety

- Basic Enemy: balanced HP, speed, damage, and XP.
- Fast Enemy: lower HP, faster movement, smaller orange visual, lower contact damage.
- Tank Enemy: higher HP, slower movement, larger purple visual, higher XP reward.

Enemy availability changes over time:

- `0:00` Basic only.
- `1:00` mostly Basic, some Fast.
- `2:00+` Basic, Fast, and Tank.

## Weapon Variety

- `Basic Bolt`: the starting auto-fire projectile weapon. It targets the nearest enemy and continues to use projectile pooling, Damage Up, Fire Rate Up, Projectile Size Up, Multi Shot, damage numbers, HP bars, and SFX.
- `Aura Pulse`: unlocked from level-up choices. It pulses around the player every few seconds, damages enemies in its radius, uses the existing pooled feedback pulse visual, and triggers existing procedural SFX through normal hit/shoot paths.
- `Orbit Blade`: unlocked from level-up choices. It creates a small orbiting placeholder blade around the player; enemies touching it take periodic damage with a short per-enemy cooldown.

Aura Pulse and Orbit Blade unlock choices stop appearing after their weapon is acquired.

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
