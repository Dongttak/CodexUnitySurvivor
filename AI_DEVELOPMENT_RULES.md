# AI Development Rules

These rules apply to future autonomous work on this Unity survivor MVP.

## Branch And Commit Safety

- Always create a new branch for each feature, fix, polish, documentation, or validation pass.
- Never merge to `main` automatically.
- Never rewrite the whole project unless the user explicitly asks for that scope.
- Prefer small stable changes over broad speculative refactors.
- Use focused commit messages that describe the pass, not a vague checkpoint.

## Unity And Hera Verification

- Always run `/Users/dongttak/go/bin/hera-agent-unity status` before Unity-related work.
- Always run `/Users/dongttak/go/bin/hera-agent-unity console --type error` before changing Unity code or scene objects.
- Inspect the active scene and root objects before modifying scene setup.
- After code changes, refresh/compile Unity through Hera when available.
- Never commit without a final Unity console error check.
- Start and stop Play Mode through Hera when possible for any gameplay, UI, audio, pooling, or scene-affecting change.
- Fix compile errors before continuing to unrelated work.

## Staging Rules

- Never stage `Library`, `Temp`, `Logs`, `UserSettings`, `Build`, `Builds`, generated build outputs, screenshots, or local caches unless the user explicitly asks.
- Stage only the intended project files for the current pass.
- Check `git status --short` before staging and before the final report.

## Asset And Package Rules

- Never import external assets or packages without explicit permission.
- Do not add paid assets.
- Do not add BGM or audio files unless explicitly requested.
- Prefer runtime placeholder visuals and procedural placeholder audio until the user approves an asset pass.

## Gameplay Change Rules

- Preserve the existing loop: move, survive, auto-attack, kill enemies, collect XP, level up, choose upgrades, survive longer, game over.
- Do not delete existing working systems.
- Do not add new enemies, weapons, upgrades, bosses, save/load, menus, or build systems unless the current pass requests them.
- Keep upgrades, pooling, SFX, pause, game over, stats, damage numbers, and HP bars connected unless a pass explicitly changes them.

## Documentation Rules

- Update `DEVLOG.md` every pass with branch name, Hera status, console status, changes made, and known limitations.
- Update `README.md` when controls, setup, behavior, or troubleshooting changes.
- Update `PLAYTEST_CHECKLIST.md` when behavior, UI, audio, balance, or validation expectations change.
- Document known limitations honestly.
- Do not claim a device-specific issue is fixed unless it was actually verified on that device.

## Validation And Reporting

- Report the branch name, commit hash, files changed, validation commands, console status, known limitations, and whether the project is safe for manual playtesting.
- If Hera is unavailable or Unity cannot compile, stop and report the blocker instead of guessing.
- If a requested optimization or feature is risky, implement the safer subset and document what remains.
