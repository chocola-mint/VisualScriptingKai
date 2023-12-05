using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingPlus
{
    [UnitTitle("Has Function")]
    [UnitCategory("VSPlus\\Functions")]
    public class HasFunctionUnit : Unit, IFunctionUnit
    {
        [Serialize, Inspectable]
        public ScriptGraphAsset functionDefinition { get; private set; }
        [DoNotSerialize]
        public ValueInput target; // GameObject
        [DoNotSerialize, PortLabelHidden]
        public ValueOutput result; // bool
        [DoNotSerialize]
        public override IEnumerable<ISerializationDependency> deserializationDependencies => 
        functionDefinition == null ? base.deserializationDependencies 
        : new ISerializationDependency[]{ functionDefinition };
        protected override void Definition()
        {
            target = ValueInput<GameObject>("target");
            
            result = ValueOutput<bool>("result", flow => 
            {
                return flow.GetValue<GameObject>(target).TryGetFunctionImplementation(functionDefinition, out var _);
            });
            
            Requirement(target, result);
        }
    }
}
