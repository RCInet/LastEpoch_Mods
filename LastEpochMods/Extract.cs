using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using LastEpochMods.Db;
using MelonLoader;
using UnhollowerBaseLib;
using UnityEngine;
using UniverseLib;

namespace LastEpochMods
{
    public class Extract : MelonMod
    {
        //Functions        
        private UnityEngine.Object GetObject(string name)
        {
            UnityEngine.Object objet = new UnityEngine.Object();
            foreach (UnityEngine.Object obj in RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((name != "") && (obj.name.Contains(name)))
                {
                    System.Type type = obj.GetActualType();
                    if (type != typeof(TextAsset))
                    {
                        objet = obj;
                        break;
                    }                                 
                }
            }
            return objet;
        }        
        private void GetUniqueAndSetList()
        {
            UnityEngine.Object obj = GetObject("UniqueList");
            System.Type type = obj.GetActualType();
            if (type == typeof(UniqueList))
            {
                UniqueList unique_list = obj.TryCast<UniqueList>();
                List<UniqueList.Entry> Uniques_List_Entry = unique_list.uniques;
                LoggerInstance.Msg("Unique List Count = " + Uniques_List_Entry.Count);
                foreach (UniqueList.Entry ul_entry in Uniques_List_Entry)
                {
                    int base_type = ul_entry.baseType;
                    string unique_name = ul_entry.name;
                    int old_unique_id = ul_entry.oldUniqueID;
                    int new_unique_id = ul_entry.uniqueID;
                    bool is_set = ul_entry.isSetItem;
                    if (!is_set)
                    {
                        LoggerInstance.Msg("Unique : Type = " + base_type + ", Name = " + unique_name + ", Id = " + new_unique_id);
                    }
                    else
                    {
                        LoggerInstance.Msg("Set : Type = " + base_type + ", Name = " + unique_name + ", Id = " + new_unique_id);
                    }
                }
            }
        }
        private void GetAffixsList()
        {
            UnityEngine.Object obj = GetObject("MasterAffixesList");
            System.Type type = obj.GetActualType();
            if (type == typeof(AffixList))
            {
                if (!Db.Data.Affixs_init) { Db.Data.InitAffixs(); }
                AffixList item_list = obj.TryCast<AffixList>();
                Il2CppReferenceArray<AffixList.SingleAffix> single_affix = item_list.singleAffixes;
                Il2CppReferenceArray<AffixList.MultiAffix> multi_affix = item_list.multiAffixes;
                int i = 0;
                foreach (AffixList.SingleAffix a in single_affix)
                {
                    Db.Json.Affixs.Affix shard = Data.SingleAffixToShard(a);
                    Db.Data.DB_Single_Affixs.List.Add(shard);
                    i++;
                }
                LoggerInstance.Msg(Db.Data.DB_Single_Affixs.List.Count + " / " +
                    single_affix.Count + " Single Affixs Add in Data.Prefixs");
                i = 0;
                foreach (AffixList.MultiAffix a in multi_affix)
                {
                    Db.Json.Affixs.Affix shard = Data.MultiAffixToShard(a);
                    Db.Data.DB_Multi_Affixs.List.Add(shard);
                    i++;
                }
                LoggerInstance.Msg(Db.Data.DB_Multi_Affixs.List.Count + " / " +
                    multi_affix.Count + " Multi Affixs Add in Data.Suffixs");
                LoggerInstance.Msg("Save " + (Db.Data.DB_Single_Affixs.List.Count + Db.Data.DB_Multi_Affixs.List.Count) + " affixs in Affixs.json");
                Db.Save.AffixsJson(this);
            }
        }
        //Events
        private string scene_name = "";
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            scene_name = sceneName;
        }
        public override void OnLateUpdate()
        {
            if (scene_name != "")
            {
                if ((scene_name != "PersistentUI") && (scene_name != "Login") &&
                    (scene_name != "CharacterSelectScene"))
                {
                    if (Input.GetKeyDown(KeyCode.F10)) { GetAffixsList(); }
                    else if (Input.GetKeyDown(KeyCode.F11)) { GetUniqueAndSetList(); }
                }
                else { LoggerInstance.Msg("Lauch a character before doing this"); }
            }
            else { LoggerInstance.Msg("You're not in a scene"); }
        }
    }
}
