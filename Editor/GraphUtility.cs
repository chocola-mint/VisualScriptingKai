using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Linq;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEditor.Search;

namespace CHM.VisualScriptingPlus.Editor
{
    public static class GraphUtility
    {
        // TODO: Maybe turn this into an Object Reference Finder feature?
        public static IEnumerable<IGraphElementTrace> FindReferencesToObject(IEnumerable<GraphSource> sources, Object target, bool embedSubgraphsOnly = true)
        {
            HashSet<Graph> visited = new();
            foreach(var source in sources)
                foreach(var exceptionTrace in FindReferencesToObject(source, target, embedSubgraphsOnly, visited))
                    yield return exceptionTrace;
        }
        public static IEnumerable<IGraphElementTrace> FindReferencesToObject(GraphSource source, Object target, bool embedSubgraphsOnly = true, HashSet<Graph> visited = null)
        {
            // Ignore missing references / null.
            if(!target)
                yield break;
            foreach(var (unit, path) in source.GetUnitsRecursive(embedSubgraphsOnly, visited))
            {
                bool hasSameReference = false;
                foreach(var value in unit.defaultValues.Values)
                {
                    if(value is Object valueAsObject 
                    && valueAsObject
                    && valueAsObject == target)
                    {
                        hasSameReference = true;
                        break;
                    }
                }
                hasSameReference |= unit is Literal literal
                && literal.value is Object literalRef
                && literalRef
                && literalRef == target;
                if(hasSameReference)
                {
                    var reference = GraphReference.New((Object) source, path, false);
                    yield return new NodeTrace(){
                        unit = unit,
                        Reference = reference,
                        Source = source,
                        Score = 0,
                    };
                }
            }
        }
        public static IEnumerable<IGraphElementTrace> FindAllExceptions(IEnumerable<GraphSource> sources, bool embedSubgraphsOnly = false)
        {
            HashSet<Graph> visited = new();
            foreach(var source in sources)
                foreach(var exceptionTrace in FindAllExceptions(source, embedSubgraphsOnly, visited))
                    yield return exceptionTrace;
        }
        public static IEnumerable<IGraphElementTrace> FindAllExceptions(GraphSource source, bool embedSubgraphsOnly = false, HashSet<Graph> visited = null)
        {
            foreach(var (unit, path) in source.GetUnitsRecursive(embedSubgraphsOnly, visited))
            {
                var reference = GraphReference.New((Object) source, path, false);
                var exception = unit.GetException(reference);
                if(exception != null)
                {
                    yield return new ExceptionTrace(){
                        unit = unit,
                        Reference = reference,
                        Source = source,
                    };
                }
            }
        }
        public static IEnumerable<IGraphElementTrace> FindWarnings(IEnumerable<GraphSource> sources, WarningLevel warningLevel, bool embedSubgraphsOnly = false)
        {
            HashSet<Graph> visited = new();
            foreach(var source in sources)
                foreach(var exceptionTrace in FindWarnings(source, warningLevel, embedSubgraphsOnly, visited))
                    yield return exceptionTrace;
        }
        public static IEnumerable<IGraphElementTrace> FindWarnings(GraphSource source, WarningLevel warningLevel, bool embedSubgraphsOnly = true, HashSet<Graph> visited = null)
        {
            foreach(var (unit, path) in source.GetUnitsRecursive(embedSubgraphsOnly, visited))
            {
                var reference = GraphReference.New((Object) source, path, false);
                if(unit.Analysis<UnitAnalysis>(reference) is var unitAnalysis
                && unitAnalysis.warnings.Count > 0)
                {
                    foreach(var warning in unitAnalysis.warnings)
                    {
                        if((int)warning.level >= (int)warningLevel)
                        {
                            yield return new WarningTrace(){
                                unit = unit,
                                warning = warning,
                                Reference = reference,
                                Source = source,
                            };
                        }
                    }
                }
            }
        }
        public static IEnumerable<IGraphElementTrace> FindNodes(IEnumerable<GraphSource> sources, string pattern, bool embedSubgraphsOnly = true)
        {
            HashSet<Graph> visited = new();
            foreach(var source in sources)
                foreach(var nodeTrace in FindNodes(source, pattern, embedSubgraphsOnly, visited))
                    yield return nodeTrace;
        }
        public static IEnumerable<IGraphElementTrace> FindNodes(GraphSource source, string pattern, bool embedSubgraphsOnly = true, HashSet<Graph> visited = null)
        {
            if(pattern.Length == 0) 
                yield break;
            foreach (var (unit, path) in source.GetUnitsRecursive(embedSubgraphsOnly, visited))
            {
                // Why isn't this out??? Even though it is always overwritten internally???
                // Note: Bigger outScore -> better
                long outScore = 0;
                if(FuzzySearch.FuzzyMatch(pattern, unit.Name(), ref outScore))
                {
                    yield return new NodeTrace()
                    {
                        unit = unit,
                        Reference = GraphReference.New((Object) source, path, false),
                        Source = source,
                        Score = outScore,
                    };
                }
            }
        }
        public static IEnumerable<IGraphElementTrace> FindStickyNotes(IEnumerable<GraphSource> sources, string pattern, bool embedSubgraphsOnly = true)
        {
            // TODO: Expose this visited set as parameter
            HashSet<Graph> visited = new();
            foreach(var source in sources)
                foreach(var nodeTrace in FindStickyNotes(source, pattern, embedSubgraphsOnly, visited))
                    yield return nodeTrace;
        }
        public static IEnumerable<IGraphElementTrace> FindStickyNotes(GraphSource source, string pattern, bool embedSubgraphsOnly = true, HashSet<Graph> visited = null)
        {
            if(pattern.Length == 0) 
                yield break;
            foreach (var (stickyNote, path) in source.GetStickyNotesRecursive(embedSubgraphsOnly, visited))
            {
                long outScore = 0;
                if(FuzzySearch.FuzzyMatch(pattern, stickyNote.title, ref outScore))
                {
                    yield return new StickyNoteTrace()
                    {
                        stickyNote = stickyNote,
                        Reference = GraphReference.New((Object) source, path, false),
                        Source = source,
                        Score = outScore,
                    };
                }
            }
        }
        public static IEnumerable<IGraphElementTrace> FindStates(IEnumerable<GraphSource> sources, string pattern, bool embedSubgraphsOnly = true)
        {
            // TODO: Expose this visited set as parameter
            HashSet<Graph> visited = new();
            foreach(var source in sources)
                foreach(var nodeTrace in FindStates(source, pattern, embedSubgraphsOnly, visited))
                    yield return nodeTrace;
        }
        public static IEnumerable<IGraphElementTrace> FindStates(GraphSource source, string pattern, bool embedSubgraphsOnly = true, HashSet<Graph> visited = null)
        {
            if(pattern.Length == 0) 
                yield break;
            foreach (var (state, path) in source.GetStatesRecursive(embedSubgraphsOnly, visited))
            {
                long outScore = 0;
                if(FuzzySearch.FuzzyMatch(pattern, state.Name(), ref outScore))
                {
                    yield return new StateTrace()
                    {
                        state = state,
                        Reference = GraphReference.New((Object) source, path, false),
                        Source = source,
                        Score = outScore,
                    };
                }
            }
        }
        public static IEnumerable<IGraphElementTrace> FindStateTransitions(IEnumerable<GraphSource> sources, string pattern, bool embedSubgraphsOnly = true)
        {
            // TODO: Expose this visited set as parameter
            HashSet<Graph> visited = new();
            foreach(var source in sources)
                foreach(var nodeTrace in FindStateTransitions(source, pattern, embedSubgraphsOnly, visited))
                    yield return nodeTrace;
        }
        public static IEnumerable<IGraphElementTrace> FindStateTransitions(GraphSource source, string pattern, bool embedSubgraphsOnly = true, HashSet<Graph> visited = null)
        {
            if(pattern.Length == 0) 
                yield break;
            foreach (var (stateTransition, path) in source.GetStateTransitionsRecursive(embedSubgraphsOnly, visited))
            {
                long outScore = 0;
                if(FuzzySearch.FuzzyMatch(pattern, stateTransition.Name(), ref outScore))
                {
                    yield return new StateTransitionTrace()
                    {
                        stateTransition = stateTransition,
                        Reference = GraphReference.New((Object) source, path, false),
                        Source = source,
                        Score = outScore,
                    };
                }
            }
        }
        [MenuItem("Tools/Visual Scripting/Clear exceptions in active window")]
        public static void ClearExceptionsInActiveWindow()
        {
            if(GraphWindow.activeContext == null)
                return;
            if(GraphWindow.activeContext.graph is FlowGraph flowGraph)
            {
                foreach(var (unit, path) in GraphTraversalUtility.GetUnitsRecursive(flowGraph, GraphWindow.activeReference.root, false))
                    unit.SetException(GraphWindow.activeReference, null);
            }
            else if(GraphWindow.activeContext.graph is StateGraph stateGraph)
            {
                foreach(var (unit, path) in GraphTraversalUtility.GetUnitsRecursive(stateGraph, GraphWindow.activeReference.root, false))
                    unit.SetException(GraphWindow.activeReference, null);
            }
            else Debug.LogWarning($"Unknown graph type: {GraphWindow.activeContext.graph.GetType()}");
        }
        public static GraphSource GetEditedGraph()
        {
            var rootObject = GraphWindow.activeContext?.reference?.rootObject;
            if(rootObject != null)
            {
                if(rootObject is ScriptGraphAsset scriptGraphAsset)
                {
                    return new GraphSource(scriptGraphAsset);
                }
                else if(rootObject is StateGraphAsset stateGraphAsset)
                {
                    return new GraphSource(stateGraphAsset);
                }
                else if(rootObject is ScriptMachine scriptMachine)
                {
                    return new GraphSource(scriptMachine);
                }
                else if(rootObject is StateMachine stateMachine)
                {
                    return new GraphSource(stateMachine);
                }
            }
            return null;
        }
        /// <summary>
        /// Find all ScriptGraphAsset/StateGraphAssets in the specified folders.
        /// </summary>
        /// <param name="searchInFolders">Folders to search in.</param>
        /// <returns></returns>
        public static IEnumerable<GraphSource> FindAllGraphAssets(string[] searchInFolders = null)
        {
            return AssetUtility.FindAssetsByType<ScriptGraphAsset>(searchInFolders)
            .Select(x => new GraphSource(x))
            .Concat(
                AssetUtility.FindAssetsByType<StateGraphAsset>(searchInFolders)
                .Select(x => new GraphSource(x)));
        }
        /// <summary>
        /// Runtime graph sources simply refer to loaded ScriptMachine/StateMachine instances.
        /// You may want to use runtime graph sources for runtime debugging purposes.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GraphSource> FindAllRuntimeGraphSources()
        {
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include;
            FindObjectsSortMode sortMode = FindObjectsSortMode.None;
            return Object.FindObjectsByType<ScriptMachine>(findObjectsInactive, sortMode)
            .Select(x => new GraphSource(x))
            .Concat(
                Object.FindObjectsByType<StateMachine>(findObjectsInactive, sortMode)
                .Select(x => new GraphSource(x)));
        }
        /// <summary>
        /// Find all graph sources, which can then be used in queries.
        /// </summary>
        /// <remarks>
        /// Note: ScriptGraphs/StateGraphs embedded in ScriptMachines/StateMachines
        /// can only be found in the currently-loaded scene/prefab.
        /// <br/><br/>
        /// The alternative would be to additively-load all scenes and prefabs in searchInFolders,
        /// and would be very taxing on the editor application, so this is just not supported.
        /// <br/><br/>
        /// In general, to make the best use of Visual Scripting Plus,
        /// avoid using embed graphs in ScriptMachines/StateMachines as much as possible.
        /// </remarks>
        /// <param name="searchInFolders">Folders to search in for ScriptGraphAssets/StateGraphAssets.</param>
        /// <returns></returns>
        public static IEnumerable<GraphSource> FindAllGraphSources(string[] searchInFolders = null)
        {
            return FindAllGraphAssets(searchInFolders)
            .Concat(FindAllRuntimeGraphSources());
        }
        
    }
}
