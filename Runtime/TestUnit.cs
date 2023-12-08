using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace CHM.VisualScriptingPlus
{
    [UnitTitle("Test")]
    [UnitCategory("Events\\VSPlus")]//Set the path to find the node in the fuzzy finder as Events > My Events.
    public class TestUnit : EventUnit<int>
    {
        protected override bool register => false;
    }
}
