using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CHM.VisualScriptingKai.Editor
{
    // ! You may need to close all windows every time you change EditorWindow code.
    public abstract class GraphQueryWindowBase : EditorWindow
    {
        protected bool executeQueryScheduled = false;
        public void CreateGUI()
        {
            OnCreateGUI();
            
            GraphAssetListTracker.OnGraphAssetListChanged += OnGraphAssetListChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            GraphWindow.activeContextChanged += OnGraphWindowActiveContextChanged;
            OnGraphWindowActiveContextChanged(GraphWindow.activeContext);
        }
        protected virtual void OnDestroy() 
        {
            GraphAssetListTracker.OnGraphAssetListChanged -= OnGraphAssetListChanged;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            GraphWindow.activeContextChanged -= OnGraphWindowActiveContextChanged;
        }
        protected abstract void OnCreateGUI();
        protected virtual void OnGraphWindowActiveContextChanged(IGraphContext context)
        {
            if(context == null)
                return;
            if(context.graph == null)
                return;
            
            context.graph.elements.CollectionChanged += () => {
                // Defer ExecuteQuery to the next Update, because
                // CollectionChanged might be invoked by the serializer, 
                // causing possible deadlocks.
                executeQueryScheduled = true;
                // Note: Adding/deleting nodes won't introduce new exceptions, but might
                // remove old ones.
            };
            context.canvas.window = GraphWindow.active;
        }
        protected virtual void OnPlayModeChanged(PlayModeStateChange stateChange)
        {

        }
        
        protected virtual void OnGraphAssetListChanged()
        {

        }
        protected void FetchElement<T>(VisualElement root, string name, out T result) where T : VisualElement
        {
            result = root.Q<T>(name);
            if(result == null)
                Debug.LogWarning($"Couldn't find {typeof(T)} named {name}. Try reopening {titleContent.text}.");
        }
    }
}
