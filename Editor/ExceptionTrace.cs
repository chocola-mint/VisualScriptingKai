using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingKai.Editor
{
    public struct ExceptionTrace : IGraphElementTrace
    {
        public readonly IGraphElement GraphElement => unit;
        public IUnit unit;
        public GraphReference Reference { get; set; }
        public GraphSource Source { get; set; }
        public long Score { get; set; }
        public readonly Vector2 GraphPosition => unit.position;
        public readonly int CompareTo(IGraphElementTrace other)
        {
            // Deeper nodes go first. Then sort by title.
            int compare = -Reference.parentElementGuids.Count().CompareTo(other.Reference.parentElementGuids.Count());
            if(compare != 0)
                return compare;
            return GraphElement.Description().title.CompareTo(other.Description().title);
        }
        public readonly string GetInfo()
        {
            var exception = unit.GetException(Reference);
            return $"<b><size=14>{unit.Name()}</size></b>"
            + $"\n{Source.ShortInfo}"
            + $"\n<b>{exception.GetType()}:</b>  {exception.Message}";
        }
        public readonly Texture2D GetIcon(int resolution)
        {
            // Cursed operator overload. Gets texture with resolution.
            return unit.Icon()[resolution];
        }
    }
}
