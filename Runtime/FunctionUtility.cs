using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingKai
{
    public static class FunctionUtility
    {
        public static bool TryGetFunctionImplementation(this GameObject gameObject, ScriptGraphAsset functionDefinition, out FunctionImplementation impl)
        {
            var lookupTable = FunctionLookupTable.GetFromGameObject(gameObject);
            if(lookupTable == null)
            {
                impl = null;
                return false;
            }
            return lookupTable.TryGetImplementation(functionDefinition, out impl);
        }
    }
}
