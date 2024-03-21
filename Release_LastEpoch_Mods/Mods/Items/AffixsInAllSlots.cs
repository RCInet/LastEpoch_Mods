using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Items
{
    public class AffixsInAllSlots
    {
        private static bool Enable_AllEquipmentsSlots = true;
        private static Il2CppSystem.Collections.Generic.List<EquipmentType> AllEquipements = null;
        private static void Init_AllEquipments()
        {
            if (AllEquipements == null)
            {
                AllEquipements = new Il2CppSystem.Collections.Generic.List<EquipmentType>();
                AllEquipements.Add(EquipmentType.HELMET);
                AllEquipements.Add(EquipmentType.BODY_ARMOR);
                AllEquipements.Add(EquipmentType.BELT);
                AllEquipements.Add(EquipmentType.BOOTS);
                AllEquipements.Add(EquipmentType.GLOVES);
                AllEquipements.Add(EquipmentType.ONE_HANDED_AXE);
                AllEquipements.Add(EquipmentType.ONE_HANDED_DAGGER);
                AllEquipements.Add(EquipmentType.ONE_HANDED_MACES);
                AllEquipements.Add(EquipmentType.ONE_HANDED_SCEPTRE);
                AllEquipements.Add(EquipmentType.ONE_HANDED_SWORD);
                AllEquipements.Add(EquipmentType.WAND);
                AllEquipements.Add(EquipmentType.ONE_HANDED_FIST);
                AllEquipements.Add(EquipmentType.TWO_HANDED_AXE);
                AllEquipements.Add(EquipmentType.TWO_HANDED_MACE);
                AllEquipements.Add(EquipmentType.TWO_HANDED_SPEAR);
                AllEquipements.Add(EquipmentType.TWO_HANDED_STAFF);
                AllEquipements.Add(EquipmentType.TWO_HANDED_SWORD);
                AllEquipements.Add(EquipmentType.QUIVER);
                AllEquipements.Add(EquipmentType.SHIELD);
                AllEquipements.Add(EquipmentType.CATALYST);
                AllEquipements.Add(EquipmentType.AMULET);
                AllEquipements.Add(EquipmentType.RING);
                AllEquipements.Add(EquipmentType.RELIC);
                AllEquipements.Add(EquipmentType.BOW);
                AllEquipements.Add(EquipmentType.CROSSBOW);
            }
        }
        public static void AddAllEquipementSlots(ref AffixList.Affix affix)
        {
            if (AllEquipements == null) { Init_AllEquipments(); }
            if (Enable_AllEquipmentsSlots)
            {
                if (affix.canRollOn != AllEquipements) { affix.canRollOn = AllEquipements; }
            }
        }

        private static bool Enable_AllIdolsSlots = false;
        private static Il2CppSystem.Collections.Generic.List<EquipmentType> AllIdols = null;
        private static void Init_AllIdols()
        {
            AllIdols = new Il2CppSystem.Collections.Generic.List<EquipmentType>();
            AllIdols.Add(EquipmentType.IDOL_1x1_ETERRA);
            AllIdols.Add(EquipmentType.IDOL_1x1_LAGON);
            AllIdols.Add(EquipmentType.IDOL_1x2);
            AllIdols.Add(EquipmentType.IDOL_1x3);
            AllIdols.Add(EquipmentType.IDOL_1x4);
            AllIdols.Add(EquipmentType.IDOL_2x1);
            AllIdols.Add(EquipmentType.IDOL_2x2);
            AllIdols.Add(EquipmentType.IDOL_3x1);
            AllIdols.Add(EquipmentType.IDOL_4x1);
        }
        public static void AddAllIdolSlots(ref AffixList.Affix affix)
        {
            if (AllIdols == null) { Init_AllIdols(); }
            if (Enable_AllIdolsSlots)
            {
                if (affix.canRollOn != AllIdols) { affix.canRollOn = AllIdols; }
            }
        }

        [HarmonyPatch(typeof(AffixList), "get")]
        public class AffixList_Get
        {
            [HarmonyPostfix]
            static void Postfix(ref AffixList __result)
            {
                if (Save_Manager.Data.UserData.Items.RemoveReq.Enable_AllAffixsInAllSlots)
                {
                    foreach (AffixList.SingleAffix affix in AffixList.instance.singleAffixes)
                    {
                        AffixList.Affix aff = affix.TryCast<AffixList.Affix>();
                        if (affix.rollsOn == AffixList.RollsOn.Idols) { AddAllIdolSlots(ref aff); }
                        else { AddAllEquipementSlots(ref aff); }
                    }
                    foreach (AffixList.MultiAffix affix in AffixList.instance.multiAffixes)
                    {
                        AffixList.Affix aff = affix.TryCast<AffixList.Affix>();
                        if (affix.rollsOn == AffixList.RollsOn.Idols) { AddAllIdolSlots(ref aff); }
                        else { AddAllEquipementSlots(ref aff); }
                    }
                }
            }
        }
    }
}
