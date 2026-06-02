using System.IO;
using UnityEditor;
using UnityEngine;

namespace Jade.EditorTools
{
    public sealed class ShanhaiGateFinalTerrainImportPostprocessor : AssetPostprocessor
    {
        private const string FinalTerrainPath = "Assets/Art/Environments/ShanhaiGate/FinalTerrain/";
        private const string CaveTerrainPath = "Assets/Art/Environments/ShanhaiGate/CaveTerrain/";
        private const string BackgroundPath = "Assets/Art/Environments/ShanhaiGate/Backgrounds/";
        private const string MonsterLobberPath = "Assets/Art/Enemies/MonsterLobber/";

        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(FinalTerrainPath)
                && !assetPath.StartsWith(CaveTerrainPath)
                && !assetPath.StartsWith(BackgroundPath)
                && !assetPath.StartsWith(MonsterLobberPath))
            {
                return;
            }

            string extension = Path.GetExtension(assetPath).ToLowerInvariant();
            if (extension != ".png" && extension != ".psd" && extension != ".tga")
            {
                return;
            }

            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100f;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.npotScale = TextureImporterNPOTScale.None;

            TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
            defaultSettings.maxTextureSize = 4096;
            defaultSettings.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SetPlatformTextureSettings(defaultSettings);
        }
    }
}
