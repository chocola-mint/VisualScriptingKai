using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;

namespace CHM.VisualScriptingKai.Editor
{
    /// <summary>
    /// Union class that lets all kinds of graph assets share the same API, without excessive code duplication.
    /// </summary>
    public class GraphSource
    {
        public readonly ScriptGraphAsset scriptGraphAsset;
        public readonly StateGraphAsset stateGraphAsset;
        public readonly ScriptMachine scriptMachine;
        public readonly StateMachine stateMachine;
        public GraphSource(ScriptGraphAsset asset) => scriptGraphAsset = asset;
        public GraphSource(StateGraphAsset asset) => stateGraphAsset = asset;
        public GraphSource(ScriptMachine machine) => scriptMachine = machine;
        public GraphSource(StateMachine machine) => stateMachine = machine;
        // Casting to Unity.Object extracts the union's non-null type.
        public static implicit operator Object(GraphSource source)
        {
            if(source.scriptGraphAsset != null)
                return source.scriptGraphAsset;
            else if(source.stateGraphAsset != null)
                return source.stateGraphAsset;
            else if(source.scriptMachine != null)
                return source.scriptMachine;
            else return source.stateMachine;
        }
        public static explicit operator ScriptableObject(GraphSource source)
        {
            return ((Object) source) as ScriptableObject;
        }
        public static explicit operator MonoBehaviour(GraphSource source)
        {
            return ((Object) source) as MonoBehaviour;
        }
        public string Name => ((Object) this).name;
        public string ShortInfo {
            get 
            {
                // Casting to Unity.Object extracts the union. (See above)
                var obj = (Object)this;
                // Note: Uses VisualScripting's built-in extension methods here for prettifying.
                string info = $"<b>Source:</b> {obj.GetType().HumanName()}";
                return info;
            }
        }
        public string Info {
            get
            {
                // Casting to Unity.Object extracts the union. (See above)
                var obj = (Object)this;
                // Note: Uses VisualScripting's built-in extension methods here for prettifying.
                string info = $"<b>Source:</b> {obj.name} ({obj.GetType().HumanName()})";
                // Downcast as needed.
                if(obj is ScriptableObject so)
                {
                    info += $"\n<b>Asset Path:</b> {AssetDatabase.GetAssetPath(so)}";
                }
                else
                {
                    var mono = obj as MonoBehaviour;
                    var gameObject = mono.gameObject;
                    // Only two cases: In a scene or in a prefab.
                    // We don't care about prefab instances in a scene, as they're
                    // just in a scene in that case.
                    if(gameObject.scene != null)
                    {
                        info += $"\n<b>Scene Path:</b> {gameObject.scene.path}";
                    }
                    else 
                    {
                        info += $"\n<b>Prefab Path:</b> {PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject)}";
                    }
                }
                return info;
            }
        }
        public int GetInstanceID() => ((Object) this).GetInstanceID();
        public IGraphRoot GraphRoot
        {
            get {
                if(scriptGraphAsset != null)
                    return scriptGraphAsset;
                else if(stateGraphAsset != null)
                    return stateGraphAsset;
                else if(scriptMachine != null)
                    return scriptMachine;
                else return stateMachine;
            }
        }
        public bool TryGetScenePath(out string scenePath)
        {
            var obj = (Object) this;
            if(obj is MonoBehaviour mono)
            {
                if(mono.gameObject.scene != null)
                    scenePath = mono.gameObject.scene.path;
                else scenePath = string.Empty;
                return true;
            }
            else 
            {
                scenePath = string.Empty;
                return false;
            }
        }
        public bool TryGetGameObject(out GameObject gameObject)
        {
            var obj = (Object) this;
            if(obj is MonoBehaviour mono)
            {
                gameObject = mono.gameObject;
                return true;
            }
            else
            {
                gameObject = null;
                return false;
            }
        }
    }
}
