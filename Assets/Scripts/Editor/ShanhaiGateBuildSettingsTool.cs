using System.Collections.Generic;
using UnityEditor;

namespace Jade.EditorTools
{
    public static class ShanhaiGateBuildSettingsTool
    {
        [MenuItem("Jade/Add Shanhai Main Scenes To Build Settings")]
        private static void AddShanhaiMainScenesToBuildSettings()
        {
            string[] requiredScenes =
            {
                "Assets/Scenes/ShanhaiGate.scene",
                "Assets/Scenes/ShanhaiCave01.scene",
                "Assets/Scenes/ShanhaiCave02.scene",
            };

            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            foreach (string path in requiredScenes)
            {
                bool exists = false;
                for (int i = 0; i < scenes.Count; i++)
                {
                    if (scenes[i].path == path)
                    {
                        scenes[i] = new EditorBuildSettingsScene(path, true);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    scenes.Add(new EditorBuildSettingsScene(path, true));
                }
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            AssetDatabase.SaveAssets();
        }
    }
}
