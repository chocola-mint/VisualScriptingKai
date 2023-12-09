using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingKai
{
    [UnitTitle("Wait For Flow Coroutine")]
    [UnitCategory("VSKai\\Coroutines")]
    public class FlowCoroutineWaitUnit : WaitUnit
    {
        [DoNotSerialize]
        public ValueInput flowCoroutine { get; private set; }
        protected override void Definition()
        {
            base.Definition();
            flowCoroutine = ValueInput<Flow>(nameof(flowCoroutine));
            Requirement(flowCoroutine, enter);
        }

        protected override IEnumerator Await(Flow flow)
        {
            Flow flowToWaitFor = flow.GetValue<Flow>(flowCoroutine);
            yield return new WaitWhile(() => flowToWaitFor.isCoroutine);
            yield return exit;
        }
    }
}
