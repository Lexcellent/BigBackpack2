using System;
using Duckov.UI;
using HarmonyLib;
using ItemStatsSystem;
using ItemStatsSystem.Data;
using UnityEngine;

namespace BigBackpack2
{
    [HarmonyPatch(typeof(CharacterMainControl))]
    public static class InventoryCapacityPatch
    {
        // 将通知逻辑提取为协程方法
        private static System.Collections.IEnumerator DelayedNotification( int addCount)
        {
            yield return new WaitForSeconds(1f); // 延迟1秒
      
        }
        
        [HarmonyPatch("InventoryCapacity", MethodType.Getter)]
        [HarmonyPostfix]
        public static void PostfixInventoryCapacity(CharacterMainControl __instance, ref float __result)
        {
            try
            {
                // Debug.Log($"BigBackpack2模组：角色所属阵营{__instance.Team},原始背包容量：{__result}");
                if (__instance != null && __instance.Team == Teams.player)
                {
                    __result += ModBehaviour.InventoryCapacityIncrease;
                    // 检查玩家背包大小是否超过背包内物品量

                    var backpackItems = __instance.CharacterItem.Inventory.Content;
                    if (backpackItems != null && backpackItems.Count > __result)
                    {
                        var addCount = 0;
                        // 将超出的部分存入 马蜂自提点
                        for (int i = 0; i < backpackItems.Count; i++)
                        {
                            if (i >= __result)
                            {
                                var backItem = backpackItems[i];
                                PlayerStorage.IncomingItemBuffer.Add(ItemTreeData.FromItem(backItem));
                                backItem.Detach();
                                backItem.DestroyTree();
                                addCount += 1;
                            }
                        }
                        if (addCount > 0)
                        {
                            try
                            {
                                NotificationText.Push($"检测到背包容量异常，有{addCount}个物品发送到[马蜂自提点]");
                            }
                            catch (Exception e)
                            {
                                Debug.Log($"BigBackpack2模组：错误：{e.Message}");
                            }
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"BigBackpack2模组：错误：{e.Message}");
            }
        }

        [HarmonyPatch("MaxWeight", MethodType.Getter)]
        [HarmonyPostfix]
        public static void PostfixMaxWeight(CharacterMainControl __instance, ref float __result)
        {
            try
            {
                if (__instance != null && __instance.Team == Teams.player)
                {
                    __result += ModBehaviour.MaxWeightIncrease;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"BigBackpack2模组：错误：{e.Message}");
            }
        }
    }
}