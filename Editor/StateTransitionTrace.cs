using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;

namespace CHM.VisualScriptingKai.Editor
{
    public struct StateTransitionTrace : IGraphElementTrace
    {
        public readonly IGraphElement GraphElement => stateTransition;
        public IStateTransition stateTransition;
        public GraphReference Reference { get; set; }
        public GraphSource Source { get; set; }
        public long Score { get; set; }
        public readonly Vector2 GraphPosition => (stateTransition.source.position + stateTransition.destination.position) / 2;
        public readonly int CompareTo(IGraphElementTrace other)
        => this.DefaultCompareTo(other);
        public readonly string GetInfo()
        {
            return $"<b><size=14>{stateTransition.Name()}</size></b>"
            + $"\n{Source.Info}";
        }
        public readonly Texture2D GetIcon(int resolution)
        {
            // Cursed operator overload. Gets texture with resolution.
            return stateTransition.Icon()[resolution];
        }
    }
}
