using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SillyCompany
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class SillyCompany : BaseUnityPlugin
    {
        public static List<string> PosterFolders = new List<string>();
        public static readonly List<string> PosterFiles = new List<string>();
        public static readonly List<string> TipFiles = new List<string>();
        private static System.Random Rand = new System.Random();

        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            PosterFolders = Directory.GetDirectories(Paths.PluginPath, PluginInfo.PLUGIN_NAME, SearchOption.AllDirectories).ToList();

            foreach (var folder in PosterFolders)
            {
                foreach (var file in Directory.GetFiles(Path.Combine(folder, "posters")))
                {
                    PosterFiles.Add(file);
                }

                foreach (var file in Directory.GetFiles(Path.Combine(folder, "tips")))
                {
                    Logger.LogInfo($"{file}");
                    TipFiles.Add(file);
                }
            }

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
            Rand = new System.Random(seed);

            var materials = GameObject.Find("HangarShip/Plane.001").GetComponent<MeshRenderer>().materials;

            UpdateTexture(PosterFiles, materials[0]);
            UpdateTexture(TipFiles, materials[1]);
        }

        private static void UpdateTexture(IReadOnlyList<string> files, Material material)
        {
            if (files.Count == 0) { return; }

            var index = Rand.Next(files.Count);

            var texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(files[index]));

            material.mainTexture = texture;
        }
    }
}
