using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    public class Skills_UnlockPassives_SpendPoints
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Skills.Enable_AllSkills;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(CharacterClass), "getLevelUnlockedBaseClassAbilities")]
        public class CharacterClass_getLevelUnlockedBaseClassAbilities
        {
            [HarmonyPrefix]
            static void Prefix(ref CharacterClass __instance, Il2CppSystem.Collections.Generic.List<Ability> __result)
            {
                if (CanRun())
                {
                    foreach (var a in __instance.unlockableAbilities) { a.level = 0; }
                    foreach (Mastery m in __instance.masteries)
                    {
                        foreach (var b in m.abilities) { b.level = 0; }
                    }
                }
            }

        }
    }
}
