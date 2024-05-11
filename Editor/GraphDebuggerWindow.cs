using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CHM.VisualScriptingKai.Editor
{
    public class GraphDebuggerWindow : GraphQueryWindowBase, IHasCustomMenu
    {
        private Toggle debugEnabled;
        private QueryResultsListView queryResultsListView;
        [MenuItem("Window/Visual Scripting/Graph Debugger")]
        public static void OpenWindow()
        {
            GraphDebuggerWindow wnd = GetWindow<GraphDebuggerWindow>(
                typeof(GraphQueryWindowBase));
            // TODO: Add Graph Lens icon to GUIContent.
            wnd.titleContent = new GUIContent("Graph Debugger");
        }
        private static class EditorPrefKeys
        {
            public static readonly string DebugEnabled = Application.dataPath + "/GraphDebugger/debug-enabled";
        }
        protected override void OnCreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = PackageUtility.LoadPackageAsset<VisualTreeAsset>("Editor/Resources/GraphDebuggerWindow.uxml");
            VisualElement visualTreeRoot = visualTree.Instantiate();
            root.Add(visualTreeRoot);

            FetchElement(root, "debug-enabled", out debugEnabled);
            FetchElement(root, "query-results", out queryResultsListView);

            debugEnabled.value = EditorPrefs.GetBool(EditorPrefKeys.DebugEnabled, false);
            debugEnabled.RegisterValueChangedCallback(changeEvent => 
            {
                EditorPrefs.SetBool(EditorPrefKeys.DebugEnabled, changeEvent.newValue);
            });
            queryResultsListView = root.Q<QueryResultsListView>("query-results");
            queryResultsListView.style.backgroundColor = new Color(0.5f, 0.25f, 0.25f);
        }
        void Update()
        {
            if(debugEnabled.value 
            && Application.isPlaying
            && executeQueryScheduled)
            {
                // We only ever use this to remove entries, so an optimization here is
                // to skip if there's nothing left.
                if(queryResultsListView.Count > 0)
                    ExecuteQuery();
                executeQueryScheduled = false;
            }
        }
        private void ExecuteQuery()
        {
            // Expect the caller to pause first.
            // Debug.Assert(Application.isPlaying);
            var sources = GraphUtility.FindAllRuntimeGraphSources();
            queryResultsListView.LoadQueryResults(GraphUtility.FindAllExceptions(sources));
            // To make the results look consistent, we need to flush results on
            // EditorApplication.pauseStateChanged.
        }
        protected override void OnPlayModeChanged(PlayModeStateChange stateChange)
        {
            if(!debugEnabled.value)
                return;
            if(stateChange == PlayModeStateChange.ExitingEditMode)
            {
                Application.logMessageReceived += OnLogMessageReceived;
                EditorApplication.pauseStateChanged  += OnPauseModeChanged;
                executeQueryScheduled = false;
            }
            else if(stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                Application.logMessageReceived -= OnLogMessageReceived;
                EditorApplication.pauseStateChanged  -= OnPauseModeChanged;
            }
            else if(stateChange == PlayModeStateChange.EnteredEditMode)
            {
                // TODO: Figure out a way to replace runtime graph references with editor ones.
                // Until then, we have no choice but to flush.
                queryResultsListView.FlushQueryResults();
                // // Redo the query to replace runtime graph references with editor ones.
                // ExecuteQuery();
            }
        }
        private void OnPauseModeChanged(PauseState pauseState)
        {
            if(!debugEnabled.value)
                return;
            if(pauseState == PauseState.Unpaused)
            {
                queryResultsListView.FlushQueryResults();
            }
        }
        private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            if(!debugEnabled.value)
                return;
            if(type == LogType.Error || type == LogType.Exception)
            {
                if(stackTrace.Contains("Unity.VisualScripting.Flow.Invoke"))
                {
                    // Pause the editor, then look for exception nodes.
                    Debug.Break();
                    // Highlight the debugger.
                    GetWindow<GraphDebuggerWindow>(false).Show();
                    ExecuteQuery();
                }
            }
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            if(debugEnabled == null)
                return;
            if(menu == null)
                return;
            menu.AddItem(
                new GUIContent("Toggle Enabled"), 
                debugEnabled.value, () => {
                    debugEnabled.value = !debugEnabled.value;
                });
        }
    }
}
