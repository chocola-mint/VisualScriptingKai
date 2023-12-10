using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CHM.VisualScriptingKai.Editor
{
    public static class PackageUtility
    {
        public const string PackageRoot = "Packages/com.chocola-mint.visual-scripting-kai/";
        /// <summary>
        /// Loads an asset from com.chocola-mint.visual-scripting-kai using relative paths.
        /// </summary>
        public static T LoadPackageAsset<T>(string packageAssetPath)
        where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(PackageRoot + packageAssetPath);
        }
    }
}
