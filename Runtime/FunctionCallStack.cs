using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingPlus
{
    public sealed class FunctionCallStack
    {
        public struct Frame
        {
            public FunctionCallUnit caller;
            public GraphStack callerGraphStack;
        }
        public const string FlowVariableKey = "##VSPlusFunctionCallStack";
        public readonly Stack<Frame> stack = new();
        public void PropagateException(Flow flow, System.Exception ex)
        {
            foreach(Frame frame in stack)
            {
                frame.caller.SetException(flow.stack, ex);
            }
        }
        public static FunctionCallStack GetFromFlow(Flow flow)
        {
            FunctionCallStack callStack;
            if(!flow.variables.IsDefined(FlowVariableKey))
            {
                callStack = new();
                flow.variables.Set(FlowVariableKey, callStack);
            }
            else
                callStack = flow.variables.Get<FunctionCallStack>(FlowVariableKey);
            return callStack;
        }
        public void Push(Frame frame) => stack.Push(frame);
        public Frame Pop() => stack.Pop();
        public Frame Peek() => stack.Peek();
    }
}
