using TankAssault.Enemies;
using TankAssault.Enemies.EnemyTypes;
using TankAssault.Player;
using TankAssault.VFX;
using TankAssault.Weapons;
using UnityEditor;
using UnityEngine;

namespace TankAssault.EditorTools
{
    /// <summary>
    /// Generates fully-wired, primitive-based placeholder prefabs (player tank, 10 enemy types,
    /// projectiles, VFX) so the project can be pressed Play on immediately without waiting on
    /// externally authored 3D art. Replace the primitive meshes/materials with real art assets
    /// per TankData/EnemyData without touching any gameplay script.
    /// </summary>
    public static class PrefabBuilder
    {
        private const string PlayerPrefabFolder = "Assets/_Project/Prefabs/Player";
        private const string EnemyPrefabFolder = "Assets/_Project/Prefabs/Enemies";
        private const string WeaponPrefabFolder = "Assets/_Project/Prefabs/Weapons";
        private const string VfxPrefabFolder = "Assets/_Project/Prefabs/VFX";
        private const string MaterialFolder = "Assets/_Project/Materials/Placeholder";
        private const string IraqFlagTexturePath = "Assets/_Project/Textures/Flags/IraqFlag_Diffuse.png";

        public const string StandardProjectilePath = WeaponPrefabFolder + "/Projectile_Standard.prefab";
        public const string HomingProjectilePath = WeaponPrefabFolder + "/Projectile_Homing.prefab";

        [MenuItem("Tank Assault/1. Build Projectiles and VFX")]
        public static void BuildProjectilesAndVfx()
        {
            EditorFolderUtil.EnsureFolder(WeaponPrefabFolder);
            EditorFolderUtil.EnsureFolder(VfxPrefabFolder);
            EditorFolderUtil.EnsureFolder(MaterialFolder);
            EditorFolderUtil.RefreshAndSave();

            BuildProjectilePrefab("Projectile_Standard", homing: false);
            BuildProjectilePrefab("Projectile_Homing", homing: true);
            BuildMuzzleFlashPrefab();
            BuildExplosionPrefab();
            BuildImpactPrefab();

            EditorFolderUtil.RefreshAndSave();
            Debug.Log("[PrefabBuilder] Projectile and VFX prefabs generated under Assets/_Project/Prefabs.");
        }

        [MenuItem("Tank Assault/3. Build Character Prefabs")]
        public static void BuildCharacterPrefabs()
        {
            EditorFolderUtil.EnsureFolder(PlayerPrefabFolder);
            EditorFolderUtil.EnsureFolder(EnemyPrefabFolder);
            EditorFolderUtil.EnsureFolder(MaterialFolder);
            EditorFolderUtil.RefreshAndSave();

            var standardProjectile = AssetDatabase.LoadAssetAtPath<GameObject>(StandardProjectilePath);
            if (standardProjectile == null)
            {
                Debug.LogError("[PrefabBuilder] Run 'Tank Assault/1. Build Projectiles and VFX' first.");
                return;
            }

            BuildPlayerTankPrefab(standardProjectile);

            BuildLightTank();
            BuildHeavyTank();
            BuildSniperTank();
            BuildRocketTank();
            BuildDrone();
            BuildHelicopter();
            BuildJet();
            BuildTurret();
            BuildWalkingRobot();
            BuildBossTank();

            EditorFolderUtil.RefreshAndSave();
            Debug.Log("[PrefabBuilder] Character prefabs generated under Assets/_Project/Prefabs.");
        }

        private static Material CreateColorMaterial(string name, Color color)
        {
            string path = $"{MaterialFolder}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader) { color = color };
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        private static Material CreateFlagMaterial(string name, string texturePath)
        {
            string path = $"{MaterialFolder}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            var importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer != null && !importer.alphaIsTransparency)
            {
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.SaveAndReimport();
            }
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", texture);
            if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", texture);
            if (mat.HasProperty("_Cutoff")) mat.SetFloat("_Cutoff", 0.5f);
            if (mat.HasProperty("_AlphaClip")) mat.SetFloat("_AlphaClip", 1f);
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.SetOverrideTag("RenderType", "TransparentCutout");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;

            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        private static GameObject SavePrefab(GameObject instance, string folder, string name)
        {
            string path = $"{folder}/{name}.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(instance, path);
            Object.DestroyImmediate(instance);
            return prefab;
        }

        private static Transform CreateChildPrimitive(Transform parent, PrimitiveType type, string name, Vector3 localPos, Vector3 localScale, Material mat, bool keepCollider = false)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;
            go.transform.localRotation = Quaternion.identity;

            if (!keepCollider)
                Object.DestroyImmediate(go.GetComponent<Collider>());

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null && mat != null) renderer.sharedMaterial = mat;

            return go.transform;
        }

        private static Transform CreateEmpty(Transform parent, string name, Vector3 localPos)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = localPos;
            return go.transform;
        }

        // ---------------------------------------------------------------- Projectiles / VFX

        private static GameObject BuildProjectilePrefab(string name, bool homing)
        {
            var mat = CreateColorMaterial("Mat_Projectile", new Color(1f, 0.6f, 0.1f));
            var root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            root.name = name;
            root.transform.localScale = Vector3.one * 0.35f;
            root.GetComponent<Renderer>().sharedMaterial = mat;

            Object.DestroyImmediate(root.GetComponent<Collider>());
            var col = root.AddComponent<SphereCollider>();
            col.isTrigger = true;

            var rb = root.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            Component projectileComponent = homing ? root.AddComponent<HomingProjectile>() : root.AddComponent<Projectile>();
            SetPrivateField(projectileComponent, "hitMask", (LayerMask)~0);

            return SavePrefab(root, WeaponPrefabFolder, name);
        }

        private static void BuildMuzzleFlashPrefab()
        {
            var root = new GameObject("MuzzleFlash_Standard");
            var ps = root.AddComponent<ParticleSystem>();
            ConfigureBurstParticles(ps, new Color(1f, 0.85f, 0.3f), 0.15f, 12);
            root.AddComponent<MuzzleFlashEffect>();
            SavePrefab(root, VfxPrefabFolder, "MuzzleFlash_Standard");
        }

        private static void BuildExplosionPrefab()
        {
            var root = new GameObject("Explosion_Standard");
            var ps = root.AddComponent<ParticleSystem>();
            ConfigureBurstParticles(ps, new Color(1f, 0.4f, 0.05f), 1.2f, 40);

            var lightGo = new GameObject("Flash");
            lightGo.transform.SetParent(root.transform);
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.6f, 0.2f);
            light.range = 8f;
            light.intensity = 6f;

            var effect = root.AddComponent<ExplosionEffect>();
            SavePrefab(root, VfxPrefabFolder, "Explosion_Standard");
        }

        private static void BuildImpactPrefab()
        {
            var root = new GameObject("Impact_Standard");
            var ps = root.AddComponent<ParticleSystem>();
            ConfigureBurstParticles(ps, Color.gray, 0.5f, 10);
            root.AddComponent<ImpactEffect>();
            SavePrefab(root, VfxPrefabFolder, "Impact_Standard");
        }

        private static void ConfigureBurstParticles(ParticleSystem ps, Color color, float lifetime, int burstCount)
        {
            var main = ps.main;
            main.startLifetime = lifetime;
            main.startSpeed = 3f;
            main.startSize = 0.4f;
            main.startColor = color;
            main.loop = false;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)burstCount) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.1f;
        }

        // ---------------------------------------------------------------- Player

        private static void BuildPlayerTankPrefab(GameObject projectilePrefab)
        {
            var hullMat = CreateColorMaterial("Mat_PlayerHull", new Color(0.2f, 0.45f, 0.25f));
            var turretMat = CreateColorMaterial("Mat_PlayerTurret", new Color(0.15f, 0.35f, 0.2f));
            var wheelMat = CreateColorMaterial("Mat_Wheel", new Color(0.1f, 0.1f, 0.1f));
            var poleMat = CreateColorMaterial("Mat_FlagPole", new Color(0.3f, 0.3f, 0.3f));
            var flagMat = CreateFlagMaterial("Mat_IraqFlag", IraqFlagTexturePath);

            var root = new GameObject("PlayerTank");
            root.tag = "Player";
            var rb = root.AddComponent<Rigidbody>();
            rb.mass = 500f;

            root.AddComponent<BoxCollider>().size = new Vector3(2.2f, 0.8f, 1.4f);

            var hull = CreateChildPrimitive(root.transform, PrimitiveType.Cube, "Hull", new Vector3(0, 0.5f, 0), new Vector3(2.2f, 0.6f, 1.3f), hullMat);

            // Iraqi flag decal (cropped from the supplied 3D map/flags reference asset) on a pole
            // at the rear of the hull. A cutout-textured quad reads better at this scale than
            // trying to reproduce the flag's script and emblem out of flat-color primitives.
            const float flagWidth = 0.55f;
            const float flagAspect = 153f / 246f;
            const float flagHeight = flagWidth * flagAspect;

            CreateChildPrimitive(root.transform, PrimitiveType.Cylinder, "FlagPole", new Vector3(-0.9f, 1.15f, 0.5f), new Vector3(0.025f, 0.35f, 0.025f), poleMat);
            CreateChildPrimitive(root.transform, PrimitiveType.Quad, "IraqFlag", new Vector3(-0.9f - flagWidth * 0.5f, 1.35f, 0.5f), new Vector3(flagWidth, flagHeight, 1f), flagMat);

            var turretPivot = CreateEmpty(root.transform, "TurretPivot", new Vector3(0, 0.9f, 0));
            CreateChildPrimitive(turretPivot, PrimitiveType.Cylinder, "TurretBody", Vector3.zero, new Vector3(0.7f, 0.25f, 0.7f), turretMat);

            var cannonPivot = CreateEmpty(turretPivot, "CannonPivot", new Vector3(0.3f, 0f, 0f));
            var cannonMesh = CreateChildPrimitive(cannonPivot, PrimitiveType.Cylinder, "CannonMesh", new Vector3(0.6f, 0, 0), new Vector3(0.15f, 0.6f, 0.15f), turretMat);
            cannonMesh.localRotation = Quaternion.Euler(0, 0, 90);

            var muzzlePoint = CreateEmpty(cannonPivot, "MuzzlePoint", new Vector3(1.2f, 0f, 0f));
            var missileMuzzle = CreateEmpty(turretPivot, "MissileMuzzlePoint", new Vector3(0.3f, 0.3f, 0f));

            // Wheels (4, purely cosmetic suspension targets).
            var wheelPositions = new[]
            {
                new Vector3(-0.8f, 0.35f, 0.65f), new Vector3(0.8f, 0.35f, 0.65f),
                new Vector3(-0.8f, 0.35f, -0.65f), new Vector3(0.8f, 0.35f, -0.65f)
            };

            var suspension = root.AddComponent<SuspensionController>();
            var wheelMounts = new SuspensionController.WheelMount[wheelPositions.Length];

            for (int i = 0; i < wheelPositions.Length; i++)
            {
                var mount = CreateEmpty(root.transform, $"WheelMount_{i}", wheelPositions[i]);
                var wheelVisual = CreateChildPrimitive(mount, PrimitiveType.Cylinder, $"WheelVisual_{i}", Vector3.zero, new Vector3(0.4f, 0.15f, 0.4f), wheelMat);
                wheelVisual.localRotation = Quaternion.Euler(0, 0, 90);
                wheelMounts[i] = new SuspensionController.WheelMount { MountPoint = mount, WheelVisual = wheelVisual };
            }

            typeof(SuspensionController).GetField("wheels", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(suspension, wheelMounts);

            var tankData = AssetDatabase.LoadAssetAtPath<TankAssault.Data.TankData>(
                $"{ContentDataGenerator.TankDataFolder}/TankData_ScoutRaider.asset");
            var cannonData = AssetDatabase.LoadAssetAtPath<TankAssault.Data.WeaponData>(
                $"{ContentDataGenerator.WeaponDataFolder}/WeaponData_Cannon.asset");
            var missileData = AssetDatabase.LoadAssetAtPath<TankAssault.Data.WeaponData>(
                $"{ContentDataGenerator.WeaponDataFolder}/WeaponData_HomingMissile.asset");

            var controller = root.AddComponent<TankController>();
            SetPrivateField(controller, "tankData", tankData);
            SetPrivateField(controller, "groundMask", (LayerMask)~0);

            var health = root.AddComponent<TankHealth>();
            SetPrivateField(health, "tankData", tankData);

            root.AddComponent<TankAnimationController>();

            var turret = root.AddComponent<TurretController>();
            SetPrivateField(turret, "turretPivot", turretPivot);
            SetPrivateField(turret, "cannonPivot", cannonPivot);

            root.AddComponent<FuelSystem>();

            var cannonWeaponGo = new GameObject("PrimaryWeapon_Cannon");
            cannonWeaponGo.transform.SetParent(cannonPivot);
            var cannonWeapon = cannonWeaponGo.AddComponent<CannonWeapon>();
            SetPrivateField(cannonWeapon, "muzzlePoint", muzzlePoint);
            SetPrivateField(cannonWeapon, "weaponData", cannonData);
            SetPrivateField(cannonWeapon, "targetMask", (LayerMask)~0);

            var missileWeaponGo = new GameObject("MissileWeapon_Homing");
            missileWeaponGo.transform.SetParent(turretPivot);
            var missileWeapon = missileWeaponGo.AddComponent<HomingMissileWeapon>();
            SetPrivateField(missileWeapon, "muzzlePoint", missileMuzzle);
            SetPrivateField(missileWeapon, "weaponData", missileData);
            SetPrivateField(missileWeapon, "targetMask", (LayerMask)~0);

            var weaponManager = root.AddComponent<PlayerWeaponManager>();
            SetPrivateField(weaponManager, "primaryWeapon", cannonWeapon);
            SetPrivateField(weaponManager, "missileWeapon", missileWeapon);
            SetPrivateField(weaponManager, "turretController", turret);
            SetPrivateField(weaponManager, "animationController", root.GetComponent<TankAnimationController>());

            SavePrefab(root, PlayerPrefabFolder, "PlayerTank");
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(target, value);
        }

        // ---------------------------------------------------------------- Enemies

        private static GameObject CreateEnemyBase(string name, Color color, PrimitiveType bodyShape, Vector3 bodyScale)
        {
            var mat = CreateColorMaterial($"Mat_Enemy_{name}", color);
            var root = new GameObject(name);
            root.AddComponent<BoxCollider>().size = bodyScale;
            CreateChildPrimitive(root.transform, bodyShape, "Body", new Vector3(0, bodyScale.y * 0.5f, 0), bodyScale, mat);
            root.AddComponent<EnemyHealth>();
            root.AddComponent<EnemyContactDamage>();
            return root;
        }

        private static void AssignEnemyData(Component ai, string enemyDataAssetName)
        {
            var data = AssetDatabase.LoadAssetAtPath<TankAssault.Data.EnemyData>(
                $"{ContentDataGenerator.EnemyDataFolder}/EnemyData_{enemyDataAssetName}.asset");
            if (data == null)
            {
                Debug.LogWarning($"[PrefabBuilder] EnemyData_{enemyDataAssetName} not found. Run 'Tank Assault/2. Generate Sample Content Data' first.");
                return;
            }
            SetPrivateField(ai, "enemyData", data);
        }

        private static void BuildLightTank()
        {
            var root = CreateEnemyBase("LightTank", new Color(0.6f, 0.2f, 0.2f), PrimitiveType.Cube, new Vector3(1.5f, 0.5f, 1f));
            var muzzle = CreateEmpty(root.transform, "MuzzlePoint", new Vector3(0.9f, 0.4f, 0f));
            var ai = root.AddComponent<LightTankAI>();
            SetPrivateField(ai, "muzzlePoint", muzzle);
            AssignEnemyData(ai, "LightTank");
            SavePrefab(root, EnemyPrefabFolder, "LightTank");
        }

        private static void BuildHeavyTank()
        {
            var root = CreateEnemyBase("HeavyTank", new Color(0.35f, 0.15f, 0.15f), PrimitiveType.Cube, new Vector3(2.2f, 0.8f, 1.4f));
            var muzzle = CreateEmpty(root.transform, "MuzzlePoint", new Vector3(1.2f, 0.5f, 0f));
            var ai = root.AddComponent<HeavyTankAI>();
            SetPrivateField(ai, "muzzlePoint", muzzle);
            AssignEnemyData(ai, "HeavyTank");
            SavePrefab(root, EnemyPrefabFolder, "HeavyTank");
        }

        private static void BuildSniperTank()
        {
            var root = CreateEnemyBase("SniperTank", new Color(0.5f, 0.5f, 0.15f), PrimitiveType.Cube, new Vector3(1.3f, 0.5f, 0.9f));
            var muzzle = CreateEmpty(root.transform, "MuzzlePoint", new Vector3(1f, 0.4f, 0f));
            var telegraph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            telegraph.name = "ChargeTelegraph";
            telegraph.transform.SetParent(root.transform);
            telegraph.transform.localPosition = new Vector3(1f, 0.4f, 0f);
            telegraph.transform.localScale = Vector3.one * 0.2f;
            Object.DestroyImmediate(telegraph.GetComponent<Collider>());
            telegraph.SetActive(false);

            var ai = root.AddComponent<SniperTankAI>();
            SetPrivateField(ai, "muzzlePoint", muzzle);
            SetPrivateField(ai, "chargeTelegraphVfx", telegraph);
            AssignEnemyData(ai, "SniperTank");
            SavePrefab(root, EnemyPrefabFolder, "SniperTank");
        }

        private static void BuildRocketTank()
        {
            var root = CreateEnemyBase("RocketTank", new Color(0.5f, 0.3f, 0.1f), PrimitiveType.Cube, new Vector3(1.6f, 0.6f, 1.1f));
            var muzzle = CreateEmpty(root.transform, "MuzzlePoint", new Vector3(1f, 0.5f, 0f));
            var ai = root.AddComponent<RocketTankAI>();
            SetPrivateField(ai, "muzzlePoint", muzzle);
            AssignEnemyData(ai, "RocketTank");
            SavePrefab(root, EnemyPrefabFolder, "RocketTank");
        }

        private static void BuildDrone()
        {
            var root = CreateEnemyBase("Drone", new Color(0.2f, 0.4f, 0.6f), PrimitiveType.Sphere, new Vector3(0.8f, 0.8f, 0.8f));
            var muzzle = CreateEmpty(root.transform, "MuzzlePoint", new Vector3(0.5f, 0f, 0f));
            var ai = root.AddComponent<DroneAI>();
            SetPrivateField(ai, "muzzlePoint", muzzle);
            AssignEnemyData(ai, "Drone");
            SavePrefab(root, EnemyPrefabFolder, "Drone");
        }

        private static void BuildHelicopter()
        {
            var root = CreateEnemyBase("Helicopter", new Color(0.25f, 0.3f, 0.2f), PrimitiveType.Cube, new Vector3(1.8f, 0.6f, 1f));
            var bombPoint = CreateEmpty(root.transform, "BombDropPoint", new Vector3(0f, -0.4f, 0f));
            var ai = root.AddComponent<HelicopterAI>();
            SetPrivateField(ai, "bombDropPoint", bombPoint);
            AssignEnemyData(ai, "Helicopter");
            SavePrefab(root, EnemyPrefabFolder, "Helicopter");
        }

        private static void BuildJet()
        {
            var root = CreateEnemyBase("Jet", new Color(0.5f, 0.5f, 0.55f), PrimitiveType.Cube, new Vector3(2.2f, 0.4f, 0.9f));
            var muzzle = CreateEmpty(root.transform, "MuzzlePoint", new Vector3(-1.1f, 0f, 0f));
            var ai = root.AddComponent<JetAI>();
            SetPrivateField(ai, "muzzlePoint", muzzle);
            AssignEnemyData(ai, "Jet");
            SavePrefab(root, EnemyPrefabFolder, "Jet");
        }

        private static void BuildTurret()
        {
            var root = CreateEnemyBase("Turret", new Color(0.4f, 0.4f, 0.4f), PrimitiveType.Cylinder, new Vector3(1f, 0.4f, 1f));
            var barrelPivot = CreateEmpty(root.transform, "BarrelPivot", new Vector3(0, 0.5f, 0));
            CreateChildPrimitive(barrelPivot, PrimitiveType.Cylinder, "Barrel", new Vector3(0.5f, 0, 0), new Vector3(0.1f, 0.5f, 0.1f), null).localRotation = Quaternion.Euler(0, 0, 90);
            var muzzle = CreateEmpty(barrelPivot, "MuzzlePoint", new Vector3(1f, 0f, 0f));
            var ai = root.AddComponent<TurretAI>();
            SetPrivateField(ai, "barrelPivot", barrelPivot);
            SetPrivateField(ai, "muzzlePoint", muzzle);
            AssignEnemyData(ai, "Turret");
            SavePrefab(root, EnemyPrefabFolder, "Turret");
        }

        private static void BuildWalkingRobot()
        {
            var root = CreateEnemyBase("WalkingRobot", new Color(0.3f, 0.3f, 0.35f), PrimitiveType.Capsule, new Vector3(0.8f, 1f, 0.8f));
            var ai = root.AddComponent<WalkingRobotAI>();
            AssignEnemyData(ai, "WalkingRobot");
            SavePrefab(root, EnemyPrefabFolder, "WalkingRobot");
        }

        private static void BuildBossTank()
        {
            var root = CreateEnemyBase("BossTank", new Color(0.5f, 0.1f, 0.1f), PrimitiveType.Cube, new Vector3(3.5f, 1.3f, 2f));
            var cannonMuzzle = CreateEmpty(root.transform, "CannonMuzzle", new Vector3(2f, 0.8f, 0f));
            var rocketMuzzle = CreateEmpty(root.transform, "RocketMuzzle", new Vector3(2f, 0.3f, 0.6f));

            var standardProjectile = AssetDatabase.LoadAssetAtPath<GameObject>(StandardProjectilePath);

            var ai = root.AddComponent<BossTankAI>();
            SetPrivateField(ai, "cannonMuzzle", cannonMuzzle);
            SetPrivateField(ai, "rocketMuzzle", rocketMuzzle);
            SetPrivateField(ai, "cannonProjectilePrefab", standardProjectile);
            SetPrivateField(ai, "rocketProjectilePrefab", standardProjectile);
            AssignEnemyData(ai, "BossTank");
            SavePrefab(root, EnemyPrefabFolder, "BossTank");
        }
    }
}
