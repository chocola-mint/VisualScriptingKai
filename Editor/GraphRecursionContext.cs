using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System;

namespace CHM.VisualScriptingPlus.Editor
{
    /// <summary>
    /// Internal data structure that keeps track of the visited set to avoid repeats,
    /// as well as the current path.
    /// </summary>
    internal class GraphRecursionContext
    {
        public IGraphRoot root;
        public List<Guid> path;
        public HashSet<Graph> visited;
        public bool embedSubgraphsOnly = false;
    }
}
