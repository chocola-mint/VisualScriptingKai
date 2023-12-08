using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System;

namespace CHM.VisualScriptingKai.Editor
{
    [Widget(typeof(TestUnit))]
    public class TestUnitWidget : UnitWidget<TestUnit>
    {
        public override bool foregroundRequiresInput => true;
        public TestUnitWidget(FlowCanvas canvas, TestUnit unit) : base(canvas, unit)
        {
        }
        
        public override void DrawForeground()
        {
            base.DrawForeground();
            // We implement the button here instead of using a custom button attribute
            // on TestUnit, because crucially a node needs a GraphReference when triggered.
            // Units don't have direct access to GraphReferences, but widgets do.
            Rect buttonPosition = portsBackgroundPosition;
            Vector2 margin = Vector2.one * 8;
            buttonPosition.position += margin / 2;
            buttonPosition.size -= margin;
            buttonPosition.size -= Vector2.right * 22;
            if(GUI.Button(buttonPosition, "Run"))
            {
                Execute();
            }
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
