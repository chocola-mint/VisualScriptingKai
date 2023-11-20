using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace CHM.VisualScriptingPlus.Editor
{
    public struct WarningTrace : IGraphElementTrace
    {
        public readonly IGraphElement GraphElement => unit;
        public IUnit unit;
        public Warning warning;
        public GraphReference Reference { get; set; }
        public GraphSource Source { get; set; }
        public long Score { get; set; }
        public readonly Vector2 GraphPosition => unit.position;
        public readonly int CompareTo(IGraphElementTrace other)
        {
            // int compare = GraphElement.Description().title.CompareTo(other.GraphElement.Description().title);
            // if(compare != 0)
            //     return compare;
            return this.DefaultCompareTo(other);
        }
        public readonly string GetInfo()
        {
            return $"<b><size=14>{unit.Option().haystack}</size></b>"
            + $"\n{Source.ShortInfo}"
            + $"\n<b>{warning.MessageType}:</b> {warning.message}";
        }
        public readonly Texture2D GetIcon(int resolution)
        {
            // Cursed operator overload. Gets texture with resolution.
            return unit.Option().icon[resolution];
        }
    }
}
