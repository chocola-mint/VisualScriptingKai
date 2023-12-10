using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using static CHM.VisualScriptingKai.Editor.GraphUtility;
using Unity.VisualScripting;
using System.Linq;

namespace CHM.VisualScriptingKai.Editor
{
    // TODO: Search for warnings ((IUnit.Analysis() as UnitAnalysis).warnings)
    // TODO: [Future work] Operations on query items (maybe replace-all is possible?)
    public class GraphLensWindow : GraphQueryWindowBase
    {
        private DropdownField queryType;
        private TextField queryFolders;
        private TextField queryString;
        private QueryResultsListView queryResultsListView;
        private readonly List<GraphSource> graphAssetSourceCache = new();
        private static class QueryType
        {
            public static readonly string Nodes = "Nodes";
            public static readonly string StickyNotes = "Sticky Notes";
            public static readonly string States = "States";
            public static readonly string StateTransitions = "State Transitions";
        }
        private readonly List<string> queryOptions = new(){
            QueryType.Nodes,
            QueryType.StickyNotes,
            QueryType.States,
            QueryType.StateTransitions,
        };
        private static class EditorPrefKeys
        {
            public static readonly string QueryType = Application.dataPath + "/GraphLens/query-type";
            public static readonly string QueryFolders = Application.dataPath + "/GraphLens/query-folders";
            public static readonly string QueryString = Application.dataPath + "/GraphLens/query-string";
        }
        [MenuItem("Window/Visual Scripting/Graph Lens")]
        public static void OpenWindow()
        {
            GraphLensWindow wnd = GetWindow<GraphLensWindow>(
                typeof(GraphQueryWindowBase));
            // TODO: Add Graph Lens icon to GUIContent.
            wnd.titleContent = new GUIContent("Graph Lens");
        }
        protected override void OnCreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = PackageUtility.LoadPackageAsset<VisualTreeAsset>("Editor/Resources/GraphLensWindow.uxml");
            VisualElement visualTreeRoot = visualTree.Instantiate();
            root.Add(visualTreeRoot);

            // Set up references.
            FetchElement(visualTreeRoot, "query-type", out queryType);
            FetchElement(visualTreeRoot, "query-folders", out queryFolders);
            FetchElement(visualTreeRoot, "query-string", out queryString);
            FetchElement(visualTreeRoot, "query-results", out queryResultsListView);

            queryFolders.value = EditorPrefs.GetString(EditorPrefKeys.QueryFolders, "Assets");
            queryFolders.RegisterValueChangedCallback(changeEvent => {
                EditorPrefs.SetString(EditorPrefKeys.QueryFolders, queryFolders.value);
                // Folders changed, so the cache needs to be updated.
                UpdateGraphAssetSourceCache();
                ExecuteQuery();
            });

            queryType.choices = queryOptions;
            queryType.index = EditorPrefs.GetInt(EditorPrefKeys.QueryType, 0);
            if(queryType.index >= queryOptions.Count)
                queryType.index = 0;
            queryType.RegisterValueChangedCallback(changeEvent => {
                EditorPrefs.SetInt(EditorPrefKeys.QueryType, queryType.index);
                ExecuteQuery();
            });

            queryString.value = EditorPrefs.GetString(EditorPrefKeys.QueryString, "");
            queryString.RegisterValueChangedCallback(changeEvent => {
                // TrimStart because leading whitespaces make fuzzy search fail.
                queryString.SetValueWithoutNotify(
                    changeEvent.newValue.TrimStart());
                ExecuteQuery();
            });

            // First query.
            UpdateGraphAssetSourceCache();
            ExecuteQuery();
        }
        protected override void OnGraphAssetListChanged()
        {
            UpdateGraphAssetSourceCache();
            ExecuteQuery();
        }
        protected override void OnPlayModeChanged(PlayModeStateChange stateChange)
        {
            // Redo ExecuteQuery here because GraphReferences become invalid 
            // when switching between modes.
            if(stateChange == PlayModeStateChange.EnteredEditMode
            || stateChange == PlayModeStateChange.EnteredPlayMode)
            {
                ExecuteQuery();
            }
        }
        void Update()
        {
            if(hasFocus && executeQueryScheduled)
            {
                // Possibly faster than a full query, because it doesn't need to
                // visit every node in scope.
                // Still needs to sort the query results again, though.
                // QueryEditedGraph();

                // FIXME: Using this because QueryEditedGraph is buggy.
                ExecuteQuery();
                executeQueryScheduled = false;
            }
        }
        private void ExecuteQuery()
        {
            EditorPrefs.SetString(EditorPrefKeys.QueryString, queryString.value);
            var sources = graphAssetSourceCache.Concat(FindAllRuntimeGraphSources());
            var editedGraph = GetEditedGraph();
            if(editedGraph != null) 
                sources.Append(editedGraph);
            if (queryType.value == QueryType.Nodes)
            {
                queryResultsListView.LoadQueryResults(FindNodes(
                    sources,
                    queryString.value));
            }
            else if (queryType.value == QueryType.StickyNotes)
            {
                queryResultsListView.LoadQueryResults(FindStickyNotes(
                    sources,
                    queryString.value));
            }
            else if (queryType.value == QueryType.States)
            {
                queryResultsListView.LoadQueryResults(FindStates(
                    sources,
                    queryString.value));
            }
            else if (queryType.value == QueryType.StateTransitions)
            {
                queryResultsListView.LoadQueryResults(FindStateTransitions(
                    sources,
                    queryString.value));
            }
            else throw new System.ArgumentException($"Unknown query type: {queryType.text}");
        }
        private void QueryEditedGraph()
        {
            var editedGraphSource = GetEditedGraph();
            if (editedGraphSource != null)
            {
                if (queryType.value == QueryType.Nodes)
                {
                    queryResultsListView.UpdateQueryResults(
                        editedGraphSource,
                        FindNodes(
                            editedGraphSource,
                            queryString.value));
                }
                else if (queryType.value == QueryType.StickyNotes)
                {
                    queryResultsListView.UpdateQueryResults(
                        editedGraphSource,
                        FindStickyNotes(
                            editedGraphSource,
                            queryString.value));
                }
                else if (queryType.value == QueryType.States)
                {
                    queryResultsListView.UpdateQueryResults(
                        editedGraphSource,
                        FindStates(
                            editedGraphSource,
                            queryString.value));
                }
                else if (queryType.value == QueryType.StateTransitions)
                {
                    queryResultsListView.UpdateQueryResults(
                        editedGraphSource,
                        FindStateTransitions(
                            editedGraphSource,
                            queryString.value));
                }
                else throw new System.ArgumentException($"Unknown query type: {queryType.text}");
            }
            else
            {
                if(LudiqEditorUtility.editedObject.value == null)
                {
                    Debug.Log("No edited object found.");
                }
                else
                {
                    Debug.Log("Edited object is type: " + LudiqEditorUtility.editedObject.value.GetType());
                }
            }
        }
        private string[] GetQueryFolders()
        {
            return queryFolders.value.Split(
                new char[] { ' ', ',', '\n', '\t', '\r' },
                System.StringSplitOptions.RemoveEmptyEntries);
        }
        private void UpdateGraphAssetSourceCache()
        {
            graphAssetSourceCache.Clear();
            graphAssetSourceCache.AddRange(FindAllGraphAssets(GetQueryFolders()));
        }
    }
}