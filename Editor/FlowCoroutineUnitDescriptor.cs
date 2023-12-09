using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingKai.Editor
{
    public class FlowCoroutineUnitDescriptor<T> : UnitDescriptor<T>
    where T : Unit
    {
        public FlowCoroutineUnitDescriptor(T target) : base(target)
        {
        }

        protected override EditorTexture DefinedIcon()
        {
            return typeof(Coroutine).Icon();
        }
    }

    [Descriptor(typeof(FlowCoroutineStartUnit))]
    public class FlowCoroutineStartUnitDescriptor : FlowCoroutineUnitDescriptor<FlowCoroutineStartUnit>
    {
        public FlowCoroutineStartUnitDescriptor(FlowCoroutineStartUnit target) : base(target)
        {
        }

        protected override string DefinedSummary()
        {
            return "Starts a new Flow Coroutine, independent from the flow that lead to this node."
            + "\nYou may want to use the Sequence node to do other stuff after starting the coroutine.";
        }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);
            if (port == target.flowCoroutine)
            {
                description.icon = typeof(Coroutine).Icon();
                description.summary = "The new coroutine as a Flow object."
                + "This can be used by other Flow Coroutine nodes like Stop Flow Coroutine and Wait For Flow Coroutine.";
            }
            else if (port == target.coroutineStart)
            {
                description.summary = "The flow from here on out is a coroutine, independent from the previous flow.";
            }
        }
    }
    
    [Descriptor(typeof(FlowCoroutineStopUnit))]
    public class FlowCoroutineStopUnitDescriptor : FlowCoroutineUnitDescriptor<FlowCoroutineStopUnit>
    {
        public FlowCoroutineStopUnitDescriptor(FlowCoroutineStopUnit target) : base(target)
        {
        }

        protected override string DefinedSummary()
        {
            return "Stops the given Flow Coroutine.";
        }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);
            if (port == target.flowCoroutine)
            {
                description.icon = typeof(Coroutine).Icon();
                description.summary = "The coroutine to stop.";
            }
        }
    }
    
    [Descriptor(typeof(FlowCoroutineStopAllUnit))]
    public class FlowCoroutineStopAllUnitDescriptor : FlowCoroutineUnitDescriptor<FlowCoroutineStopAllUnit>
    {
        public FlowCoroutineStopAllUnitDescriptor(FlowCoroutineStopAllUnit target) : base(target)
        {
        }

        protected override string DefinedSummary()
        {
            return "Stops all running Flow Coroutines.";
        }
    }
    
    [Descriptor(typeof(FlowCoroutineIsRunningUnit))]
    public class FlowCoroutineIsRunningUnitDescriptor : FlowCoroutineUnitDescriptor<FlowCoroutineIsRunningUnit>
    {
        public FlowCoroutineIsRunningUnitDescriptor(FlowCoroutineIsRunningUnit target) : base(target)
        {
        }

        protected override string DefinedSummary()
        {
            return "Check if the given Flow Coroutine is still running.";
        }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);
            if (port == target.flowCoroutine)
            {
                description.icon = typeof(Coroutine).Icon();
                description.summary = "The coroutine to check.";
            }
        }
    }

    [Descriptor(typeof(FlowCoroutineWaitUnit))]
    public class FlowCoroutineWaitUnitDescriptor : UnitDescriptor<FlowCoroutineWaitUnit>
    {
        // Note: Doesn't inherit from FlowCoroutine as we're using the default WaitUnit icon.
        public FlowCoroutineWaitUnitDescriptor(FlowCoroutineWaitUnit target) : base(target)
        {
        }

        protected override string DefinedSummary()
        {
            return "Pauses execution until the given Flow Coroutine is finished or stopped.";
        }

        protected override void DefinedPort(IUnitPort port, UnitPortDescription description)
        {
            base.DefinedPort(port, description);
            if (port == target.flowCoroutine)
            {
                description.icon = typeof(Coroutine).Icon();
                description.summary = "The coroutine to wait for.";
            }
        }
    }
}
