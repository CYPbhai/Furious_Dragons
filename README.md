# Furious Dragons

## Tools used 
- Unity 6000.6.0f1
- Krita
- Audacity
- Claude
- Gemini

## How to Play

### Controls
- **Movement:** WASD 
- **Basic Attack**: 1
- **Claw Attack**: 2
- **Flame Attack**: 3
- **Fly Flame Attack**: 4

Abilities can also be triggered via the on-screen ability buttons, which show a radial cooldown fill and disable themselves while on cooldown.

## Features Implemented

- WASD movement using Rigidbody-based physics (`MovePosition`/`MoveRotation`), with the dragon always facing its opponent independent of movement direction.
- Full combat system: 4 distinct abilities (bite, claw, flame breath, and fly flame attack), each on an independent cooldown.
- On Screen buttons with unique icon for each button.
- Flame breath VFX created using VFX Graph in Unity with custom textures created in Krita.
- Flame breath deals continuous tick damage for as long as the target stays in range, rather than one instant hit - meant to feel like an actual sustained breath rather than a single "hitscan" attack.
- Enemy AI with a timed decision loop (re-evaluates roughly every 0.3s rather than every frame): maintains a preferred distance from the player, strafes unpredictably, retreats when too close, and picks between abilities based on range — with a deliberate chance to *not* attack even when able to, so it doesn't read as a robotic 100%-uptime attacker.
- Shared architecture: the player and the AI dragon both run on the same `DragonController` component, differing only in which `IInputProvider` feeds them input (`PlayerInputProvider` reading the keyboard/UI, `AIEnemyInputProvider` computing decisions). This means the AI automatically respects the exact same cooldowns, state rules, and attack-locking logic as the player - there's no separate AI combat script that could drift out of sync.
- Input queuing designed to avoid stale/stacked presses: attack inputs are consumed (read and cleared) every frame regardless of whether the dragon is currently able to act, so a press made mid-attack is discarded rather than firing late once the attack ends.
- Health system with UI health bars, driven by events rather than polled every frame.
- Hit point animation driven by events with runtime damage amount.
- Win condition with a winner screen naming the victor and a Restart button.
- Sound effects driven by events from animation clips.

## Asset Sources

- Dragon models & animations: [Dragon for Boss Monster : PBR by Dungeon Mason](https://assetstore.unity.com/packages/3d/characters/creatures/dragon-for-boss-monster-pbr-78923)
- Arena: [Low Poly Gladiators Arena by Leonardo Olivieri Carvalho](https://assetstore.unity.com/packages/3d/environments/fantasy/low-poly-gladiators-arena-167116)
- Dragon Sounds: [Dragon Sounds by DRAGON-STUDIO](https://pixabay.com/users/dragon-studio-38165424/)
- Dragon Hurt Sound: [Dragon Hurt](https://pixabay.com/sound-effects/horror-dragon-hurt-47161/)

## Architecture Overview

- **`IInputProvider`** : interface abstracting the source of input (keyboard/UI vs. AI decision logic), so one controller class can drive either a human-controlled or AI-controlled dragon.
- **`DragonController`** : core state machine (`Idle` / `Moving` / `Attacking` / `Hit` / `Dead`), cooldown tracking per ability, movement, and facing logic. Shared by both dragons.
- **`Combat`** : hit detection and damage application per ability, including a coroutine-based continuous-damage channel for the flame breath.
- **`Health`** : damage and death events, fully decoupled from UI.
- **`DragonVisual`** : listens to `DragonController`'s events and drives Animator triggers.
- **`GameInput`** : wraps the new Input System, exposing consuming getters (`GetIsAttacking1()`, etc.).

## AI Usage Note

**Tools used:** Claude, Gemini

**What I used it for:**
- Debugging specific issues as they came up during development and taking help as new features needed to be implemented.
- Getting a second opinion on code structure (e.g. it flagged that a separate `EnemyAI` class I was about to write would duplicate the player's combat logic, and suggested unifying both into the shared controller instead).
- Created sprite sheet for attack buttons using Gemini.

**An example of where I had to find the real issue myself:**
- My dragon could still move while an attack animation was playing, which looked wrong. My `FixedUpdate` computed a `canAct` bool once at the top of the frame, used it to gate the attack-start logic, and then reused that same `canAct` value later to decide whether to transition into the Moving state. The problem: if an attack started earlier in that same frame, `state` had already changed to `Attacking`, but `canAct` was a stale snapshot taken before that happened - so the later movement check still thought it was allowed to run, and immediately overwrote `Attacking` back to `Moving` in the same frame. The fix was to re-check the *live* `state` value right before using it, rather than trusting an earlier cached copy. Finding this meant actually tracing the order of operations within a single frame rather than assuming the first suggested fix was complete.
- Refined AI script myself with custom ranges and removing things which were unnecessary.

**How it made the work faster:**
It was most useful for catching this kind of frame-order/stale-state logic bug, and for suggesting the shared-controller architecture upfront, which avoided building and maintaining two parallel sets of combat logic for the player and the AI. I wrote all the code myself and understood each change before applying it, rather than pasting anything I couldn't explain.
