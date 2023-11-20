using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CHM.VisualScriptingPlus.Editor
{
    public static class AssetUtility
    {
        public static IEnumerable<T> FindAssetsByType<T>(string[] searchInFolders = null) where T : Object
        {
            // Do this check ourselves so AssetDatabase.FindAssets won't throw.
            foreach(string folder in searchInFolders)
            {
                if(!AssetDatabase.IsValidFolder(folder))
                    yield break;
            }
            foreach(var guid in AssetDatabase.FindAssets($"t:{typeof(T)}", searchInFolders))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if(asset != null)
                    yield return asset;
            }
        }
    }
}
