using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingKai
{
    [UnitTitle("Stop Flow Coroutine")]
    [UnitCategory("VSKai\\Coroutines")]
    public class FlowCoroutineStopUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }
        [DoNotSerialize]
        public ValueInput flowCoroutine { get; private set; } // Flow
        protected override void Definition()
        {
            flowCoroutine = ValueInput<Flow>(nameof(flowCoroutine));
            enter = ControlInput(nameof(enter), flow =>
            {
                var flowToStop = flow.GetValue<Flow>(flowCoroutine);
                // Prevents trying to stop finished/stopped coroutines.
                if(flowToStop.isCoroutine)
                    flowToStop.StopCoroutine(false);
                return exit;
            });
            exit = ControlOutput(nameof(exit));
            Succession(enter, exit);
            Requirement(flowCoroutine, enter);
        }
    }
}
