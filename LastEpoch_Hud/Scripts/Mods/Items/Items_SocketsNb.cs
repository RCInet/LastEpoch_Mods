using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    [RegisterTypeInIl2Cpp]
    public class Items_SocketsNb : MonoBehaviour
    {
        public static Items_SocketsNb instance { get; private set; }
        public Items_SocketsNb(System.IntPtr ptr) : base(ptr) { }
        private bool Done = false;
        private bool Started = false;

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if ((!Refs_Manager.item_list.IsNullOrDestroyed()) &&
                (!Save_Manager.instance.IsNullOrDestroyed()) &&
                (!Done))
            {
                if (!Started)
                {
                    Started = true;
                    if (SetSocketMax((int)Save_Manager.instance.data.Items.Drop.AffixCount_Max)) { Done = true; }
                    Started = false;
                }
            }
        }
        bool SetSocketMax(int nb_socket)
        {
            foreach (ItemList.BaseEquipmentItem base_item in Refs_Manager.item_list.EquippableItems)
            {
                if (base_item.baseTypeID < 25) { base_item.maximumAffixes = 6; } //Unlock 6 Affixs
                //else if (base_item.baseTypeID < 34) { base_item.maximumAffixes = nb_socket; } //unlock if you need same for idols
            }

            if (Refs_Manager.item_list.EquippableItems.Count > 24)
            {
                Main.logger_instance.Msg("Items Max Sockets Done");
                return true;
            }
            else { return false; }
        }
    }
}
