using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using System.IO;

namespace BigBackpack2
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony? _harmony = null;
        public static float InventoryCapacityIncrease { get; private set; } = 50f;
        public static float MaxWeightIncrease { get; private set; } = 200f;

        protected override void OnAfterSetup()
        {
            Debug.Log("BigBackpack2模组：OnAfterSetup方法被调用");
            LoadConfig();
            if (_harmony != null)
            {
                Debug.Log("BigBackpack2模组：已修补 先卸载");
                _harmony.UnpatchAll();
            }

            Debug.Log("BigBackpack2模组：执行修补");
            _harmony = new Harmony("Lexcellent.BigBackpack2");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Debug.Log("BigBackpack2模组：修补完成");
        }

        protected override void OnBeforeDeactivate()
        {
            Debug.Log("BigBackpack2模组：OnBeforeDeactivate方法被调用");
            Debug.Log("BigBackpack2模组：执行取消修补");
            if (_harmony != null)
            {
                _harmony.UnpatchAll();
            }

            Debug.Log("BigBackpack2模组：执行取消修补完毕");
        }

        private void LoadConfig()
        {
            try
            {
                string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "info.ini");
                if (File.Exists(configPath))
                {
                    string[] lines = File.ReadAllLines(configPath);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("InventoryCapacity="))
                        {
                            string value = line.Substring("InventoryCapacity=".Length).Trim();
                            if (float.TryParse(value, out float capacity))
                            {
                                InventoryCapacityIncrease = capacity;
                                Debug.Log($"BigBackpack2模组：已从配置文件读取InventoryCapacity值: {capacity}");
                            }
                        }
                        else if (line.StartsWith("MaxWeight="))
                        {
                            string value = line.Substring("MaxWeight=".Length).Trim();
                            if (float.TryParse(value, out float weight))
                            {
                                MaxWeightIncrease = weight;
                                Debug.Log($"BigBackpack2模组：已从配置文件读取MaxWeight值: {weight}");
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("BigBackpack2模组：未找到info.ini文件，使用默认值");
                }
            }
            catch (Exception e)
            {
                Debug.Log($"BigBackpack2模组：读取配置文件时出错：{e.Message}，使用默认值");
            }
        }
    }
}