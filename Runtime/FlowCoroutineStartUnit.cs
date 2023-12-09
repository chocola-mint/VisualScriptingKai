using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingKai
{
    [UnitTitle("Start Flow Coroutine")]
    [UnitCategory("VSKai\\Coroutines")]
    public class FlowCoroutineStartUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput coroutineStart { get; private set; }
        [DoNotSerialize]
        [PortLabel("Flow Coroutine")]
        public ValueOutput flowCoroutine { get; private set; } // Flow
        [DoNotSerialize]
        private Flow coroFlow;
        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), flow =>
            {
                coroFlow = FlowCoroutineManager.GetFromGameObject(flow.stack.gameObject)
                .StartFlowCoroutine(coroutineStart, flow.stack);
                return null;
            });
            coroutineStart = ControlOutput(nameof(coroutineStart));
            flowCoroutine = ValueOutput<Flow>(nameof(flowCoroutine), flow => coroFlow);
            Assignment(enter, flowCoroutine);
            // This is a half-lie: A new flow with the same GraphStack is used for coroutineStart.
            Succession(enter, coroutineStart);
        }
    }
}
