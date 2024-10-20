using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;

namespace AbilityDropdown {
    [StaticConstructorOnStartup]
    public class AbilityDropdown {
        static AbilityDropdown() {
            Log.Message("[AbilityDropdown] Now active");
            var harmony = new Harmony("kaitorisenkou.AbilityDropdown");
            var inner_PATGetGizmos =
                typeof(Pawn_AbilityTracker)
                .GetNestedTypes(AccessTools.all)
                .FirstOrDefault((Type t) => t.Name.Contains("<GetGizmos>"));
            harmony.Patch(
                AccessTools.Method(inner_PATGetGizmos, "MoveNext", null, null),
                null,
                null,
                new HarmonyMethod(typeof(AbilityDropdown), nameof(Patch_PATGetGizmos), null),
                null
                );
            harmony.Patch(
                AccessTools.Method(typeof(Pawn_AbilityTracker), nameof(Pawn_AbilityTracker.GetGizmos), null, null),
                null,
                new HarmonyMethod(typeof(AbilityDropdown), nameof(Patch_PATGetGizmos_Postfix), null),
                null,
                null
                );
            Log.Message("[AbilityDropdown] Harmony patch complete!");
        }


        public static IEnumerable<CodeInstruction> Patch_PATGetGizmos(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {

            var instructionList = instructions.ToList();
            int patchCount = 0;
            MethodInfo targetInfo = AccessTools.Method(typeof(Ability), nameof(Ability.GizmosVisible));
            var innerType = typeof(Pawn_AbilityTracker).GetNestedTypes(AccessTools.all).First(t => t.Name.Contains("GetGizmos"));
            FieldInfo a_field = innerType.GetFields(AccessTools.all).First(t => t.Name.Contains("<a>"));
            for (int i = 0; i < instructionList.Count; i++) {
                if (instructionList[i].opcode == OpCodes.Callvirt && (MethodInfo)instructionList[i].operand == targetInfo) {
                    int ii = i;
                    for (; instructionList[ii].opcode != OpCodes.Brfalse_S; ii--) { }
                    var brfalseInst = instructionList[ii];
                    instructionList.InsertRange(i+1, new CodeInstruction[]{
                        new CodeInstruction(brfalseInst),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, a_field),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AbilityDropdown),nameof(HideGroupedAbility)))
                    });
                    patchCount++;
                    break;
                }
            }
            if (patchCount < 1) {
                Log.Error("[AbilityDropdown] Patch_PATGetGizmos seems failed!");
            }
            return instructionList;
        }
        public static bool HideGroupedAbility(Ability ability) {
            return ability.CompOfType<CompAbility_DropdownGroup>() == null;
        }


        public static void Patch_PATGetGizmos_Postfix(ref IEnumerable<Gizmo> __result, Pawn_AbilityTracker __instance) {
            __result = Int_PATGetGizmosPostfix(__result, __instance);
        }
        static IEnumerable<Gizmo> Int_PATGetGizmosPostfix(IEnumerable<Gizmo> __result, Pawn_AbilityTracker __instance) {
            foreach(var i in __result) {
                yield return i;
            }
            Pawn pawn = __instance.pawn;
            foreach (var groupDef in DefDatabase<AbilityDropdownGroupCategoryDef>.AllDefsListForReading) {
                if (!groupDef.HasGroup(pawn)) continue;
                yield return groupDef.GetGizmo();
                if (groupDef.Open) {
                    float order = groupDef.uiOrder + 0.01f;
                    foreach (var element in groupDef.GetGroup(pawn))
                        foreach (var gizmo in element.parent.GetGizmos()) {
                            gizmo.Order = order;
                            yield return gizmo;
                            order += 0.01f;
                        }
                }
                
            }
            yield break;
        }
    }
}
