using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CHM.VisualScriptingPlus.Editor
{
    public static class UnitUtility
    {
        public static string NodeName(this IUnit unit)
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
            else return description.title;
        }
        public static EditorTexture NodeIcon(this IUnit unit)
        {
            var description = unit.Description<UnitDescription>();
            return description.icon;
        }
    }
}
