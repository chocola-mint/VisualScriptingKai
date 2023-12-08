using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace CHM.VisualScriptingKai.Editor
{
    public class GraphAssetListTracker : AssetPostprocessor
    {
        public static event System.Action OnGraphAssetListChanged;
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            // Mark dirty if graph assets are found.
            if(didDomainReload
            || PathsContainGraphAssets(importedAssets)
            || PathsContainGraphAssets(deletedAssets)
            || PathsContainGraphAssets(movedAssets))
            {
                OnGraphAssetListChanged?.Invoke();
            }
        }
        private static bool PathsContainGraphAssets(string[] paths)
        {
            foreach(var path in paths)
            {
                var graphAsset = AssetDatabase.LoadAssetAtPath<MacroScriptableObject>(path);
                if(graphAsset != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
