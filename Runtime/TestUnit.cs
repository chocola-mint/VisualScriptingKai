using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace CHM.VisualScriptingKai
{
    [UnitTitle("Test")]
    [UnitCategory("Events\\VSKai")]//Set the path to find the node in the fuzzy finder as Events > My Events.
    public class TestUnit : EventUnit<int>
    {
        protected override bool register => false;
    }
}
