using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AbilityDropdown {
    public class AbilityDropdownGroupCategoryDef : Def {
        public float uiOrder = 0;

        bool openInt = false;
        public bool Open {
            get => openInt;
            set => openInt = value;
        }
        public IEnumerable<CompAbility_DropdownGroup> GetGroup(Pawn pawn) => CompAbility_DropdownGroup.GetGroup(this, pawn);
        public bool HasGroup(Pawn pawn) => CompAbility_DropdownGroup.HasGroupedAbility(this, pawn);
        [NoTranslate]
        public string iconPath;

        Texture uiIcon = null;
        public Texture GetIcon (){
            if (uiIcon == null) {
                uiIcon=ContentFinder<Texture2D>.Get(this.iconPath, true);
            }
            return uiIcon;
        }

        Command_AbilityDropdown gizmo;
        public Command_AbilityDropdown GetGizmo() {
            if (gizmo == null) {
                this.gizmo = new Command_AbilityDropdown(this);
                gizmo.Order = uiOrder;
            }

            return gizmo;
        }

        public static void TryClose(Ability ability) {
            var comp = ability.CompOfType<CompAbility_DropdownGroup>();
            TryClose(comp);
        }
        public static void TryClose(CompAbility_DropdownGroup comp) {
            if (comp != null) {
                comp.Props.groupDef.Open = false;
            }
        }
    }
}
