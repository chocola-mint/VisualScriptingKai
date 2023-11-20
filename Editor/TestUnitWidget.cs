using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System;

namespace CHM.VisualScriptingPlus.Editor
{
    [Widget(typeof(TestUnit))]
    public class TestUnitWidget : UnitWidget<TestUnit>
    {
        public TestUnitWidget(FlowCanvas canvas, TestUnit unit) : base(canvas, unit)
        {
        }
        protected override void OnDoubleClick()
        {
            base.OnDoubleClick();
            Execute();
        }

        private void Execute()
        {
            if (unit.coroutine)
            {
                Debug.LogWarning("Can't run coroutine tests in Edit Mode!");
                return;
            }
            unit.Trigger(reference, 0);
        }
        private void ClearExceptions()
        {
            GraphUtility.ClearExceptionsInActiveWindow();
        }
        protected override IEnumerable<DropdownOption> contextOptions
        {
            get
            {
                yield return new DropdownOption(
                    (Action) Execute, "Execute");
                yield return new DropdownOption(
                    (Action) ClearExceptions, "Clear Exceptions");
                foreach (var baseOption in base.contextOptions)
                {
                    yield return baseOption;
                }
            }
        }
        protected override NodeColorMix baseColor => new(){
            green = 1f,
            orange = 0.5f,
            teal = 1.0f,
        };
    }
}
