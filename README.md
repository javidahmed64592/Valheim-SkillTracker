[![CI](https://img.shields.io/github/actions/workflow/status/javidahmed64592/valheim-skilltracker/ci.yml?branch=main&style=flat-square&label=CI&logo=github)](https://github.com/javidahmed64592/valheim-skilltracker/actions/workflows/ci.yml)
[![Package](https://img.shields.io/github/actions/workflow/status/javidahmed64592/valheim-skilltracker/package.yml?branch=main&style=flat-square&label=Package&logo=github)](https://github.com/javidahmed64592/valheim-skilltracker/actions/workflows/package.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

# SkillTracker

See your skills grow: a tracker below the minimap shows each skill as you train it, with its level and progress to the next level, and the experience of every gain rises beside it.

Valheim only tells you when a skill levels up, so it's hard to see what you're training or how close the next level is. SkillTracker shows it as it happens.

## Features

- Whenever you earn experience in a skill, it appears below the minimap with its icon, name, level and a bar showing progress towards the next level.
- The experience each gain gives rises and fades beside it, so you can see what pays off: a sword hit, a block, a second of sprinting.
- Up to three skills show at once, and each fades out a few seconds after its last gain.
- The level flashes gold when it goes up.
- Works for every skill, including skills added or changed by other mods that use the vanilla skill system, such as [ActiveCombat](https://thunderstore.io/c/valheim/p/Aestelwen/ActiveCombat/).

## Configuration

Settings are in `BepInEx/config/javidahmed64592.skilltracker.cfg`: turn the tracker or the experience numbers off, or change how long a skill stays on screen.

## Compatibility

- Only changes what you see, so it's fine for some players on a server to use it and others not.
- Mods that put their own interface below the minimap may overlap it.
- This mod has not been tested with a gamepad, only with keyboard and mouse.

## Uninstalling

Nothing is saved to your world or character, so the mod can be removed at any time.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/javidahmed64592/Valheim-SkillTracker/blob/main/LICENSE) file for details.
