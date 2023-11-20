using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;

namespace CHM.VisualScriptingPlus.Editor
{
    public interface IGraphElementTrace : System.IComparable<IGraphElementTrace>
    {
        abstract IGraphElement GraphElement { get; }
        GraphReference Reference { get; set; }
        GraphSource Source { get; set; }
        public long Score { get; set; }
        public Vector2 GraphPosition { get; }
        string GetInfo();
        Texture2D GetIcon(int resolution);
        /// <summary>
        /// Open the graph editor and make it jump to the graph element.
        /// </summary>
        void GraphWindowJumpToLocation()
        {
            if(!Reference.isValid)
            {
                Debug.LogWarning("Invalid graph reference. This may be a bug. Please try again.");
                return;
            }
            var sourceObject = (Object) Source;
            // TODO: This part of the code will destroy the graph reference.
            // Will probably need to restore the reference somehow afterwards.
            // For now this is okay though, any potential scene here must be 
            // the currently-loaded scene.
            // // If the source is a GameObject, open its scene or prefab first.
            // if(source.TryGetGameObject(out var gameObject))
            // {
            //     if(gameObject.scene != null)
            //         AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<SceneAsset>(gameObject.scene.path));
            //     else AssetDatabase.OpenAsset(gameObject);
            // }

            // ! For some reasons, parent graphs that can be accessed after jumping
            // ! cause the VisualScriptingCanvas to lose its GraphWindow reference.

            // ! OnGUI -> current context's canvas's window is set
            // ! -> Clicking breadcrumb -> context changes, but new context's canvas's window is not set
            // ! -> OnGUI continues, null reference with canvas.window
            // ! Ultimately this is because the breadcrumb reference became invalid, which made the decorator get a new context.
            // * Hacky fix implemented in GraphLensWindow by using the context changed callback to fix the reference.

            // Either way, we still need to open the graph asset or MonoBehaviour script.
            AssetDatabase.OpenAsset(sourceObject);
            EditorGUIUtility.PingObject(sourceObject);
            GraphWindow.OpenActive(Reference);

            // Pan the graph to the node's position.
            // Consider tweening? Maybe not?
            GraphWindow.activeContext.graph.pan = GraphPosition;
        }
    }
    public static class GraphElementTraceExtensions
    {
        /// <summary>
        /// Default CompareTo implementation for IGraphElementTrace.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static int DefaultCompareTo(this IGraphElementTrace @this, IGraphElementTrace other)
        {
            // Sort descending.
            int compare = -@this.Score.CompareTo(other.Score);
            if(compare != 0) 
                return compare;
            compare = -@this.Source.GetInstanceID().CompareTo(other.Source.GetInstanceID());
            if(compare != 0)
                return compare;
            // Don't use this, graph is transient and you can end up with different instances pointing to the "same" graph.
            // compare = @this.Reference.graph.GetHashCode().CompareTo(other.Reference.graph.GetHashCode());
            // if(compare != 0)
            //     return compare;
            compare = @this.GraphPosition.x.CompareTo(other.GraphPosition.x);
            if(compare != 0)
                return compare;
            return @this.GraphPosition.y.CompareTo(other.GraphPosition.y);
        }
    }
}
