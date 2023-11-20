using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingPlus.Editor
{
    [Descriptor(typeof(TestUnit))]
    public class TestUnitDescriptor<TEvent> : UnitDescriptor<TEvent>
        where TEvent : class, IEventUnit
    {
        public TestUnitDescriptor(TEvent @event) : base(@event) { }

        // protected override string DefinedSubtitle()
        // {
        //     return "Event";
        // }
        protected override string DefinedSummary()
        {
            return "Double click to execute this node."
            + "\nWorks in both Edit Mode and Play Mode."
            + "\nNote 1: Coroutine tests are only usable in Play Mode."
            + "\nNote 2: Exceptions in edit mode won't go away on their own. "
            + "You can use \"Tools > Clear exceptions in active window\" or "
            + "right click on this node and select \"Clear Exceptions\" to clear them.";
        }
        protected override IEnumerable<EditorTexture> DefinedIcons()
        {
            if (unit.coroutine)
            {
                yield return BoltFlow.Icons.coroutine;
            }
        }
    }
}
