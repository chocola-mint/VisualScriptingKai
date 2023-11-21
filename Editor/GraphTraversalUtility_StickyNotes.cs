using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Linq;
using UnityEditor;
using System;


namespace CHM.VisualScriptingPlus.Editor
{
    public static partial class GraphTraversalUtility
    {
        public static IEnumerable<(StickyNote, List<Guid>)> GetStickyNotesRecursive(this GraphSource source, bool embedSubgraphsOnly = true, HashSet<Graph> visited = null)
        {
            if(source.scriptGraphAsset != null)
                return GetStickyNotesRecursive(source.scriptGraphAsset.graph, source.scriptGraphAsset, embedSubgraphsOnly, visited);
            else if(source.stateGraphAsset != null)
                return GetStickyNotesRecursive(source.stateGraphAsset.graph, source.stateGraphAsset, embedSubgraphsOnly, visited);
            else if(source.scriptMachine != null)
                return GetStickyNotesRecursive(source.scriptMachine.graph, source.scriptMachine, embedSubgraphsOnly, visited);
            else if(source.stateMachine != null)
                return GetStickyNotesRecursive(source.stateMachine.graph, source.stateMachine, embedSubgraphsOnly, visited);
            else throw new System.NullReferenceException(); // Shouldn't be reachable.
        }
        public static IEnumerable<(StickyNote, List<Guid>)> GetStickyNotesRecursive(FlowGraph graph, IGraphRoot root, bool embedSubgraphsOnly = true, HashSet<Graph> visited = null)
        {
            return GetStickyNotesRecursiveInternal(
                graph, 
                new(){ 
                    root = root,
                    path = new(),
                    visited = visited ?? new(),
                    embedSubgraphsOnly = embedSubgraphsOnly});
        }
        public static IEnumerable<(StickyNote, List<Guid>)> GetStickyNotesRecursive(StateGraph graph, IGraphRoot root, bool embedSubgraphsOnly = true, HashSet<Graph> visited = null)
        {
            return GetStickyNotesRecursiveInternal(
                graph, 
                new(){ 
                    root = root,
                    path = new(),
                    visited = visited ?? new(),
                    embedSubgraphsOnly = embedSubgraphsOnly});
        }
        private static IEnumerable<(StickyNote, List<Guid>)> GetStickyNotesRecursiveInternal(FlowGraph graph, GraphRecursionContext context)
        {
            if(context.visited.Contains(graph))
                yield break;
            context.visited.Add(graph);

            foreach(var unit in graph.units)
            {
                if(unit is SubgraphUnit subgraphUnit)
                {
                    if(context.embedSubgraphsOnly 
                    && subgraphUnit.nest.source != Unity.VisualScripting.GraphSource.Embed)
                        continue;
                    var nestedGraph = subgraphUnit.nest.graph;
                    if(nestedGraph != null)
                    {
                        context.path.Add(subgraphUnit.guid);
                        foreach(var nestedUnit in GetStickyNotesRecursiveInternal(nestedGraph, context))
                        {
                            yield return nestedUnit;
                        }
                        context.path.RemoveAt(context.path.Count - 1); // Back-travel.
                    }
                }
                else if(unit is StateUnit stateUnit)
                {
                    // ! Warning: Mutual recursion with the StateGraph version of GetStickyNotesRecursiveInternal.
                    if(context.embedSubgraphsOnly 
                    && stateUnit.nest.source != Unity.VisualScripting.GraphSource.Embed)
                        continue;
                    var nestedGraph = stateUnit.nest.graph;
                    if(nestedGraph != null)
                    {
                        context.path.Add(stateUnit.guid);
                        foreach(var nestedUnit in GetStickyNotesRecursiveInternal(nestedGraph, context))
                        {
                            yield return nestedUnit;
                        }
                        context.path.RemoveAt(context.path.Count - 1); // Back-travel.
                    }
                }
            }
            // Depth-first, so we yield the sticky notes after recursing.
            foreach(var stickyNote in graph.sticky)
            {
                yield return (stickyNote, context.path);
            }
        }
        private static IEnumerable<(StickyNote, List<Guid>)> GetStickyNotesRecursiveInternal(this StateGraph graph, GraphRecursionContext context)
        {
            // Note that the base case here is handled by FlowGraph version of GetUnitsRecursive.
            // We only care about units and not states here.
            if(context.visited.Contains(graph))
                yield break;
            context.visited.Add(graph);

            // Traverse states.
            foreach (var state in graph.states)
            {
                // The code below may look almost identical, but the FlowState case uses the FlowGraph version of GetUnitsRecursive instead.
                // There's no common interface unfortunately so we have to live with code duplication.
                if (state is FlowState flowState)
                {
                    if(context.embedSubgraphsOnly 
                    && flowState.nest.source != Unity.VisualScripting.GraphSource.Embed)
                        continue;
                    var nestedGraph = flowState.nest.graph;
                    if(nestedGraph != null)
                    {
                        context.path.Add(flowState.guid);
                        foreach (var nestedUnit in GetStickyNotesRecursiveInternal(nestedGraph, context))
                        {
                            yield return nestedUnit;
                        }
                        context.path.RemoveAt(context.path.Count - 1);
                    }
                }
                else if(state is SuperState superState)
                {
                    if(context.embedSubgraphsOnly 
                    && superState.nest.source != Unity.VisualScripting.GraphSource.Embed)
                        continue;
                    var nestedGraph = superState.nest.graph;
                    if (nestedGraph != null)
                    {
                        context.path.Add(superState.guid);
                        foreach (var nestedUnit in GetStickyNotesRecursiveInternal(nestedGraph, context))
                        {
                            yield return nestedUnit;
                        }
                        context.path.RemoveAt(context.path.Count - 1);
                    }
                }
            }
            // Traverse transitions, whose graphs are always FlowGraphs.
            foreach(var transition in graph.transitions)
            {
                if(transition is FlowStateTransition flowStateTransition)
                {
                    if(context.embedSubgraphsOnly 
                    && flowStateTransition.nest.source != Unity.VisualScripting.GraphSource.Embed)
                        continue;
                    var nestedGraph = flowStateTransition.nest.graph;
                    if (nestedGraph != null)
                    {
                        context.path.Add(flowStateTransition.guid);
                        foreach (var nestedUnit in GetStickyNotesRecursiveInternal(nestedGraph, context))
                        {
                            yield return nestedUnit;
                        }
                        context.path.RemoveAt(context.path.Count - 1);
                    }
                }
                // There doesn't seem to be other transition types.
            }
            // Depth-first, so we yield the sticky notes after recursing.
            foreach(var stickyNote in graph.sticky)
            {
                yield return (stickyNote, context.path);
            }
        }
    }
}
