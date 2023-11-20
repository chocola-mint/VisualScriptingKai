using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingPlus.Editor
{
    public struct StickyNoteTrace : IGraphElementTrace
    {
        public readonly IGraphElement GraphElement => stickyNote;
        public StickyNote stickyNote;
        public GraphReference Reference { get; set; }
        public GraphSource Source { get; set; }
        public long Score { get; set; }
        public readonly Vector2 GraphPosition => stickyNote.position.center;
        public readonly int CompareTo(IGraphElementTrace other)
        => this.DefaultCompareTo(other);
        public readonly string GetInfo()
        {
            return $"<b><size=14>{stickyNote.title}</size></b>"
            + $"\n{Source.Info}";
        }
        public readonly Texture2D GetIcon(int resolution)
        {
            return Icons.Type(typeof(StickyNote))[resolution];
        }
    }
}
