using BepInEx;
using HarmonyLib;
using System.IO;
using UnityEngine;

namespace SillyCompany
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class SillyCompany : BaseUnityPlugin
    {
        private const string PLUGIN_OWNER_PREFIX = "silly_capybara-";

        private static readonly string PostersImagePath = Path.Combine(
            Paths.PluginPath,
            PLUGIN_OWNER_PREFIX + PluginInfo.PLUGIN_NAME + "-" + PluginInfo.PLUGIN_VERSION, "posters.png"
        );
        private static readonly string TipsImagePath = Path.Combine(
            Paths.PluginPath,
            PLUGIN_OWNER_PREFIX + PluginInfo.PLUGIN_NAME + "-" + PluginInfo.PLUGIN_VERSION, "tips.png"
        );

        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            harmony.PatchAll(typeof(SillyCompany));
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        private static void StartPatch()
        {
            UpdateMaterials(0);
        }

        [HarmonyPatch(typeof(RoundManager), "GenerateNewLevelClientRpc")]
        [HarmonyPostfix]
        private static void GenerateNewLevelClientRpcPatch(int randomSeed)
        {
            UpdateMaterials(randomSeed);
        }

        private static void UpdateMaterials(int seed)
        {
            var materials = GameObject.Find("HangarShip/Plane.001").GetComponent<MeshRenderer>().materials;

            UpdateTexture(PostersImagePath, materials[0]);
            UpdateTexture(TipsImagePath, materials[1]);
        }

        private static void UpdateTexture(string filePath, Material material)
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(filePath));

            material.mainTexture = texture;
        }
    }
}
