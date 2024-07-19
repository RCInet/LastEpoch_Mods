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
                SetSocketMax(Save_Manager.instance.data.modsNotInHud.Items_MaxSockets);
            }
        }
        void SetSocketMax(int nb_socket)
        {
            foreach (ItemList.BaseEquipmentItem base_item in Refs_Manager.item_list.EquippableItems)
            {
                base_item.maximumAffixes = nb_socket;
                //base_item.maxSockets = nb_socket;
            }
        }
    }
}
