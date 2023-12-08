using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingKai
{
    /// <summary>
    /// A lookup table that maps function definitions (ScriptGraphAssets) to implementations.
    /// </summary>
    [RequireComponent(typeof(ScriptMachine))]
    [DisallowMultipleComponent]
    public sealed class FunctionLookupTable : MonoBehaviour
    {
        // Possible optimization? Serialize the dictionaries and make it possible to bake
        // lookup results (maybe as a pre-build step?)
        // But Units are transient, instantiated by graphs that own them, so their references
        // probably can't be serialized directly.
        private readonly Dictionary<ScriptGraphAsset, FunctionImplementation> startUnits = new();
        private readonly Dictionary<ScriptGraphAsset, FunctionImplementation> returnUnits = new();
        void Awake()
        {
            foreach (var scriptMachine in GetComponents<ScriptMachine>())
            {
                if (scriptMachine.graph == null)
                    continue;
                foreach (var unit in scriptMachine.graph.units)
                {
                    if (unit is FunctionStartUnit startUnit)
                    {
                        #if UNITY_EDITOR
                        if (!startUnits.TryAdd(startUnit.functionDefinition, new()
                        {
                            scriptMachine = scriptMachine,
                            startUnit = startUnit,
                        }))
                            Debug.LogWarning($"Found multiple Function Start nodes with definition {startUnit.functionDefinition.name}");
                        #else
                        startUnits.TryAdd(startUnit.functionDefinition, new()
                        {
                            scriptMachine = scriptMachine,
                            startUnit = startUnit,
                        });
                        #endif
                    }
                    else if (unit is FunctionReturnUnit returnUnit)
                    {
                        #if UNITY_EDITOR
                        if (!returnUnits.TryAdd(returnUnit.functionDefinition, new()
                        {
                            scriptMachine = scriptMachine,
                            returnUnit = returnUnit,
                        }))
                            Debug.LogWarning($"Found multiple Function Return nodes with definition {returnUnit.functionDefinition.name}");
                        #else
                        returnUnits.TryAdd(returnUnit.functionDefinition, new()
                        {
                            scriptMachine = scriptMachine,
                            returnUnit = returnUnit,
                        });
                        #endif
                    }
                }
            }
        }

        public static FunctionLookupTable GetFromGameObject(GameObject gameObject)
        {
            if(gameObject.TryGetComponent<FunctionLookupTable>(out var functionLookupTable))
                return functionLookupTable;
            else if (gameObject.TryGetComponent<ScriptMachine>(out var _))
                return gameObject.AddComponent<FunctionLookupTable>();
            else
                return null;
        }

        public bool TryGetImplementation(ScriptGraphAsset functionDefinition, out FunctionImplementation impl)
        {
            startUnits.TryGetValue(functionDefinition, out var startImpl);
            returnUnits.TryGetValue(functionDefinition, out var returnImpl);
            
            // If either dictionary is a miss, then the implementation is incomplete.
            if (startImpl == null || returnImpl == null)
            {
                impl = null;
                return false;
            }
            else if (startImpl.scriptMachine != returnImpl.scriptMachine)
            {
                impl = null;
                Debug.LogWarning($"{gameObject}: {functionDefinition.name}'s Function Start and Function Return aren't implemented on the same ScriptMachine. Ignoring implementation.");
                return false;
            }
            impl = new()
            {
                scriptMachine = startImpl.scriptMachine,
                startUnit = startImpl.startUnit,
                returnUnit = returnImpl.returnUnit,
            };
            return true;
        }
    }
}
