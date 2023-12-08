using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace CHM.VisualScriptingKai
{
    [UnitTitle("Check Function")]
    [UnitCategory("VSKai\\Functions")]
    public class FunctionCheckUnit : Unit, IFunctionUnit
    {
        [Serialize, Inspectable]
        public ScriptGraphAsset functionDefinition { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger;
        [DoNotSerialize]
        public ValueInput target; // GameObject
        [DoNotSerialize, PortLabel("True")]
        public ControlOutput trueTrigger;
        [DoNotSerialize, PortLabel("False")]
        public ControlOutput falseTrigger;
        [DoNotSerialize]
        public override IEnumerable<ISerializationDependency> deserializationDependencies => 
        functionDefinition == null ? base.deserializationDependencies 
        : new ISerializationDependency[]{ functionDefinition };

        protected override void Definition()
        {
            trueTrigger = ControlOutput("trueTrigger");
            falseTrigger = ControlOutput("falseTrigger");

            target = ValueInput<GameObject>("target");
            
            inputTrigger = ControlInput("inputTrigger", flow =>
            {
                if(flow.GetValue<GameObject>(target).TryGetFunctionImplementation(functionDefinition, out var _))
                    return trueTrigger;
                else
                    return falseTrigger;
            });
            Requirement(target, inputTrigger);
            Succession(inputTrigger, trueTrigger);
            Succession(inputTrigger, falseTrigger);
        }
    }
}
