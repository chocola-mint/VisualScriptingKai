using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace CHM.VisualScriptingPlus.Editor
{
    public abstract class FunctionUnitDescriptor<TFunctionUnit> : UnitDescriptor<TFunctionUnit>
    where TFunctionUnit : Unit, IFunctionUnit
    {
        protected FunctionUnitDescriptor(TFunctionUnit target) : base(target)
        {
        }
        
        protected override string DefinedSubtitle()
        {
            if(target.functionDefinition != null)
                return target.functionDefinition.name;
            return base.DefinedSubtitle();
        }
    }
    
    [Descriptor(typeof(FunctionCheckUnit))]
    public sealed class FunctionCheckUnitDescriptor : FunctionUnitDescriptor<FunctionCheckUnit>
    {
        public FunctionCheckUnitDescriptor(FunctionCheckUnit target) : base(target)
        {
        }
        
        protected override string DefinedTitle()
        {
            if(target.functionDefinition != null)
                return "Check " + target.functionDefinition.name;
            return base.DefinedTitle();
        }

        protected override string DefaultSummary()
        {
            return "Check if the target GameObject implements a function "
            + "with the same function definition as this node.";
        }
    }
    
    [Descriptor(typeof(HasFunctionUnit))]
    public sealed class HasFunctionUnitDescriptor : FunctionUnitDescriptor<HasFunctionUnit>
    {
        public HasFunctionUnitDescriptor(HasFunctionUnit target) : base(target)
        {
        }
        
        protected override string DefinedTitle()
        {
            if(target.functionDefinition != null)
                return "Has " + target.functionDefinition.name;
            return base.DefinedTitle();
        }

        protected override string DefaultSummary()
        {
            return "Check if the target GameObject implements a function "
            + "with the same function definition as this node, and output the answer.";
        }
    }
    
    [Descriptor(typeof(FunctionStartUnit))]
    public sealed class FunctionStartUnitDescriptor : FunctionUnitDescriptor<FunctionStartUnit>
    {
        public FunctionStartUnitDescriptor(FunctionStartUnit target) : base(target)
        {
        }
        
        protected override string DefinedTitle()
        {
            if(target.functionDefinition != null)
                return "Start " + target.functionDefinition.name;
            return base.DefinedTitle();
        }
        
        protected override string DefaultSummary()
        {
            return "The start of a function implementation. " 
            + "This node's ports will change according to its function definition.";
        }
    }
    
    [Descriptor(typeof(FunctionReturnUnit))]
    public sealed class FunctionReturnUnitDescriptor : FunctionUnitDescriptor<FunctionReturnUnit>
    {
        public FunctionReturnUnitDescriptor(FunctionReturnUnit target) : base(target)
        {
        }
        
        protected override string DefinedTitle()
        {
            if(target.functionDefinition != null)
                return "Return " + target.functionDefinition.name;
            return base.DefinedTitle();
        }
        
        protected override string DefaultSummary()
        {
            return "The end of a function implementation. " 
            + "This node's ports will change according to its function definition.";
        }
    }
    
    [Descriptor(typeof(FunctionCallUnit))]
    public sealed class FunctionCallUnitDescriptor : FunctionUnitDescriptor<FunctionCallUnit>
    {
        public FunctionCallUnitDescriptor(FunctionCallUnit target) : base(target)
        {
        }

        protected override string DefinedTitle()
        {
            if(target.functionDefinition != null)
                return "Call " + target.functionDefinition.name;
            return base.DefinedTitle();
        }
        
        protected override string DefaultSummary()
        {
            return "Call the specified function implemented by the target GameObject. " 
            + "This node's ports will change according to its function definition.";
        }
    }
}
