using System.IO;
using UnityEditor;
using UnityEngine;

namespace TankAssault.EditorTools
{
    /// <summary>Shared helper for the placeholder-content generator scripts to guarantee target folders exist before AssetDatabase writes.</summary>
    public static class EditorFolderUtil
    {
        public static void EnsureFolder(string assetsRelativePath)
        {
            string fullPath = Path.Combine(Application.dataPath, "..", assetsRelativePath);
            Directory.CreateDirectory(fullPath);
        }

        public static void RefreshAndSave()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
