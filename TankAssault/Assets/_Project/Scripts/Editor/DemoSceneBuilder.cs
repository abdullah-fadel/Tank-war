using TankAssault.CameraSystem;
using TankAssault.Core;
using TankAssault.Economy;
using TankAssault.Input;
using TankAssault.LevelSystem;
using TankAssault.Progression;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TankAssault.EditorTools
{
    /// <summary>
    /// Builds two scenes so the project can be pressed Play immediately:
    /// Bootstrap (persistent singleton managers, loaded first) and Gameplay_Demo
    /// (ground plane, spawned player tank, a handful of enemies, camera rig, sun light).
    /// Run '1. Build Projectiles and VFX', '2. Generate Sample Content Data', and
    /// '3. Build Character Prefabs' first so the prefabs referenced here exist.
    /// </summary>
    public static class DemoSceneBuilder
    {
        private const string ScenesFolder = "Assets/_Project/Scenes";

        [MenuItem("Tank Assault/4. Build Bootstrap + Demo Scenes")]
        public static void BuildScenes()
        {
            EditorFolderUtil.EnsureFolder(ScenesFolder);
            EditorFolderUtil.RefreshAndSave();

            BuildBootstrapScene();
            BuildGameplayDemoScene();

            EditorFolderUtil.RefreshAndSave();
            Debug.Log("[DemoSceneBuilder] Bootstrap and Gameplay_Demo scenes created under Assets/_Project/Scenes.");
        }

        private static void BuildBootstrapScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var managers = new GameObject("--- Managers ---");
            managers.AddComponent<GameManager>();
            managers.AddComponent<SaveSystem>();
            managers.AddComponent<PoolManager>();
            managers.AddComponent<InputManager>();
            managers.AddComponent<CurrencyManager>();
            managers.AddComponent<InventoryManager>();
            managers.AddComponent<UpgradeManager>();
            managers.AddComponent<XPManager>();
            managers.AddComponent<ComboSystem>();
            managers.AddComponent<MissionManager>();
            managers.AddComponent<AchievementManager>();
            managers.AddComponent<DailyRewardManager>();
            managers.AddComponent<Audio.AudioManager>();

            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            EditorSceneManager.SaveScene(scene, $"{ScenesFolder}/Bootstrap.unity");
        }

        private static void BuildGameplayDemoScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Lighting
            var sunGo = new GameObject("Directional Light");
            var sun = sunGo.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.2f;
            sunGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            RenderSettings.sun = sun;

            // Ground
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(20f, 1f, 20f);

            // Player
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Player/PlayerTank.prefab");
            GameObject player = null;
            if (playerPrefab != null)
            {
                player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
                player.transform.position = new Vector3(0f, 1f, 0f);
            }
            else
            {
                Debug.LogWarning("[DemoSceneBuilder] PlayerTank prefab not found. Run step 3 first.");
            }

            // Camera
            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            var cam = cameraGo.AddComponent<Camera>();
            cam.fieldOfView = 50f;
            cameraGo.AddComponent<AudioListener>();
            cameraGo.AddComponent<CameraShake>();
            var rig = cameraGo.AddComponent<CameraRig>();
            if (player != null)
                typeof(CameraRig).GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(rig, player.transform);

            // A few enemies for an immediate playable sample.
            SpawnEnemyIfExists("LightTank", new Vector3(15f, 1f, 0f));
            SpawnEnemyIfExists("HeavyTank", new Vector3(25f, 1f, 0f));
            SpawnEnemyIfExists("Turret", new Vector3(35f, 0.5f, 0f));
            SpawnEnemyIfExists("Drone", new Vector3(20f, 6f, 0f));

            // Level end zone.
            var endZoneGo = new GameObject("LevelEndZone");
            endZoneGo.transform.position = new Vector3(60f, 1f, 0f);
            var endCollider = endZoneGo.AddComponent<BoxCollider>();
            endCollider.size = new Vector3(2f, 5f, 10f);
            endCollider.isTrigger = true;
            endZoneGo.AddComponent<LevelEndZone>();

            // Minimal world-space HUD text via a UI canvas would normally go in a separate
            // Canvas-driving scene; for the runnable demo we keep this scene focused on core
            // gameplay systems (movement, aiming, shooting, AI, pooling, camera).
            EditorSceneManager.SaveScene(scene, $"{ScenesFolder}/Gameplay_Demo.unity");
        }

        private static void SpawnEnemyIfExists(string prefabName, Vector3 position)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/Enemies/{prefabName}.prefab");
            if (prefab == null)
            {
                Debug.LogWarning($"[DemoSceneBuilder] Enemy prefab '{prefabName}' not found. Run step 3 first.");
                return;
            }
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = position;
        }
    }
}
