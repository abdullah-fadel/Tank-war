# Architecture

Tank Assault's code is organized by feature under `Assets/_Project/Scripts/`, following
SOLID boundaries: gameplay systems depend on data (ScriptableObjects) and communicate
through a decoupled event bus rather than reaching into each other directly.

## Core (`Scripts/Core`)

- **`EventBus` / `GameEvents.cs`** — static publish/subscribe hub. Every cross-system
  notification (damage taken, enemy killed, currency changed, mission progress, combo
  changed, level completed, game state changed, ...) is a small `readonly struct` event.
  UI, audio, and progression systems subscribe independently; gameplay code never calls
  into UI or vice versa.
- **`StateMachine` / `IState`** — a minimal generic FSM (`AddTransition`, `AddAnyTransition`,
  `Tick`, `FixedTick`). Used by enemy AI and available for any future player state needs.
- **`ObjectPool<T>` / `PoolManager`** — generic pooling keyed by prefab reference. Every
  projectile, muzzle flash, explosion, and impact effect is spawned/despawned through
  `PoolManager.Instance.Spawn/Despawn`, never `Instantiate`/`Destroy`, which is essential
  for stable frame pacing on mobile.
- **`SaveSystem` / `SaveData`** — JSON (`JsonUtility`) persistence to
  `Application.persistentDataPath`, autosaves every 30s and on pause/quit, keeps a `.bak`
  backup copy. `SaveData` is one flat serializable class covering currency, unlocked
  tanks/achievements, per-tank upgrade levels, level progress, settings, and daily-reward
  streak — easy to version/migrate.
- **`GameManager`** — owns the top-level `GameState` enum (Boot/MainMenu/LevelSelect/
  Loading/Playing/Paused/Victory/Defeat) and current session score/combo. State changes
  publish `GameStateChangedEvent`; `UIManager` reacts to it to show/hide the right screens.

## Data (`Scripts/Data`)

Every piece of tunable content is a `ScriptableObject`, so designers balance the game by
editing assets, not code:

| Asset | Covers |
|---|---|
| `WeaponData` | 7 weapon types, delivery mode (Projectile/Hitscan/Continuous), damage/fire-rate/reload/range/splash, 5 upgrade levels, VFX/SFX refs |
| `TankData` | Base stats, rarity, default loadout, skill id, unlock cost |
| `EnemyData` | Stats, attack tuning (`BehaviourParamA/B/C` for type-specific tuning: sniper charge time, drone strafe amplitude, boss phase thresholds, ...), rewards |
| `WorldConfig` / `LevelInfo` | 8 world themes × N levels each, environment feature flags (bridges/hills/elevators/barrels/breakable walls/hidden paths), star thresholds |
| `UpgradeData` | One asset per upgrade track (Engine/Armor/Tracks/Cannon/MissileLauncher/FireRate/ReloadSpeed/Health/CriticalChance), cost curve |
| `MissionData` / `AchievementData` | Daily/story missions and meta-achievements |

## Player (`Scripts/Player`)

`TankController` drives horizontal movement (this is a side-scroller: only X-axis input
matters) and raycasts down each frame to follow terrain height (hills/bridges/platforms).
`TurretController` rotates the turret/cannon pivots independently toward the aim
direction. `SuspensionController` + `WheelAnimator` are purely cosmetic — per-wheel spring
raycasts and mesh rotation/track scrolling. `TankHealth` implements `IDamageable` (shared
with enemies/barrels/walls in `Scripts/Combat/IDamageable.cs`) with armor mitigation and a
critical-damage-state multiplier. `FuelSystem` is an optional boost resource.
`PlayerWeaponManager` routes the Fire/Aim/Missile input from `InputManager` to the
equipped `WeaponBase` instances.

## Weapons (`Scripts/Weapons`)

`WeaponBase` owns cooldown/magazine/reload bookkeeping and calls an abstract
`FireInternal`. The seven concrete weapons (`MachineGunWeapon`, `CannonWeapon`,
`RocketLauncherWeapon`, `LaserWeapon`, `PlasmaWeapon`, `HomingMissileWeapon`,
`FlameThrowerWeapon`) each implement one of three delivery mechanisms:
- **Hitscan** (`HitscanWeapon.FireRay`) — MachineGun, Laser.
- **Projectile** (`Projectile` / `HomingProjectile`, both pooled) — Cannon, Rocket, Plasma,
  Homing Missile.
- **Continuous** — FlameThrower ticks damage to everything inside a forward cone each
  `DamageTickRate` interval while firing.

## Enemies (`Scripts/Enemies`)

`EnemyBase` + `EnemyHealth` provide shared plumbing (player reference, reward payout on
death, pooled despawn). Each of the 10 enemy types (`Scripts/Enemies/EnemyTypes`) composes
the reusable states in `Scripts/Enemies/AIStates` (`Idle/Patrol/Chase/Attack/Flee/Dead`)
into a genuinely distinct behaviour:

| Type | Behaviour |
|---|---|
| LightTank | Chase into cannon range, single shots |
| HeavyTank | Slow, high-armor, never flees, big shells |
| SniperTank | Max range, telegraphed charge-up shot, flees if closed on |
| RocketTank | Mid-range salvos of splash-damage rockets |
| Drone | Hovering strafe pattern while firing |
| Helicopter | High hover, tracks player X, drops bombs when overhead |
| Jet | High-speed strafing passes across the screen, loops back |
| Turret | Stationary, rotates to face player, high accuracy |
| WalkingRobot | Ground melee lunge-and-strike |
| BossTank | 3 health-gated phases (cannon → +rockets → enrage), drives the camera's cinematic boss zoom |

## Level Systems (`Scripts/LevelSystem`)

`LevelManager` owns checkpoint/respawn state and star-rating calculation.
`ExplosiveBarrel` / `BreakableWall` implement `IDamageable` as environmental hazards/
obstacles. `MovingElevator` is an oscillating platform that parents the player on contact.
`HiddenPathTrigger` reveals secret areas. `EndlessLevelGenerator` recycles terrain chunk
prefabs ahead of/behind the player for the endless mode.

## Economy & Progression (`Scripts/Economy`, `Scripts/Progression`)

`CurrencyManager` (coins/gems) → `ShopManager` (tank/weapon purchases) →
`InventoryManager` (ownership/equipped tank) → `UpgradeManager` (per-track leveling with
an exponential cost curve). `AdsManager` is an `IAdsProvider` abstraction with a
`MockAdsProvider` — swap in a real mediation SDK without touching call sites.
`XPManager`, `ComboSystem` (also computes the score multiplier), `MissionManager`,
`AchievementManager`, and `DailyRewardManager` all listen to the event bus rather than
being called directly by gameplay code.

## UI (`Scripts/UI`)

`UIScreen` is the base class every panel inherits for a consistent fade+scale transition.
`UIManager` is a screen router: one "page" screen (menu/shop/garage/level-select) visible
at a time, with HUD/overlay screens (pause/victory/defeat) stacking on top during
gameplay, driven entirely by `GameStateChangedEvent`. All nine required screens
(MainMenu, Settings, Shop, Inventory, Garage, LevelSelect, Pause, Victory, Defeat) plus the
gameplay HUD are implemented.

## Audio / VFX / Optimization

`AudioManager` routes music/SFX through an `AudioMixer` with a pooled one-shot SFX source
array for 3D spatial audio. `ExplosionEffect`/`MuzzleFlashEffect`/`ImpactEffect` are pooled
VFX components. `Scripts/Optimization` has helpers for LOD group construction, an editor
checklist for occlusion-culling static flags, and a GPU-instancing enabler for shared
materials.

## Editor Tooling (`Scripts/Editor`)

Because this delivery ships code, not final art, `PrefabBuilder`, `ContentDataGenerator`,
and `DemoSceneBuilder` (menu **Tank Assault/1-4**) generate a fully wired, primitive-mesh
placeholder build — 20 tanks, 7 weapons, 10 enemies, 8 worlds worth of data, and two
scenes — so the project runs end-to-end before any art lands. See `SETUP_GUIDE.md`.
