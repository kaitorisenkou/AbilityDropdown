using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AbilityDropdown {
    public class CompAbility_DropdownGroup : CompAbilityEffect {
        public new CompProperties_AbilityDropdown Props {
            get {
                return (CompProperties_AbilityDropdown)this.props;
            }
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest) {
            AbilityDropdownGroupCategoryDef.TryClose(this);
        }

        public IEnumerable<CompAbility_DropdownGroup> GetGroup() => 
            CompAbility_DropdownGroup.GetGroup(Props.groupDef, parent.pawn);
        public static IEnumerable<CompAbility_DropdownGroup> GetGroup(AbilityDropdownGroupCategoryDef def,Pawn pawn) {
            return pawn.abilities.abilities
                .Select(t => t.CompOfType<CompAbility_DropdownGroup>())
                .Where(t => t != null && t.Props.groupDef == def);
        }
        public static bool HasGroupedAbility(AbilityDropdownGroupCategoryDef def, Pawn pawn) {
            return pawn.abilities.abilities.Any(t => t.CompOfType<CompAbility_DropdownGroup>() != null);
        }
    }

    public class CompProperties_AbilityDropdown : AbilityCompProperties {
        public CompProperties_AbilityDropdown() {
            this.compClass = typeof(CompAbility_DropdownGroup);
        }
        public AbilityDropdownGroupCategoryDef groupDef = null;
    }
}
