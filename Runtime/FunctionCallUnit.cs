using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace CHM.VisualScriptingKai
{
    [UnitTitle("Call Function")]
    [UnitCategory("VSKai\\Functions")]
    public class FunctionCallUnit : Unit, IFunctionUnit
    {
        [Serialize, Inspectable]
        public ScriptGraphAsset functionDefinition { get; private set; }
        
        [DoNotSerialize]
        private ScriptGraphAsset cachedFuncDef;

        [DoNotSerialize]
        public ValueInput target; // GameObject

        // Function units should be loaded after their function definitions are loaded,
        // or else there will be annoying errors.
        [DoNotSerialize]
        public override IEnumerable<ISerializationDependency> deserializationDependencies => 
        functionDefinition == null ? base.deserializationDependencies 
        : new ISerializationDependency[]{ functionDefinition };

        protected override void Definition()
        {
            target = ValueInput<GameObject>("target");
            if(cachedFuncDef != null)
                cachedFuncDef.graph.onPortDefinitionsChanged -= Define;
            // No function definition, so no ports.
            if(functionDefinition == null)
                return;
            functionDefinition.graph.onPortDefinitionsChanged += Define;
            cachedFuncDef = functionDefinition;
            
            foreach (var definition in functionDefinition.graph.validPortDefinitions)
            {
                if (definition is ControlInputDefinition)
                {
                    var controlInputDefinition = (ControlInputDefinition)definition;
                    var key = controlInputDefinition.key;

                    var input = ControlInput(key, (flow) =>
                    {
                        var callTarget = flow.GetValue<GameObject>(target);
                        if(callTarget.TryGetFunctionImplementation(functionDefinition, out var impl))
                        {
                            GraphStack graphStack = flow.stack.Clone();
                            flow.stack.CopyFrom(impl.scriptMachine.GetReference());
                            FunctionCallStack.GetFromFlow(flow).Push(new()
                            {
                                caller = this,
                                callerGraphStack = graphStack,
                            });
                            return impl.startUnit.controlOutputs[key];
                        }
                        else
                            throw new System.Exception($"{callTarget.name} does not implement {functionDefinition.name}. Use Check Function or Has Function to check before calling!");
                    });
                    Requirement(target, input);
                }
                else if (definition is ValueInputDefinition)
                {
                    var valueInputDefinition = (ValueInputDefinition)definition;
                    var key = valueInputDefinition.key;
                    var type = valueInputDefinition.type;
                    var hasDefaultValue = valueInputDefinition.hasDefaultValue;
                    var defaultValue = valueInputDefinition.defaultValue;

                    var port = ValueInput(type, key);

                    if (hasDefaultValue)
                    {
                        port.SetDefaultValue(defaultValue);
                    }
                }
                else if (definition is ControlOutputDefinition)
                {
                    var controlOutputDefinition = (ControlOutputDefinition)definition;
                    var key = controlOutputDefinition.key;

                    ControlOutput(key);
                }
                else if (definition is ValueOutputDefinition)
                {
                    var valueOutputDefinition = (ValueOutputDefinition)definition;
                    var key = valueOutputDefinition.key;
                    var type = valueOutputDefinition.type;

                    ValueOutput(type, key, (flow) =>
                    {
                        var callTarget = flow.GetValue<GameObject>(target);
                        if(callTarget.TryGetFunctionImplementation(functionDefinition, out var impl))
                        {
                            var callStack = FunctionCallStack.GetFromFlow(flow);
                            GraphStack graphStack = flow.stack.Clone();
                            flow.stack.CopyFrom(impl.scriptMachine.GetReference());
                            callStack.Push(new()
                            {
                                caller = this,
                                callerGraphStack = graphStack,
                            });
                            var value = flow.GetValue(impl.returnUnit.valueInputs[key]);
                            flow.stack.CopyFrom(callStack.Pop().callerGraphStack);
                            return value;
                        }

                        throw new System.InvalidOperationException("Missing output node when to get value.");
                    });
                }
            }
            // Assume that all controls are connected.
            // TODO: Make it possible to infer relations from the function definition's graph.
            foreach(var controlInput in controlInputs)
                foreach(var controlOutput in controlOutputs)
                    Succession(controlInput, controlOutput);
        }
    }
}
