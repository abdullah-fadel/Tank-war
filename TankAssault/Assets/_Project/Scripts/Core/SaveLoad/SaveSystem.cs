using System;
using System.IO;
using UnityEngine;

namespace TankAssault.Core
{
    /// <summary>
    /// JSON-backed save/load with autosave. Writes to Application.persistentDataPath so it
    /// survives app updates on Android/iOS. A .bak copy guards against corruption from a
    /// crash mid-write.
    /// </summary>
    public class SaveSystem : Singleton<SaveSystem>
    {
        private const string FileName = "tankassault_save.json";
        private const float AutoSaveIntervalSeconds = 30f;

        public SaveData Current { get; private set; }

        private float _autoSaveTimer;
        private string FilePath => Path.Combine(Application.persistentDataPath, FileName);
        private string BackupPath => FilePath + ".bak";

        protected override void Awake()
        {
            base.Awake();
            Load();
        }

        private void Update()
        {
            _autoSaveTimer += Time.unscaledDeltaTime;
            if (_autoSaveTimer >= AutoSaveIntervalSeconds)
            {
                _autoSaveTimer = 0f;
                Save();
            }
        }

        public void Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    Current = JsonUtility.FromJson<SaveData>(json);
                }
                else if (File.Exists(BackupPath))
                {
                    string json = File.ReadAllText(BackupPath);
                    Current = JsonUtility.FromJson<SaveData>(json);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveSystem] Failed to load save, starting fresh. {e.Message}");
            }

            Current ??= new SaveData();
        }

        public void Save()
        {
            try
            {
                Current.LastSaveUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string json = JsonUtility.ToJson(Current, prettyPrint: true);

                if (File.Exists(FilePath))
                    File.Copy(FilePath, BackupPath, overwrite: true);

                File.WriteAllText(FilePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Failed to save. {e.Message}");
            }
        }

        public void ResetSave()
        {
            Current = new SaveData();
            Save();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) Save();
        }

        private void OnApplicationQuit()
        {
            Save();
        }
    }
}
