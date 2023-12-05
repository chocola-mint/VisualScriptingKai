using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingPlus
{
    public interface IFunctionUnit
    {
        public ScriptGraphAsset functionDefinition { get; }
    }
}
