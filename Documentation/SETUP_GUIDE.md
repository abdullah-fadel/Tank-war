# Setup Guide

## Requirements

- Unity **2022.3 LTS** (`2022.3.21f1` recommended — see `TankAssault/ProjectSettings/ProjectVersion.txt`).
- Android Build Support module (for on-device testing) — optional for iterating in-Editor.

## 1. Open the project

Open Unity Hub → Add → select the `TankAssault/` folder (not the repo root). First open
will trigger package resolution for URP, Input System, Cinemachine, TextMeshPro, and
Addressables (see `Packages/manifest.json`).

Unity will prompt two one-time dialogs on first open:

1. **TextMeshPro Essentials import** — click "Import TMP Essentials". Required once;
   after this the UI screens' `TMP_Text` components render correctly.
2. **Active Input Handling** — installing the Input System package prompts a restart.
   Set **Edit → Project Settings → Player → Active Input Handling** to **Both** (the
   touch controls use `UnityEngine.EventSystems` interfaces that work with either input
   backend, so "Both" is the safest default).

## 2. Generate the playable placeholder build

The repo ships **code only** — no 3D art/animation/audio binaries. Four Editor menu
commands under **Tank Assault** generate a fully wired, primitive-mesh build so you can
press Play immediately. Run them **in order** (each depends on assets the previous step
created):

1. **Tank Assault → 1. Build Projectiles and VFX**
   Creates the pooled projectile prefabs (standard + homing) and muzzle
   flash/explosion/impact particle prefabs under `Assets/_Project/Prefabs/`.

2. **Tank Assault → 2. Generate Sample Content Data**
   Creates the `ScriptableObject` assets under `Assets/_Project/ScriptableObjects/`: 7
   weapons, 20 tanks, 10 enemy types, 8 world configs (3 levels each), 9 upgrade tracks,
   5 sample missions, 5 achievements.

3. **Tank Assault → 3. Build Character Prefabs**
   Creates the player tank prefab (wired to `TankData_ScoutRaider`, `WeaponData_Cannon`,
   `WeaponData_HomingMissile`) and all 10 enemy prefabs (each wired to its matching
   `EnemyData` asset) under `Assets/_Project/Prefabs/`.

4. **Tank Assault → 4. Build Bootstrap + Demo Scenes**
   Creates `Assets/_Project/Scenes/Bootstrap.unity` (all persistent singleton managers)
   and `Assets/_Project/Scenes/Gameplay_Demo.unity` (ground plane, spawned player tank,
   4 sample enemies, camera rig, sun light, level-end trigger).

Re-running any step is safe/idempotent — existing assets at the same path are reused, not
duplicated.

## 3. Press Play

Fastest loop: open `Assets/_Project/Scenes/Gameplay_Demo.unity` directly and press Play —
managers auto-instantiate lazily via the `Singleton<T>` base class the first time
something calls `.Instance`, so the demo scene runs standalone without loading Bootstrap
first. Drive the tank left/right with keyboard arrow keys once you wire a keyboard
fallback in `InputManager` (touch controls need on-device or Editor touch simulation —
see `Window → Analysis → Device Simulator` to test the virtual joystick/buttons in-Editor).

For the full flow (Bootstrap → Main Menu → Level Select → Gameplay), add both scenes to
**File → Build Settings → Scenes In Build** with Bootstrap first, and build out the menu
Canvas UI referencing the `UI` scripts in `Scripts/UI` (the scripts are complete and
functional; the Canvas/RectTransform layout and visual styling is art/UX production work
to be done in the Editor — see `CONTENT_GUIDE.md`).

## 4. Known limitations of the generated placeholder content

- All meshes are Unity primitives (cubes/cylinders/spheres/capsules) with flat-color
  materials — swap `ModelPrefab` references and materials per `TankData`/`EnemyData` once
  real art lands; no gameplay script needs to change.
- Animator Controllers are not generated (no rig to animate yet). `TankAnimationController`
  and enemy scripts call `Animator.SetFloat/SetTrigger` defensively (null-checked) so they
  no-op harmlessly until an Animator + Controller is assigned.
- Audio clips, an `AudioMixer` asset, and UI Canvas layouts are not generated — these are
  the parts of a "premium mobile UI" / "dynamic music" experience that genuinely require a
  human ear/eye in the Editor, not a script.
- The generated projectile prefab uses a single shared physics layer for all factions
  (player and enemy projectiles alike); before shipping, split into per-faction prefab
  variants with proper layer-based `hitMask`s (see `Scripts/Weapons/Projectile.cs`).
