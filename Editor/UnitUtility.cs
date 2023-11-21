using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingPlus.Editor
{
    public static class UnitUtility
    {
        public static string Name(this IUnit unit)
        {
            // var option = unit.Option<IUnitOption>();
            // if(option != null)
            //     return option.haystack;
            var description = unit.Description<UnitDescription>();
            bool hasSurtitle = description.surtitle != null && description.surtitle.Length > 0;
            bool hasTitle = description.title != null && description.title.Length > 0;
            if(hasSurtitle && hasTitle)
                return description.surtitle + ": " + description.title;
            else if(hasSurtitle && !hasTitle)
                return description.surtitle;
            else if(!hasSurtitle && hasTitle)
                return description.title;
            else return unit.GetType().HumanName();
        }
        public static EditorTexture Icon(this IUnit unit)
        {
            var description = unit.Description<UnitDescription>();
            return description.icon;
        }
        public static string Name(this IState state)
        {
            var description = state.Description<StateDescription>();
            bool hasTitle = description.title != null && description.title.Length > 0;
            if(hasTitle)
                return description.title;
            return state.GetType().HumanName();
        }
        public static EditorTexture Icon(this IState state)
        {
            var description = state.Description<StateDescription>();
            return description.icon;
        }
        public static string Name(this IStateTransition stateTransition)
        {
            var description = stateTransition.Description<StateTransitionDescription>();
            bool hasTitle = description.title != null && description.title.Length > 0
            && description.title != "(No Event)";
            if(hasTitle)
                return description.title;
            return stateTransition.GetType().HumanName();
        }
        public static EditorTexture Icon(this IStateTransition stateTransition)
        {
            var description = stateTransition.Description<StateTransitionDescription>();
            return description.icon;
        }
    }
}
