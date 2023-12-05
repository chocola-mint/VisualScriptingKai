using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace CHM.VisualScriptingPlus
{
    [UnitTitle("Function Start")]
    [UnitCategory("VSPlus\\Functions")]
    public class FunctionStartUnit : Unit, IFunctionUnit
    {
        [Serialize, Inspectable]
        public ScriptGraphAsset functionDefinition { get; private set; }
        [DoNotSerialize]
        private ScriptGraphAsset cachedFuncDef;
        [DoNotSerialize]
        public override IEnumerable<ISerializationDependency> deserializationDependencies => 
        functionDefinition == null ? base.deserializationDependencies 
        : new ISerializationDependency[]{ functionDefinition };
        protected override void Definition()
        {
            isControlRoot = true;
            if(cachedFuncDef != null)
                cachedFuncDef.graph.onPortDefinitionsChanged -= Define;
            // No function definition, so no ports.
            if(functionDefinition == null)
                return;
            functionDefinition.graph.onPortDefinitionsChanged += Define;
            foreach (var definition in functionDefinition.graph.validPortDefinitions)
            {
                if (definition is ControlInputDefinition)
                {
                    var controlInputDefinition = (ControlInputDefinition)definition;
                    var key = controlInputDefinition.key;

                    ControlOutput(key);
                }
                else if (definition is ValueInputDefinition)
                {
                    var valueInputDefinition = (ValueInputDefinition)definition;
                    var key = valueInputDefinition.key;
                    var type = valueInputDefinition.type;

                    ValueOutput(type, key, flow =>
                    {
                        var callStack = FunctionCallStack.GetFromFlow(flow);
                        // TODO: We still need to do flow.stack.EnterParentElement so the "This" context is set correctly.
                        var frame = callStack.Pop();
                        var caller = frame.caller;
                        GraphStack graphStack = flow.stack.Clone();
                        flow.stack.CopyFrom(frame.callerGraphStack);
                        if (flow.enableDebug)
                        {
                            var editorData = flow.stack.GetElementDebugData<IUnitDebugData>(caller);

                            editorData.lastInvokeFrame = EditorTimeBinding.frame;
                            editorData.lastInvokeTime = EditorTimeBinding.time;
                        }
                        caller.EnsureDefined();
                        var value = flow.GetValue(caller.valueInputs[key], type);
                        flow.stack.CopyFrom(graphStack);
                        callStack.Push(frame);
                        return value;
                    });
                }
            }
        }
    }
}
