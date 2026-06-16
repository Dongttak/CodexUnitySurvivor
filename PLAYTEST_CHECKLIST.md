# AI Survivor MVP Playtest Checklist

## Quick Start

1. Open the project in Unity 6.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Press Play.
4. Move with `WASD` or arrow keys.
5. Choose level-up upgrades by clicking a button or pressing `1`, `2`, or `3`.
6. Pause/resume with `Esc` or `P`.
7. Toggle compact stats with `Tab`.
8. Press `T` to play an audio test tone.
9. After game over, restart with the Restart button or `R`.

## 5-Minute Manual Playtest

- [ ] Start Play Mode from `SampleScene` without console errors.
- [ ] Survive for at least 60 seconds.
- [ ] Reach at least level 2.
- [ ] Select several upgrade types across separate level-ups or separate runs.
- [ ] Confirm enemies spawn, chase, take damage, die, and drop XP.
- [ ] Confirm Basic Bolt still auto-fires at the nearest enemy.
- [ ] Confirm HP, level, XP, and timer update while playing.
- [ ] Confirm the compact runtime stats panel is visible by default.
- [ ] Toggle the compact stats panel off and on with `Tab`.
- [ ] Confirm the start hint appears at the beginning and hides after a few seconds.
- [ ] Pause and resume once with `Esc` or `P`.
- [ ] Press `T` during gameplay and confirm the audio test tone plays on the selected output.
- [ ] Press `T` while paused and confirm the audio test tone still plays.
- [ ] Let the player die and confirm game over appears.
- [ ] Restart and confirm the run resets cleanly.
- [ ] Stop Play Mode and check the Unity console for errors.

## Movement Checks

- [ ] `W`, `A`, `S`, `D` move the player in the expected directions.
- [ ] Arrow keys also move the player.
- [ ] Diagonal movement does not feel much faster than cardinal movement.
- [ ] Player movement remains responsive while enemies are nearby.
- [ ] Camera follows the player smoothly and keeps the player visible.

## Combat Checks

- [ ] Player automatically fires only when enemies are nearby.
- [ ] Basic Bolt still uses visible pooled projectiles.
- [ ] Projectiles are visible against the background.
- [ ] Projectiles travel fast enough to read as intentional shots.
- [ ] Projectiles hit enemies and disappear on impact.
- [ ] Floating damage numbers appear near enemies when projectiles hit.
- [ ] Damage numbers are large enough to read at `1920x1080`.
- [ ] Damage numbers are still readable at `1280x720`.
- [ ] Damage numbers have enough contrast against enemies and the background.
- [ ] Enemy HP bars are hidden on full-health enemies, then appear and shrink after damage.
- [ ] Enemy HP bars are readable at `1920x1080`.
- [ ] Enemy HP bars are readable at `2560x1440`.
- [ ] Enemy HP bars are readable at `3840x2160`.
- [ ] Tank enemies show HP bars even before taking damage.
- [ ] Basic and Fast enemy HP bars appear immediately after damage.
- [ ] Enemy HP bars reset and hide correctly when pooled enemies die and later respawn.
- [ ] Damage numbers fade quickly enough that repeated hits do not cover the player.
- [ ] Enemies flash or pulse when hit.
- [ ] Enemies die after a small number of hits.
- [ ] Enemy death pulse is visible without obscuring gameplay.

## Enemy Spawn Checks

- [ ] First enemy appears after a short delay.
- [ ] Enemies spawn off-screen or near the edge of the camera view, not directly on top of the player.
- [ ] Spawn pace during the first 60 seconds feels survivable.
- [ ] Spawn pace gradually increases over time.
- [ ] Enemy count does not explode so quickly that movement becomes impossible in the first minute.
- [ ] Enemies consistently chase the player.
- [ ] First minute uses only red Basic enemies.
- [ ] After about 1 minute, smaller/faster orange Fast enemies can appear.
- [ ] After about 2 minutes, larger purple Tank enemies can appear.
- [ ] Basic, Fast, and Tank enemies are visually distinguishable by color and size.

## XP And Level-Up Checks

- [ ] Dead enemies drop green XP orbs.
- [ ] XP orbs are visually distinct from enemies and projectiles.
- [ ] XP orbs move toward the player when close enough.
- [ ] XP pickup pulse is visible.
- [ ] XP text increments after pickup.
- [ ] Level-up triggers when XP reaches the threshold.
- [ ] Gameplay pauses when the level-up panel appears.

## Upgrade Checks

- [ ] Level-up panel title is readable.
- [ ] Level-up panel title says `Choose an Upgrade`.
- [ ] All three card-like upgrade buttons fit inside the panel.
- [ ] Each upgrade card shows a large shortcut, upgrade name, and short description.
- [ ] Upgrade names and descriptions do not wrap awkwardly or overflow at `1280x720`.
- [ ] The three choices vary between level-ups and do not duplicate within the same panel.
- [ ] `1` selects the first displayed upgrade.
- [ ] `2` selects the second displayed upgrade.
- [ ] `3` selects the third displayed upgrade.
- [ ] Mouse clicks also select upgrades.
- [ ] Gameplay resumes after selecting an upgrade.
- [ ] Damage upgrade makes enemies die faster in later fights.
- [ ] Fire rate upgrade makes shots happen more often.
- [ ] Move speed upgrade feels noticeable but not uncontrollable.
- [ ] Max HP Up increases max HP and also heals.
- [ ] Heal restores HP without changing max HP.
- [ ] Projectile Size Up makes projectiles visibly larger and easier to hit with.
- [ ] XP Magnet pulls XP orbs from farther away.
- [ ] Multi Shot adds one projectile per attack.
- [ ] Multi Shot still works after Projectile Size Up, and the larger pooled projectiles reset correctly between shots.
- [ ] Upgrade selection plays a short procedural SFX.
- [ ] `Unlock Aura Pulse` can appear as one of the three level-up choices while Aura Pulse is locked.
- [ ] `Unlock Orbit Blade` can appear as one of the three level-up choices while Orbit Blade is locked.
- [ ] Unlocking Aura Pulse adds the weapon and the unlock choice does not appear again in later level-ups.
- [ ] Unlocking Orbit Blade adds the weapon and the unlock choice does not appear again in later level-ups.
- [ ] Pressing `Esc` or `P` while the level-up panel is visible does not hide the level-up panel or resume gameplay.

## Weapon Variety Checks

- [ ] Basic Bolt continues to target the nearest enemy.
- [ ] Basic Bolt still works with Damage Up.
- [ ] Basic Bolt still works with Fire Rate Up.
- [ ] Basic Bolt still works with Projectile Size Up.
- [ ] Basic Bolt still works with Multi Shot.
- [ ] Aura Pulse unlock creates periodic visible pulses around the player.
- [ ] Aura Pulse damages multiple enemies within its radius.
- [ ] Aura Pulse hit enemies show damage numbers and HP bar updates.
- [ ] Aura Pulse does not fire while paused, choosing upgrades, or game over.
- [ ] Orbit Blade unlock creates a small orbiting blade around the player.
- [ ] Orbit Blade damages enemies it touches.
- [ ] Orbit Blade damage has a readable tick pace and does not melt enemies instantly.
- [ ] Orbit Blade hit enemies show damage numbers and HP bar updates.
- [ ] Orbit Blade does not move or deal damage while paused, choosing upgrades, or game over.
- [ ] Aura Pulse and Orbit Blade can both be unlocked in the same run.
- [ ] Weapon visuals remain readable without covering the player.

## Pause Checks

- [ ] `Esc` pauses gameplay and shows the pause panel.
- [ ] `P` pauses gameplay and shows the pause panel.
- [ ] While paused, enemies, projectiles, timer, and player movement stop.
- [ ] `Esc` resumes from pause.
- [ ] `P` resumes from pause.
- [ ] Resume button resumes from pause.
- [ ] Restart button on the pause panel reloads the run.
- [ ] Pause panel title `Paused` is large and readable.
- [ ] Pause panel instruction says `Press Esc or P to resume`.
- [ ] Pause panel shows a readable `Current Stats` section.
- [ ] Pause stats include HP, level, XP, damage, fire rate, move speed, projectile size, multi-shot count, XP magnet range, weapon unlock state, survival time, and active enemies.
- [ ] Pause stats update after taking damage or collecting XP.
- [ ] Pause stats reflect upgrades after choosing damage, fire rate, move speed, projectile size, multi shot, XP magnet, max HP, or heal.
- [ ] Pause stats match the compact runtime stats for shared values.
- [ ] Pause panel does not appear on top of the level-up panel.
- [ ] Pause input does not resume gameplay after game over.

## Game Over And Restart Checks

- [ ] Player takes damage on enemy contact.
- [ ] HP decreases at a readable pace, not every frame instantly.
- [ ] Player flashes or pulses when damaged.
- [ ] Game over panel appears when HP reaches zero.
- [ ] Game over panel shows survival time and final level.
- [ ] Game over panel clearly shows the restart instruction.
- [ ] Game over title is large and centered.
- [ ] Timer stops while game over is displayed.
- [ ] Restart button reloads the run.
- [ ] `R` reloads the run from game over.
- [ ] Restart clears old enemies, projectiles, XP orbs, and feedback pulses.
- [ ] Restart does not leave stale pooled damage numbers or HP bars visible.

## UI Readability Checks

- [ ] CanvasScaler is set to Scale With Screen Size with a `1920x1080` reference resolution.
- [ ] CanvasScaler match mode uses a balanced width/height setting so UI scales across aspect ratios.
- [ ] At `1280x720`, HUD, timer, start hint, runtime stats, upgrade panel, pause panel, and game-over panel are still readable.
- [ ] At `1920x1080`, HUD, timer, start hint, runtime stats, upgrade panel, pause panel, and game-over panel are readable.
- [ ] At `2560x1440`, HUD, timer, start hint, runtime stats, upgrade panel, pause panel, and game-over panel are readable.
- [ ] At `3840x2160`, HUD, timer, start hint, runtime stats, upgrade panel, pause panel, and game-over panel are readable.
- [ ] At `1440x900`, HUD, timer, start hint, runtime stats, upgrade panel, pause panel, and game-over panel remain readable.
- [ ] At `2560x1600`, HUD, timer, start hint, runtime stats, upgrade panel, pause panel, and game-over panel remain readable.
- [ ] At `3440x1440`, HUD, timer, start hint, runtime stats, upgrade panel, pause panel, and game-over panel remain readable and do not drift too far from useful screen areas.
- [ ] At `3840x1600`, HUD, timer, start hint, runtime stats, upgrade panel, pause panel, and game-over panel remain readable.
- [ ] Top-left HUD, top-center timer, bottom-center start hint, and right-side runtime stats keep consistent margins.
- [ ] HP text is readable during movement and combat.
- [ ] HP bar fill changes when the player takes damage or heals.
- [ ] Level text updates after leveling.
- [ ] XP text uses the correct current and required values.
- [ ] XP progress bar fills as XP is collected and resets after level-up.
- [ ] Timer is readable in the top-center panel.
- [ ] HUD panels improve contrast without covering the center of the playfield.
- [ ] HUD panels do not overlap each other at 16:9, 16:10, 4K, or ultrawide sizes.
- [ ] Start hint includes movement, pause, and upgrade-selection controls.
- [ ] Start hint includes `Tab: Stats`.
- [ ] Start hint is readable but does not linger too long.
- [ ] Runtime stats panel appears on the right side and does not cover center combat.
- [ ] Runtime stats panel shows HP, damage, fire rate, move speed, projectile size, shot count, XP magnet range, Aura Pulse state, Orbit Blade state, level, and survival time.
- [ ] Runtime stats update during gameplay after damage, upgrades, and time passing.
- [ ] `Tab` hides and shows the runtime stats panel.
- [ ] `Tab` does not break level-up, pause, or game-over UI.
- [ ] Level-up panel text does not overlap.
- [ ] Upgrade cards remain readable at common Game view sizes.
- [ ] Game over text and restart instruction are readable.
- [ ] Pause panel text and buttons are readable.
- [ ] Pause stats rows are readable and do not overflow the stats panel.
- [ ] Pause, level-up, and game-over panels remain centered and readable on ultrawide views.
- [ ] UI remains usable at common Game view sizes.

## Game Feel Checks

- [ ] Player color is easy to track among enemies and effects.
- [ ] Enemy color is easy to distinguish from projectiles and XP.
- [ ] Projectile feedback is visible but not distracting.
- [ ] Hit and death pulses help clarify combat results.
- [ ] XP pickup feels clear and rewarding.
- [ ] Damage numbers are readable but do not clutter the screen heavily.
- [ ] Damage number shadow improves readability without looking too heavy.
- [ ] Enemy HP bars help show durability only after enemies have been damaged.
- [ ] Larger HP bars improve enemy durability feedback without cluttering basic early fights.
- [ ] Early game has enough breathing room to learn movement.
- [ ] The first level-up happens soon enough to demonstrate the core loop.

## Audio Checks

- [ ] With MacBook Speakers selected before launching Unity, press `T` and hear the test tone.
- [ ] With MacBook Speakers selected, confirm shoot, hit, enemy death, XP pickup, level-up, player damage, upgrade-select, and game-over SFX play.
- [ ] With AirPods selected before launching Unity, press `T` and note whether the test tone plays.
- [ ] With AirPods selected before launching Unity, confirm normal SFX events play or record which events are silent.
- [ ] Set macOS input to MacBook microphone or another non-AirPods input and retest AirPods output.
- [ ] Avoid reconnecting AirPods during Play Mode and verify whether audio remains stable.
- [ ] Reconnect AirPods during Play Mode once as a stress test and record whether audio drops or Unity/FMOD messages appear.
- [ ] If an FMOD output-device error appears, restart Unity Editor and retest before marking it as a project bug.
- [ ] Compare AirPods behavior with MacBook Speakers to determine whether the issue reproduces only on Bluetooth output.

## Balance Notes To Observe

- Note the time of the first level-up.
- Note the time when enemy pressure starts to feel dangerous.
- Note whether HP lasts long enough for a new player to understand the loop.
- Note whether projectile damage feels too weak or too strong.
- Note whether the spawn ramp feels fair at 1, 3, and 5 minutes.
- Note whether movement speed upgrades become too fast after repeated picks.
- Note whether projectile size or multi shot becomes visually cluttered.
- Note whether Aura Pulse visual size is readable without hiding enemies.
- Note whether Orbit Blade is easy to see while moving through crowds.
- Note whether weapon unlock choices appear too early, too late, or too often.
- Note whether damage numbers or HP bars become visually noisy with many enemies.
- Note whether the larger HP bars are readable on your 4K display without feeling oversized.
- Note whether always-visible stats are useful during combat or should default hidden later.
- Note whether damage numbers are now large enough during real combat.
- Note whether pause-menu stats are useful or should later become a toggleable HUD panel.
- Note whether the HUD panels are too large or too transparent at your preferred Game view size.
- Note whether the start hint duration feels too short or too long.
- Note whether upgrade card descriptions are clear enough to choose quickly.
- Note whether pause controls are discoverable enough after the start hint fades.
- Note whether Fast enemies feel fair when they unlock around 1 minute.
- Note whether Tank enemies feel too durable or too rewarding after 2 minutes.
- Note whether XP Magnet makes collection too automatic too quickly.
- Note whether HP upgrades and heal upgrades both feel worth choosing.
- Note if one upgrade is always obviously better than the others.
- Note whether audio issues reproduce only on Bluetooth/AirPods or also on MacBook Speakers.
- Note whether `T` test tone works while normal gameplay SFX are silent, or vice versa.
- Note whether changing macOS input away from AirPods improves output stability.

## Bugs To Watch For

- Console errors after entering or exiting Play Mode.
- Player not moving because the input backend is misconfigured.
- Enemies spawning but not chasing.
- Enemies overlapping the player immediately on spawn.
- Projectiles spawning but never hitting.
- XP orbs failing to collect.
- Level-up panel not pausing gameplay.
- Upgrade buttons or number keys not responding.
- Duplicate upgrade choices appearing in a single level-up panel.
- New upgrades appearing in UI but not changing gameplay.
- Weapon unlock choices repeating after the weapon is already unlocked.
- Aura Pulse unlocked but never pulsing.
- Aura Pulse pulse visible but not damaging enemies inside the radius.
- Orbit Blade unlocked but no blade appears.
- Orbit Blade damaging enemies too rapidly or not at all.
- Game over panel not appearing.
- Pause panel not appearing when `Esc` or `P` is pressed.
- Pause leaving gameplay running in the background.
- Pause button resume failing after clicking Resume.
- Pause restarting the scene incorrectly or leaving stale pooled objects.
- Pause input breaking level-up upgrade selection.
- Pause input resuming from game over.
- Restart leaving old runtime objects in the scene.
- Pooled projectiles reusing stale size, direction, or damage.
- Pooled XP orbs reusing stale XP values or failing to attract.
- Pooled damage numbers reusing stale text, alpha, or position.
- Pooled damage numbers reusing stale scale, font size, or shadow color.
- Pause stats showing stale values after upgrades or damage.
- Pause stats layout clipping at `1280x720`.
- Runtime stats showing stale values after upgrades or damage.
- Runtime stats panel covering too much playfield.
- `Tab` toggle not responding or interfering with other UI states.
- Enemy HP bars failing to reset when pooled enemies are reused.
- Tank HP bars not visible at full health.
- Basic/Fast HP bars not appearing immediately after damage.
- UI text missing because runtime UI construction failed.
- HUD bars not filling or resetting correctly.
- Start hint remaining visible for the whole run.
- Upgrade card labels clipping at smaller Game view sizes.
- Pause panel labels clipping at smaller Game view sizes.
- Game over final level showing the wrong value.
- Feedback pulses accumulating or lingering too long.
- `T` audio test tone not playing during gameplay or pause.
- SFX working on MacBook Speakers but not AirPods.
- Unity/FMOD output-device errors after disconnecting or reconnecting AirPods during Play Mode.
- Duplicate AudioManager instances after scene restart or domain reload.

## Suggested Next Tasks After Manual Playtest

- Record observed first-level-up timing and first-death timing.
- Tune enemy spawn pacing only after at least one full 5-minute playtest.
- Add a simple arena boundary if players can kite forever.
- Add one additional enemy type after the current enemy pacing feels stable.
- Add lightweight automated Play Mode tests for XP, level-up, and restart.
- Add BGM only after procedural SFX and core pacing feel correct.
- Consider simple health bar or damage numbers only if combat readability still feels weak.
