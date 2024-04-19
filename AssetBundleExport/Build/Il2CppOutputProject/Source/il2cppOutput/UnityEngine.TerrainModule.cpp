#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <cstring>
#include <string.h>
#include <stdio.h>
#include <cmath>
#include <limits>
#include <assert.h>
#include <stdint.h>

#include "codegen/il2cpp-codegen.h"
#include "il2cpp-object-internals.h"

template <typename T1, typename T2>
struct VirtActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3>
struct VirtActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct VirtActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R>
struct VirtFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R, typename T1>
struct VirtFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1, typename T2>
struct GenericVirtActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3>
struct GenericVirtActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct GenericVirtActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R>
struct GenericVirtFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R, typename T1>
struct GenericVirtFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1, typename T2>
struct InterfaceActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3>
struct InterfaceActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct InterfaceActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R>
struct InterfaceFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R, typename T1>
struct InterfaceFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1, typename T2>
struct GenericInterfaceActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3>
struct GenericInterfaceActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct GenericInterfaceActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R>
struct GenericInterfaceFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R, typename T1>
struct GenericInterfaceFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};

// System.AsyncCallback
struct AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4;
// System.Char[]
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2;
// System.Collections.Generic.Dictionary`2/Entry<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>[]
struct EntryU5BU5D_t60C83E552A9B7713913AA260DDAA0AFF0A75765F;
// System.Collections.Generic.Dictionary`2/Entry<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>[]
struct EntryU5BU5D_t7E283CE6C88BF1DF0DBCB399F07FA5BAFC6FF36A;
// System.Collections.Generic.Dictionary`2/KeyCollection<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>
struct KeyCollection_t542DBD0562DA7AC8F172BD71C05844EFAD576100;
// System.Collections.Generic.Dictionary`2/KeyCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>
struct KeyCollection_tCFA1A910066236156BCAB78D25ADCAC725EE054B;
// System.Collections.Generic.Dictionary`2/KeyCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>
struct KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB;
// System.Collections.Generic.Dictionary`2/ValueCollection<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>
struct ValueCollection_t60741DBA4647D7259ECB16D50D6AC4FEE46A5B88;
// System.Collections.Generic.Dictionary`2/ValueCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>
struct ValueCollection_t4570917EBE5D4FD65BC458FAECE220E22C1A632A;
// System.Collections.Generic.Dictionary`2<System.Int32,System.Object>
struct Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884;
// System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>
struct Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753;
// System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>
struct Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018;
// System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>
struct Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C;
// System.Collections.Generic.IEqualityComparer`1<System.Int32>
struct IEqualityComparer_1_t7B82AA0F8B96BAAA21E36DDF7A1FE4348BDDBE95;
// System.Collections.Generic.IEqualityComparer`1<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord>
struct IEqualityComparer_1_tC2ACA0765678CF0F5621C65BA059D29D22EB4709;
// System.Delegate
struct Delegate_t;
// System.DelegateData
struct DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE;
// System.Delegate[]
struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;
// System.IAsyncResult
struct IAsyncResult_t8E194308510B375B42432981AE5E7488C458D598;
// System.Int32[]
struct Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// System.String
struct String_t;
// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;
// UnityEngine.Behaviour
struct Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8;
// UnityEngine.Component
struct Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621;
// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/HeightmapChangedCallback
struct HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D;
// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/TextureChangedCallback
struct TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_0
struct U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_1
struct U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainGroups
struct TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap
struct TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/<>c__DisplayClass4_0
struct U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002;
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter
struct TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B;
// UnityEngine.Object
struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0;
// UnityEngine.Terrain
struct Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943;
// UnityEngine.TerrainData
struct TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2;
// UnityEngine.Terrain[]
struct TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE;
// UnityEngine.Transform
struct Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA;

IL2CPP_EXTERN_C RuntimeClass* Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_Add_m8A2F763FE35F12CE5C6B4EBDC8F490F2432721F0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_Add_mDBC40F15D74AA100E4FD3F5B7B015C453E35FC7F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_ContainsKey_m7BB92DEC475ECE3BB42DA835F9B2C568E81DCFD7_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_GetEnumerator_m6A761440C852EBB50A6DDEF89E54A0BDA6805287_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_GetEnumerator_mA00DFEC8FD1C165B6E30BA0DE649CD91C9F61171_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_TryGetValue_m5BB81EB3FE81C41FFF7B7764770B64EEB214ED2D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2__ctor_m4D0EF0F749453F91361A51A6C9C5C479B334ACF7_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2__ctor_mDF4242C91427656B19BE65AF67E6430A5E151931_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_get_Count_m25627A54AA38F254FE2F74C4BB3EC5D240A69C8A_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_get_Count_m5872BC43117B310319C83B09F904FC7B215F176F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_get_Keys_m6378916440BB5F09FDDC61026EB9E31F5BC8A9E1_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_Dispose_m1AF30CB4B9D460F0CBE9944BBBE800205A8B84AF_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_Dispose_m49E0868CF374239FC10BCEED1A260C935FC1DE9B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_Dispose_m742B154C9D11228799C88A8E5D3F4B247EEF1493_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_MoveNext_m25382F4DAE18791D44400CA8FD709589AE89E66D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_MoveNext_mA095EEA83D1389075851A35D0CBDA713A34C2F7C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_MoveNext_mED5A978C87AD73E54C6A8BE05D98339C44875192_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_get_Current_m253335A3521452DF3D85BFB9EFEA45F1F661D72F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_get_Current_m34348C5A639EBCAE828B3DBA655423086EE285A8_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerator_get_Current_mB84DB5A32BBDC3FBFB6E3419E2EE70128E320FE4_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* KeyCollection_GetEnumerator_mB9D02C9BC28992C54914385A9ADB8B31645D8C29_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* KeyValuePair_2_get_Key_m074DC0DF87A0EC210E1FAD5B15D67DC7ADC7BCB8_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* KeyValuePair_2_get_Key_m187E30244CD96C3D6A8F53AE69ABE9846E42A9BE_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* KeyValuePair_2_get_Value_mF4D8A7CFC178117B78DCC6C4E27B048F0ECD9F80_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass4_0_U3CCreateFromPlacementU3Eb__0_m51A58F8F2A072A87B2E6C84818FEE0DEC2DD6FB0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass4_1_U3CCollectTerrainsU3Eb__0_mC50AC0DBF7CB0E1D89FE0F9DA973B406782C0B25_RuntimeMethod_var;
IL2CPP_EXTERN_C const uint32_t HeightmapChangedCallback_BeginInvoke_m34DB0DA5BF64D5F303A24804C84B3C582BDEFD5D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainCallbacks_InvokeHeightmapChangedCallback_m786753AA38B90C453886C1B1011B8279D194DA54_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainCallbacks_InvokeTextureChangedCallback_m922885C44A5A7F5CD26341414414C2B78CE14A85_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainData__cctor_mB579F93C53A8F85C72D7AA2C6A266DA7F0D066C5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainData__ctor_mEF24945C9BBDA5CAFE4A1C453649B86D79DD87AF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainGroups__ctor_mCC684EF011C9EBA10D335C5BBC2A7B742CB1D940_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_AddTerrainInternal_m2E4B99FEC2C6D4BCC6CFF0E58F0D1E70E254B4C2_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_CreateFromPlacement_m2CFB7C0DD0890EBA733486F6CFF67B15471A6B57_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_CreateFromPlacement_mB23A40ABF3A46620F82C489D749EABEA1EDF27B2_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_TryToAddTerrain_m7F845FD1237F4342EAA377F5B8B078C93F0B2862_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_ValidateTerrain_m7B0154421B22B18D420FF7AB3179887AFCB320AB_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap_Validate_m31FE625EC81CDED0369413935CD78F355677237A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainMap__ctor_m7BC19CC1FA417F6D152B8E290AAD9990DB81E81A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainUtility_AutoConnect_m43FD8F195A874A511293784F8029C22AB30A428E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TerrainUtility_CollectTerrains_m1980638C0C744F59EF15670092FFA1CA9BDA9467_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TextureChangedCallback_BeginInvoke_mBEC316023C6EEA14D6EC02E363B3027A3F0151DC_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;

struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;
struct TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct U3CModuleU3E_tF1895192DB0786D7B32E637E7538ADA625D10FB5 
{
public:

public:
};


// System.Object

struct Il2CppArrayBounds;

// System.Array


// System.Collections.Generic.Dictionary`2/KeyCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>
struct KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB  : public RuntimeObject
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2/KeyCollection::dictionary
	Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * ___dictionary_0;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB, ___dictionary_0)); }
	inline Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}
};


// System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>
struct Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753  : public RuntimeObject
{
public:
	// System.Int32[] System.Collections.Generic.Dictionary`2::buckets
	Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* ___buckets_0;
	// System.Collections.Generic.Dictionary`2/Entry<TKey,TValue>[] System.Collections.Generic.Dictionary`2::entries
	EntryU5BU5D_t60C83E552A9B7713913AA260DDAA0AFF0A75765F* ___entries_1;
	// System.Int32 System.Collections.Generic.Dictionary`2::count
	int32_t ___count_2;
	// System.Int32 System.Collections.Generic.Dictionary`2::version
	int32_t ___version_3;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeList
	int32_t ___freeList_4;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeCount
	int32_t ___freeCount_5;
	// System.Collections.Generic.IEqualityComparer`1<TKey> System.Collections.Generic.Dictionary`2::comparer
	RuntimeObject* ___comparer_6;
	// System.Collections.Generic.Dictionary`2/KeyCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::keys
	KeyCollection_t542DBD0562DA7AC8F172BD71C05844EFAD576100 * ___keys_7;
	// System.Collections.Generic.Dictionary`2/ValueCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::values
	ValueCollection_t60741DBA4647D7259ECB16D50D6AC4FEE46A5B88 * ___values_8;
	// System.Object System.Collections.Generic.Dictionary`2::_syncRoot
	RuntimeObject * ____syncRoot_9;

public:
	inline static int32_t get_offset_of_buckets_0() { return static_cast<int32_t>(offsetof(Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753, ___buckets_0)); }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* get_buckets_0() const { return ___buckets_0; }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83** get_address_of_buckets_0() { return &___buckets_0; }
	inline void set_buckets_0(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* value)
	{
		___buckets_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___buckets_0), (void*)value);
	}

	inline static int32_t get_offset_of_entries_1() { return static_cast<int32_t>(offsetof(Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753, ___entries_1)); }
	inline EntryU5BU5D_t60C83E552A9B7713913AA260DDAA0AFF0A75765F* get_entries_1() const { return ___entries_1; }
	inline EntryU5BU5D_t60C83E552A9B7713913AA260DDAA0AFF0A75765F** get_address_of_entries_1() { return &___entries_1; }
	inline void set_entries_1(EntryU5BU5D_t60C83E552A9B7713913AA260DDAA0AFF0A75765F* value)
	{
		___entries_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___entries_1), (void*)value);
	}

	inline static int32_t get_offset_of_count_2() { return static_cast<int32_t>(offsetof(Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753, ___count_2)); }
	inline int32_t get_count_2() const { return ___count_2; }
	inline int32_t* get_address_of_count_2() { return &___count_2; }
	inline void set_count_2(int32_t value)
	{
		___count_2 = value;
	}

	inline static int32_t get_offset_of_version_3() { return static_cast<int32_t>(offsetof(Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753, ___version_3)); }
	inline int32_t get_version_3() const { return ___version_3; }
	inline int32_t* get_address_of_version_3() { return &___version_3; }
	inline void set_version_3(int32_t value)
	{
		___version_3 = value;
	}

	inline static int32_t get_offset_of_freeList_4() { return static_cast<int32_t>(offsetof(Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753, ___freeList_4)); }
	inline int32_t get_freeList_4() const { return ___freeList_4; }
	inline int32_t* get_address_of_freeList_4() { return &___freeList_4; }
	inline void set_freeList_4(int32_t value)
	{
		___freeList_4 = value;
	}

	inline static int32_t get_offset_of_freeCount_5() { return static_cast<int32_t>(offsetof(Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753, ___freeCount_5)); }
	inline int32_t get_freeCount_5() const { return ___freeCount_5; }
	inline int32_t* get_address_of_freeCount_5() { return &___freeCount_5; }
	inline void set_freeCount_5(int32_t value)
	{
		___freeCount_5 = value;
	}

	inline static int32_t get_offset_of_comparer_6() { return static_cast<int32_t>(offsetof(Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753, ___comparer_6)); }
	inline RuntimeObject* get_comparer_6() const { return ___comparer_6; }
	inline RuntimeObject** get_address_of_comparer_6() { return &___comparer_6; }
	inline void set_comparer_6(RuntimeObject* value)
	{
		___comparer_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___comparer_6), (void*)value);
	}

	inline static int32_t get_offset_of_keys_7() { return static_cast<int32_t>(offsetof(Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753, ___keys_7)); }
	inline KeyCollection_t542DBD0562DA7AC8F172BD71C05844EFAD576100 * get_keys_7() const { return ___keys_7; }
	inline KeyCollection_t542DBD0562DA7AC8F172BD71C05844EFAD576100 ** get_address_of_keys_7() { return &___keys_7; }
	inline void set_keys_7(KeyCollection_t542DBD0562DA7AC8F172BD71C05844EFAD576100 * value)
	{
		___keys_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___keys_7), (void*)value);
	}

	inline static int32_t get_offset_of_values_8() { return static_cast<int32_t>(offsetof(Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753, ___values_8)); }
	inline ValueCollection_t60741DBA4647D7259ECB16D50D6AC4FEE46A5B88 * get_values_8() const { return ___values_8; }
	inline ValueCollection_t60741DBA4647D7259ECB16D50D6AC4FEE46A5B88 ** get_address_of_values_8() { return &___values_8; }
	inline void set_values_8(ValueCollection_t60741DBA4647D7259ECB16D50D6AC4FEE46A5B88 * value)
	{
		___values_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___values_8), (void*)value);
	}

	inline static int32_t get_offset_of__syncRoot_9() { return static_cast<int32_t>(offsetof(Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753, ____syncRoot_9)); }
	inline RuntimeObject * get__syncRoot_9() const { return ____syncRoot_9; }
	inline RuntimeObject ** get_address_of__syncRoot_9() { return &____syncRoot_9; }
	inline void set__syncRoot_9(RuntimeObject * value)
	{
		____syncRoot_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_9), (void*)value);
	}
};


// System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>
struct Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C  : public RuntimeObject
{
public:
	// System.Int32[] System.Collections.Generic.Dictionary`2::buckets
	Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* ___buckets_0;
	// System.Collections.Generic.Dictionary`2/Entry<TKey,TValue>[] System.Collections.Generic.Dictionary`2::entries
	EntryU5BU5D_t7E283CE6C88BF1DF0DBCB399F07FA5BAFC6FF36A* ___entries_1;
	// System.Int32 System.Collections.Generic.Dictionary`2::count
	int32_t ___count_2;
	// System.Int32 System.Collections.Generic.Dictionary`2::version
	int32_t ___version_3;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeList
	int32_t ___freeList_4;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeCount
	int32_t ___freeCount_5;
	// System.Collections.Generic.IEqualityComparer`1<TKey> System.Collections.Generic.Dictionary`2::comparer
	RuntimeObject* ___comparer_6;
	// System.Collections.Generic.Dictionary`2/KeyCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::keys
	KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB * ___keys_7;
	// System.Collections.Generic.Dictionary`2/ValueCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::values
	ValueCollection_t4570917EBE5D4FD65BC458FAECE220E22C1A632A * ___values_8;
	// System.Object System.Collections.Generic.Dictionary`2::_syncRoot
	RuntimeObject * ____syncRoot_9;

public:
	inline static int32_t get_offset_of_buckets_0() { return static_cast<int32_t>(offsetof(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C, ___buckets_0)); }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* get_buckets_0() const { return ___buckets_0; }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83** get_address_of_buckets_0() { return &___buckets_0; }
	inline void set_buckets_0(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* value)
	{
		___buckets_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___buckets_0), (void*)value);
	}

	inline static int32_t get_offset_of_entries_1() { return static_cast<int32_t>(offsetof(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C, ___entries_1)); }
	inline EntryU5BU5D_t7E283CE6C88BF1DF0DBCB399F07FA5BAFC6FF36A* get_entries_1() const { return ___entries_1; }
	inline EntryU5BU5D_t7E283CE6C88BF1DF0DBCB399F07FA5BAFC6FF36A** get_address_of_entries_1() { return &___entries_1; }
	inline void set_entries_1(EntryU5BU5D_t7E283CE6C88BF1DF0DBCB399F07FA5BAFC6FF36A* value)
	{
		___entries_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___entries_1), (void*)value);
	}

	inline static int32_t get_offset_of_count_2() { return static_cast<int32_t>(offsetof(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C, ___count_2)); }
	inline int32_t get_count_2() const { return ___count_2; }
	inline int32_t* get_address_of_count_2() { return &___count_2; }
	inline void set_count_2(int32_t value)
	{
		___count_2 = value;
	}

	inline static int32_t get_offset_of_version_3() { return static_cast<int32_t>(offsetof(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C, ___version_3)); }
	inline int32_t get_version_3() const { return ___version_3; }
	inline int32_t* get_address_of_version_3() { return &___version_3; }
	inline void set_version_3(int32_t value)
	{
		___version_3 = value;
	}

	inline static int32_t get_offset_of_freeList_4() { return static_cast<int32_t>(offsetof(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C, ___freeList_4)); }
	inline int32_t get_freeList_4() const { return ___freeList_4; }
	inline int32_t* get_address_of_freeList_4() { return &___freeList_4; }
	inline void set_freeList_4(int32_t value)
	{
		___freeList_4 = value;
	}

	inline static int32_t get_offset_of_freeCount_5() { return static_cast<int32_t>(offsetof(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C, ___freeCount_5)); }
	inline int32_t get_freeCount_5() const { return ___freeCount_5; }
	inline int32_t* get_address_of_freeCount_5() { return &___freeCount_5; }
	inline void set_freeCount_5(int32_t value)
	{
		___freeCount_5 = value;
	}

	inline static int32_t get_offset_of_comparer_6() { return static_cast<int32_t>(offsetof(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C, ___comparer_6)); }
	inline RuntimeObject* get_comparer_6() const { return ___comparer_6; }
	inline RuntimeObject** get_address_of_comparer_6() { return &___comparer_6; }
	inline void set_comparer_6(RuntimeObject* value)
	{
		___comparer_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___comparer_6), (void*)value);
	}

	inline static int32_t get_offset_of_keys_7() { return static_cast<int32_t>(offsetof(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C, ___keys_7)); }
	inline KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB * get_keys_7() const { return ___keys_7; }
	inline KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB ** get_address_of_keys_7() { return &___keys_7; }
	inline void set_keys_7(KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB * value)
	{
		___keys_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___keys_7), (void*)value);
	}

	inline static int32_t get_offset_of_values_8() { return static_cast<int32_t>(offsetof(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C, ___values_8)); }
	inline ValueCollection_t4570917EBE5D4FD65BC458FAECE220E22C1A632A * get_values_8() const { return ___values_8; }
	inline ValueCollection_t4570917EBE5D4FD65BC458FAECE220E22C1A632A ** get_address_of_values_8() { return &___values_8; }
	inline void set_values_8(ValueCollection_t4570917EBE5D4FD65BC458FAECE220E22C1A632A * value)
	{
		___values_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___values_8), (void*)value);
	}

	inline static int32_t get_offset_of__syncRoot_9() { return static_cast<int32_t>(offsetof(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C, ____syncRoot_9)); }
	inline RuntimeObject * get__syncRoot_9() const { return ____syncRoot_9; }
	inline RuntimeObject ** get_address_of__syncRoot_9() { return &____syncRoot_9; }
	inline void set__syncRoot_9(RuntimeObject * value)
	{
		____syncRoot_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_9), (void*)value);
	}
};


// System.String
struct String_t  : public RuntimeObject
{
public:
	// System.Int32 System.String::m_stringLength
	int32_t ___m_stringLength_0;
	// System.Char System.String::m_firstChar
	Il2CppChar ___m_firstChar_1;

public:
	inline static int32_t get_offset_of_m_stringLength_0() { return static_cast<int32_t>(offsetof(String_t, ___m_stringLength_0)); }
	inline int32_t get_m_stringLength_0() const { return ___m_stringLength_0; }
	inline int32_t* get_address_of_m_stringLength_0() { return &___m_stringLength_0; }
	inline void set_m_stringLength_0(int32_t value)
	{
		___m_stringLength_0 = value;
	}

	inline static int32_t get_offset_of_m_firstChar_1() { return static_cast<int32_t>(offsetof(String_t, ___m_firstChar_1)); }
	inline Il2CppChar get_m_firstChar_1() const { return ___m_firstChar_1; }
	inline Il2CppChar* get_address_of_m_firstChar_1() { return &___m_firstChar_1; }
	inline void set_m_firstChar_1(Il2CppChar value)
	{
		___m_firstChar_1 = value;
	}
};

struct String_t_StaticFields
{
public:
	// System.String System.String::Empty
	String_t* ___Empty_5;

public:
	inline static int32_t get_offset_of_Empty_5() { return static_cast<int32_t>(offsetof(String_t_StaticFields, ___Empty_5)); }
	inline String_t* get_Empty_5() const { return ___Empty_5; }
	inline String_t** get_address_of_Empty_5() { return &___Empty_5; }
	inline void set_Empty_5(String_t* value)
	{
		___Empty_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Empty_5), (void*)value);
	}
};


// System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF  : public RuntimeObject
{
public:

public:
};

// Native definition for P/Invoke marshalling of System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF_marshaled_com
{
};

// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks
struct TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5  : public RuntimeObject
{
public:

public:
};

struct TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_StaticFields
{
public:
	// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/HeightmapChangedCallback UnityEngine.Experimental.TerrainAPI.TerrainCallbacks::heightmapChanged
	HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D * ___heightmapChanged_0;
	// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/TextureChangedCallback UnityEngine.Experimental.TerrainAPI.TerrainCallbacks::textureChanged
	TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E * ___textureChanged_1;

public:
	inline static int32_t get_offset_of_heightmapChanged_0() { return static_cast<int32_t>(offsetof(TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_StaticFields, ___heightmapChanged_0)); }
	inline HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D * get_heightmapChanged_0() const { return ___heightmapChanged_0; }
	inline HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D ** get_address_of_heightmapChanged_0() { return &___heightmapChanged_0; }
	inline void set_heightmapChanged_0(HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D * value)
	{
		___heightmapChanged_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___heightmapChanged_0), (void*)value);
	}

	inline static int32_t get_offset_of_textureChanged_1() { return static_cast<int32_t>(offsetof(TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_StaticFields, ___textureChanged_1)); }
	inline TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E * get_textureChanged_1() const { return ___textureChanged_1; }
	inline TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E ** get_address_of_textureChanged_1() { return &___textureChanged_1; }
	inline void set_textureChanged_1(TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E * value)
	{
		___textureChanged_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___textureChanged_1), (void*)value);
	}
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility
struct TerrainUtility_t82C295A06EAAEA3D755971EBE55084B993C7FC7B  : public RuntimeObject
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_0
struct U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A  : public RuntimeObject
{
public:
	// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_0::onlyAutoConnectedTerrains
	bool ___onlyAutoConnectedTerrains_0;

public:
	inline static int32_t get_offset_of_onlyAutoConnectedTerrains_0() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A, ___onlyAutoConnectedTerrains_0)); }
	inline bool get_onlyAutoConnectedTerrains_0() const { return ___onlyAutoConnectedTerrains_0; }
	inline bool* get_address_of_onlyAutoConnectedTerrains_0() { return &___onlyAutoConnectedTerrains_0; }
	inline void set_onlyAutoConnectedTerrains_0(bool value)
	{
		___onlyAutoConnectedTerrains_0 = value;
	}
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_1
struct U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407  : public RuntimeObject
{
public:
	// UnityEngine.Terrain UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_1::t
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___t_0;
	// UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_0 UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_1::CS$<>8__locals1
	U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A * ___CSU24U3CU3E8__locals1_1;

public:
	inline static int32_t get_offset_of_t_0() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407, ___t_0)); }
	inline Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * get_t_0() const { return ___t_0; }
	inline Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 ** get_address_of_t_0() { return &___t_0; }
	inline void set_t_0(Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * value)
	{
		___t_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___t_0), (void*)value);
	}

	inline static int32_t get_offset_of_CSU24U3CU3E8__locals1_1() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407, ___CSU24U3CU3E8__locals1_1)); }
	inline U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A * get_CSU24U3CU3E8__locals1_1() const { return ___CSU24U3CU3E8__locals1_1; }
	inline U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A ** get_address_of_CSU24U3CU3E8__locals1_1() { return &___CSU24U3CU3E8__locals1_1; }
	inline void set_CSU24U3CU3E8__locals1_1(U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A * value)
	{
		___CSU24U3CU3E8__locals1_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___CSU24U3CU3E8__locals1_1), (void*)value);
	}
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/<>c__DisplayClass4_0
struct U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002  : public RuntimeObject
{
public:
	// System.Int32 UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/<>c__DisplayClass4_0::groupID
	int32_t ___groupID_0;

public:
	inline static int32_t get_offset_of_groupID_0() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002, ___groupID_0)); }
	inline int32_t get_groupID_0() const { return ___groupID_0; }
	inline int32_t* get_address_of_groupID_0() { return &___groupID_0; }
	inline void set_groupID_0(int32_t value)
	{
		___groupID_0 = value;
	}
};


// System.Boolean
struct Boolean_tB53F6830F670160873277339AA58F15CAED4399C 
{
public:
	// System.Boolean System.Boolean::m_value
	bool ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Boolean_tB53F6830F670160873277339AA58F15CAED4399C, ___m_value_0)); }
	inline bool get_m_value_0() const { return ___m_value_0; }
	inline bool* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(bool value)
	{
		___m_value_0 = value;
	}
};

struct Boolean_tB53F6830F670160873277339AA58F15CAED4399C_StaticFields
{
public:
	// System.String System.Boolean::TrueString
	String_t* ___TrueString_5;
	// System.String System.Boolean::FalseString
	String_t* ___FalseString_6;

public:
	inline static int32_t get_offset_of_TrueString_5() { return static_cast<int32_t>(offsetof(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_StaticFields, ___TrueString_5)); }
	inline String_t* get_TrueString_5() const { return ___TrueString_5; }
	inline String_t** get_address_of_TrueString_5() { return &___TrueString_5; }
	inline void set_TrueString_5(String_t* value)
	{
		___TrueString_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___TrueString_5), (void*)value);
	}

	inline static int32_t get_offset_of_FalseString_6() { return static_cast<int32_t>(offsetof(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_StaticFields, ___FalseString_6)); }
	inline String_t* get_FalseString_6() const { return ___FalseString_6; }
	inline String_t** get_address_of_FalseString_6() { return &___FalseString_6; }
	inline void set_FalseString_6(String_t* value)
	{
		___FalseString_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___FalseString_6), (void*)value);
	}
};


// System.Collections.Generic.KeyValuePair`2<System.Int32,System.Object>
struct KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C 
{
public:
	// TKey System.Collections.Generic.KeyValuePair`2::key
	int32_t ___key_0;
	// TValue System.Collections.Generic.KeyValuePair`2::value
	RuntimeObject * ___value_1;

public:
	inline static int32_t get_offset_of_key_0() { return static_cast<int32_t>(offsetof(KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C, ___key_0)); }
	inline int32_t get_key_0() const { return ___key_0; }
	inline int32_t* get_address_of_key_0() { return &___key_0; }
	inline void set_key_0(int32_t value)
	{
		___key_0 = value;
	}

	inline static int32_t get_offset_of_value_1() { return static_cast<int32_t>(offsetof(KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C, ___value_1)); }
	inline RuntimeObject * get_value_1() const { return ___value_1; }
	inline RuntimeObject ** get_address_of_value_1() { return &___value_1; }
	inline void set_value_1(RuntimeObject * value)
	{
		___value_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___value_1), (void*)value);
	}
};


// System.Collections.Generic.KeyValuePair`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>
struct KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574 
{
public:
	// TKey System.Collections.Generic.KeyValuePair`2::key
	int32_t ___key_0;
	// TValue System.Collections.Generic.KeyValuePair`2::value
	TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * ___value_1;

public:
	inline static int32_t get_offset_of_key_0() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574, ___key_0)); }
	inline int32_t get_key_0() const { return ___key_0; }
	inline int32_t* get_address_of_key_0() { return &___key_0; }
	inline void set_key_0(int32_t value)
	{
		___key_0 = value;
	}

	inline static int32_t get_offset_of_value_1() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574, ___value_1)); }
	inline TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * get_value_1() const { return ___value_1; }
	inline TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C ** get_address_of_value_1() { return &___value_1; }
	inline void set_value_1(TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * value)
	{
		___value_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___value_1), (void*)value);
	}
};


// System.Enum
struct Enum_t2AF27C02B8653AE29442467390005ABC74D8F521  : public ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF
{
public:

public:
};

struct Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_StaticFields
{
public:
	// System.Char[] System.Enum::enumSeperatorCharArray
	CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* ___enumSeperatorCharArray_0;

public:
	inline static int32_t get_offset_of_enumSeperatorCharArray_0() { return static_cast<int32_t>(offsetof(Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_StaticFields, ___enumSeperatorCharArray_0)); }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* get_enumSeperatorCharArray_0() const { return ___enumSeperatorCharArray_0; }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2** get_address_of_enumSeperatorCharArray_0() { return &___enumSeperatorCharArray_0; }
	inline void set_enumSeperatorCharArray_0(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* value)
	{
		___enumSeperatorCharArray_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumSeperatorCharArray_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.Enum
struct Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.Enum
struct Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_marshaled_com
{
};

// System.Int32
struct Int32_t585191389E07734F19F3156FF88FB3EF4800D102 
{
public:
	// System.Int32 System.Int32::m_value
	int32_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Int32_t585191389E07734F19F3156FF88FB3EF4800D102, ___m_value_0)); }
	inline int32_t get_m_value_0() const { return ___m_value_0; }
	inline int32_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int32_t value)
	{
		___m_value_0 = value;
	}
};


// System.IntPtr
struct IntPtr_t 
{
public:
	// System.Void* System.IntPtr::m_value
	void* ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(IntPtr_t, ___m_value_0)); }
	inline void* get_m_value_0() const { return ___m_value_0; }
	inline void** get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(void* value)
	{
		___m_value_0 = value;
	}
};

struct IntPtr_t_StaticFields
{
public:
	// System.IntPtr System.IntPtr::Zero
	intptr_t ___Zero_1;

public:
	inline static int32_t get_offset_of_Zero_1() { return static_cast<int32_t>(offsetof(IntPtr_t_StaticFields, ___Zero_1)); }
	inline intptr_t get_Zero_1() const { return ___Zero_1; }
	inline intptr_t* get_address_of_Zero_1() { return &___Zero_1; }
	inline void set_Zero_1(intptr_t value)
	{
		___Zero_1 = value;
	}
};


// System.Single
struct Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1 
{
public:
	// System.Single System.Single::m_value
	float ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1, ___m_value_0)); }
	inline float get_m_value_0() const { return ___m_value_0; }
	inline float* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(float value)
	{
		___m_value_0 = value;
	}
};


// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017 
{
public:
	union
	{
		struct
		{
		};
		uint8_t Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017__padding[1];
	};

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainGroups
struct TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5  : public Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord
struct TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA 
{
public:
	// System.Int32 UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord::tileX
	int32_t ___tileX_0;
	// System.Int32 UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord::tileZ
	int32_t ___tileZ_1;

public:
	inline static int32_t get_offset_of_tileX_0() { return static_cast<int32_t>(offsetof(TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA, ___tileX_0)); }
	inline int32_t get_tileX_0() const { return ___tileX_0; }
	inline int32_t* get_address_of_tileX_0() { return &___tileX_0; }
	inline void set_tileX_0(int32_t value)
	{
		___tileX_0 = value;
	}

	inline static int32_t get_offset_of_tileZ_1() { return static_cast<int32_t>(offsetof(TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA, ___tileZ_1)); }
	inline int32_t get_tileZ_1() const { return ___tileZ_1; }
	inline int32_t* get_address_of_tileZ_1() { return &___tileZ_1; }
	inline void set_tileZ_1(int32_t value)
	{
		___tileZ_1 = value;
	}
};


// UnityEngine.RectInt
struct RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A 
{
public:
	// System.Int32 UnityEngine.RectInt::m_XMin
	int32_t ___m_XMin_0;
	// System.Int32 UnityEngine.RectInt::m_YMin
	int32_t ___m_YMin_1;
	// System.Int32 UnityEngine.RectInt::m_Width
	int32_t ___m_Width_2;
	// System.Int32 UnityEngine.RectInt::m_Height
	int32_t ___m_Height_3;

public:
	inline static int32_t get_offset_of_m_XMin_0() { return static_cast<int32_t>(offsetof(RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A, ___m_XMin_0)); }
	inline int32_t get_m_XMin_0() const { return ___m_XMin_0; }
	inline int32_t* get_address_of_m_XMin_0() { return &___m_XMin_0; }
	inline void set_m_XMin_0(int32_t value)
	{
		___m_XMin_0 = value;
	}

	inline static int32_t get_offset_of_m_YMin_1() { return static_cast<int32_t>(offsetof(RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A, ___m_YMin_1)); }
	inline int32_t get_m_YMin_1() const { return ___m_YMin_1; }
	inline int32_t* get_address_of_m_YMin_1() { return &___m_YMin_1; }
	inline void set_m_YMin_1(int32_t value)
	{
		___m_YMin_1 = value;
	}

	inline static int32_t get_offset_of_m_Width_2() { return static_cast<int32_t>(offsetof(RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A, ___m_Width_2)); }
	inline int32_t get_m_Width_2() const { return ___m_Width_2; }
	inline int32_t* get_address_of_m_Width_2() { return &___m_Width_2; }
	inline void set_m_Width_2(int32_t value)
	{
		___m_Width_2 = value;
	}

	inline static int32_t get_offset_of_m_Height_3() { return static_cast<int32_t>(offsetof(RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A, ___m_Height_3)); }
	inline int32_t get_m_Height_3() const { return ___m_Height_3; }
	inline int32_t* get_address_of_m_Height_3() { return &___m_Height_3; }
	inline void set_m_Height_3(int32_t value)
	{
		___m_Height_3 = value;
	}
};


// UnityEngine.Vector2
struct Vector2_tA85D2DD88578276CA8A8796756458277E72D073D 
{
public:
	// System.Single UnityEngine.Vector2::x
	float ___x_0;
	// System.Single UnityEngine.Vector2::y
	float ___y_1;

public:
	inline static int32_t get_offset_of_x_0() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D, ___x_0)); }
	inline float get_x_0() const { return ___x_0; }
	inline float* get_address_of_x_0() { return &___x_0; }
	inline void set_x_0(float value)
	{
		___x_0 = value;
	}

	inline static int32_t get_offset_of_y_1() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D, ___y_1)); }
	inline float get_y_1() const { return ___y_1; }
	inline float* get_address_of_y_1() { return &___y_1; }
	inline void set_y_1(float value)
	{
		___y_1 = value;
	}
};

struct Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields
{
public:
	// UnityEngine.Vector2 UnityEngine.Vector2::zeroVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___zeroVector_2;
	// UnityEngine.Vector2 UnityEngine.Vector2::oneVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___oneVector_3;
	// UnityEngine.Vector2 UnityEngine.Vector2::upVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___upVector_4;
	// UnityEngine.Vector2 UnityEngine.Vector2::downVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___downVector_5;
	// UnityEngine.Vector2 UnityEngine.Vector2::leftVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___leftVector_6;
	// UnityEngine.Vector2 UnityEngine.Vector2::rightVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___rightVector_7;
	// UnityEngine.Vector2 UnityEngine.Vector2::positiveInfinityVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___positiveInfinityVector_8;
	// UnityEngine.Vector2 UnityEngine.Vector2::negativeInfinityVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___negativeInfinityVector_9;

public:
	inline static int32_t get_offset_of_zeroVector_2() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___zeroVector_2)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_zeroVector_2() const { return ___zeroVector_2; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_zeroVector_2() { return &___zeroVector_2; }
	inline void set_zeroVector_2(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___zeroVector_2 = value;
	}

	inline static int32_t get_offset_of_oneVector_3() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___oneVector_3)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_oneVector_3() const { return ___oneVector_3; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_oneVector_3() { return &___oneVector_3; }
	inline void set_oneVector_3(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___oneVector_3 = value;
	}

	inline static int32_t get_offset_of_upVector_4() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___upVector_4)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_upVector_4() const { return ___upVector_4; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_upVector_4() { return &___upVector_4; }
	inline void set_upVector_4(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___upVector_4 = value;
	}

	inline static int32_t get_offset_of_downVector_5() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___downVector_5)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_downVector_5() const { return ___downVector_5; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_downVector_5() { return &___downVector_5; }
	inline void set_downVector_5(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___downVector_5 = value;
	}

	inline static int32_t get_offset_of_leftVector_6() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___leftVector_6)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_leftVector_6() const { return ___leftVector_6; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_leftVector_6() { return &___leftVector_6; }
	inline void set_leftVector_6(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___leftVector_6 = value;
	}

	inline static int32_t get_offset_of_rightVector_7() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___rightVector_7)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_rightVector_7() const { return ___rightVector_7; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_rightVector_7() { return &___rightVector_7; }
	inline void set_rightVector_7(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___rightVector_7 = value;
	}

	inline static int32_t get_offset_of_positiveInfinityVector_8() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___positiveInfinityVector_8)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_positiveInfinityVector_8() const { return ___positiveInfinityVector_8; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_positiveInfinityVector_8() { return &___positiveInfinityVector_8; }
	inline void set_positiveInfinityVector_8(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___positiveInfinityVector_8 = value;
	}

	inline static int32_t get_offset_of_negativeInfinityVector_9() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___negativeInfinityVector_9)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_negativeInfinityVector_9() const { return ___negativeInfinityVector_9; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_negativeInfinityVector_9() { return &___negativeInfinityVector_9; }
	inline void set_negativeInfinityVector_9(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___negativeInfinityVector_9 = value;
	}
};


// UnityEngine.Vector3
struct Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 
{
public:
	// System.Single UnityEngine.Vector3::x
	float ___x_2;
	// System.Single UnityEngine.Vector3::y
	float ___y_3;
	// System.Single UnityEngine.Vector3::z
	float ___z_4;

public:
	inline static int32_t get_offset_of_x_2() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___x_2)); }
	inline float get_x_2() const { return ___x_2; }
	inline float* get_address_of_x_2() { return &___x_2; }
	inline void set_x_2(float value)
	{
		___x_2 = value;
	}

	inline static int32_t get_offset_of_y_3() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___y_3)); }
	inline float get_y_3() const { return ___y_3; }
	inline float* get_address_of_y_3() { return &___y_3; }
	inline void set_y_3(float value)
	{
		___y_3 = value;
	}

	inline static int32_t get_offset_of_z_4() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___z_4)); }
	inline float get_z_4() const { return ___z_4; }
	inline float* get_address_of_z_4() { return &___z_4; }
	inline void set_z_4(float value)
	{
		___z_4 = value;
	}
};

struct Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields
{
public:
	// UnityEngine.Vector3 UnityEngine.Vector3::zeroVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___zeroVector_5;
	// UnityEngine.Vector3 UnityEngine.Vector3::oneVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___oneVector_6;
	// UnityEngine.Vector3 UnityEngine.Vector3::upVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___upVector_7;
	// UnityEngine.Vector3 UnityEngine.Vector3::downVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___downVector_8;
	// UnityEngine.Vector3 UnityEngine.Vector3::leftVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___leftVector_9;
	// UnityEngine.Vector3 UnityEngine.Vector3::rightVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___rightVector_10;
	// UnityEngine.Vector3 UnityEngine.Vector3::forwardVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___forwardVector_11;
	// UnityEngine.Vector3 UnityEngine.Vector3::backVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___backVector_12;
	// UnityEngine.Vector3 UnityEngine.Vector3::positiveInfinityVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___positiveInfinityVector_13;
	// UnityEngine.Vector3 UnityEngine.Vector3::negativeInfinityVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___negativeInfinityVector_14;

public:
	inline static int32_t get_offset_of_zeroVector_5() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___zeroVector_5)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_zeroVector_5() const { return ___zeroVector_5; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_zeroVector_5() { return &___zeroVector_5; }
	inline void set_zeroVector_5(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___zeroVector_5 = value;
	}

	inline static int32_t get_offset_of_oneVector_6() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___oneVector_6)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_oneVector_6() const { return ___oneVector_6; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_oneVector_6() { return &___oneVector_6; }
	inline void set_oneVector_6(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___oneVector_6 = value;
	}

	inline static int32_t get_offset_of_upVector_7() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___upVector_7)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_upVector_7() const { return ___upVector_7; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_upVector_7() { return &___upVector_7; }
	inline void set_upVector_7(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___upVector_7 = value;
	}

	inline static int32_t get_offset_of_downVector_8() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___downVector_8)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_downVector_8() const { return ___downVector_8; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_downVector_8() { return &___downVector_8; }
	inline void set_downVector_8(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___downVector_8 = value;
	}

	inline static int32_t get_offset_of_leftVector_9() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___leftVector_9)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_leftVector_9() const { return ___leftVector_9; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_leftVector_9() { return &___leftVector_9; }
	inline void set_leftVector_9(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___leftVector_9 = value;
	}

	inline static int32_t get_offset_of_rightVector_10() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___rightVector_10)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_rightVector_10() const { return ___rightVector_10; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_rightVector_10() { return &___rightVector_10; }
	inline void set_rightVector_10(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___rightVector_10 = value;
	}

	inline static int32_t get_offset_of_forwardVector_11() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___forwardVector_11)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_forwardVector_11() const { return ___forwardVector_11; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_forwardVector_11() { return &___forwardVector_11; }
	inline void set_forwardVector_11(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___forwardVector_11 = value;
	}

	inline static int32_t get_offset_of_backVector_12() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___backVector_12)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_backVector_12() const { return ___backVector_12; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_backVector_12() { return &___backVector_12; }
	inline void set_backVector_12(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___backVector_12 = value;
	}

	inline static int32_t get_offset_of_positiveInfinityVector_13() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___positiveInfinityVector_13)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_positiveInfinityVector_13() const { return ___positiveInfinityVector_13; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_positiveInfinityVector_13() { return &___positiveInfinityVector_13; }
	inline void set_positiveInfinityVector_13(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___positiveInfinityVector_13 = value;
	}

	inline static int32_t get_offset_of_negativeInfinityVector_14() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___negativeInfinityVector_14)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_negativeInfinityVector_14() const { return ___negativeInfinityVector_14; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_negativeInfinityVector_14() { return &___negativeInfinityVector_14; }
	inline void set_negativeInfinityVector_14(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___negativeInfinityVector_14 = value;
	}
};


// System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,System.Object>
struct Enumerator_tC0410598D958D8FA61F7F1A803A0B7F242CEF1C6 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2/Enumerator::dictionary
	Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884 * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::version
	int32_t ___version_1;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::index
	int32_t ___index_2;
	// System.Collections.Generic.KeyValuePair`2<TKey,TValue> System.Collections.Generic.Dictionary`2/Enumerator::current
	KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C  ___current_3;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::getEnumeratorRetType
	int32_t ___getEnumeratorRetType_4;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_tC0410598D958D8FA61F7F1A803A0B7F242CEF1C6, ___dictionary_0)); }
	inline Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884 * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884 ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884 * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_version_1() { return static_cast<int32_t>(offsetof(Enumerator_tC0410598D958D8FA61F7F1A803A0B7F242CEF1C6, ___version_1)); }
	inline int32_t get_version_1() const { return ___version_1; }
	inline int32_t* get_address_of_version_1() { return &___version_1; }
	inline void set_version_1(int32_t value)
	{
		___version_1 = value;
	}

	inline static int32_t get_offset_of_index_2() { return static_cast<int32_t>(offsetof(Enumerator_tC0410598D958D8FA61F7F1A803A0B7F242CEF1C6, ___index_2)); }
	inline int32_t get_index_2() const { return ___index_2; }
	inline int32_t* get_address_of_index_2() { return &___index_2; }
	inline void set_index_2(int32_t value)
	{
		___index_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_tC0410598D958D8FA61F7F1A803A0B7F242CEF1C6, ___current_3)); }
	inline KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C  get_current_3() const { return ___current_3; }
	inline KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C * get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C  value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___current_3))->___value_1), (void*)NULL);
	}

	inline static int32_t get_offset_of_getEnumeratorRetType_4() { return static_cast<int32_t>(offsetof(Enumerator_tC0410598D958D8FA61F7F1A803A0B7F242CEF1C6, ___getEnumeratorRetType_4)); }
	inline int32_t get_getEnumeratorRetType_4() const { return ___getEnumeratorRetType_4; }
	inline int32_t* get_address_of_getEnumeratorRetType_4() { return &___getEnumeratorRetType_4; }
	inline void set_getEnumeratorRetType_4(int32_t value)
	{
		___getEnumeratorRetType_4 = value;
	}
};


// System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>
struct Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2/Enumerator::dictionary
	Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::version
	int32_t ___version_1;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::index
	int32_t ___index_2;
	// System.Collections.Generic.KeyValuePair`2<TKey,TValue> System.Collections.Generic.Dictionary`2/Enumerator::current
	KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574  ___current_3;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::getEnumeratorRetType
	int32_t ___getEnumeratorRetType_4;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B, ___dictionary_0)); }
	inline Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_version_1() { return static_cast<int32_t>(offsetof(Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B, ___version_1)); }
	inline int32_t get_version_1() const { return ___version_1; }
	inline int32_t* get_address_of_version_1() { return &___version_1; }
	inline void set_version_1(int32_t value)
	{
		___version_1 = value;
	}

	inline static int32_t get_offset_of_index_2() { return static_cast<int32_t>(offsetof(Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B, ___index_2)); }
	inline int32_t get_index_2() const { return ___index_2; }
	inline int32_t* get_address_of_index_2() { return &___index_2; }
	inline void set_index_2(int32_t value)
	{
		___index_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B, ___current_3)); }
	inline KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574  get_current_3() const { return ___current_3; }
	inline KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574 * get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574  value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___current_3))->___value_1), (void*)NULL);
	}

	inline static int32_t get_offset_of_getEnumeratorRetType_4() { return static_cast<int32_t>(offsetof(Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B, ___getEnumeratorRetType_4)); }
	inline int32_t get_getEnumeratorRetType_4() const { return ___getEnumeratorRetType_4; }
	inline int32_t* get_address_of_getEnumeratorRetType_4() { return &___getEnumeratorRetType_4; }
	inline void set_getEnumeratorRetType_4(int32_t value)
	{
		___getEnumeratorRetType_4 = value;
	}
};


// System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>
struct Enumerator_tEC4F5C6184F5E671D53C872D88266C1500841E16 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator::dictionary
	Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator::version
	int32_t ___version_2;
	// TKey System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator::currentKey
	TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  ___currentKey_3;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_tEC4F5C6184F5E671D53C872D88266C1500841E16, ___dictionary_0)); }
	inline Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_tEC4F5C6184F5E671D53C872D88266C1500841E16, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_tEC4F5C6184F5E671D53C872D88266C1500841E16, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_currentKey_3() { return static_cast<int32_t>(offsetof(Enumerator_tEC4F5C6184F5E671D53C872D88266C1500841E16, ___currentKey_3)); }
	inline TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  get_currentKey_3() const { return ___currentKey_3; }
	inline TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA * get_address_of_currentKey_3() { return &___currentKey_3; }
	inline void set_currentKey_3(TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  value)
	{
		___currentKey_3 = value;
	}
};


// System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>
struct Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator::dictionary
	Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator::index
	int32_t ___index_1;
	// System.Int32 System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator::version
	int32_t ___version_2;
	// TKey System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator::currentKey
	TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  ___currentKey_3;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639, ___dictionary_0)); }
	inline Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}

	inline static int32_t get_offset_of_version_2() { return static_cast<int32_t>(offsetof(Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639, ___version_2)); }
	inline int32_t get_version_2() const { return ___version_2; }
	inline int32_t* get_address_of_version_2() { return &___version_2; }
	inline void set_version_2(int32_t value)
	{
		___version_2 = value;
	}

	inline static int32_t get_offset_of_currentKey_3() { return static_cast<int32_t>(offsetof(Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639, ___currentKey_3)); }
	inline TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  get_currentKey_3() const { return ___currentKey_3; }
	inline TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA * get_address_of_currentKey_3() { return &___currentKey_3; }
	inline void set_currentKey_3(TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  value)
	{
		___currentKey_3 = value;
	}
};


// System.Collections.Generic.KeyValuePair`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>
struct KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342 
{
public:
	// TKey System.Collections.Generic.KeyValuePair`2::key
	TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  ___key_0;
	// TValue System.Collections.Generic.KeyValuePair`2::value
	RuntimeObject * ___value_1;

public:
	inline static int32_t get_offset_of_key_0() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342, ___key_0)); }
	inline TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  get_key_0() const { return ___key_0; }
	inline TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA * get_address_of_key_0() { return &___key_0; }
	inline void set_key_0(TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  value)
	{
		___key_0 = value;
	}

	inline static int32_t get_offset_of_value_1() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342, ___value_1)); }
	inline RuntimeObject * get_value_1() const { return ___value_1; }
	inline RuntimeObject ** get_address_of_value_1() { return &___value_1; }
	inline void set_value_1(RuntimeObject * value)
	{
		___value_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___value_1), (void*)value);
	}
};


// System.Collections.Generic.KeyValuePair`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>
struct KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2 
{
public:
	// TKey System.Collections.Generic.KeyValuePair`2::key
	TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  ___key_0;
	// TValue System.Collections.Generic.KeyValuePair`2::value
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___value_1;

public:
	inline static int32_t get_offset_of_key_0() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2, ___key_0)); }
	inline TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  get_key_0() const { return ___key_0; }
	inline TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA * get_address_of_key_0() { return &___key_0; }
	inline void set_key_0(TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  value)
	{
		___key_0 = value;
	}

	inline static int32_t get_offset_of_value_1() { return static_cast<int32_t>(offsetof(KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2, ___value_1)); }
	inline Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * get_value_1() const { return ___value_1; }
	inline Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 ** get_address_of_value_1() { return &___value_1; }
	inline void set_value_1(Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * value)
	{
		___value_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___value_1), (void*)value);
	}
};


// System.Delegate
struct Delegate_t  : public RuntimeObject
{
public:
	// System.IntPtr System.Delegate::method_ptr
	Il2CppMethodPointer ___method_ptr_0;
	// System.IntPtr System.Delegate::invoke_impl
	intptr_t ___invoke_impl_1;
	// System.Object System.Delegate::m_target
	RuntimeObject * ___m_target_2;
	// System.IntPtr System.Delegate::method
	intptr_t ___method_3;
	// System.IntPtr System.Delegate::delegate_trampoline
	intptr_t ___delegate_trampoline_4;
	// System.IntPtr System.Delegate::extra_arg
	intptr_t ___extra_arg_5;
	// System.IntPtr System.Delegate::method_code
	intptr_t ___method_code_6;
	// System.Reflection.MethodInfo System.Delegate::method_info
	MethodInfo_t * ___method_info_7;
	// System.Reflection.MethodInfo System.Delegate::original_method_info
	MethodInfo_t * ___original_method_info_8;
	// System.DelegateData System.Delegate::data
	DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * ___data_9;
	// System.Boolean System.Delegate::method_is_virtual
	bool ___method_is_virtual_10;

public:
	inline static int32_t get_offset_of_method_ptr_0() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_ptr_0)); }
	inline Il2CppMethodPointer get_method_ptr_0() const { return ___method_ptr_0; }
	inline Il2CppMethodPointer* get_address_of_method_ptr_0() { return &___method_ptr_0; }
	inline void set_method_ptr_0(Il2CppMethodPointer value)
	{
		___method_ptr_0 = value;
	}

	inline static int32_t get_offset_of_invoke_impl_1() { return static_cast<int32_t>(offsetof(Delegate_t, ___invoke_impl_1)); }
	inline intptr_t get_invoke_impl_1() const { return ___invoke_impl_1; }
	inline intptr_t* get_address_of_invoke_impl_1() { return &___invoke_impl_1; }
	inline void set_invoke_impl_1(intptr_t value)
	{
		___invoke_impl_1 = value;
	}

	inline static int32_t get_offset_of_m_target_2() { return static_cast<int32_t>(offsetof(Delegate_t, ___m_target_2)); }
	inline RuntimeObject * get_m_target_2() const { return ___m_target_2; }
	inline RuntimeObject ** get_address_of_m_target_2() { return &___m_target_2; }
	inline void set_m_target_2(RuntimeObject * value)
	{
		___m_target_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_target_2), (void*)value);
	}

	inline static int32_t get_offset_of_method_3() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_3)); }
	inline intptr_t get_method_3() const { return ___method_3; }
	inline intptr_t* get_address_of_method_3() { return &___method_3; }
	inline void set_method_3(intptr_t value)
	{
		___method_3 = value;
	}

	inline static int32_t get_offset_of_delegate_trampoline_4() { return static_cast<int32_t>(offsetof(Delegate_t, ___delegate_trampoline_4)); }
	inline intptr_t get_delegate_trampoline_4() const { return ___delegate_trampoline_4; }
	inline intptr_t* get_address_of_delegate_trampoline_4() { return &___delegate_trampoline_4; }
	inline void set_delegate_trampoline_4(intptr_t value)
	{
		___delegate_trampoline_4 = value;
	}

	inline static int32_t get_offset_of_extra_arg_5() { return static_cast<int32_t>(offsetof(Delegate_t, ___extra_arg_5)); }
	inline intptr_t get_extra_arg_5() const { return ___extra_arg_5; }
	inline intptr_t* get_address_of_extra_arg_5() { return &___extra_arg_5; }
	inline void set_extra_arg_5(intptr_t value)
	{
		___extra_arg_5 = value;
	}

	inline static int32_t get_offset_of_method_code_6() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_code_6)); }
	inline intptr_t get_method_code_6() const { return ___method_code_6; }
	inline intptr_t* get_address_of_method_code_6() { return &___method_code_6; }
	inline void set_method_code_6(intptr_t value)
	{
		___method_code_6 = value;
	}

	inline static int32_t get_offset_of_method_info_7() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_info_7)); }
	inline MethodInfo_t * get_method_info_7() const { return ___method_info_7; }
	inline MethodInfo_t ** get_address_of_method_info_7() { return &___method_info_7; }
	inline void set_method_info_7(MethodInfo_t * value)
	{
		___method_info_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___method_info_7), (void*)value);
	}

	inline static int32_t get_offset_of_original_method_info_8() { return static_cast<int32_t>(offsetof(Delegate_t, ___original_method_info_8)); }
	inline MethodInfo_t * get_original_method_info_8() const { return ___original_method_info_8; }
	inline MethodInfo_t ** get_address_of_original_method_info_8() { return &___original_method_info_8; }
	inline void set_original_method_info_8(MethodInfo_t * value)
	{
		___original_method_info_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___original_method_info_8), (void*)value);
	}

	inline static int32_t get_offset_of_data_9() { return static_cast<int32_t>(offsetof(Delegate_t, ___data_9)); }
	inline DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * get_data_9() const { return ___data_9; }
	inline DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE ** get_address_of_data_9() { return &___data_9; }
	inline void set_data_9(DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * value)
	{
		___data_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___data_9), (void*)value);
	}

	inline static int32_t get_offset_of_method_is_virtual_10() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_is_virtual_10)); }
	inline bool get_method_is_virtual_10() const { return ___method_is_virtual_10; }
	inline bool* get_address_of_method_is_virtual_10() { return &___method_is_virtual_10; }
	inline void set_method_is_virtual_10(bool value)
	{
		___method_is_virtual_10 = value;
	}
};

// Native definition for P/Invoke marshalling of System.Delegate
struct Delegate_t_marshaled_pinvoke
{
	intptr_t ___method_ptr_0;
	intptr_t ___invoke_impl_1;
	Il2CppIUnknown* ___m_target_2;
	intptr_t ___method_3;
	intptr_t ___delegate_trampoline_4;
	intptr_t ___extra_arg_5;
	intptr_t ___method_code_6;
	MethodInfo_t * ___method_info_7;
	MethodInfo_t * ___original_method_info_8;
	DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * ___data_9;
	int32_t ___method_is_virtual_10;
};
// Native definition for COM marshalling of System.Delegate
struct Delegate_t_marshaled_com
{
	intptr_t ___method_ptr_0;
	intptr_t ___invoke_impl_1;
	Il2CppIUnknown* ___m_target_2;
	intptr_t ___method_3;
	intptr_t ___delegate_trampoline_4;
	intptr_t ___extra_arg_5;
	intptr_t ___method_code_6;
	MethodInfo_t * ___method_info_7;
	MethodInfo_t * ___original_method_info_8;
	DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * ___data_9;
	int32_t ___method_is_virtual_10;
};

// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/ErrorCode
struct ErrorCode_tCC2BF2B1CF6C6645A76C1DEE65D4AA4A527FEC7A 
{
public:
	// System.Int32 UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/ErrorCode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(ErrorCode_tCC2BF2B1CF6C6645A76C1DEE65D4AA4A527FEC7A, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Object
struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0  : public RuntimeObject
{
public:
	// System.IntPtr UnityEngine.Object::m_CachedPtr
	intptr_t ___m_CachedPtr_0;

public:
	inline static int32_t get_offset_of_m_CachedPtr_0() { return static_cast<int32_t>(offsetof(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0, ___m_CachedPtr_0)); }
	inline intptr_t get_m_CachedPtr_0() const { return ___m_CachedPtr_0; }
	inline intptr_t* get_address_of_m_CachedPtr_0() { return &___m_CachedPtr_0; }
	inline void set_m_CachedPtr_0(intptr_t value)
	{
		___m_CachedPtr_0 = value;
	}
};

struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_StaticFields
{
public:
	// System.Int32 UnityEngine.Object::OffsetOfInstanceIDInCPlusPlusObject
	int32_t ___OffsetOfInstanceIDInCPlusPlusObject_1;

public:
	inline static int32_t get_offset_of_OffsetOfInstanceIDInCPlusPlusObject_1() { return static_cast<int32_t>(offsetof(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_StaticFields, ___OffsetOfInstanceIDInCPlusPlusObject_1)); }
	inline int32_t get_OffsetOfInstanceIDInCPlusPlusObject_1() const { return ___OffsetOfInstanceIDInCPlusPlusObject_1; }
	inline int32_t* get_address_of_OffsetOfInstanceIDInCPlusPlusObject_1() { return &___OffsetOfInstanceIDInCPlusPlusObject_1; }
	inline void set_OffsetOfInstanceIDInCPlusPlusObject_1(int32_t value)
	{
		___OffsetOfInstanceIDInCPlusPlusObject_1 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.Object
struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_marshaled_pinvoke
{
	intptr_t ___m_CachedPtr_0;
};
// Native definition for COM marshalling of UnityEngine.Object
struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_marshaled_com
{
	intptr_t ___m_CachedPtr_0;
};

// UnityEngine.TerrainData/BoundaryValueType
struct BoundaryValueType_tAD21AC67F7D3AC4FF445A0D6002E7914F07000EA 
{
public:
	// System.Int32 UnityEngine.TerrainData/BoundaryValueType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(BoundaryValueType_tAD21AC67F7D3AC4FF445A0D6002E7914F07000EA, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>
struct Enumerator_t71B93BE98D0EF45AF866DB4944B21CC1D0957B69 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2/Enumerator::dictionary
	Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::version
	int32_t ___version_1;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::index
	int32_t ___index_2;
	// System.Collections.Generic.KeyValuePair`2<TKey,TValue> System.Collections.Generic.Dictionary`2/Enumerator::current
	KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342  ___current_3;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::getEnumeratorRetType
	int32_t ___getEnumeratorRetType_4;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_t71B93BE98D0EF45AF866DB4944B21CC1D0957B69, ___dictionary_0)); }
	inline Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_version_1() { return static_cast<int32_t>(offsetof(Enumerator_t71B93BE98D0EF45AF866DB4944B21CC1D0957B69, ___version_1)); }
	inline int32_t get_version_1() const { return ___version_1; }
	inline int32_t* get_address_of_version_1() { return &___version_1; }
	inline void set_version_1(int32_t value)
	{
		___version_1 = value;
	}

	inline static int32_t get_offset_of_index_2() { return static_cast<int32_t>(offsetof(Enumerator_t71B93BE98D0EF45AF866DB4944B21CC1D0957B69, ___index_2)); }
	inline int32_t get_index_2() const { return ___index_2; }
	inline int32_t* get_address_of_index_2() { return &___index_2; }
	inline void set_index_2(int32_t value)
	{
		___index_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_t71B93BE98D0EF45AF866DB4944B21CC1D0957B69, ___current_3)); }
	inline KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342  get_current_3() const { return ___current_3; }
	inline KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342 * get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342  value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___current_3))->___value_1), (void*)NULL);
	}

	inline static int32_t get_offset_of_getEnumeratorRetType_4() { return static_cast<int32_t>(offsetof(Enumerator_t71B93BE98D0EF45AF866DB4944B21CC1D0957B69, ___getEnumeratorRetType_4)); }
	inline int32_t get_getEnumeratorRetType_4() const { return ___getEnumeratorRetType_4; }
	inline int32_t* get_address_of_getEnumeratorRetType_4() { return &___getEnumeratorRetType_4; }
	inline void set_getEnumeratorRetType_4(int32_t value)
	{
		___getEnumeratorRetType_4 = value;
	}
};


// System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>
struct Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8 
{
public:
	// System.Collections.Generic.Dictionary`2<TKey,TValue> System.Collections.Generic.Dictionary`2/Enumerator::dictionary
	Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * ___dictionary_0;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::version
	int32_t ___version_1;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::index
	int32_t ___index_2;
	// System.Collections.Generic.KeyValuePair`2<TKey,TValue> System.Collections.Generic.Dictionary`2/Enumerator::current
	KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2  ___current_3;
	// System.Int32 System.Collections.Generic.Dictionary`2/Enumerator::getEnumeratorRetType
	int32_t ___getEnumeratorRetType_4;

public:
	inline static int32_t get_offset_of_dictionary_0() { return static_cast<int32_t>(offsetof(Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8, ___dictionary_0)); }
	inline Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * get_dictionary_0() const { return ___dictionary_0; }
	inline Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C ** get_address_of_dictionary_0() { return &___dictionary_0; }
	inline void set_dictionary_0(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * value)
	{
		___dictionary_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dictionary_0), (void*)value);
	}

	inline static int32_t get_offset_of_version_1() { return static_cast<int32_t>(offsetof(Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8, ___version_1)); }
	inline int32_t get_version_1() const { return ___version_1; }
	inline int32_t* get_address_of_version_1() { return &___version_1; }
	inline void set_version_1(int32_t value)
	{
		___version_1 = value;
	}

	inline static int32_t get_offset_of_index_2() { return static_cast<int32_t>(offsetof(Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8, ___index_2)); }
	inline int32_t get_index_2() const { return ___index_2; }
	inline int32_t* get_address_of_index_2() { return &___index_2; }
	inline void set_index_2(int32_t value)
	{
		___index_2 = value;
	}

	inline static int32_t get_offset_of_current_3() { return static_cast<int32_t>(offsetof(Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8, ___current_3)); }
	inline KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2  get_current_3() const { return ___current_3; }
	inline KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2 * get_address_of_current_3() { return &___current_3; }
	inline void set_current_3(KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2  value)
	{
		___current_3 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___current_3))->___value_1), (void*)NULL);
	}

	inline static int32_t get_offset_of_getEnumeratorRetType_4() { return static_cast<int32_t>(offsetof(Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8, ___getEnumeratorRetType_4)); }
	inline int32_t get_getEnumeratorRetType_4() const { return ___getEnumeratorRetType_4; }
	inline int32_t* get_address_of_getEnumeratorRetType_4() { return &___getEnumeratorRetType_4; }
	inline void set_getEnumeratorRetType_4(int32_t value)
	{
		___getEnumeratorRetType_4 = value;
	}
};


// System.MulticastDelegate
struct MulticastDelegate_t  : public Delegate_t
{
public:
	// System.Delegate[] System.MulticastDelegate::delegates
	DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* ___delegates_11;

public:
	inline static int32_t get_offset_of_delegates_11() { return static_cast<int32_t>(offsetof(MulticastDelegate_t, ___delegates_11)); }
	inline DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* get_delegates_11() const { return ___delegates_11; }
	inline DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86** get_address_of_delegates_11() { return &___delegates_11; }
	inline void set_delegates_11(DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* value)
	{
		___delegates_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___delegates_11), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.MulticastDelegate
struct MulticastDelegate_t_marshaled_pinvoke : public Delegate_t_marshaled_pinvoke
{
	Delegate_t_marshaled_pinvoke** ___delegates_11;
};
// Native definition for COM marshalling of System.MulticastDelegate
struct MulticastDelegate_t_marshaled_com : public Delegate_t_marshaled_com
{
	Delegate_t_marshaled_com** ___delegates_11;
};

// UnityEngine.Component
struct Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap
struct TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C  : public RuntimeObject
{
public:
	// UnityEngine.Vector3 UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::m_patchSize
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_patchSize_0;
	// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/ErrorCode UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::m_errorCode
	int32_t ___m_errorCode_1;
	// System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain> UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::m_terrainTiles
	Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * ___m_terrainTiles_2;

public:
	inline static int32_t get_offset_of_m_patchSize_0() { return static_cast<int32_t>(offsetof(TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C, ___m_patchSize_0)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_patchSize_0() const { return ___m_patchSize_0; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_patchSize_0() { return &___m_patchSize_0; }
	inline void set_m_patchSize_0(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_patchSize_0 = value;
	}

	inline static int32_t get_offset_of_m_errorCode_1() { return static_cast<int32_t>(offsetof(TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C, ___m_errorCode_1)); }
	inline int32_t get_m_errorCode_1() const { return ___m_errorCode_1; }
	inline int32_t* get_address_of_m_errorCode_1() { return &___m_errorCode_1; }
	inline void set_m_errorCode_1(int32_t value)
	{
		___m_errorCode_1 = value;
	}

	inline static int32_t get_offset_of_m_terrainTiles_2() { return static_cast<int32_t>(offsetof(TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C, ___m_terrainTiles_2)); }
	inline Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * get_m_terrainTiles_2() const { return ___m_terrainTiles_2; }
	inline Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C ** get_address_of_m_terrainTiles_2() { return &___m_terrainTiles_2; }
	inline void set_m_terrainTiles_2(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * value)
	{
		___m_terrainTiles_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_terrainTiles_2), (void*)value);
	}
};


// UnityEngine.TerrainData
struct TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
{
public:

public:
};

struct TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields
{
public:
	// System.Int32 UnityEngine.TerrainData::k_MaximumResolution
	int32_t ___k_MaximumResolution_4;
	// System.Int32 UnityEngine.TerrainData::k_MinimumDetailResolutionPerPatch
	int32_t ___k_MinimumDetailResolutionPerPatch_5;
	// System.Int32 UnityEngine.TerrainData::k_MaximumDetailResolutionPerPatch
	int32_t ___k_MaximumDetailResolutionPerPatch_6;
	// System.Int32 UnityEngine.TerrainData::k_MaximumDetailPatchCount
	int32_t ___k_MaximumDetailPatchCount_7;
	// System.Int32 UnityEngine.TerrainData::k_MaximumDetailsPerRes
	int32_t ___k_MaximumDetailsPerRes_8;
	// System.Int32 UnityEngine.TerrainData::k_MinimumAlphamapResolution
	int32_t ___k_MinimumAlphamapResolution_9;
	// System.Int32 UnityEngine.TerrainData::k_MaximumAlphamapResolution
	int32_t ___k_MaximumAlphamapResolution_10;
	// System.Int32 UnityEngine.TerrainData::k_MinimumBaseMapResolution
	int32_t ___k_MinimumBaseMapResolution_11;
	// System.Int32 UnityEngine.TerrainData::k_MaximumBaseMapResolution
	int32_t ___k_MaximumBaseMapResolution_12;

public:
	inline static int32_t get_offset_of_k_MaximumResolution_4() { return static_cast<int32_t>(offsetof(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields, ___k_MaximumResolution_4)); }
	inline int32_t get_k_MaximumResolution_4() const { return ___k_MaximumResolution_4; }
	inline int32_t* get_address_of_k_MaximumResolution_4() { return &___k_MaximumResolution_4; }
	inline void set_k_MaximumResolution_4(int32_t value)
	{
		___k_MaximumResolution_4 = value;
	}

	inline static int32_t get_offset_of_k_MinimumDetailResolutionPerPatch_5() { return static_cast<int32_t>(offsetof(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields, ___k_MinimumDetailResolutionPerPatch_5)); }
	inline int32_t get_k_MinimumDetailResolutionPerPatch_5() const { return ___k_MinimumDetailResolutionPerPatch_5; }
	inline int32_t* get_address_of_k_MinimumDetailResolutionPerPatch_5() { return &___k_MinimumDetailResolutionPerPatch_5; }
	inline void set_k_MinimumDetailResolutionPerPatch_5(int32_t value)
	{
		___k_MinimumDetailResolutionPerPatch_5 = value;
	}

	inline static int32_t get_offset_of_k_MaximumDetailResolutionPerPatch_6() { return static_cast<int32_t>(offsetof(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields, ___k_MaximumDetailResolutionPerPatch_6)); }
	inline int32_t get_k_MaximumDetailResolutionPerPatch_6() const { return ___k_MaximumDetailResolutionPerPatch_6; }
	inline int32_t* get_address_of_k_MaximumDetailResolutionPerPatch_6() { return &___k_MaximumDetailResolutionPerPatch_6; }
	inline void set_k_MaximumDetailResolutionPerPatch_6(int32_t value)
	{
		___k_MaximumDetailResolutionPerPatch_6 = value;
	}

	inline static int32_t get_offset_of_k_MaximumDetailPatchCount_7() { return static_cast<int32_t>(offsetof(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields, ___k_MaximumDetailPatchCount_7)); }
	inline int32_t get_k_MaximumDetailPatchCount_7() const { return ___k_MaximumDetailPatchCount_7; }
	inline int32_t* get_address_of_k_MaximumDetailPatchCount_7() { return &___k_MaximumDetailPatchCount_7; }
	inline void set_k_MaximumDetailPatchCount_7(int32_t value)
	{
		___k_MaximumDetailPatchCount_7 = value;
	}

	inline static int32_t get_offset_of_k_MaximumDetailsPerRes_8() { return static_cast<int32_t>(offsetof(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields, ___k_MaximumDetailsPerRes_8)); }
	inline int32_t get_k_MaximumDetailsPerRes_8() const { return ___k_MaximumDetailsPerRes_8; }
	inline int32_t* get_address_of_k_MaximumDetailsPerRes_8() { return &___k_MaximumDetailsPerRes_8; }
	inline void set_k_MaximumDetailsPerRes_8(int32_t value)
	{
		___k_MaximumDetailsPerRes_8 = value;
	}

	inline static int32_t get_offset_of_k_MinimumAlphamapResolution_9() { return static_cast<int32_t>(offsetof(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields, ___k_MinimumAlphamapResolution_9)); }
	inline int32_t get_k_MinimumAlphamapResolution_9() const { return ___k_MinimumAlphamapResolution_9; }
	inline int32_t* get_address_of_k_MinimumAlphamapResolution_9() { return &___k_MinimumAlphamapResolution_9; }
	inline void set_k_MinimumAlphamapResolution_9(int32_t value)
	{
		___k_MinimumAlphamapResolution_9 = value;
	}

	inline static int32_t get_offset_of_k_MaximumAlphamapResolution_10() { return static_cast<int32_t>(offsetof(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields, ___k_MaximumAlphamapResolution_10)); }
	inline int32_t get_k_MaximumAlphamapResolution_10() const { return ___k_MaximumAlphamapResolution_10; }
	inline int32_t* get_address_of_k_MaximumAlphamapResolution_10() { return &___k_MaximumAlphamapResolution_10; }
	inline void set_k_MaximumAlphamapResolution_10(int32_t value)
	{
		___k_MaximumAlphamapResolution_10 = value;
	}

	inline static int32_t get_offset_of_k_MinimumBaseMapResolution_11() { return static_cast<int32_t>(offsetof(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields, ___k_MinimumBaseMapResolution_11)); }
	inline int32_t get_k_MinimumBaseMapResolution_11() const { return ___k_MinimumBaseMapResolution_11; }
	inline int32_t* get_address_of_k_MinimumBaseMapResolution_11() { return &___k_MinimumBaseMapResolution_11; }
	inline void set_k_MinimumBaseMapResolution_11(int32_t value)
	{
		___k_MinimumBaseMapResolution_11 = value;
	}

	inline static int32_t get_offset_of_k_MaximumBaseMapResolution_12() { return static_cast<int32_t>(offsetof(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields, ___k_MaximumBaseMapResolution_12)); }
	inline int32_t get_k_MaximumBaseMapResolution_12() const { return ___k_MaximumBaseMapResolution_12; }
	inline int32_t* get_address_of_k_MaximumBaseMapResolution_12() { return &___k_MaximumBaseMapResolution_12; }
	inline void set_k_MaximumBaseMapResolution_12(int32_t value)
	{
		___k_MaximumBaseMapResolution_12 = value;
	}
};


// System.AsyncCallback
struct AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Behaviour
struct Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8  : public Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/HeightmapChangedCallback
struct HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/TextureChangedCallback
struct TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter
struct TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Transform
struct Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA  : public Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621
{
public:

public:
};


// UnityEngine.Terrain
struct Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943  : public Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8
{
public:

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
// UnityEngine.Terrain[]
struct TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * m_Items[1];

public:
	inline Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// System.Delegate[]
struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Delegate_t * m_Items[1];

public:
	inline Delegate_t * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Delegate_t ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Delegate_t * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Delegate_t * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Delegate_t ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Delegate_t * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};


// System.Boolean System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::ContainsKey(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Dictionary_2_ContainsKey_mBF5574232A8F79C279DFD71FFBFF85B4E2B6312D_gshared (Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884 * __this, int32_t ___key0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::Add(!0,!1)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2_Add_mF7AEA0EFA07EEBC1A4B283A26A7CB720EE7A4C20_gshared (Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884 * __this, int32_t ___key0, RuntimeObject * ___value1, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::get_Count()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Dictionary_2_get_Count_m69345D9DEE55AA0CE574D19CB7C430AC638C01A9_gshared (Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884 * __this, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Enumerator_tC0410598D958D8FA61F7F1A803A0B7F242CEF1C6  Dictionary_2_GetEnumerator_mE47C13FC1B2BACBEE6F795B0E40E6406CAC55D4B_gshared (Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884 * __this, const RuntimeMethod* method);
// System.Collections.Generic.KeyValuePair`2<!0,!1> System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,System.Object>::get_Current()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C  Enumerator_get_Current_m73F8FC135A20BDFEA0846D8FC876863ED85FEA49_gshared_inline (Enumerator_tC0410598D958D8FA61F7F1A803A0B7F242CEF1C6 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.KeyValuePair`2<System.Int32,System.Object>::get_Key()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR int32_t KeyValuePair_2_get_Key_m89B687D373FBE3D230647CA0C536235E12AD829C_gshared_inline (KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C * __this, const RuntimeMethod* method);
// !1 System.Collections.Generic.KeyValuePair`2<System.Int32,System.Object>::get_Value()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR RuntimeObject * KeyValuePair_2_get_Value_mAD6801F3BC9BA1E99D4E0F72B2B420182D0494FC_gshared_inline (KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C * __this, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Enumerator_t71B93BE98D0EF45AF866DB4944B21CC1D0957B69  Dictionary_2_GetEnumerator_m6E0BF2C3FD1968EFBD38963B604A3A193D5D28F8_gshared (Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * __this, const RuntimeMethod* method);
// System.Collections.Generic.KeyValuePair`2<!0,!1> System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::get_Current()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342  Enumerator_get_Current_m752CB433D17F421B29602443499F2A739C474AEF_gshared_inline (Enumerator_t71B93BE98D0EF45AF866DB4944B21CC1D0957B69 * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.KeyValuePair`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::get_Key()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  KeyValuePair_2_get_Key_m33312D77486DAFBE08CA5AC179C8E82D027B1E12_gshared_inline (KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_m3274F75D4EE16334E50FEEA29C9FF2212D768FF3_gshared (Enumerator_t71B93BE98D0EF45AF866DB4944B21CC1D0957B69 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Enumerator_Dispose_m3EAD1068D4D68CBCD60D5DC3D5B5F38408AAC9A0_gshared (Enumerator_t71B93BE98D0EF45AF866DB4944B21CC1D0957B69 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_m637AE52AC4BDE5BEB1D89BE4B75E49B4C328EFC9_gshared (Enumerator_tC0410598D958D8FA61F7F1A803A0B7F242CEF1C6 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Enumerator_Dispose_m772E7049D8DA96AF5021EA09C3E6322CD4BFBDFC_gshared (Enumerator_tC0410598D958D8FA61F7F1A803A0B7F242CEF1C6 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2__ctor_m7D745ADE56151C2895459668F4A4242985E526D8_gshared (Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::TryGetValue(!0,!1&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Dictionary_2_TryGetValue_mDC342725E8BDB1E283514A3A49A4A5C9E466EE2F_gshared (Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * __this, TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  ___key0, RuntimeObject ** ___value1, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::get_Count()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Dictionary_2_get_Count_mBE16BFDE94C35EAD824C09E9AC0E58926B0160D1_gshared (Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2__ctor_mD721F6D425019314C1037D5D20B42AF94CF2F4DB_gshared (Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::Add(!0,!1)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2_Add_mC812D3CEC119E86EFB942E1723BE9077F7997E75_gshared (Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * __this, TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  ___key0, RuntimeObject * ___value1, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/KeyCollection<!0,!1> System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::get_Keys()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyCollection_tCFA1A910066236156BCAB78D25ADCAC725EE054B * Dictionary_2_get_Keys_m326B9BF2B5A618A990AD6547B050BDFCBBF38893_gshared (Dictionary_2_t283EECF1507E23DEAA390A103471739BF608C018 * __this, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2/KeyCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::GetEnumerator()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Enumerator_tEC4F5C6184F5E671D53C872D88266C1500841E16  KeyCollection_GetEnumerator_mB00FEDCCB71B00FD0A500FDC41B4288122A5BB13_gshared (KeyCollection_tCFA1A910066236156BCAB78D25ADCAC725EE054B * __this, const RuntimeMethod* method);
// !0 System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::get_Current()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  Enumerator_get_Current_mD5C37459762167496A915F0F05399DFDAC6B001D_gshared_inline (Enumerator_tEC4F5C6184F5E671D53C872D88266C1500841E16 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::MoveNext()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Enumerator_MoveNext_mCC12E468FBF33821FB2DF64132CB531ADB5A13F3_gshared (Enumerator_tEC4F5C6184F5E671D53C872D88266C1500841E16 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,System.Object>::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Enumerator_Dispose_m49988C47226A00A23F5363B1959407717E6B3663_gshared (Enumerator_tEC4F5C6184F5E671D53C872D88266C1500841E16 * __this, const RuntimeMethod* method);

// UnityEngine.Terrain[] UnityEngine.TerrainData::get_users()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* TerrainData_get_users_m8DC41DB242FD51BDA25CE01F0AC2C019E05F8F76 (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/HeightmapChangedCallback::Invoke(UnityEngine.Terrain,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HeightmapChangedCallback_Invoke_mE74C19A53A5B04D3C1C21270BBA570B0B4E427C6 (HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain0, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A  ___heightRegion1, bool ___synched2, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/TextureChangedCallback::Invoke(UnityEngine.Terrain,System.String,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TextureChangedCallback_Invoke_mD18A617FB8779E1C66D1167D018B3F2EF585EC66 (TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain0, String_t* ___textureName1, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A  ___texelRegion2, bool ___synched3, const RuntimeMethod* method);
// UnityEngine.Terrain[] UnityEngine.Terrain::get_activeTerrains()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA (const RuntimeMethod* method);
// System.Boolean UnityEngine.Terrain::get_allowAutoConnect()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Terrain_get_allowAutoConnect_m0968C0D1D5628726A19734808D1E37C44CA4F146 (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Terrain::SetNeighbors(UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Terrain_SetNeighbors_mA28EDA87B310AE170885473F6168B18849B55356 (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___left0, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___top1, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___right2, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___bottom3, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_0::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_0__ctor_mD2B394ACE1FD321151050D44AABFA010B5351509 (U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility::HasValidTerrains()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainUtility_HasValidTerrains_m1E41C13C6ADCA00BB57A79651C0CD9FCEFE05EA3 (const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainGroups::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainGroups__ctor_mCC684EF011C9EBA10D335C5BBC2A7B742CB1D940 (TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_1::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_1__ctor_mABF012661ABB1E98C9DA2A3237B45E15D793723F (U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.Terrain::get_groupingID()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Terrain_get_groupingID_mF2A964B8B4B049E4E443782AA951C4E85C6EC132 (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::ContainsKey(!0)
inline bool Dictionary_2_ContainsKey_m7BB92DEC475ECE3BB42DA835F9B2C568E81DCFD7 (Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 * __this, int32_t ___key0, const RuntimeMethod* method)
{
	return ((  bool (*) (Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 *, int32_t, const RuntimeMethod*))Dictionary_2_ContainsKey_mBF5574232A8F79C279DFD71FFBFF85B4E2B6312D_gshared)(__this, ___key0, method);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainFilter__ctor_m60B330ACE5AE8B4833AFB8D9BB095D6783DB2F1E (TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method);
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::CreateFromPlacement(UnityEngine.Terrain,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * TerrainMap_CreateFromPlacement_mB23A40ABF3A46620F82C489D749EABEA1EDF27B2 (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___originTerrain0, TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * ___filter1, bool ___fullValidation2, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::Add(!0,!1)
inline void Dictionary_2_Add_mDBC40F15D74AA100E4FD3F5B7B015C453E35FC7F (Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 * __this, int32_t ___key0, TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * ___value1, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 *, int32_t, TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C *, const RuntimeMethod*))Dictionary_2_Add_mF7AEA0EFA07EEBC1A4B283A26A7CB720EE7A4C20_gshared)(__this, ___key0, ___value1, method);
}
// System.Int32 System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::get_Count()
inline int32_t Dictionary_2_get_Count_m25627A54AA38F254FE2F74C4BB3EC5D240A69C8A (Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 *, const RuntimeMethod*))Dictionary_2_get_Count_m69345D9DEE55AA0CE574D19CB7C430AC638C01A9_gshared)(__this, method);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility::ClearConnectivity()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainUtility_ClearConnectivity_mC60E6D3178548AFDCF76483F99E4BB6F831FC3F5 (const RuntimeMethod* method);
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainGroups UnityEngine.Experimental.TerrainAPI.TerrainUtility::CollectTerrains(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * TerrainUtility_CollectTerrains_m1980638C0C744F59EF15670092FFA1CA9BDA9467 (bool ___onlyAutoConnectedTerrains0, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::GetEnumerator()
inline Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B  Dictionary_2_GetEnumerator_mA00DFEC8FD1C165B6E30BA0DE649CD91C9F61171 (Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 * __this, const RuntimeMethod* method)
{
	return ((  Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B  (*) (Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 *, const RuntimeMethod*))Dictionary_2_GetEnumerator_mE47C13FC1B2BACBEE6F795B0E40E6406CAC55D4B_gshared)(__this, method);
}
// System.Collections.Generic.KeyValuePair`2<!0,!1> System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::get_Current()
inline KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574  Enumerator_get_Current_m34348C5A639EBCAE828B3DBA655423086EE285A8_inline (Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B * __this, const RuntimeMethod* method)
{
	return ((  KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574  (*) (Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B *, const RuntimeMethod*))Enumerator_get_Current_m73F8FC135A20BDFEA0846D8FC876863ED85FEA49_gshared_inline)(__this, method);
}
// !0 System.Collections.Generic.KeyValuePair`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::get_Key()
inline int32_t KeyValuePair_2_get_Key_m074DC0DF87A0EC210E1FAD5B15D67DC7ADC7BCB8_inline (KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574 * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574 *, const RuntimeMethod*))KeyValuePair_2_get_Key_m89B687D373FBE3D230647CA0C536235E12AD829C_gshared_inline)(__this, method);
}
// !1 System.Collections.Generic.KeyValuePair`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::get_Value()
inline TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * KeyValuePair_2_get_Value_mF4D8A7CFC178117B78DCC6C4E27B048F0ECD9F80_inline (KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574 * __this, const RuntimeMethod* method)
{
	return ((  TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * (*) (KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574 *, const RuntimeMethod*))KeyValuePair_2_get_Value_mAD6801F3BC9BA1E99D4E0F72B2B420182D0494FC_gshared_inline)(__this, method);
}
// System.Collections.Generic.Dictionary`2/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::GetEnumerator()
inline Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8  Dictionary_2_GetEnumerator_m6A761440C852EBB50A6DDEF89E54A0BDA6805287 (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * __this, const RuntimeMethod* method)
{
	return ((  Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8  (*) (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C *, const RuntimeMethod*))Dictionary_2_GetEnumerator_m6E0BF2C3FD1968EFBD38963B604A3A193D5D28F8_gshared)(__this, method);
}
// System.Collections.Generic.KeyValuePair`2<!0,!1> System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::get_Current()
inline KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2  Enumerator_get_Current_m253335A3521452DF3D85BFB9EFEA45F1F661D72F_inline (Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8 * __this, const RuntimeMethod* method)
{
	return ((  KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2  (*) (Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8 *, const RuntimeMethod*))Enumerator_get_Current_m752CB433D17F421B29602443499F2A739C474AEF_gshared_inline)(__this, method);
}
// !0 System.Collections.Generic.KeyValuePair`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::get_Key()
inline TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  KeyValuePair_2_get_Key_m187E30244CD96C3D6A8F53AE69ABE9846E42A9BE_inline (KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2 * __this, const RuntimeMethod* method)
{
	return ((  TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  (*) (KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2 *, const RuntimeMethod*))KeyValuePair_2_get_Key_m33312D77486DAFBE08CA5AC179C8E82D027B1E12_gshared_inline)(__this, method);
}
// UnityEngine.Terrain UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::GetTerrain(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635 (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::MoveNext()
inline bool Enumerator_MoveNext_mED5A978C87AD73E54C6A8BE05D98339C44875192 (Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8 * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8 *, const RuntimeMethod*))Enumerator_MoveNext_m3274F75D4EE16334E50FEEA29C9FF2212D768FF3_gshared)(__this, method);
}
// System.Void System.Collections.Generic.Dictionary`2/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::Dispose()
inline void Enumerator_Dispose_m49E0868CF374239FC10BCEED1A260C935FC1DE9B (Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8 * __this, const RuntimeMethod* method)
{
	((  void (*) (Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8 *, const RuntimeMethod*))Enumerator_Dispose_m3EAD1068D4D68CBCD60D5DC3D5B5F38408AAC9A0_gshared)(__this, method);
}
// System.Boolean System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::MoveNext()
inline bool Enumerator_MoveNext_m25382F4DAE18791D44400CA8FD709589AE89E66D (Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B *, const RuntimeMethod*))Enumerator_MoveNext_m637AE52AC4BDE5BEB1D89BE4B75E49B4C328EFC9_gshared)(__this, method);
}
// System.Void System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::Dispose()
inline void Enumerator_Dispose_m742B154C9D11228799C88A8E5D3F4B247EEF1493 (Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B * __this, const RuntimeMethod* method)
{
	((  void (*) (Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B *, const RuntimeMethod*))Enumerator_Dispose_m772E7049D8DA96AF5021EA09C3E6322CD4BFBDFC_gshared)(__this, method);
}
// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap>::.ctor()
inline void Dictionary_2__ctor_mDF4242C91427656B19BE65AF67E6430A5E151931 (Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 * __this, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_t651CE851D569289A981D44DC5543BEA956206753 *, const RuntimeMethod*))Dictionary_2__ctor_m7D745ADE56151C2895459668F4A4242985E526D8_gshared)(__this, method);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord::.ctor(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileCoord__ctor_mAA64B48F381F5DCBB58B7EA137AD4073076177ED (TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::TryGetValue(!0,!1&)
inline bool Dictionary_2_TryGetValue_m5BB81EB3FE81C41FFF7B7764770B64EEB214ED2D (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * __this, TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  ___key0, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 ** ___value1, const RuntimeMethod* method)
{
	return ((  bool (*) (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C *, TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA , Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 **, const RuntimeMethod*))Dictionary_2_TryGetValue_mDC342725E8BDB1E283514A3A49A4A5C9E466EE2F_gshared)(__this, ___key0, ___value1, method);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/<>c__DisplayClass4_0::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_0__ctor_m933028B07653F2470BD281F03CCA0AB74231B0A8 (U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Object::op_Equality(UnityEngine.Object,UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___x0, Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___y1, const RuntimeMethod* method);
// UnityEngine.TerrainData UnityEngine.Terrain::get_terrainData()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2 (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * __this, const RuntimeMethod* method);
// UnityEngine.Transform UnityEngine.Component::get_transform()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9 (Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621 * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Transform::get_position()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294 (Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.TerrainData::get_size()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  TerrainData_get_size_m0987D18D442D824D5F9CF1CF5B42CCF1A6A42D51 (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Vector2::.ctor(System.Single,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Vector2__ctor_mEE8FB117AB1F8DB746FB8B3EB4C0DA3BF2A230D0 (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * __this, float ___x0, float ___y1, const RuntimeMethod* method);
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::CreateFromPlacement(UnityEngine.Vector2,UnityEngine.Vector2,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * TerrainMap_CreateFromPlacement_m2CFB7C0DD0890EBA733486F6CFF67B15471A6B57 (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___gridOrigin0, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___gridSize1, TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * ___filter2, bool ___fullValidation3, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap__ctor_m7BC19CC1FA417F6D152B8E290AAD9990DB81E81A (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter::Invoke(UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainFilter_Invoke_mB9F861A5CB34474898F150197A7F7CB90AFB1AF9 (TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain0, const RuntimeMethod* method);
// System.Int32 UnityEngine.Mathf::RoundToInt(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Mathf_RoundToInt_m0EAD8BD38FCB72FA1D8A04E96337C820EC83F041 (float ___f0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::TryToAddTerrain(System.Int32,System.Int32,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainMap_TryToAddTerrain_m7F845FD1237F4342EAA377F5B8B078C93F0B2862 (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, int32_t ___tileX0, int32_t ___tileZ1, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain2, const RuntimeMethod* method);
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/ErrorCode UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::Validate()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t TerrainMap_Validate_m31FE625EC81CDED0369413935CD78F355677237A (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, const RuntimeMethod* method);
// System.Int32 System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::get_Count()
inline int32_t Dictionary_2_get_Count_m5872BC43117B310319C83B09F904FC7B215F176F (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C *, const RuntimeMethod*))Dictionary_2_get_Count_mBE16BFDE94C35EAD824C09E9AC0E58926B0160D1_gshared)(__this, method);
}
// System.Void System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::.ctor()
inline void Dictionary_2__ctor_m4D0EF0F749453F91361A51A6C9C5C479B334ACF7 (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * __this, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C *, const RuntimeMethod*))Dictionary_2__ctor_mD721F6D425019314C1037D5D20B42AF94CF2F4DB_gshared)(__this, method);
}
// System.Boolean UnityEngine.Vector3::op_Inequality(UnityEngine.Vector3,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Vector3_op_Inequality_mFEEAA4C4BF743FB5B8A47FF4967A5E2C73273D6E (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___lhs0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___rhs1, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::Add(!0,!1)
inline void Dictionary_2_Add_m8A2F763FE35F12CE5C6B4EBDC8F490F2432721F0 (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * __this, TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  ___key0, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___value1, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C *, TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA , Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, const RuntimeMethod*))Dictionary_2_Add_mC812D3CEC119E86EFB942E1723BE9077F7997E75_gshared)(__this, ___key0, ___value1, method);
}
// System.Boolean UnityEngine.Object::op_Inequality(UnityEngine.Object,UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1 (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___x0, Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___y1, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::AddTerrainInternal(System.Int32,System.Int32,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap_AddTerrainInternal_m2E4B99FEC2C6D4BCC6CFF0E58F0D1E70E254B4C2 (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, int32_t ___x0, int32_t ___z1, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain2, const RuntimeMethod* method);
// System.Boolean UnityEngine.Object::op_Implicit(UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Implicit_m8B2A44B4B1406ED346D1AE6D962294FD58D0D534 (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___exists0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Mathf::Approximately(System.Single,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Mathf_Approximately_m91AF00403E0D2DEA1AAE68601AD218CFAD70DF7E (float ___a0, float ___b1, const RuntimeMethod* method);
// System.Collections.Generic.Dictionary`2/KeyCollection<!0,!1> System.Collections.Generic.Dictionary`2<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::get_Keys()
inline KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB * Dictionary_2_get_Keys_m6378916440BB5F09FDDC61026EB9E31F5BC8A9E1 (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * __this, const RuntimeMethod* method)
{
	return ((  KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB * (*) (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C *, const RuntimeMethod*))Dictionary_2_get_Keys_m326B9BF2B5A618A990AD6547B050BDFCBBF38893_gshared)(__this, method);
}
// System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<!0,!1> System.Collections.Generic.Dictionary`2/KeyCollection<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::GetEnumerator()
inline Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639  KeyCollection_GetEnumerator_mB9D02C9BC28992C54914385A9ADB8B31645D8C29 (KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB * __this, const RuntimeMethod* method)
{
	return ((  Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639  (*) (KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB *, const RuntimeMethod*))KeyCollection_GetEnumerator_mB00FEDCCB71B00FD0A500FDC41B4288122A5BB13_gshared)(__this, method);
}
// !0 System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::get_Current()
inline TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  Enumerator_get_Current_mB84DB5A32BBDC3FBFB6E3419E2EE70128E320FE4_inline (Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639 * __this, const RuntimeMethod* method)
{
	return ((  TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  (*) (Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639 *, const RuntimeMethod*))Enumerator_get_Current_mD5C37459762167496A915F0F05399DFDAC6B001D_gshared_inline)(__this, method);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::ValidateTerrain(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap_ValidateTerrain_m7B0154421B22B18D420FF7AB3179887AFCB320AB (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::MoveNext()
inline bool Enumerator_MoveNext_mA095EEA83D1389075851A35D0CBDA713A34C2F7C (Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639 * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639 *, const RuntimeMethod*))Enumerator_MoveNext_mCC12E468FBF33821FB2DF64132CB531ADB5A13F3_gshared)(__this, method);
}
// System.Void System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord,UnityEngine.Terrain>::Dispose()
inline void Enumerator_Dispose_m1AF30CB4B9D460F0CBE9944BBBE800205A8B84AF (Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639 * __this, const RuntimeMethod* method)
{
	((  void (*) (Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639 *, const RuntimeMethod*))Enumerator_Dispose_m49988C47226A00A23F5363B1959407717E6B3663_gshared)(__this, method);
}
// System.Void UnityEngine.Behaviour::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Behaviour__ctor_m409AEC21511ACF9A4CC0654DF4B8253E0D81D22C (Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m091EBAEBC7919B0391ABDAFB7389ADC12206525B (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.TerrainData::Internal_Create(UnityEngine.TerrainData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData_Internal_Create_m02C792919F391601D1EE4CF6DF70182FBD646F16 (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * ___terrainData0, const RuntimeMethod* method);
// System.Void UnityEngine.TerrainData::get_size_Injected(UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData_get_size_Injected_mF6DEEE266FBF9CEC3AF2B6B77593B9704B299A68 (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___ret0, const RuntimeMethod* method);
// System.Int32 UnityEngine.TerrainData::GetBoundaryValue(UnityEngine.TerrainData/BoundaryValueType)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB (int32_t ___type0, const RuntimeMethod* method);
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks::InvokeHeightmapChangedCallback(UnityEngine.TerrainData,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainCallbacks_InvokeHeightmapChangedCallback_m786753AA38B90C453886C1B1011B8279D194DA54 (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * ___terrainData0, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A  ___heightRegion1, bool ___synched2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainCallbacks_InvokeHeightmapChangedCallback_m786753AA38B90C453886C1B1011B8279D194DA54_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* V_1 = NULL;
	int32_t V_2 = 0;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_3 = NULL;
	{
		HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D * L_0 = ((TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_StaticFields*)il2cpp_codegen_static_fields_for(TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_il2cpp_TypeInfo_var))->get_heightmapChanged_0();
		V_0 = (bool)((!(((RuntimeObject*)(HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D *)L_0) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_0037;
		}
	}
	{
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_2 = ___terrainData0;
		NullCheck(L_2);
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_3 = TerrainData_get_users_m8DC41DB242FD51BDA25CE01F0AC2C019E05F8F76(L_2, /*hidden argument*/NULL);
		V_1 = L_3;
		V_2 = 0;
		goto IL_0030;
	}

IL_001a:
	{
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_4 = V_1;
		int32_t L_5 = V_2;
		NullCheck(L_4);
		int32_t L_6 = L_5;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_7 = (L_4)->GetAt(static_cast<il2cpp_array_size_t>(L_6));
		V_3 = L_7;
		HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D * L_8 = ((TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_StaticFields*)il2cpp_codegen_static_fields_for(TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_il2cpp_TypeInfo_var))->get_heightmapChanged_0();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_9 = V_3;
		RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A  L_10 = ___heightRegion1;
		bool L_11 = ___synched2;
		NullCheck(L_8);
		HeightmapChangedCallback_Invoke_mE74C19A53A5B04D3C1C21270BBA570B0B4E427C6(L_8, L_9, L_10, L_11, /*hidden argument*/NULL);
		int32_t L_12 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_12, (int32_t)1));
	}

IL_0030:
	{
		int32_t L_13 = V_2;
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_14 = V_1;
		NullCheck(L_14);
		if ((((int32_t)L_13) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_14)->max_length)))))))
		{
			goto IL_001a;
		}
	}
	{
	}

IL_0037:
	{
		return;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks::InvokeTextureChangedCallback(UnityEngine.TerrainData,System.String,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainCallbacks_InvokeTextureChangedCallback_m922885C44A5A7F5CD26341414414C2B78CE14A85 (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * ___terrainData0, String_t* ___textureName1, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A  ___texelRegion2, bool ___synched3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainCallbacks_InvokeTextureChangedCallback_m922885C44A5A7F5CD26341414414C2B78CE14A85_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* V_1 = NULL;
	int32_t V_2 = 0;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_3 = NULL;
	{
		TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E * L_0 = ((TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_StaticFields*)il2cpp_codegen_static_fields_for(TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_il2cpp_TypeInfo_var))->get_textureChanged_1();
		V_0 = (bool)((!(((RuntimeObject*)(TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E *)L_0) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_1 = V_0;
		if (!L_1)
		{
			goto IL_0038;
		}
	}
	{
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_2 = ___terrainData0;
		NullCheck(L_2);
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_3 = TerrainData_get_users_m8DC41DB242FD51BDA25CE01F0AC2C019E05F8F76(L_2, /*hidden argument*/NULL);
		V_1 = L_3;
		V_2 = 0;
		goto IL_0031;
	}

IL_001a:
	{
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_4 = V_1;
		int32_t L_5 = V_2;
		NullCheck(L_4);
		int32_t L_6 = L_5;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_7 = (L_4)->GetAt(static_cast<il2cpp_array_size_t>(L_6));
		V_3 = L_7;
		TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E * L_8 = ((TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_StaticFields*)il2cpp_codegen_static_fields_for(TerrainCallbacks_t5D6C605DD5B2D840575AD8A477DCEDF764BC9AB5_il2cpp_TypeInfo_var))->get_textureChanged_1();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_9 = V_3;
		String_t* L_10 = ___textureName1;
		RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A  L_11 = ___texelRegion2;
		bool L_12 = ___synched3;
		NullCheck(L_8);
		TextureChangedCallback_Invoke_mD18A617FB8779E1C66D1167D018B3F2EF585EC66(L_8, L_9, L_10, L_11, L_12, /*hidden argument*/NULL);
		int32_t L_13 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_13, (int32_t)1));
	}

IL_0031:
	{
		int32_t L_14 = V_2;
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_15 = V_1;
		NullCheck(L_15);
		if ((((int32_t)L_14) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_15)->max_length)))))))
		{
			goto IL_001a;
		}
	}
	{
	}

IL_0038:
	{
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/HeightmapChangedCallback::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HeightmapChangedCallback__ctor_mD8C4C7A562D2D9F1F2F156D7A069AA4ED6DFB60F (HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/HeightmapChangedCallback::Invoke(UnityEngine.Terrain,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HeightmapChangedCallback_Invoke_mE74C19A53A5B04D3C1C21270BBA570B0B4E427C6 (HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain0, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A  ___heightRegion1, bool ___synched2, const RuntimeMethod* method)
{
	DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* delegateArrayToInvoke = __this->get_delegates_11();
	Delegate_t** delegatesToInvoke;
	il2cpp_array_size_t length;
	if (delegateArrayToInvoke != NULL)
	{
		length = delegateArrayToInvoke->max_length;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(delegateArrayToInvoke->GetAddressAtUnchecked(0));
	}
	else
	{
		length = 1;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(&__this);
	}

	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		Delegate_t* currentDelegate = delegatesToInvoke[i];
		Il2CppMethodPointer targetMethodPointer = currentDelegate->get_method_ptr_0();
		RuntimeObject* targetThis = currentDelegate->get_m_target_2();
		RuntimeMethod* targetMethod = (RuntimeMethod*)(currentDelegate->get_method_3());
		if (!il2cpp_codegen_method_is_virtual(targetMethod))
		{
			il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found(targetMethod);
		}
		bool ___methodIsStatic = MethodIsStatic(targetMethod);
		int ___parameterCount = il2cpp_codegen_method_parameter_count(targetMethod);
		if (___methodIsStatic)
		{
			if (___parameterCount == 3)
			{
				// open
				typedef void (*FunctionPointerType) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___terrain0, ___heightRegion1, ___synched2, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, ___heightRegion1, ___synched2, targetMethod);
			}
		}
		else if (___parameterCount != 3)
		{
			// open
			if (il2cpp_codegen_method_is_virtual(targetMethod) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker2< RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(targetMethod, ___terrain0, ___heightRegion1, ___synched2);
					else
						GenericVirtActionInvoker2< RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(targetMethod, ___terrain0, ___heightRegion1, ___synched2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker2< RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___terrain0, ___heightRegion1, ___synched2);
					else
						VirtActionInvoker2< RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___terrain0, ___heightRegion1, ___synched2);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(&___heightRegion1) - 1), ___synched2, targetMethod);
				}
				typedef void (*FunctionPointerType) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___terrain0, ___heightRegion1, ___synched2, targetMethod);
			}
		}
		else
		{
			// closed
			if (targetThis != NULL && il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker3< Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(targetMethod, targetThis, ___terrain0, ___heightRegion1, ___synched2);
					else
						GenericVirtActionInvoker3< Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(targetMethod, targetThis, ___terrain0, ___heightRegion1, ___synched2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker3< Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___terrain0, ___heightRegion1, ___synched2);
					else
						VirtActionInvoker3< Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___terrain0, ___heightRegion1, ___synched2);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___terrain0) - 1), ___heightRegion1, ___synched2, targetMethod);
				}
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___terrain0, ___heightRegion1, ___synched2, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, ___heightRegion1, ___synched2, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/HeightmapChangedCallback::BeginInvoke(UnityEngine.Terrain,UnityEngine.RectInt,System.Boolean,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* HeightmapChangedCallback_BeginInvoke_m34DB0DA5BF64D5F303A24804C84B3C582BDEFD5D (HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain0, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A  ___heightRegion1, bool ___synched2, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback3, RuntimeObject * ___object4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (HeightmapChangedCallback_BeginInvoke_m34DB0DA5BF64D5F303A24804C84B3C582BDEFD5D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = ___terrain0;
	__d_args[1] = Box(RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A_il2cpp_TypeInfo_var, &___heightRegion1);
	__d_args[2] = Box(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var, &___synched2);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback3, (RuntimeObject*)___object4);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/HeightmapChangedCallback::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HeightmapChangedCallback_EndInvoke_m144223021166831E422245E7C5AB3E2AE3E49CBA (HeightmapChangedCallback_t632AB8CCEEA7F63E16056E63E998974C878D1F5D * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/TextureChangedCallback::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TextureChangedCallback__ctor_m11F1CEC86B40E26B1E59AFC40458F609632B0926 (TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/TextureChangedCallback::Invoke(UnityEngine.Terrain,System.String,UnityEngine.RectInt,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TextureChangedCallback_Invoke_mD18A617FB8779E1C66D1167D018B3F2EF585EC66 (TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain0, String_t* ___textureName1, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A  ___texelRegion2, bool ___synched3, const RuntimeMethod* method)
{
	DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* delegateArrayToInvoke = __this->get_delegates_11();
	Delegate_t** delegatesToInvoke;
	il2cpp_array_size_t length;
	if (delegateArrayToInvoke != NULL)
	{
		length = delegateArrayToInvoke->max_length;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(delegateArrayToInvoke->GetAddressAtUnchecked(0));
	}
	else
	{
		length = 1;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(&__this);
	}

	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		Delegate_t* currentDelegate = delegatesToInvoke[i];
		Il2CppMethodPointer targetMethodPointer = currentDelegate->get_method_ptr_0();
		RuntimeObject* targetThis = currentDelegate->get_m_target_2();
		RuntimeMethod* targetMethod = (RuntimeMethod*)(currentDelegate->get_method_3());
		if (!il2cpp_codegen_method_is_virtual(targetMethod))
		{
			il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found(targetMethod);
		}
		bool ___methodIsStatic = MethodIsStatic(targetMethod);
		int ___parameterCount = il2cpp_codegen_method_parameter_count(targetMethod);
		if (___methodIsStatic)
		{
			if (___parameterCount == 4)
			{
				// open
				typedef void (*FunctionPointerType) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___terrain0, ___textureName1, ___texelRegion2, ___synched3, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3, targetMethod);
			}
		}
		else if (___parameterCount != 4)
		{
			// open
			if (il2cpp_codegen_method_is_virtual(targetMethod) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker3< String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(targetMethod, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
					else
						GenericVirtActionInvoker3< String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(targetMethod, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker3< String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
					else
						VirtActionInvoker3< String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___textureName1) - 1), ___texelRegion2, ___synched3, targetMethod);
				}
				typedef void (*FunctionPointerType) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___terrain0, ___textureName1, ___texelRegion2, ___synched3, targetMethod);
			}
		}
		else
		{
			// closed
			if (targetThis != NULL && il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker4< Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(targetMethod, targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
					else
						GenericVirtActionInvoker4< Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(targetMethod, targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker4< Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
					else
						VirtActionInvoker4< Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef void (*FunctionPointerType) (RuntimeObject*, String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___terrain0) - 1), ___textureName1, ___texelRegion2, ___synched3, targetMethod);
				}
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___terrain0, ___textureName1, ___texelRegion2, ___synched3, targetMethod);
				}
				else
				{
					typedef void (*FunctionPointerType) (void*, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, String_t*, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A , bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, ___textureName1, ___texelRegion2, ___synched3, targetMethod);
				}
			}
		}
	}
}
// System.IAsyncResult UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/TextureChangedCallback::BeginInvoke(UnityEngine.Terrain,System.String,UnityEngine.RectInt,System.Boolean,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TextureChangedCallback_BeginInvoke_mBEC316023C6EEA14D6EC02E363B3027A3F0151DC (TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain0, String_t* ___textureName1, RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A  ___texelRegion2, bool ___synched3, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback4, RuntimeObject * ___object5, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TextureChangedCallback_BeginInvoke_mBEC316023C6EEA14D6EC02E363B3027A3F0151DC_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[5] = {0};
	__d_args[0] = ___terrain0;
	__d_args[1] = ___textureName1;
	__d_args[2] = Box(RectInt_t595A63F7EE2BC91A4D2DE5403C5FE94D3C3A6F7A_il2cpp_TypeInfo_var, &___texelRegion2);
	__d_args[3] = Box(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var, &___synched3);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback4, (RuntimeObject*)___object5);
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainCallbacks/TextureChangedCallback::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TextureChangedCallback_EndInvoke_m0AB06E4885E25D5A7D8BBA38ECFA263FBB577DE2 (TextureChangedCallback_tC699198D2EACFE62AE42E8D2F4A7FD8B533A602E * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility::HasValidTerrains()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainUtility_HasValidTerrains_m1E41C13C6ADCA00BB57A79651C0CD9FCEFE05EA3 (const RuntimeMethod* method)
{
	bool V_0 = false;
	int32_t G_B3_0 = 0;
	{
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_0 = Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA(/*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_1 = Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA(/*hidden argument*/NULL);
		NullCheck(L_1);
		G_B3_0 = ((!(((uint32_t)(((RuntimeArray*)L_1)->max_length)) <= ((uint32_t)0)))? 1 : 0);
		goto IL_0014;
	}

IL_0013:
	{
		G_B3_0 = 0;
	}

IL_0014:
	{
		V_0 = (bool)G_B3_0;
		goto IL_0017;
	}

IL_0017:
	{
		bool L_2 = V_0;
		return L_2;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility::ClearConnectivity()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainUtility_ClearConnectivity_mC60E6D3178548AFDCF76483F99E4BB6F831FC3F5 (const RuntimeMethod* method)
{
	TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* V_0 = NULL;
	int32_t V_1 = 0;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_2 = NULL;
	bool V_3 = false;
	{
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_0 = Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA(/*hidden argument*/NULL);
		V_0 = L_0;
		V_1 = 0;
		goto IL_002b;
	}

IL_000c:
	{
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_1 = V_0;
		int32_t L_2 = V_1;
		NullCheck(L_1);
		int32_t L_3 = L_2;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		V_2 = L_4;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_5 = V_2;
		NullCheck(L_5);
		bool L_6 = Terrain_get_allowAutoConnect_m0968C0D1D5628726A19734808D1E37C44CA4F146(L_5, /*hidden argument*/NULL);
		V_3 = L_6;
		bool L_7 = V_3;
		if (!L_7)
		{
			goto IL_0026;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_8 = V_2;
		NullCheck(L_8);
		Terrain_SetNeighbors_mA28EDA87B310AE170885473F6168B18849B55356(L_8, (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *)NULL, (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *)NULL, (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *)NULL, (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *)NULL, /*hidden argument*/NULL);
	}

IL_0026:
	{
		int32_t L_9 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_9, (int32_t)1));
	}

IL_002b:
	{
		int32_t L_10 = V_1;
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_11 = V_0;
		NullCheck(L_11);
		if ((((int32_t)L_10) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_11)->max_length)))))))
		{
			goto IL_000c;
		}
	}
	{
		return;
	}
}
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainGroups UnityEngine.Experimental.TerrainAPI.TerrainUtility::CollectTerrains(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * TerrainUtility_CollectTerrains_m1980638C0C744F59EF15670092FFA1CA9BDA9467 (bool ___onlyAutoConnectedTerrains0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainUtility_CollectTerrains_m1980638C0C744F59EF15670092FFA1CA9BDA9467_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A * V_0 = NULL;
	TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * V_1 = NULL;
	bool V_2 = false;
	TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * V_3 = NULL;
	TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* V_4 = NULL;
	int32_t V_5 = 0;
	U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * V_6 = NULL;
	bool V_7 = false;
	bool V_8 = false;
	TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * V_9 = NULL;
	bool V_10 = false;
	int32_t G_B6_0 = 0;
	TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * G_B18_0 = NULL;
	{
		U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A * L_0 = (U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A *)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass4_0__ctor_mD2B394ACE1FD321151050D44AABFA010B5351509(L_0, /*hidden argument*/NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A * L_1 = V_0;
		bool L_2 = ___onlyAutoConnectedTerrains0;
		NullCheck(L_1);
		L_1->set_onlyAutoConnectedTerrains_0(L_2);
		bool L_3 = TerrainUtility_HasValidTerrains_m1E41C13C6ADCA00BB57A79651C0CD9FCEFE05EA3(/*hidden argument*/NULL);
		V_2 = (bool)((((int32_t)L_3) == ((int32_t)0))? 1 : 0);
		bool L_4 = V_2;
		if (!L_4)
		{
			goto IL_0021;
		}
	}
	{
		V_3 = (TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 *)NULL;
		goto IL_00f5;
	}

IL_0021:
	{
		TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * L_5 = (TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 *)il2cpp_codegen_object_new(TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5_il2cpp_TypeInfo_var);
		TerrainGroups__ctor_mCC684EF011C9EBA10D335C5BBC2A7B742CB1D940(L_5, /*hidden argument*/NULL);
		V_1 = L_5;
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_6 = Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA(/*hidden argument*/NULL);
		V_4 = L_6;
		V_5 = 0;
		goto IL_00db;
	}

IL_0037:
	{
		U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * L_7 = (U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 *)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass4_1__ctor_mABF012661ABB1E98C9DA2A3237B45E15D793723F(L_7, /*hidden argument*/NULL);
		V_6 = L_7;
		U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * L_8 = V_6;
		U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A * L_9 = V_0;
		NullCheck(L_8);
		L_8->set_CSU24U3CU3E8__locals1_1(L_9);
		U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * L_10 = V_6;
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_11 = V_4;
		int32_t L_12 = V_5;
		NullCheck(L_11);
		int32_t L_13 = L_12;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_14 = (L_11)->GetAt(static_cast<il2cpp_array_size_t>(L_13));
		NullCheck(L_10);
		L_10->set_t_0(L_14);
		U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * L_15 = V_6;
		NullCheck(L_15);
		U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A * L_16 = L_15->get_CSU24U3CU3E8__locals1_1();
		NullCheck(L_16);
		bool L_17 = L_16->get_onlyAutoConnectedTerrains_0();
		if (!L_17)
		{
			goto IL_0072;
		}
	}
	{
		U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * L_18 = V_6;
		NullCheck(L_18);
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_19 = L_18->get_t_0();
		NullCheck(L_19);
		bool L_20 = Terrain_get_allowAutoConnect_m0968C0D1D5628726A19734808D1E37C44CA4F146(L_19, /*hidden argument*/NULL);
		G_B6_0 = ((((int32_t)L_20) == ((int32_t)0))? 1 : 0);
		goto IL_0073;
	}

IL_0072:
	{
		G_B6_0 = 0;
	}

IL_0073:
	{
		V_7 = (bool)G_B6_0;
		bool L_21 = V_7;
		if (!L_21)
		{
			goto IL_007b;
		}
	}
	{
		goto IL_00d5;
	}

IL_007b:
	{
		TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * L_22 = V_1;
		U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * L_23 = V_6;
		NullCheck(L_23);
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_24 = L_23->get_t_0();
		NullCheck(L_24);
		int32_t L_25 = Terrain_get_groupingID_mF2A964B8B4B049E4E443782AA951C4E85C6EC132(L_24, /*hidden argument*/NULL);
		NullCheck(L_22);
		bool L_26 = Dictionary_2_ContainsKey_m7BB92DEC475ECE3BB42DA835F9B2C568E81DCFD7(L_22, L_25, /*hidden argument*/Dictionary_2_ContainsKey_m7BB92DEC475ECE3BB42DA835F9B2C568E81DCFD7_RuntimeMethod_var);
		V_8 = (bool)((((int32_t)L_26) == ((int32_t)0))? 1 : 0);
		bool L_27 = V_8;
		if (!L_27)
		{
			goto IL_00d4;
		}
	}
	{
		U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * L_28 = V_6;
		NullCheck(L_28);
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_29 = L_28->get_t_0();
		U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * L_30 = V_6;
		TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * L_31 = (TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B *)il2cpp_codegen_object_new(TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B_il2cpp_TypeInfo_var);
		TerrainFilter__ctor_m60B330ACE5AE8B4833AFB8D9BB095D6783DB2F1E(L_31, L_30, (intptr_t)((intptr_t)U3CU3Ec__DisplayClass4_1_U3CCollectTerrainsU3Eb__0_mC50AC0DBF7CB0E1D89FE0F9DA973B406782C0B25_RuntimeMethod_var), /*hidden argument*/NULL);
		TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_32 = TerrainMap_CreateFromPlacement_mB23A40ABF3A46620F82C489D749EABEA1EDF27B2(L_29, L_31, (bool)1, /*hidden argument*/NULL);
		V_9 = L_32;
		TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_33 = V_9;
		V_10 = (bool)((!(((RuntimeObject*)(TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C *)L_33) <= ((RuntimeObject*)(RuntimeObject *)NULL)))? 1 : 0);
		bool L_34 = V_10;
		if (!L_34)
		{
			goto IL_00d3;
		}
	}
	{
		TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * L_35 = V_1;
		U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * L_36 = V_6;
		NullCheck(L_36);
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_37 = L_36->get_t_0();
		NullCheck(L_37);
		int32_t L_38 = Terrain_get_groupingID_mF2A964B8B4B049E4E443782AA951C4E85C6EC132(L_37, /*hidden argument*/NULL);
		TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_39 = V_9;
		NullCheck(L_35);
		Dictionary_2_Add_mDBC40F15D74AA100E4FD3F5B7B015C453E35FC7F(L_35, L_38, L_39, /*hidden argument*/Dictionary_2_Add_mDBC40F15D74AA100E4FD3F5B7B015C453E35FC7F_RuntimeMethod_var);
	}

IL_00d3:
	{
	}

IL_00d4:
	{
	}

IL_00d5:
	{
		int32_t L_40 = V_5;
		V_5 = ((int32_t)il2cpp_codegen_add((int32_t)L_40, (int32_t)1));
	}

IL_00db:
	{
		int32_t L_41 = V_5;
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_42 = V_4;
		NullCheck(L_42);
		if ((((int32_t)L_41) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_42)->max_length)))))))
		{
			goto IL_0037;
		}
	}
	{
		TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * L_43 = V_1;
		NullCheck(L_43);
		int32_t L_44 = Dictionary_2_get_Count_m25627A54AA38F254FE2F74C4BB3EC5D240A69C8A(L_43, /*hidden argument*/Dictionary_2_get_Count_m25627A54AA38F254FE2F74C4BB3EC5D240A69C8A_RuntimeMethod_var);
		if (L_44)
		{
			goto IL_00f1;
		}
	}
	{
		G_B18_0 = ((TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 *)(NULL));
		goto IL_00f2;
	}

IL_00f1:
	{
		TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * L_45 = V_1;
		G_B18_0 = L_45;
	}

IL_00f2:
	{
		V_3 = G_B18_0;
		goto IL_00f5;
	}

IL_00f5:
	{
		TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * L_46 = V_3;
		return L_46;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility::AutoConnect()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainUtility_AutoConnect_m43FD8F195A874A511293784F8029C22AB30A428E (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainUtility_AutoConnect_m43FD8F195A874A511293784F8029C22AB30A428E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * V_0 = NULL;
	bool V_1 = false;
	bool V_2 = false;
	Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B  V_3;
	memset((&V_3), 0, sizeof(V_3));
	KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574  V_4;
	memset((&V_4), 0, sizeof(V_4));
	int32_t V_5 = 0;
	TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * V_6 = NULL;
	Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8  V_7;
	memset((&V_7), 0, sizeof(V_7));
	KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2  V_8;
	memset((&V_8), 0, sizeof(V_8));
	TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  V_9;
	memset((&V_9), 0, sizeof(V_9));
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_10 = NULL;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_11 = NULL;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_12 = NULL;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_13 = NULL;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_14 = NULL;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 2);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		bool L_0 = TerrainUtility_HasValidTerrains_m1E41C13C6ADCA00BB57A79651C0CD9FCEFE05EA3(/*hidden argument*/NULL);
		V_1 = (bool)((((int32_t)L_0) == ((int32_t)0))? 1 : 0);
		bool L_1 = V_1;
		if (!L_1)
		{
			goto IL_0012;
		}
	}
	{
		goto IL_0143;
	}

IL_0012:
	{
		TerrainUtility_ClearConnectivity_mC60E6D3178548AFDCF76483F99E4BB6F831FC3F5(/*hidden argument*/NULL);
		TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * L_2 = TerrainUtility_CollectTerrains_m1980638C0C744F59EF15670092FFA1CA9BDA9467((bool)1, /*hidden argument*/NULL);
		V_0 = L_2;
		TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * L_3 = V_0;
		V_2 = (bool)((((RuntimeObject*)(TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 *)L_3) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0);
		bool L_4 = V_2;
		if (!L_4)
		{
			goto IL_002c;
		}
	}
	{
		goto IL_0143;
	}

IL_002c:
	{
		TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * L_5 = V_0;
		NullCheck(L_5);
		Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B  L_6 = Dictionary_2_GetEnumerator_mA00DFEC8FD1C165B6E30BA0DE649CD91C9F61171(L_5, /*hidden argument*/Dictionary_2_GetEnumerator_mA00DFEC8FD1C165B6E30BA0DE649CD91C9F61171_RuntimeMethod_var);
		V_3 = L_6;
	}

IL_0034:
	try
	{ // begin try (depth: 1)
		{
			goto IL_0126;
		}

IL_0039:
		{
			KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574  L_7 = Enumerator_get_Current_m34348C5A639EBCAE828B3DBA655423086EE285A8_inline((Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B *)(&V_3), /*hidden argument*/Enumerator_get_Current_m34348C5A639EBCAE828B3DBA655423086EE285A8_RuntimeMethod_var);
			V_4 = L_7;
			int32_t L_8 = KeyValuePair_2_get_Key_m074DC0DF87A0EC210E1FAD5B15D67DC7ADC7BCB8_inline((KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574 *)(&V_4), /*hidden argument*/KeyValuePair_2_get_Key_m074DC0DF87A0EC210E1FAD5B15D67DC7ADC7BCB8_RuntimeMethod_var);
			V_5 = L_8;
			TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_9 = KeyValuePair_2_get_Value_mF4D8A7CFC178117B78DCC6C4E27B048F0ECD9F80_inline((KeyValuePair_2_t66F6496942DCFC8D590F8DA04910990C5DA39574 *)(&V_4), /*hidden argument*/KeyValuePair_2_get_Value_mF4D8A7CFC178117B78DCC6C4E27B048F0ECD9F80_RuntimeMethod_var);
			V_6 = L_9;
			TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_10 = V_6;
			NullCheck(L_10);
			Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * L_11 = L_10->get_m_terrainTiles_2();
			NullCheck(L_11);
			Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8  L_12 = Dictionary_2_GetEnumerator_m6A761440C852EBB50A6DDEF89E54A0BDA6805287(L_11, /*hidden argument*/Dictionary_2_GetEnumerator_m6A761440C852EBB50A6DDEF89E54A0BDA6805287_RuntimeMethod_var);
			V_7 = L_12;
		}

IL_0064:
		try
		{ // begin try (depth: 2)
			{
				goto IL_0108;
			}

IL_0069:
			{
				KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2  L_13 = Enumerator_get_Current_m253335A3521452DF3D85BFB9EFEA45F1F661D72F_inline((Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8 *)(&V_7), /*hidden argument*/Enumerator_get_Current_m253335A3521452DF3D85BFB9EFEA45F1F661D72F_RuntimeMethod_var);
				V_8 = L_13;
				TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_14 = KeyValuePair_2_get_Key_m187E30244CD96C3D6A8F53AE69ABE9846E42A9BE_inline((KeyValuePair_2_t1042B27C0272463F95B4736D997596598DFACDF2 *)(&V_8), /*hidden argument*/KeyValuePair_2_get_Key_m187E30244CD96C3D6A8F53AE69ABE9846E42A9BE_RuntimeMethod_var);
				V_9 = L_14;
				TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_15 = V_6;
				TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_16 = V_9;
				int32_t L_17 = L_16.get_tileX_0();
				TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_18 = V_9;
				int32_t L_19 = L_18.get_tileZ_1();
				NullCheck(L_15);
				Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_20 = TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635(L_15, L_17, L_19, /*hidden argument*/NULL);
				V_10 = L_20;
				TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_21 = V_6;
				TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_22 = V_9;
				int32_t L_23 = L_22.get_tileX_0();
				TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_24 = V_9;
				int32_t L_25 = L_24.get_tileZ_1();
				NullCheck(L_21);
				Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_26 = TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635(L_21, ((int32_t)il2cpp_codegen_subtract((int32_t)L_23, (int32_t)1)), L_25, /*hidden argument*/NULL);
				V_11 = L_26;
				TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_27 = V_6;
				TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_28 = V_9;
				int32_t L_29 = L_28.get_tileX_0();
				TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_30 = V_9;
				int32_t L_31 = L_30.get_tileZ_1();
				NullCheck(L_27);
				Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_32 = TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635(L_27, ((int32_t)il2cpp_codegen_add((int32_t)L_29, (int32_t)1)), L_31, /*hidden argument*/NULL);
				V_12 = L_32;
				TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_33 = V_6;
				TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_34 = V_9;
				int32_t L_35 = L_34.get_tileX_0();
				TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_36 = V_9;
				int32_t L_37 = L_36.get_tileZ_1();
				NullCheck(L_33);
				Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_38 = TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635(L_33, L_35, ((int32_t)il2cpp_codegen_add((int32_t)L_37, (int32_t)1)), /*hidden argument*/NULL);
				V_13 = L_38;
				TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_39 = V_6;
				TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_40 = V_9;
				int32_t L_41 = L_40.get_tileX_0();
				TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_42 = V_9;
				int32_t L_43 = L_42.get_tileZ_1();
				NullCheck(L_39);
				Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_44 = TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635(L_39, L_41, ((int32_t)il2cpp_codegen_subtract((int32_t)L_43, (int32_t)1)), /*hidden argument*/NULL);
				V_14 = L_44;
				Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_45 = V_10;
				Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_46 = V_11;
				Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_47 = V_13;
				Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_48 = V_12;
				Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_49 = V_14;
				NullCheck(L_45);
				Terrain_SetNeighbors_mA28EDA87B310AE170885473F6168B18849B55356(L_45, L_46, L_47, L_48, L_49, /*hidden argument*/NULL);
			}

IL_0108:
			{
				bool L_50 = Enumerator_MoveNext_mED5A978C87AD73E54C6A8BE05D98339C44875192((Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8 *)(&V_7), /*hidden argument*/Enumerator_MoveNext_mED5A978C87AD73E54C6A8BE05D98339C44875192_RuntimeMethod_var);
				if (L_50)
				{
					goto IL_0069;
				}
			}

IL_0114:
			{
				IL2CPP_LEAVE(0x125, FINALLY_0116);
			}
		} // end try (depth: 2)
		catch(Il2CppExceptionWrapper& e)
		{
			__last_unhandled_exception = (Exception_t *)e.ex;
			goto FINALLY_0116;
		}

FINALLY_0116:
		{ // begin finally (depth: 2)
			Enumerator_Dispose_m49E0868CF374239FC10BCEED1A260C935FC1DE9B((Enumerator_t29B808F8AC6D4B77DE3720AC4C08C0A71E500BB8 *)(&V_7), /*hidden argument*/Enumerator_Dispose_m49E0868CF374239FC10BCEED1A260C935FC1DE9B_RuntimeMethod_var);
			IL2CPP_END_FINALLY(278)
		} // end finally (depth: 2)
		IL2CPP_CLEANUP(278)
		{
			IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
			IL2CPP_JUMP_TBL(0x125, IL_0125)
		}

IL_0125:
		{
		}

IL_0126:
		{
			bool L_51 = Enumerator_MoveNext_m25382F4DAE18791D44400CA8FD709589AE89E66D((Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B *)(&V_3), /*hidden argument*/Enumerator_MoveNext_m25382F4DAE18791D44400CA8FD709589AE89E66D_RuntimeMethod_var);
			if (L_51)
			{
				goto IL_0039;
			}
		}

IL_0132:
		{
			IL2CPP_LEAVE(0x143, FINALLY_0134);
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0134;
	}

FINALLY_0134:
	{ // begin finally (depth: 1)
		Enumerator_Dispose_m742B154C9D11228799C88A8E5D3F4B247EEF1493((Enumerator_t5CAB4B85782B849B0F05E35FFAED39CF4A78598B *)(&V_3), /*hidden argument*/Enumerator_Dispose_m742B154C9D11228799C88A8E5D3F4B247EEF1493_RuntimeMethod_var);
		IL2CPP_END_FINALLY(308)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(308)
	{
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
		IL2CPP_JUMP_TBL(0x143, IL_0143)
	}

IL_0143:
	{
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_0::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_0__ctor_mD2B394ACE1FD321151050D44AABFA010B5351509 (U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_1::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_1__ctor_mABF012661ABB1E98C9DA2A3237B45E15D793723F (U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility/<>c__DisplayClass4_1::<CollectTerrains>b__0(UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CU3Ec__DisplayClass4_1_U3CCollectTerrainsU3Eb__0_mC50AC0DBF7CB0E1D89FE0F9DA973B406782C0B25 (U3CU3Ec__DisplayClass4_1_tC63A89C846069E68CAA87EA14D0149555C357407 * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___x0, const RuntimeMethod* method)
{
	int32_t G_B4_0 = 0;
	int32_t G_B6_0 = 0;
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_0 = ___x0;
		NullCheck(L_0);
		int32_t L_1 = Terrain_get_groupingID_mF2A964B8B4B049E4E443782AA951C4E85C6EC132(L_0, /*hidden argument*/NULL);
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_2 = __this->get_t_0();
		NullCheck(L_2);
		int32_t L_3 = Terrain_get_groupingID_mF2A964B8B4B049E4E443782AA951C4E85C6EC132(L_2, /*hidden argument*/NULL);
		if ((!(((uint32_t)L_1) == ((uint32_t)L_3))))
		{
			goto IL_002b;
		}
	}
	{
		U3CU3Ec__DisplayClass4_0_t79B6EBFB785C0608F276376E6D35800FA6574B4A * L_4 = __this->get_CSU24U3CU3E8__locals1_1();
		NullCheck(L_4);
		bool L_5 = L_4->get_onlyAutoConnectedTerrains_0();
		if (!L_5)
		{
			goto IL_0028;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_6 = ___x0;
		NullCheck(L_6);
		bool L_7 = Terrain_get_allowAutoConnect_m0968C0D1D5628726A19734808D1E37C44CA4F146(L_6, /*hidden argument*/NULL);
		G_B4_0 = ((int32_t)(L_7));
		goto IL_0029;
	}

IL_0028:
	{
		G_B4_0 = 1;
	}

IL_0029:
	{
		G_B6_0 = G_B4_0;
		goto IL_002c;
	}

IL_002b:
	{
		G_B6_0 = 0;
	}

IL_002c:
	{
		return (bool)G_B6_0;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainGroups::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainGroups__ctor_mCC684EF011C9EBA10D335C5BBC2A7B742CB1D940 (TerrainGroups_t88B87E7C8DA6A97E904D74167C43D631796ECBC5 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainGroups__ctor_mCC684EF011C9EBA10D335C5BBC2A7B742CB1D940_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Dictionary_2__ctor_mDF4242C91427656B19BE65AF67E6430A5E151931(__this, /*hidden argument*/Dictionary_2__ctor_mDF4242C91427656B19BE65AF67E6430A5E151931_RuntimeMethod_var);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// UnityEngine.Terrain UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::GetTerrain(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635 (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_0 = NULL;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_1 = NULL;
	{
		V_0 = (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *)NULL;
		Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * L_0 = __this->get_m_terrainTiles_2();
		int32_t L_1 = ___tileX0;
		int32_t L_2 = ___tileZ1;
		TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_3;
		memset((&L_3), 0, sizeof(L_3));
		TileCoord__ctor_mAA64B48F381F5DCBB58B7EA137AD4073076177ED((&L_3), L_1, L_2, /*hidden argument*/NULL);
		NullCheck(L_0);
		Dictionary_2_TryGetValue_m5BB81EB3FE81C41FFF7B7764770B64EEB214ED2D(L_0, L_3, (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 **)(&V_0), /*hidden argument*/Dictionary_2_TryGetValue_m5BB81EB3FE81C41FFF7B7764770B64EEB214ED2D_RuntimeMethod_var);
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_4 = V_0;
		V_1 = L_4;
		goto IL_001c;
	}

IL_001c:
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_5 = V_1;
		return L_5;
	}
}
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::CreateFromPlacement(UnityEngine.Terrain,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * TerrainMap_CreateFromPlacement_mB23A40ABF3A46620F82C489D749EABEA1EDF27B2 (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___originTerrain0, TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * ___filter1, bool ___fullValidation2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_CreateFromPlacement_mB23A40ABF3A46620F82C489D749EABEA1EDF27B2_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002 * V_0 = NULL;
	float V_1 = 0.0f;
	float V_2 = 0.0f;
	float V_3 = 0.0f;
	float V_4 = 0.0f;
	bool V_5 = false;
	TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * V_6 = NULL;
	bool V_7 = false;
	bool V_8 = false;
	int32_t G_B4_0 = 0;
	{
		U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002 * L_0 = (U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002 *)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass4_0__ctor_m933028B07653F2470BD281F03CCA0AB74231B0A8(L_0, /*hidden argument*/NULL);
		V_0 = L_0;
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_1 = Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA(/*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_001f;
		}
	}
	{
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_2 = Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA(/*hidden argument*/NULL);
		NullCheck(L_2);
		if (!(((RuntimeArray*)L_2)->max_length))
		{
			goto IL_001f;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_3 = ___originTerrain0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_4 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_3, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		G_B4_0 = ((int32_t)(L_4));
		goto IL_0020;
	}

IL_001f:
	{
		G_B4_0 = 1;
	}

IL_0020:
	{
		V_5 = (bool)G_B4_0;
		bool L_5 = V_5;
		if (!L_5)
		{
			goto IL_002e;
		}
	}
	{
		V_6 = (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C *)NULL;
		goto IL_00cb;
	}

IL_002e:
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_6 = ___originTerrain0;
		NullCheck(L_6);
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_7 = Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2(L_6, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_8 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_7, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		V_7 = L_8;
		bool L_9 = V_7;
		if (!L_9)
		{
			goto IL_0048;
		}
	}
	{
		V_6 = (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C *)NULL;
		goto IL_00cb;
	}

IL_0048:
	{
		U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002 * L_10 = V_0;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_11 = ___originTerrain0;
		NullCheck(L_11);
		int32_t L_12 = Terrain_get_groupingID_mF2A964B8B4B049E4E443782AA951C4E85C6EC132(L_11, /*hidden argument*/NULL);
		NullCheck(L_10);
		L_10->set_groupID_0(L_12);
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_13 = ___originTerrain0;
		NullCheck(L_13);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_14 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_13, /*hidden argument*/NULL);
		NullCheck(L_14);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_15 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_14, /*hidden argument*/NULL);
		float L_16 = L_15.get_x_2();
		V_1 = L_16;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_17 = ___originTerrain0;
		NullCheck(L_17);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_18 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_17, /*hidden argument*/NULL);
		NullCheck(L_18);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_19 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_18, /*hidden argument*/NULL);
		float L_20 = L_19.get_z_4();
		V_2 = L_20;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_21 = ___originTerrain0;
		NullCheck(L_21);
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_22 = Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2(L_21, /*hidden argument*/NULL);
		NullCheck(L_22);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_23 = TerrainData_get_size_m0987D18D442D824D5F9CF1CF5B42CCF1A6A42D51(L_22, /*hidden argument*/NULL);
		float L_24 = L_23.get_x_2();
		V_3 = L_24;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_25 = ___originTerrain0;
		NullCheck(L_25);
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_26 = Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2(L_25, /*hidden argument*/NULL);
		NullCheck(L_26);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_27 = TerrainData_get_size_m0987D18D442D824D5F9CF1CF5B42CCF1A6A42D51(L_26, /*hidden argument*/NULL);
		float L_28 = L_27.get_z_4();
		V_4 = L_28;
		TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * L_29 = ___filter1;
		V_8 = (bool)((((RuntimeObject*)(TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B *)L_29) == ((RuntimeObject*)(RuntimeObject *)NULL))? 1 : 0);
		bool L_30 = V_8;
		if (!L_30)
		{
			goto IL_00b1;
		}
	}
	{
		U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002 * L_31 = V_0;
		TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * L_32 = (TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B *)il2cpp_codegen_object_new(TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B_il2cpp_TypeInfo_var);
		TerrainFilter__ctor_m60B330ACE5AE8B4833AFB8D9BB095D6783DB2F1E(L_32, L_31, (intptr_t)((intptr_t)U3CU3Ec__DisplayClass4_0_U3CCreateFromPlacementU3Eb__0_m51A58F8F2A072A87B2E6C84818FEE0DEC2DD6FB0_RuntimeMethod_var), /*hidden argument*/NULL);
		___filter1 = L_32;
	}

IL_00b1:
	{
		float L_33 = V_1;
		float L_34 = V_2;
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_35;
		memset((&L_35), 0, sizeof(L_35));
		Vector2__ctor_mEE8FB117AB1F8DB746FB8B3EB4C0DA3BF2A230D0((&L_35), L_33, L_34, /*hidden argument*/NULL);
		float L_36 = V_3;
		float L_37 = V_4;
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_38;
		memset((&L_38), 0, sizeof(L_38));
		Vector2__ctor_mEE8FB117AB1F8DB746FB8B3EB4C0DA3BF2A230D0((&L_38), L_36, L_37, /*hidden argument*/NULL);
		TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * L_39 = ___filter1;
		bool L_40 = ___fullValidation2;
		TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_41 = TerrainMap_CreateFromPlacement_m2CFB7C0DD0890EBA733486F6CFF67B15471A6B57(L_35, L_38, L_39, L_40, /*hidden argument*/NULL);
		V_6 = L_41;
		goto IL_00cb;
	}

IL_00cb:
	{
		TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_42 = V_6;
		return L_42;
	}
}
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::CreateFromPlacement(UnityEngine.Vector2,UnityEngine.Vector2,UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * TerrainMap_CreateFromPlacement_m2CFB7C0DD0890EBA733486F6CFF67B15471A6B57 (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___gridOrigin0, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___gridSize1, TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * ___filter2, bool ___fullValidation3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_CreateFromPlacement_m2CFB7C0DD0890EBA733486F6CFF67B15471A6B57_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * V_0 = NULL;
	float V_1 = 0.0f;
	float V_2 = 0.0f;
	bool V_3 = false;
	TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * V_4 = NULL;
	TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* V_5 = NULL;
	int32_t V_6 = 0;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_7 = NULL;
	bool V_8 = false;
	bool V_9 = false;
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_10;
	memset((&V_10), 0, sizeof(V_10));
	int32_t V_11 = 0;
	int32_t V_12 = 0;
	bool V_13 = false;
	int32_t G_B3_0 = 0;
	int32_t G_B11_0 = 0;
	TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * G_B21_0 = NULL;
	{
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_0 = Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA(/*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_1 = Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA(/*hidden argument*/NULL);
		NullCheck(L_1);
		G_B3_0 = ((((int32_t)(((RuntimeArray*)L_1)->max_length)) == ((int32_t)0))? 1 : 0);
		goto IL_0014;
	}

IL_0013:
	{
		G_B3_0 = 1;
	}

IL_0014:
	{
		V_3 = (bool)G_B3_0;
		bool L_2 = V_3;
		if (!L_2)
		{
			goto IL_0020;
		}
	}
	{
		V_4 = (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C *)NULL;
		goto IL_0102;
	}

IL_0020:
	{
		TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_3 = (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C *)il2cpp_codegen_object_new(TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C_il2cpp_TypeInfo_var);
		TerrainMap__ctor_m7BC19CC1FA417F6D152B8E290AAD9990DB81E81A(L_3, /*hidden argument*/NULL);
		V_0 = L_3;
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_4 = ___gridSize1;
		float L_5 = L_4.get_x_0();
		V_1 = ((float)((float)(1.0f)/(float)L_5));
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_6 = ___gridSize1;
		float L_7 = L_6.get_y_1();
		V_2 = ((float)((float)(1.0f)/(float)L_7));
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_8 = Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA(/*hidden argument*/NULL);
		V_5 = L_8;
		V_6 = 0;
		goto IL_00d3;
	}

IL_0050:
	{
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_9 = V_5;
		int32_t L_10 = V_6;
		NullCheck(L_9);
		int32_t L_11 = L_10;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_12 = (L_9)->GetAt(static_cast<il2cpp_array_size_t>(L_11));
		V_7 = L_12;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_13 = V_7;
		NullCheck(L_13);
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_14 = Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2(L_13, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_15 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_14, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		V_8 = L_15;
		bool L_16 = V_8;
		if (!L_16)
		{
			goto IL_006d;
		}
	}
	{
		goto IL_00cd;
	}

IL_006d:
	{
		TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * L_17 = ___filter2;
		if (!L_17)
		{
			goto IL_007a;
		}
	}
	{
		TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * L_18 = ___filter2;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_19 = V_7;
		NullCheck(L_18);
		bool L_20 = TerrainFilter_Invoke_mB9F861A5CB34474898F150197A7F7CB90AFB1AF9(L_18, L_19, /*hidden argument*/NULL);
		G_B11_0 = ((int32_t)(L_20));
		goto IL_007b;
	}

IL_007a:
	{
		G_B11_0 = 1;
	}

IL_007b:
	{
		V_9 = (bool)G_B11_0;
		bool L_21 = V_9;
		if (!L_21)
		{
			goto IL_00cc;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_22 = V_7;
		NullCheck(L_22);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_23 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_22, /*hidden argument*/NULL);
		NullCheck(L_23);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_24 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_23, /*hidden argument*/NULL);
		V_10 = L_24;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_25 = V_10;
		float L_26 = L_25.get_x_2();
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_27 = ___gridOrigin0;
		float L_28 = L_27.get_x_0();
		float L_29 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		int32_t L_30 = Mathf_RoundToInt_m0EAD8BD38FCB72FA1D8A04E96337C820EC83F041(((float)il2cpp_codegen_multiply((float)((float)il2cpp_codegen_subtract((float)L_26, (float)L_28)), (float)L_29)), /*hidden argument*/NULL);
		V_11 = L_30;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_31 = V_10;
		float L_32 = L_31.get_z_4();
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_33 = ___gridOrigin0;
		float L_34 = L_33.get_y_1();
		float L_35 = V_2;
		int32_t L_36 = Mathf_RoundToInt_m0EAD8BD38FCB72FA1D8A04E96337C820EC83F041(((float)il2cpp_codegen_multiply((float)((float)il2cpp_codegen_subtract((float)L_32, (float)L_34)), (float)L_35)), /*hidden argument*/NULL);
		V_12 = L_36;
		TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_37 = V_0;
		int32_t L_38 = V_11;
		int32_t L_39 = V_12;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_40 = V_7;
		NullCheck(L_37);
		TerrainMap_TryToAddTerrain_m7F845FD1237F4342EAA377F5B8B078C93F0B2862(L_37, L_38, L_39, L_40, /*hidden argument*/NULL);
	}

IL_00cc:
	{
	}

IL_00cd:
	{
		int32_t L_41 = V_6;
		V_6 = ((int32_t)il2cpp_codegen_add((int32_t)L_41, (int32_t)1));
	}

IL_00d3:
	{
		int32_t L_42 = V_6;
		TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* L_43 = V_5;
		NullCheck(L_43);
		if ((((int32_t)L_42) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_43)->max_length)))))))
		{
			goto IL_0050;
		}
	}
	{
		bool L_44 = ___fullValidation3;
		V_13 = L_44;
		bool L_45 = V_13;
		if (!L_45)
		{
			goto IL_00ec;
		}
	}
	{
		TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_46 = V_0;
		NullCheck(L_46);
		TerrainMap_Validate_m31FE625EC81CDED0369413935CD78F355677237A(L_46, /*hidden argument*/NULL);
	}

IL_00ec:
	{
		TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_47 = V_0;
		NullCheck(L_47);
		Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * L_48 = L_47->get_m_terrainTiles_2();
		NullCheck(L_48);
		int32_t L_49 = Dictionary_2_get_Count_m5872BC43117B310319C83B09F904FC7B215F176F(L_48, /*hidden argument*/Dictionary_2_get_Count_m5872BC43117B310319C83B09F904FC7B215F176F_RuntimeMethod_var);
		if ((((int32_t)L_49) > ((int32_t)0)))
		{
			goto IL_00fd;
		}
	}
	{
		G_B21_0 = ((TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C *)(NULL));
		goto IL_00fe;
	}

IL_00fd:
	{
		TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_50 = V_0;
		G_B21_0 = L_50;
	}

IL_00fe:
	{
		V_4 = G_B21_0;
		goto IL_0102;
	}

IL_0102:
	{
		TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * L_51 = V_4;
		return L_51;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap__ctor_m7BC19CC1FA417F6D152B8E290AAD9990DB81E81A (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap__ctor_m7BC19CC1FA417F6D152B8E290AAD9990DB81E81A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		__this->set_m_errorCode_1(0);
		Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * L_0 = (Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C *)il2cpp_codegen_object_new(Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C_il2cpp_TypeInfo_var);
		Dictionary_2__ctor_m4D0EF0F749453F91361A51A6C9C5C479B334ACF7(L_0, /*hidden argument*/Dictionary_2__ctor_m4D0EF0F749453F91361A51A6C9C5C479B334ACF7_RuntimeMethod_var);
		__this->set_m_terrainTiles_2(L_0);
		return;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::AddTerrainInternal(System.Int32,System.Int32,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap_AddTerrainInternal_m2E4B99FEC2C6D4BCC6CFF0E58F0D1E70E254B4C2 (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, int32_t ___x0, int32_t ___z1, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_AddTerrainInternal_m2E4B99FEC2C6D4BCC6CFF0E58F0D1E70E254B4C2_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	bool V_1 = false;
	{
		Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * L_0 = __this->get_m_terrainTiles_2();
		NullCheck(L_0);
		int32_t L_1 = Dictionary_2_get_Count_m5872BC43117B310319C83B09F904FC7B215F176F(L_0, /*hidden argument*/Dictionary_2_get_Count_m5872BC43117B310319C83B09F904FC7B215F176F_RuntimeMethod_var);
		V_0 = (bool)((((int32_t)L_1) == ((int32_t)0))? 1 : 0);
		bool L_2 = V_0;
		if (!L_2)
		{
			goto IL_0026;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_3 = ___terrain2;
		NullCheck(L_3);
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_4 = Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2(L_3, /*hidden argument*/NULL);
		NullCheck(L_4);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_5 = TerrainData_get_size_m0987D18D442D824D5F9CF1CF5B42CCF1A6A42D51(L_4, /*hidden argument*/NULL);
		__this->set_m_patchSize_0(L_5);
		goto IL_0052;
	}

IL_0026:
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_6 = ___terrain2;
		NullCheck(L_6);
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_7 = Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2(L_6, /*hidden argument*/NULL);
		NullCheck(L_7);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_8 = TerrainData_get_size_m0987D18D442D824D5F9CF1CF5B42CCF1A6A42D51(L_7, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_9 = __this->get_m_patchSize_0();
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		bool L_10 = Vector3_op_Inequality_mFEEAA4C4BF743FB5B8A47FF4967A5E2C73273D6E(L_8, L_9, /*hidden argument*/NULL);
		V_1 = L_10;
		bool L_11 = V_1;
		if (!L_11)
		{
			goto IL_0051;
		}
	}
	{
		int32_t L_12 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_12|(int32_t)4)));
	}

IL_0051:
	{
	}

IL_0052:
	{
		Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * L_13 = __this->get_m_terrainTiles_2();
		int32_t L_14 = ___x0;
		int32_t L_15 = ___z1;
		TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_16;
		memset((&L_16), 0, sizeof(L_16));
		TileCoord__ctor_mAA64B48F381F5DCBB58B7EA137AD4073076177ED((&L_16), L_14, L_15, /*hidden argument*/NULL);
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_17 = ___terrain2;
		NullCheck(L_13);
		Dictionary_2_Add_m8A2F763FE35F12CE5C6B4EBDC8F490F2432721F0(L_13, L_16, L_17, /*hidden argument*/Dictionary_2_Add_m8A2F763FE35F12CE5C6B4EBDC8F490F2432721F0_RuntimeMethod_var);
		return;
	}
}
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::TryToAddTerrain(System.Int32,System.Int32,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainMap_TryToAddTerrain_m7F845FD1237F4342EAA377F5B8B078C93F0B2862 (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, int32_t ___tileX0, int32_t ___tileZ1, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_TryToAddTerrain_m7F845FD1237F4342EAA377F5B8B078C93F0B2862_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	bool V_1 = false;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_2 = NULL;
	bool V_3 = false;
	bool V_4 = false;
	bool V_5 = false;
	{
		V_0 = (bool)0;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_0 = ___terrain2;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_0, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		V_1 = L_1;
		bool L_2 = V_1;
		if (!L_2)
		{
			goto IL_0053;
		}
	}
	{
		int32_t L_3 = ___tileX0;
		int32_t L_4 = ___tileZ1;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_5 = TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635(__this, L_3, L_4, /*hidden argument*/NULL);
		V_2 = L_5;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_6 = V_2;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_7 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_6, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		V_3 = L_7;
		bool L_8 = V_3;
		if (!L_8)
		{
			goto IL_0044;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_9 = V_2;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_10 = ___terrain2;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_11 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_9, L_10, /*hidden argument*/NULL);
		V_4 = L_11;
		bool L_12 = V_4;
		if (!L_12)
		{
			goto IL_0041;
		}
	}
	{
		int32_t L_13 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_13|(int32_t)1)));
	}

IL_0041:
	{
		goto IL_0052;
	}

IL_0044:
	{
		int32_t L_14 = ___tileX0;
		int32_t L_15 = ___tileZ1;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_16 = ___terrain2;
		TerrainMap_AddTerrainInternal_m2E4B99FEC2C6D4BCC6CFF0E58F0D1E70E254B4C2(__this, L_14, L_15, L_16, /*hidden argument*/NULL);
		V_0 = (bool)1;
	}

IL_0052:
	{
	}

IL_0053:
	{
		bool L_17 = V_0;
		V_5 = L_17;
		goto IL_0058;
	}

IL_0058:
	{
		bool L_18 = V_5;
		return L_18;
	}
}
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::ValidateTerrain(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainMap_ValidateTerrain_m7B0154421B22B18D420FF7AB3179887AFCB320AB (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_ValidateTerrain_m7B0154421B22B18D420FF7AB3179887AFCB320AB_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_0 = NULL;
	bool V_1 = false;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_2 = NULL;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_3 = NULL;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_4 = NULL;
	Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * V_5 = NULL;
	bool V_6 = false;
	bool V_7 = false;
	bool V_8 = false;
	bool V_9 = false;
	bool V_10 = false;
	bool V_11 = false;
	bool V_12 = false;
	bool V_13 = false;
	int32_t G_B5_0 = 0;
	int32_t G_B12_0 = 0;
	int32_t G_B19_0 = 0;
	int32_t G_B26_0 = 0;
	{
		int32_t L_0 = ___tileX0;
		int32_t L_1 = ___tileZ1;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_2 = TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635(__this, L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_3 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_4 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_3, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		V_1 = L_4;
		bool L_5 = V_1;
		if (!L_5)
		{
			goto IL_026d;
		}
	}
	{
		int32_t L_6 = ___tileX0;
		int32_t L_7 = ___tileZ1;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_8 = TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635(__this, ((int32_t)il2cpp_codegen_subtract((int32_t)L_6, (int32_t)1)), L_7, /*hidden argument*/NULL);
		V_2 = L_8;
		int32_t L_9 = ___tileX0;
		int32_t L_10 = ___tileZ1;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_11 = TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635(__this, ((int32_t)il2cpp_codegen_add((int32_t)L_9, (int32_t)1)), L_10, /*hidden argument*/NULL);
		V_3 = L_11;
		int32_t L_12 = ___tileX0;
		int32_t L_13 = ___tileZ1;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_14 = TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635(__this, L_12, ((int32_t)il2cpp_codegen_add((int32_t)L_13, (int32_t)1)), /*hidden argument*/NULL);
		V_4 = L_14;
		int32_t L_15 = ___tileX0;
		int32_t L_16 = ___tileZ1;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_17 = TerrainMap_GetTerrain_m2580E4949922965E6B2F1EF0AF7669D3EEE5E635(__this, L_15, ((int32_t)il2cpp_codegen_subtract((int32_t)L_16, (int32_t)1)), /*hidden argument*/NULL);
		V_5 = L_17;
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_18 = V_2;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_19 = Object_op_Implicit_m8B2A44B4B1406ED346D1AE6D962294FD58D0D534(L_18, /*hidden argument*/NULL);
		V_6 = L_19;
		bool L_20 = V_6;
		if (!L_20)
		{
			goto IL_00cf;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_21 = V_0;
		NullCheck(L_21);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_22 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_21, /*hidden argument*/NULL);
		NullCheck(L_22);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_23 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_22, /*hidden argument*/NULL);
		float L_24 = L_23.get_x_2();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_25 = V_2;
		NullCheck(L_25);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_26 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_25, /*hidden argument*/NULL);
		NullCheck(L_26);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_27 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_26, /*hidden argument*/NULL);
		float L_28 = L_27.get_x_2();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_29 = V_2;
		NullCheck(L_29);
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_30 = Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2(L_29, /*hidden argument*/NULL);
		NullCheck(L_30);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_31 = TerrainData_get_size_m0987D18D442D824D5F9CF1CF5B42CCF1A6A42D51(L_30, /*hidden argument*/NULL);
		float L_32 = L_31.get_x_2();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		bool L_33 = Mathf_Approximately_m91AF00403E0D2DEA1AAE68601AD218CFAD70DF7E(L_24, ((float)il2cpp_codegen_add((float)L_28, (float)L_32)), /*hidden argument*/NULL);
		if (!L_33)
		{
			goto IL_00b7;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_34 = V_0;
		NullCheck(L_34);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_35 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_34, /*hidden argument*/NULL);
		NullCheck(L_35);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_36 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_35, /*hidden argument*/NULL);
		float L_37 = L_36.get_z_4();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_38 = V_2;
		NullCheck(L_38);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_39 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_38, /*hidden argument*/NULL);
		NullCheck(L_39);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_40 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_39, /*hidden argument*/NULL);
		float L_41 = L_40.get_z_4();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		bool L_42 = Mathf_Approximately_m91AF00403E0D2DEA1AAE68601AD218CFAD70DF7E(L_37, L_41, /*hidden argument*/NULL);
		G_B5_0 = ((((int32_t)L_42) == ((int32_t)0))? 1 : 0);
		goto IL_00b8;
	}

IL_00b7:
	{
		G_B5_0 = 1;
	}

IL_00b8:
	{
		V_7 = (bool)G_B5_0;
		bool L_43 = V_7;
		if (!L_43)
		{
			goto IL_00ce;
		}
	}
	{
		int32_t L_44 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_44|(int32_t)8)));
	}

IL_00ce:
	{
	}

IL_00cf:
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_45 = V_3;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_46 = Object_op_Implicit_m8B2A44B4B1406ED346D1AE6D962294FD58D0D534(L_45, /*hidden argument*/NULL);
		V_8 = L_46;
		bool L_47 = V_8;
		if (!L_47)
		{
			goto IL_0156;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_48 = V_0;
		NullCheck(L_48);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_49 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_48, /*hidden argument*/NULL);
		NullCheck(L_49);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_50 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_49, /*hidden argument*/NULL);
		float L_51 = L_50.get_x_2();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_52 = V_0;
		NullCheck(L_52);
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_53 = Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2(L_52, /*hidden argument*/NULL);
		NullCheck(L_53);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_54 = TerrainData_get_size_m0987D18D442D824D5F9CF1CF5B42CCF1A6A42D51(L_53, /*hidden argument*/NULL);
		float L_55 = L_54.get_x_2();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_56 = V_3;
		NullCheck(L_56);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_57 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_56, /*hidden argument*/NULL);
		NullCheck(L_57);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_58 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_57, /*hidden argument*/NULL);
		float L_59 = L_58.get_x_2();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		bool L_60 = Mathf_Approximately_m91AF00403E0D2DEA1AAE68601AD218CFAD70DF7E(((float)il2cpp_codegen_add((float)L_51, (float)L_55)), L_59, /*hidden argument*/NULL);
		if (!L_60)
		{
			goto IL_013e;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_61 = V_0;
		NullCheck(L_61);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_62 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_61, /*hidden argument*/NULL);
		NullCheck(L_62);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_63 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_62, /*hidden argument*/NULL);
		float L_64 = L_63.get_z_4();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_65 = V_3;
		NullCheck(L_65);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_66 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_65, /*hidden argument*/NULL);
		NullCheck(L_66);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_67 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_66, /*hidden argument*/NULL);
		float L_68 = L_67.get_z_4();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		bool L_69 = Mathf_Approximately_m91AF00403E0D2DEA1AAE68601AD218CFAD70DF7E(L_64, L_68, /*hidden argument*/NULL);
		G_B12_0 = ((((int32_t)L_69) == ((int32_t)0))? 1 : 0);
		goto IL_013f;
	}

IL_013e:
	{
		G_B12_0 = 1;
	}

IL_013f:
	{
		V_9 = (bool)G_B12_0;
		bool L_70 = V_9;
		if (!L_70)
		{
			goto IL_0155;
		}
	}
	{
		int32_t L_71 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_71|(int32_t)8)));
	}

IL_0155:
	{
	}

IL_0156:
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_72 = V_4;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_73 = Object_op_Implicit_m8B2A44B4B1406ED346D1AE6D962294FD58D0D534(L_72, /*hidden argument*/NULL);
		V_10 = L_73;
		bool L_74 = V_10;
		if (!L_74)
		{
			goto IL_01e0;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_75 = V_0;
		NullCheck(L_75);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_76 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_75, /*hidden argument*/NULL);
		NullCheck(L_76);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_77 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_76, /*hidden argument*/NULL);
		float L_78 = L_77.get_x_2();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_79 = V_4;
		NullCheck(L_79);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_80 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_79, /*hidden argument*/NULL);
		NullCheck(L_80);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_81 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_80, /*hidden argument*/NULL);
		float L_82 = L_81.get_x_2();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		bool L_83 = Mathf_Approximately_m91AF00403E0D2DEA1AAE68601AD218CFAD70DF7E(L_78, L_82, /*hidden argument*/NULL);
		if (!L_83)
		{
			goto IL_01c8;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_84 = V_0;
		NullCheck(L_84);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_85 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_84, /*hidden argument*/NULL);
		NullCheck(L_85);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_86 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_85, /*hidden argument*/NULL);
		float L_87 = L_86.get_z_4();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_88 = V_0;
		NullCheck(L_88);
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_89 = Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2(L_88, /*hidden argument*/NULL);
		NullCheck(L_89);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_90 = TerrainData_get_size_m0987D18D442D824D5F9CF1CF5B42CCF1A6A42D51(L_89, /*hidden argument*/NULL);
		float L_91 = L_90.get_z_4();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_92 = V_4;
		NullCheck(L_92);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_93 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_92, /*hidden argument*/NULL);
		NullCheck(L_93);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_94 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_93, /*hidden argument*/NULL);
		float L_95 = L_94.get_z_4();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		bool L_96 = Mathf_Approximately_m91AF00403E0D2DEA1AAE68601AD218CFAD70DF7E(((float)il2cpp_codegen_add((float)L_87, (float)L_91)), L_95, /*hidden argument*/NULL);
		G_B19_0 = ((((int32_t)L_96) == ((int32_t)0))? 1 : 0);
		goto IL_01c9;
	}

IL_01c8:
	{
		G_B19_0 = 1;
	}

IL_01c9:
	{
		V_11 = (bool)G_B19_0;
		bool L_97 = V_11;
		if (!L_97)
		{
			goto IL_01df;
		}
	}
	{
		int32_t L_98 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_98|(int32_t)8)));
	}

IL_01df:
	{
	}

IL_01e0:
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_99 = V_5;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_100 = Object_op_Implicit_m8B2A44B4B1406ED346D1AE6D962294FD58D0D534(L_99, /*hidden argument*/NULL);
		V_12 = L_100;
		bool L_101 = V_12;
		if (!L_101)
		{
			goto IL_026b;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_102 = V_0;
		NullCheck(L_102);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_103 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_102, /*hidden argument*/NULL);
		NullCheck(L_103);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_104 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_103, /*hidden argument*/NULL);
		float L_105 = L_104.get_x_2();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_106 = V_5;
		NullCheck(L_106);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_107 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_106, /*hidden argument*/NULL);
		NullCheck(L_107);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_108 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_107, /*hidden argument*/NULL);
		float L_109 = L_108.get_x_2();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		bool L_110 = Mathf_Approximately_m91AF00403E0D2DEA1AAE68601AD218CFAD70DF7E(L_105, L_109, /*hidden argument*/NULL);
		if (!L_110)
		{
			goto IL_0253;
		}
	}
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_111 = V_0;
		NullCheck(L_111);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_112 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_111, /*hidden argument*/NULL);
		NullCheck(L_112);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_113 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_112, /*hidden argument*/NULL);
		float L_114 = L_113.get_z_4();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_115 = V_5;
		NullCheck(L_115);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_116 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_115, /*hidden argument*/NULL);
		NullCheck(L_116);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_117 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_116, /*hidden argument*/NULL);
		float L_118 = L_117.get_z_4();
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_119 = V_5;
		NullCheck(L_119);
		TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * L_120 = Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2(L_119, /*hidden argument*/NULL);
		NullCheck(L_120);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_121 = TerrainData_get_size_m0987D18D442D824D5F9CF1CF5B42CCF1A6A42D51(L_120, /*hidden argument*/NULL);
		float L_122 = L_121.get_z_4();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		bool L_123 = Mathf_Approximately_m91AF00403E0D2DEA1AAE68601AD218CFAD70DF7E(L_114, ((float)il2cpp_codegen_add((float)L_118, (float)L_122)), /*hidden argument*/NULL);
		G_B26_0 = ((((int32_t)L_123) == ((int32_t)0))? 1 : 0);
		goto IL_0254;
	}

IL_0253:
	{
		G_B26_0 = 1;
	}

IL_0254:
	{
		V_13 = (bool)G_B26_0;
		bool L_124 = V_13;
		if (!L_124)
		{
			goto IL_026a;
		}
	}
	{
		int32_t L_125 = __this->get_m_errorCode_1();
		__this->set_m_errorCode_1(((int32_t)((int32_t)L_125|(int32_t)8)));
	}

IL_026a:
	{
	}

IL_026b:
	{
	}

IL_026d:
	{
		return;
	}
}
// UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/ErrorCode UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap::Validate()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t TerrainMap_Validate_m31FE625EC81CDED0369413935CD78F355677237A (TerrainMap_t8D09DC412F632DAB9EA8FB7A11A34EB7464D547C * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainMap_Validate_m31FE625EC81CDED0369413935CD78F355677237A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639  V_0;
	memset((&V_0), 0, sizeof(V_0));
	TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  V_1;
	memset((&V_1), 0, sizeof(V_1));
	int32_t V_2 = 0;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		Dictionary_2_tEB34CAE1B4D0E725777A5B9D419AF51BE918C30C * L_0 = __this->get_m_terrainTiles_2();
		NullCheck(L_0);
		KeyCollection_tE38E8CA5A5DAD8EDFE46CBE8440DE1FB77D92EAB * L_1 = Dictionary_2_get_Keys_m6378916440BB5F09FDDC61026EB9E31F5BC8A9E1(L_0, /*hidden argument*/Dictionary_2_get_Keys_m6378916440BB5F09FDDC61026EB9E31F5BC8A9E1_RuntimeMethod_var);
		NullCheck(L_1);
		Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639  L_2 = KeyCollection_GetEnumerator_mB9D02C9BC28992C54914385A9ADB8B31645D8C29(L_1, /*hidden argument*/KeyCollection_GetEnumerator_mB9D02C9BC28992C54914385A9ADB8B31645D8C29_RuntimeMethod_var);
		V_0 = L_2;
	}

IL_0013:
	try
	{ // begin try (depth: 1)
		{
			goto IL_0032;
		}

IL_0015:
		{
			TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_3 = Enumerator_get_Current_mB84DB5A32BBDC3FBFB6E3419E2EE70128E320FE4_inline((Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639 *)(&V_0), /*hidden argument*/Enumerator_get_Current_mB84DB5A32BBDC3FBFB6E3419E2EE70128E320FE4_RuntimeMethod_var);
			V_1 = L_3;
			TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_4 = V_1;
			int32_t L_5 = L_4.get_tileX_0();
			TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_6 = V_1;
			int32_t L_7 = L_6.get_tileZ_1();
			TerrainMap_ValidateTerrain_m7B0154421B22B18D420FF7AB3179887AFCB320AB(__this, L_5, L_7, /*hidden argument*/NULL);
		}

IL_0032:
		{
			bool L_8 = Enumerator_MoveNext_mA095EEA83D1389075851A35D0CBDA713A34C2F7C((Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639 *)(&V_0), /*hidden argument*/Enumerator_MoveNext_mA095EEA83D1389075851A35D0CBDA713A34C2F7C_RuntimeMethod_var);
			if (L_8)
			{
				goto IL_0015;
			}
		}

IL_003b:
		{
			IL2CPP_LEAVE(0x4C, FINALLY_003d);
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_003d;
	}

FINALLY_003d:
	{ // begin finally (depth: 1)
		Enumerator_Dispose_m1AF30CB4B9D460F0CBE9944BBBE800205A8B84AF((Enumerator_tC227AD4879B6DF12FD67520BE6891D0106F2A639 *)(&V_0), /*hidden argument*/Enumerator_Dispose_m1AF30CB4B9D460F0CBE9944BBBE800205A8B84AF_RuntimeMethod_var);
		IL2CPP_END_FINALLY(61)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(61)
	{
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
		IL2CPP_JUMP_TBL(0x4C, IL_004c)
	}

IL_004c:
	{
		int32_t L_9 = __this->get_m_errorCode_1();
		V_2 = L_9;
		goto IL_0055;
	}

IL_0055:
	{
		int32_t L_10 = V_2;
		return L_10;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/<>c__DisplayClass4_0::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass4_0__ctor_m933028B07653F2470BD281F03CCA0AB74231B0A8 (U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002 * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/<>c__DisplayClass4_0::<CreateFromPlacement>b__0(UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool U3CU3Ec__DisplayClass4_0_U3CCreateFromPlacementU3Eb__0_m51A58F8F2A072A87B2E6C84818FEE0DEC2DD6FB0 (U3CU3Ec__DisplayClass4_0_t0F8BF12DC466A9414D9AF02F686C301DBAFED002 * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___x0, const RuntimeMethod* method)
{
	{
		Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * L_0 = ___x0;
		NullCheck(L_0);
		int32_t L_1 = Terrain_get_groupingID_mF2A964B8B4B049E4E443782AA951C4E85C6EC132(L_0, /*hidden argument*/NULL);
		int32_t L_2 = __this->get_groupID_0();
		return (bool)((((int32_t)L_1) == ((int32_t)L_2))? 1 : 0);
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainFilter__ctor_m60B330ACE5AE8B4833AFB8D9BB095D6783DB2F1E (TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter::Invoke(UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainFilter_Invoke_mB9F861A5CB34474898F150197A7F7CB90AFB1AF9 (TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain0, const RuntimeMethod* method)
{
	bool result = false;
	DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* delegateArrayToInvoke = __this->get_delegates_11();
	Delegate_t** delegatesToInvoke;
	il2cpp_array_size_t length;
	if (delegateArrayToInvoke != NULL)
	{
		length = delegateArrayToInvoke->max_length;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(delegateArrayToInvoke->GetAddressAtUnchecked(0));
	}
	else
	{
		length = 1;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(&__this);
	}

	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		Delegate_t* currentDelegate = delegatesToInvoke[i];
		Il2CppMethodPointer targetMethodPointer = currentDelegate->get_method_ptr_0();
		RuntimeObject* targetThis = currentDelegate->get_m_target_2();
		RuntimeMethod* targetMethod = (RuntimeMethod*)(currentDelegate->get_method_3());
		if (!il2cpp_codegen_method_is_virtual(targetMethod))
		{
			il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found(targetMethod);
		}
		bool ___methodIsStatic = MethodIsStatic(targetMethod);
		int ___parameterCount = il2cpp_codegen_method_parameter_count(targetMethod);
		if (___methodIsStatic)
		{
			if (___parameterCount == 1)
			{
				// open
				typedef bool (*FunctionPointerType) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___terrain0, targetMethod);
			}
			else
			{
				// closed
				typedef bool (*FunctionPointerType) (void*, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, targetMethod);
			}
		}
		else if (___parameterCount != 1)
		{
			// open
			if (il2cpp_codegen_method_is_virtual(targetMethod) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker0< bool >::Invoke(targetMethod, ___terrain0);
					else
						result = GenericVirtFuncInvoker0< bool >::Invoke(targetMethod, ___terrain0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker0< bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___terrain0);
					else
						result = VirtFuncInvoker0< bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___terrain0);
				}
			}
			else
			{
				typedef bool (*FunctionPointerType) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___terrain0, targetMethod);
			}
		}
		else
		{
			// closed
			if (targetThis != NULL && il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker1< bool, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * >::Invoke(targetMethod, targetThis, ___terrain0);
					else
						result = GenericVirtFuncInvoker1< bool, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * >::Invoke(targetMethod, targetThis, ___terrain0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker1< bool, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___terrain0);
					else
						result = VirtFuncInvoker1< bool, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___terrain0);
				}
			}
			else
			{
				if (targetThis == NULL && il2cpp_codegen_class_is_value_type(il2cpp_codegen_method_get_declaring_type(targetMethod)))
				{
					typedef bool (*FunctionPointerType) (RuntimeObject*, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)((reinterpret_cast<RuntimeObject*>(___terrain0) - 1), targetMethod);
				}
				if (targetThis == NULL)
				{
					typedef bool (*FunctionPointerType) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___terrain0, targetMethod);
				}
				else
				{
					typedef bool (*FunctionPointerType) (void*, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___terrain0, targetMethod);
				}
			}
		}
	}
	return result;
}
// System.IAsyncResult UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter::BeginInvoke(UnityEngine.Terrain,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TerrainFilter_BeginInvoke_mB6B8129534FBBB946AAAF055E7DFC909127E5021 (TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___terrain0, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback1, RuntimeObject * ___object2, const RuntimeMethod* method)
{
	void *__d_args[2] = {0};
	__d_args[0] = ___terrain0;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback1, (RuntimeObject*)___object2);
}
// System.Boolean UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TerrainFilter::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TerrainFilter_EndInvoke_m8200A6CAF424216D8AE088356ED10A055A83D2EA (TerrainFilter_t02BF9FBD8E61763D1D8484B34936B36B1046C66B * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
	return *(bool*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.TerrainAPI.TerrainUtility/TerrainMap/TileCoord::.ctor(System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileCoord__ctor_mAA64B48F381F5DCBB58B7EA137AD4073076177ED (TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___tileX0;
		__this->set_tileX_0(L_0);
		int32_t L_1 = ___tileZ1;
		__this->set_tileZ_1(L_1);
		return;
	}
}
IL2CPP_EXTERN_C  void TileCoord__ctor_mAA64B48F381F5DCBB58B7EA137AD4073076177ED_AdjustorThunk (RuntimeObject * __this, int32_t ___tileX0, int32_t ___tileZ1, const RuntimeMethod* method)
{
	int32_t _offset = 1;
	TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA * _thisAdjusted = reinterpret_cast<TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA *>(__this + _offset);
	TileCoord__ctor_mAA64B48F381F5DCBB58B7EA137AD4073076177ED(_thisAdjusted, ___tileX0, ___tileZ1, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// UnityEngine.TerrainData UnityEngine.Terrain::get_terrainData()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2 (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * __this, const RuntimeMethod* method)
{
	typedef TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * (*Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2_ftn) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *);
	static Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Terrain_get_terrainData_m85409C8644A110380504A9E71349B272941E77C2_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Terrain::get_terrainData()");
	TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Boolean UnityEngine.Terrain::get_allowAutoConnect()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Terrain_get_allowAutoConnect_m0968C0D1D5628726A19734808D1E37C44CA4F146 (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * __this, const RuntimeMethod* method)
{
	typedef bool (*Terrain_get_allowAutoConnect_m0968C0D1D5628726A19734808D1E37C44CA4F146_ftn) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *);
	static Terrain_get_allowAutoConnect_m0968C0D1D5628726A19734808D1E37C44CA4F146_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Terrain_get_allowAutoConnect_m0968C0D1D5628726A19734808D1E37C44CA4F146_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Terrain::get_allowAutoConnect()");
	bool retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Int32 UnityEngine.Terrain::get_groupingID()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Terrain_get_groupingID_mF2A964B8B4B049E4E443782AA951C4E85C6EC132 (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * __this, const RuntimeMethod* method)
{
	typedef int32_t (*Terrain_get_groupingID_mF2A964B8B4B049E4E443782AA951C4E85C6EC132_ftn) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *);
	static Terrain_get_groupingID_mF2A964B8B4B049E4E443782AA951C4E85C6EC132_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Terrain_get_groupingID_mF2A964B8B4B049E4E443782AA951C4E85C6EC132_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Terrain::get_groupingID()");
	int32_t retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Void UnityEngine.Terrain::SetNeighbors(UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Terrain_SetNeighbors_mA28EDA87B310AE170885473F6168B18849B55356 (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * __this, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___left0, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___top1, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___right2, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * ___bottom3, const RuntimeMethod* method)
{
	typedef void (*Terrain_SetNeighbors_mA28EDA87B310AE170885473F6168B18849B55356_ftn) (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *, Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 *);
	static Terrain_SetNeighbors_mA28EDA87B310AE170885473F6168B18849B55356_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Terrain_SetNeighbors_mA28EDA87B310AE170885473F6168B18849B55356_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Terrain::SetNeighbors(UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain,UnityEngine.Terrain)");
	_il2cpp_icall_func(__this, ___left0, ___top1, ___right2, ___bottom3);
}
// UnityEngine.Terrain[] UnityEngine.Terrain::get_activeTerrains()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA (const RuntimeMethod* method)
{
	typedef TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* (*Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA_ftn) ();
	static Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Terrain_get_activeTerrains_mDE09AD3E55E007F12799614A6215D2E2BFD82EDA_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Terrain::get_activeTerrains()");
	TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* retVal = _il2cpp_icall_func();
	return retVal;
}
// System.Void UnityEngine.Terrain::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Terrain__ctor_m1D3167E91CFC5220CF861F7CDE01A1F3C280BDCF (Terrain_t0BF7371FA90643325F50A87C7894D7BEBBE08943 * __this, const RuntimeMethod* method)
{
	{
		Behaviour__ctor_m409AEC21511ACF9A4CC0654DF4B8253E0D81D22C(__this, /*hidden argument*/NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Int32 UnityEngine.TerrainData::GetBoundaryValue(UnityEngine.TerrainData/BoundaryValueType)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB (int32_t ___type0, const RuntimeMethod* method)
{
	typedef int32_t (*TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB_ftn) (int32_t);
	static TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.TerrainData::GetBoundaryValue(UnityEngine.TerrainData/BoundaryValueType)");
	int32_t retVal = _il2cpp_icall_func(___type0);
	return retVal;
}
// System.Void UnityEngine.TerrainData::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData__ctor_mEF24945C9BBDA5CAFE4A1C453649B86D79DD87AF (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainData__ctor_mEF24945C9BBDA5CAFE4A1C453649B86D79DD87AF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		Object__ctor_m091EBAEBC7919B0391ABDAFB7389ADC12206525B(__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_il2cpp_TypeInfo_var);
		TerrainData_Internal_Create_m02C792919F391601D1EE4CF6DF70182FBD646F16(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.TerrainData::Internal_Create(UnityEngine.TerrainData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData_Internal_Create_m02C792919F391601D1EE4CF6DF70182FBD646F16 (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * ___terrainData0, const RuntimeMethod* method)
{
	typedef void (*TerrainData_Internal_Create_m02C792919F391601D1EE4CF6DF70182FBD646F16_ftn) (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 *);
	static TerrainData_Internal_Create_m02C792919F391601D1EE4CF6DF70182FBD646F16_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (TerrainData_Internal_Create_m02C792919F391601D1EE4CF6DF70182FBD646F16_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.TerrainData::Internal_Create(UnityEngine.TerrainData)");
	_il2cpp_icall_func(___terrainData0);
}
// UnityEngine.Vector3 UnityEngine.TerrainData::get_size()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  TerrainData_get_size_m0987D18D442D824D5F9CF1CF5B42CCF1A6A42D51 (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * __this, const RuntimeMethod* method)
{
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		TerrainData_get_size_Injected_mF6DEEE266FBF9CEC3AF2B6B77593B9704B299A68(__this, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_0), /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = V_0;
		return L_0;
	}
}
// System.Single UnityEngine.TerrainData::GetAlphamapResolutionInternal()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float TerrainData_GetAlphamapResolutionInternal_mA65CA918038A8D733648A2331826E2C0AA7316B6 (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * __this, const RuntimeMethod* method)
{
	typedef float (*TerrainData_GetAlphamapResolutionInternal_mA65CA918038A8D733648A2331826E2C0AA7316B6_ftn) (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 *);
	static TerrainData_GetAlphamapResolutionInternal_mA65CA918038A8D733648A2331826E2C0AA7316B6_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (TerrainData_GetAlphamapResolutionInternal_mA65CA918038A8D733648A2331826E2C0AA7316B6_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.TerrainData::GetAlphamapResolutionInternal()");
	float retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// UnityEngine.Terrain[] UnityEngine.TerrainData::get_users()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* TerrainData_get_users_m8DC41DB242FD51BDA25CE01F0AC2C019E05F8F76 (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * __this, const RuntimeMethod* method)
{
	typedef TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* (*TerrainData_get_users_m8DC41DB242FD51BDA25CE01F0AC2C019E05F8F76_ftn) (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 *);
	static TerrainData_get_users_m8DC41DB242FD51BDA25CE01F0AC2C019E05F8F76_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (TerrainData_get_users_m8DC41DB242FD51BDA25CE01F0AC2C019E05F8F76_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.TerrainData::get_users()");
	TerrainU5BU5D_t09516803A2C01893489D5ACAA202A907B2972BDE* retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Void UnityEngine.TerrainData::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData__cctor_mB579F93C53A8F85C72D7AA2C6A266DA7F0D066C5 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TerrainData__cctor_mB579F93C53A8F85C72D7AA2C6A266DA7F0D066C5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		int32_t L_0 = TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB(0, /*hidden argument*/NULL);
		((TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_il2cpp_TypeInfo_var))->set_k_MaximumResolution_4(L_0);
		int32_t L_1 = TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB(1, /*hidden argument*/NULL);
		((TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_il2cpp_TypeInfo_var))->set_k_MinimumDetailResolutionPerPatch_5(L_1);
		int32_t L_2 = TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB(2, /*hidden argument*/NULL);
		((TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_il2cpp_TypeInfo_var))->set_k_MaximumDetailResolutionPerPatch_6(L_2);
		int32_t L_3 = TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB(3, /*hidden argument*/NULL);
		((TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_il2cpp_TypeInfo_var))->set_k_MaximumDetailPatchCount_7(L_3);
		int32_t L_4 = TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB(4, /*hidden argument*/NULL);
		((TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_il2cpp_TypeInfo_var))->set_k_MaximumDetailsPerRes_8(L_4);
		int32_t L_5 = TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB(5, /*hidden argument*/NULL);
		((TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_il2cpp_TypeInfo_var))->set_k_MinimumAlphamapResolution_9(L_5);
		int32_t L_6 = TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB(6, /*hidden argument*/NULL);
		((TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_il2cpp_TypeInfo_var))->set_k_MaximumAlphamapResolution_10(L_6);
		int32_t L_7 = TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB(7, /*hidden argument*/NULL);
		((TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_il2cpp_TypeInfo_var))->set_k_MinimumBaseMapResolution_11(L_7);
		int32_t L_8 = TerrainData_GetBoundaryValue_m3E5DD81838828B30372AC5E200CE86B607C729AB(8, /*hidden argument*/NULL);
		((TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_StaticFields*)il2cpp_codegen_static_fields_for(TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2_il2cpp_TypeInfo_var))->set_k_MaximumBaseMapResolution_12(L_8);
		return;
	}
}
// System.Void UnityEngine.TerrainData::get_size_Injected(UnityEngine.Vector3&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TerrainData_get_size_Injected_mF6DEEE266FBF9CEC3AF2B6B77593B9704B299A68 (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___ret0, const RuntimeMethod* method)
{
	typedef void (*TerrainData_get_size_Injected_mF6DEEE266FBF9CEC3AF2B6B77593B9704B299A68_ftn) (TerrainData_t9D44396901570930AFE428DAC19ABE0C1477CFE2 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *);
	static TerrainData_get_size_Injected_mF6DEEE266FBF9CEC3AF2B6B77593B9704B299A68_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (TerrainData_get_size_Injected_mF6DEEE266FBF9CEC3AF2B6B77593B9704B299A68_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.TerrainData::get_size_Injected(UnityEngine.Vector3&)");
	_il2cpp_icall_func(__this, ___ret0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C  Enumerator_get_Current_m73F8FC135A20BDFEA0846D8FC876863ED85FEA49_gshared_inline (Enumerator_tC0410598D958D8FA61F7F1A803A0B7F242CEF1C6 * __this, const RuntimeMethod* method)
{
	{
		KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C  L_0 = (KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C )__this->get_current_3();
		return (KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C )L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR int32_t KeyValuePair_2_get_Key_m89B687D373FBE3D230647CA0C536235E12AD829C_gshared_inline (KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get_key_0();
		return (int32_t)L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR RuntimeObject * KeyValuePair_2_get_Value_mAD6801F3BC9BA1E99D4E0F72B2B420182D0494FC_gshared_inline (KeyValuePair_2_tC1FD9633618D9B27E2552BBAD347EC14A6C07C2C * __this, const RuntimeMethod* method)
{
	{
		RuntimeObject * L_0 = (RuntimeObject *)__this->get_value_1();
		return (RuntimeObject *)L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342  Enumerator_get_Current_m752CB433D17F421B29602443499F2A739C474AEF_gshared_inline (Enumerator_t71B93BE98D0EF45AF866DB4944B21CC1D0957B69 * __this, const RuntimeMethod* method)
{
	{
		KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342  L_0 = (KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342 )__this->get_current_3();
		return (KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342 )L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  KeyValuePair_2_get_Key_m33312D77486DAFBE08CA5AC179C8E82D027B1E12_gshared_inline (KeyValuePair_2_t198F3EF99C5CB706B8E678896CA900035FACF342 * __this, const RuntimeMethod* method)
{
	{
		TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_0 = (TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA )__this->get_key_0();
		return (TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA )L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  Enumerator_get_Current_mD5C37459762167496A915F0F05399DFDAC6B001D_gshared_inline (Enumerator_tEC4F5C6184F5E671D53C872D88266C1500841E16 * __this, const RuntimeMethod* method)
{
	{
		TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA  L_0 = (TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA )__this->get_currentKey_3();
		return (TileCoord_t51EDF1EA1A3A7F9C1D85C186E7A7954535C225BA )L_0;
	}
}
