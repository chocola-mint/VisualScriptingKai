using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingKai.Editor
{
    public abstract class FunctionUnitAnalyser<TFunctionUnit> : UnitAnalyser<TFunctionUnit>
    where TFunctionUnit : Unit, IFunctionUnit
    {
        protected FunctionUnitAnalyser(GraphReference reference, TFunctionUnit target) : base(reference, target)
        {
        }

        protected override IEnumerable<Warning> Warnings()
        {
            foreach(var warning in base.Warnings())
                yield return warning;
            if(target.functionDefinition == null)
                yield return Warning.Caution("Function definition should not be null.");
        }
    }

    [Analyser(typeof(FunctionCheckUnit))]
    public sealed class FunctionCheckUnitAnalyser : FunctionUnitAnalyser<FunctionCheckUnit>
    {
        public FunctionCheckUnitAnalyser(GraphReference reference, FunctionCheckUnit target) : base(reference, target)
        {
        }
    }
    
    [Analyser(typeof(HasFunctionUnit))]
    public sealed class HasFunctionUnitAnalyser : FunctionUnitAnalyser<HasFunctionUnit>
    {
        public HasFunctionUnitAnalyser(GraphReference reference, HasFunctionUnit target) : base(reference, target)
        {
        }
    }
    
    [Analyser(typeof(FunctionStartUnit))]
    public sealed class FunctionStartUnitAnalyser : FunctionUnitAnalyser<FunctionStartUnit>
    {
        public FunctionStartUnitAnalyser(GraphReference reference, FunctionStartUnit target) : base(reference, target)
        {
        }
    }
    
    [Analyser(typeof(FunctionReturnUnit))]
    public sealed class FunctionReturnUnitAnalyser : FunctionUnitAnalyser<FunctionReturnUnit>
    {
        public FunctionReturnUnitAnalyser(GraphReference reference, FunctionReturnUnit target) : base(reference, target)
        {
        }
    }
    
    [Analyser(typeof(FunctionCallUnit))]
    public sealed class FunctionCallUnitAnalyser : FunctionUnitAnalyser<FunctionCallUnit>
    {
        public FunctionCallUnitAnalyser(GraphReference reference, FunctionCallUnit target) : base(reference, target)
        {
        }
    }
}
