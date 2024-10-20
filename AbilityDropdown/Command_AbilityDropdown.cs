using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace AbilityDropdown {
    public class Command_AbilityDropdown : Command {
        AbilityDropdownGroupCategoryDef groupDef = null;
        public Command_AbilityDropdown(AbilityDropdownGroupCategoryDef def) {
            groupDef = def;
            this.icon = groupDef.GetIcon();
        }

        public static string OpenText => "AbilityDropdown_open".Translate();
        public static string CloseText => "AbilityDropdown_close".Translate();
        public override string Label {
            get {
                return (groupDef.Open ? CloseText : OpenText) + groupDef.label;
            }
        }

        public override void ProcessInput(Event ev) {
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
            groupDef.Open = !groupDef.Open;
        }


        public static readonly Texture2D PlusTex = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
        public static readonly Texture2D MinusTex = ContentFinder<Texture2D>.Get("UI/Buttons/Minus", true);
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms) {
            GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
            GUI.DrawTexture(new Rect(topLeft.x + 42f, topLeft.y + 1f, 32f, 32f), groupDef.Open ? MinusTex : PlusTex);
            return result;
        }
    }
}
