using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingKai
{
    [UnitSurtitle("Flow Coroutine")]
    [UnitTitle("Stop All Flow Coroutines")]
    [UnitCategory("VSKai\\Coroutines")]
    public class FlowCoroutineStopAllUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }
        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), flow =>
            {
                FlowCoroutineManager.GetFromGameObject(flow.stack.gameObject)
                .StopAllFlowCoroutines();
                return exit;
            });
            exit = ControlOutput(nameof(exit));
            Succession(enter, exit);
        }
    }
}
