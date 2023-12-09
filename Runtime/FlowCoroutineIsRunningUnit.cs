using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingKai
{
    [UnitSurtitle("Flow Coroutine")]
    [UnitTitle("Is Running")]
    [UnitCategory("VSKai\\Coroutines")]
    [RenamedFrom("Flow Coroutine Is Running")] // For fuzzy search.
    public class FlowCoroutineIsRunningUnit : Unit
    {
        [DoNotSerialize]
        public ValueInput flowCoroutine { get; private set; } // Flow
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isRunning { get; private set; } // bool
        protected override void Definition()
        {
            flowCoroutine = ValueInput<Flow>(nameof(flowCoroutine));
            isRunning = ValueOutput<bool>(nameof(isRunning), flow =>
            {
                return flow.GetValue<Flow>(flowCoroutine).isCoroutine;
            });
            Requirement(flowCoroutine, isRunning);
        }
    }
}
