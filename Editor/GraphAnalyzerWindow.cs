using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using static CHM.VisualScriptingPlus.Editor.GraphUtility;
using Unity.VisualScripting;

namespace CHM.VisualScriptingPlus.Editor
{
    public class GraphAnalyzerWindow : GraphQueryWindowBase
    {
        private DropdownField queryWarningLevel;
        private TextField queryFolders;
        private QueryResultsListView queryResultsListView;
        private readonly List<GraphSource> graphAssetSourceCache = new();
        private readonly List<string> queryWarningLevels = new(){
            nameof(WarningLevel.Info),
            nameof(WarningLevel.Caution),
            nameof(WarningLevel.Severe),
            nameof(WarningLevel.Error),
        };
        private static class EditorPrefKeys
        {
            public static readonly string QueryWarningLevel = Application.dataPath + "/GraphAnalyzer/query-warning-level";
            public static readonly string QueryFolders = Application.dataPath + "/GraphAnalyzer/query-folders";
        }
        [MenuItem("Window/Visual Scripting/Graph Analyzer")]
        public static void OpenWindow()
        {
            GraphAnalyzerWindow wnd = GetWindow<GraphAnalyzerWindow>(
                typeof(GraphQueryWindowBase));
            // TODO: Add Graph Lens icon to GUIContent.
            wnd.titleContent = new GUIContent("Graph Analyzer");
        }
        protected override void OnCreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.chocola-mint.visual-scripting-plus/Editor/Resources/GraphAnalyzerWindow.uxml");
            VisualElement visualTreeRoot = visualTree.Instantiate();
            root.Add(visualTreeRoot);

            // Set up references.
            FetchElement(visualTreeRoot, "query-warning-level", out queryWarningLevel);
            FetchElement(visualTreeRoot, "query-folders", out queryFolders);
            FetchElement(visualTreeRoot, "query-results", out queryResultsListView);

            queryFolders.value = EditorPrefs.GetString(EditorPrefKeys.QueryFolders, "Assets");
            queryFolders.RegisterValueChangedCallback(changeEvent => {
                EditorPrefs.SetString(EditorPrefKeys.QueryFolders, queryFolders.value);
                // Folders changed, so the cache needs to be updated.
                UpdateGraphAssetSourceCache();
                ExecuteQuery();
            });

            queryWarningLevel.choices = queryWarningLevels;
            queryWarningLevel.index = EditorPrefs.GetInt(EditorPrefKeys.QueryWarningLevel, 0);
            if(queryWarningLevel.index >= queryWarningLevels.Count)
                queryWarningLevel.index = 0;
            queryWarningLevel.RegisterValueChangedCallback(changeEvent => {
                EditorPrefs.SetInt(EditorPrefKeys.QueryWarningLevel, queryWarningLevel.index);
                ExecuteQuery();
            });
            queryResultsListView.style.backgroundColor = new Color(0.4f, 0.35f, 0f);

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
            var sources = graphAssetSourceCache.Concat(FindAllRuntimeGraphSources());
            var editedGraph = GetEditedGraph();
            if(editedGraph != null) 
                sources.Append(editedGraph);
            queryResultsListView.LoadQueryResults(
                FindWarnings(sources, (WarningLevel)queryWarningLevel.index + (int)WarningLevel.Info));
        }
        private void QueryEditedGraph()
        {
            var editedGraphSource = GetEditedGraph();
            if (editedGraphSource != null)
            {
                queryResultsListView.UpdateQueryResults(
                    editedGraphSource, FindWarnings(editedGraphSource, (WarningLevel)queryWarningLevel.index + (int)WarningLevel.Info));
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
