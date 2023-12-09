using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingKai
{
    /// <summary>
    /// This is used to simulate how EventUnit manages coroutines.
    /// Like regular MonoBehaviour coroutines, this will stop active coroutines when
    /// it is disabled.
    /// </summary>
    public sealed class FlowCoroutineManager : MonoBehaviour
    {
        private HashSet<Flow> flows = null;
        public static FlowCoroutineManager GetFromGameObject(GameObject gameObject)
        {
            if(!gameObject.TryGetComponent<FlowCoroutineManager>(out var coroutineManager))
                coroutineManager = gameObject.AddComponent<FlowCoroutineManager>();
            return coroutineManager;
        }

        public Flow StartFlowCoroutine(ControlOutput port, GraphStack stack)
        {
            Flow flow = Flow.New(stack.AsReference());
            flow.StartCoroutine(port, flows);
            return flow;
        }

        public void StopAllFlowCoroutines()
        {
            // We need to copy flows to a separate array because
            // stopping a coroutine will remove its flow automatically (on dispose).
            // There's no need to check if these flows are still running, because
            // if they aren't, they would've been gone from the HashSet anyway.
            foreach(Flow flow in flows.ToArray())
                flow.StopCoroutine(true);
        }

        void Awake() 
        {
            flows ??= HashSetPool<Flow>.New();
        }
        
        void OnEnable() 
        {
            flows ??= HashSetPool<Flow>.New();
        }
        
        void OnDisable() 
        {
            if(flows != null)
            {
                // See: EventUnit.StopListening
                foreach(Flow flow in flows)
                    flow.StopCoroutine(false);
                flows.Free();
                flows = null;
            }
        }

        [ContextMenu(nameof(PrintFlowCount))]
        public void PrintFlowCount()
        {
            if(flows != null)
                Debug.Log(flows.Count);
            else Debug.Log(0);
        }
    }
}
