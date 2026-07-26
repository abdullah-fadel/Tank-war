# Tank Assault

## Playable web version (`docs/`)

A complete, standalone 3D web game (Three.js, single file, PWA, Arabic RTL) lives in
`docs/` and is served via GitHub Pages. It is a landscape-first mobile campaign game:
**5 themed fronts (desert sunset, storm, night ops, frozen front, volcano) × 5 missions
each**, unlocked sequentially with a star rating per mission, persistent coins/upgrades
(localStorage), supply-crate pickups, mission briefings, a pause menu, and fully
customizable touch controls.

## Unity project (`TankAssault/`)

A 3D side-view (2.5D) mobile tank action game built in Unity (URP, portrait 9:16). This
repository contains a complete, production-grade **codebase and content pipeline** for the
game: clean modular C# architecture, ScriptableObject-driven game data, and Editor tooling
that generates a fully playable placeholder build (primitive meshes) so you can press Play
immediately and iterate — swap in real 3D art, animations, and audio without touching a
single gameplay script.

## What's in this repo

```
TankAssault/                   Unity project root — open this folder in Unity Hub
  Assets/_Project/
    Scripts/                   All gameplay code (see Documentation/ARCHITECTURE.md)
    ScriptableObjects/         Generated at Editor-time: tanks, weapons, enemies, worlds...
    Prefabs/                   Generated at Editor-time: player tank, 10 enemy types, VFX
    Scenes/                    Generated at Editor-time: Bootstrap + Gameplay_Demo
  Packages/manifest.json       URP, Input System, Cinemachine, TextMeshPro, Addressables...
Documentation/
  ARCHITECTURE.md              System-by-system breakdown of the codebase
  SETUP_GUIDE.md                How to open the project and generate the playable demo
  CONTENT_GUIDE.md              How to add tanks, weapons, enemies, worlds, missions
```

## Quick start

1. Open `TankAssault/` in Unity Hub with **Unity 2022.3 LTS** (URP).
2. Run the three Editor menu commands under **Tank Assault** in order (see
   `Documentation/SETUP_GUIDE.md` for details):
   `1. Build Projectiles and VFX` → `2. Generate Sample Content Data` →
   `3. Build Character Prefabs` → `4. Build Bootstrap + Demo Scenes`.
3. Open `Assets/_Project/Scenes/Bootstrap.unity`, add both Bootstrap and Gameplay_Demo to
   the Build Settings scene list, and press Play from Bootstrap (or just open
   `Gameplay_Demo.unity` directly and press Play for the fastest loop).

## Design scope

This implements every system requested for a commercial-quality mobile tank game:
7 weapons, 10 enemy archetypes with unique AI, 8 world themes, story/endless/survival
modes, boss fights, a full economy (coins/gems/ads/dailies/missions/achievements), a
20-tank collection with per-track upgrades, a complete UI screen set, pooled VFX/audio,
and mobile optimization hooks (object pooling, LOD, GPU instancing, occlusion culling).

**Scope note:** this delivery is the engineering foundation — architecture, systems, data,
and a running placeholder build. Final 3D art, animation rigs, mixed audio, and hand-tuned
level geometry are art/content production work that happens *in* the Unity Editor and is
out of scope for a code-only delivery; see `Documentation/CONTENT_GUIDE.md` for how the
existing pipeline slots that content in.
