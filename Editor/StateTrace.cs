using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;

namespace CHM.VisualScriptingPlus.Editor
{
    public struct StateTrace : IGraphElementTrace
    {
        public readonly IGraphElement GraphElement => state;
        public IState state;
        public GraphReference Reference { get; set; }
        public GraphSource Source { get; set; }
        public long Score { get; set; }
        public readonly Vector2 GraphPosition => state.position;
        public readonly int CompareTo(IGraphElementTrace other)
        => this.DefaultCompareTo(other);
        public readonly string GetInfo()
        {
            return $"<b><size=14>{state.Name()}</size></b>"
            + $"\n{Source.Info}";
        }
        public readonly Texture2D GetIcon(int resolution)
        {
            // Cursed operator overload. Gets texture with resolution.
            return state.Icon()[resolution];
        }
    }
}
