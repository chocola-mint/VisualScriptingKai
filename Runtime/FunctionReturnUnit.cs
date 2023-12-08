using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace CHM.VisualScriptingKai
{
    [UnitTitle("Function Return")]
    [UnitCategory("VSKai\\Functions")]
    public class FunctionReturnUnit : Unit, IFunctionUnit
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
            if(cachedFuncDef != null)
                cachedFuncDef.graph.onPortDefinitionsChanged -= Define;
            // No function definition, so no ports.
            if(functionDefinition == null)
                return;
            functionDefinition.graph.onPortDefinitionsChanged += Define;
            bool hasControlInput = false;
            foreach (var definition in functionDefinition.graph.validPortDefinitions)
            {
                if (definition is ControlOutputDefinition)
                {
                    var controlOutputDefinition = (ControlOutputDefinition)definition;
                    var key = controlOutputDefinition.key;

                    ControlInput(key, flow => 
                    {
                        var callStack = FunctionCallStack.GetFromFlow(flow);
                        var frame = callStack.Pop();
                        var caller = frame.caller;
                        caller.EnsureDefined();
                        flow.stack.CopyFrom(frame.callerGraphStack);
                        return caller.controlOutputs[key];
                    });
                    hasControlInput = true;
                }
                else if (definition is ValueOutputDefinition)
                {
                    var valueOutputDefinition = (ValueOutputDefinition)definition;
                    var key = valueOutputDefinition.key;
                    var type = valueOutputDefinition.type;

                    ValueInput(type, key);
                }
            }
            // Assume pure function.
            if(!hasControlInput)
                isControlRoot = true;
        }
    }
}
