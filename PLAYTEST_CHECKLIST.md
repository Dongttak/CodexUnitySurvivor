# AI Survivor MVP Playtest Checklist

## Quick Start

1. Open the project in Unity 6.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Press Play.
4. Move with `WASD` or arrow keys.
5. Choose level-up upgrades by clicking a button or pressing `1`, `2`, or `3`.
6. After game over, restart with the Restart button or `R`.

## 5-Minute Manual Playtest

- [ ] Start Play Mode from `SampleScene` without console errors.
- [ ] Survive for at least 60 seconds.
- [ ] Reach at least level 2.
- [ ] Select several upgrade types across separate level-ups or separate runs.
- [ ] Confirm enemies spawn, chase, take damage, die, and drop XP.
- [ ] Confirm HP, level, XP, and timer update while playing.
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
- [ ] Projectiles are visible against the background.
- [ ] Projectiles travel fast enough to read as intentional shots.
- [ ] Projectiles hit enemies and disappear on impact.
- [ ] Floating damage numbers appear near enemies when projectiles hit.
- [ ] Enemy HP bars appear above enemies and shrink after damage.
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
- [ ] All three upgrade buttons fit inside the panel.
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

## Game Over And Restart Checks

- [ ] Player takes damage on enemy contact.
- [ ] HP decreases at a readable pace, not every frame instantly.
- [ ] Player flashes or pulses when damaged.
- [ ] Game over panel appears when HP reaches zero.
- [ ] Timer stops while game over is displayed.
- [ ] Restart button reloads the run.
- [ ] `R` reloads the run from game over.
- [ ] Restart clears old enemies, projectiles, XP orbs, and feedback pulses.
- [ ] Restart does not leave stale pooled damage numbers or HP bars visible.

## UI Readability Checks

- [ ] HP text is readable during movement and combat.
- [ ] Level text updates after leveling.
- [ ] XP text uses the correct current and required values.
- [ ] Timer is readable in the top-right corner.
- [ ] Level-up panel text does not overlap.
- [ ] Game over text and restart instruction are readable.
- [ ] UI remains usable at common Game view sizes.

## Game Feel Checks

- [ ] Player color is easy to track among enemies and effects.
- [ ] Enemy color is easy to distinguish from projectiles and XP.
- [ ] Projectile feedback is visible but not distracting.
- [ ] Hit and death pulses help clarify combat results.
- [ ] XP pickup feels clear and rewarding.
- [ ] Damage numbers are readable but do not clutter the screen heavily.
- [ ] Enemy HP bars help show durability without covering the playfield.
- [ ] Early game has enough breathing room to learn movement.
- [ ] The first level-up happens soon enough to demonstrate the core loop.

## Balance Notes To Observe

- Note the time of the first level-up.
- Note the time when enemy pressure starts to feel dangerous.
- Note whether HP lasts long enough for a new player to understand the loop.
- Note whether projectile damage feels too weak or too strong.
- Note whether the spawn ramp feels fair at 1, 3, and 5 minutes.
- Note whether movement speed upgrades become too fast after repeated picks.
- Note whether projectile size or multi shot becomes visually cluttered.
- Note whether damage numbers or HP bars become visually noisy with many enemies.
- Note whether Fast enemies feel fair when they unlock around 1 minute.
- Note whether Tank enemies feel too durable or too rewarding after 2 minutes.
- Note whether XP Magnet makes collection too automatic too quickly.
- Note whether HP upgrades and heal upgrades both feel worth choosing.
- Note if one upgrade is always obviously better than the others.

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
- Game over panel not appearing.
- Restart leaving old runtime objects in the scene.
- Pooled projectiles reusing stale size, direction, or damage.
- Pooled XP orbs reusing stale XP values or failing to attract.
- Pooled damage numbers reusing stale text, alpha, or position.
- Enemy HP bars failing to reset when pooled enemies are reused.
- UI text missing because runtime UI construction failed.
- Feedback pulses accumulating or lingering too long.

## Suggested Next Tasks After Manual Playtest

- Record observed first-level-up timing and first-death timing.
- Tune enemy spawn pacing only after at least one full 5-minute playtest.
- Add a simple arena boundary if players can kite forever.
- Add one additional enemy type after the current enemy pacing feels stable.
- Add lightweight automated Play Mode tests for XP, level-up, and restart.
- Add BGM only after procedural SFX and core pacing feel correct.
- Consider simple health bar or damage numbers only if combat readability still feels weak.
