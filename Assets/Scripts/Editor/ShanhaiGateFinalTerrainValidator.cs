using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Jade.EditorTools
{
    public static class ShanhaiGateFinalTerrainValidator
    {
        private const string FinalTerrainPath = "Assets/Art/Environments/ShanhaiGate/FinalTerrain/";

        private static readonly TerrainSpriteSpec[] RequiredSprites =
        {
            new TerrainSpriteSpec("Platform_Long", 2048, 512, true),
            new TerrainSpriteSpec("Platform_Short", 1024, 384, true),
            new TerrainSpriteSpec("Block_Thick", 1024, 768, true),
            new TerrainSpriteSpec("Wall_Vertical", 512, 2048, false),
            new TerrainSpriteSpec("Pit_Rim", 1024, 256, false)
        };

        [MenuItem("Jade/Validate ShanhaiGate Final Terrain Sprites")]
        private static void ValidateFinalTerrainSprites()
        {
            List<string> failures = new List<string>();
            foreach (TerrainSpriteSpec spec in RequiredSprites)
            {
                ValidateSprite(spec, failures);
            }

            if (failures.Count == 0)
            {
                Debug.Log("ShanhaiGate final terrain validation passed.");
                return;
            }

            Debug.LogWarning("ShanhaiGate final terrain validation found issues:\n" + string.Join("\n", failures));
        }

        private static void ValidateSprite(TerrainSpriteSpec spec, List<string> failures)
        {
            string path = FinalTerrainPath + spec.AssetName + ".png";
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (texture == null || sprite == null || importer == null)
            {
                failures.Add($"- Missing or not imported as Sprite: {path}");
                return;
            }

            if (texture.width != spec.Width || texture.height != spec.Height)
            {
                failures.Add($"- {spec.AssetName}: expected {spec.Width}x{spec.Height}, got {texture.width}x{texture.height}.");
            }

            if (importer.textureType != TextureImporterType.Sprite)
            {
                failures.Add($"- {spec.AssetName}: Texture Type must be Sprite.");
            }

            if (importer.spritePixelsPerUnit != 100f)
            {
                failures.Add($"- {spec.AssetName}: Pixels Per Unit should be 100.");
            }

            if (importer.mipmapEnabled)
            {
                failures.Add($"- {spec.AssetName}: Mipmaps should be disabled.");
            }

            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                failures.Add($"- {spec.AssetName}: Texture compression should be Uncompressed.");
            }

            if (importer.wrapMode != TextureWrapMode.Clamp)
            {
                failures.Add($"- {spec.AssetName}: Wrap Mode should be Clamp.");
            }

            if (spec.RequiresSolidTopEdge)
            {
                ValidateTopEdge(path, texture, spec.AssetName, failures);
            }
        }

        private static void ValidateTopEdge(string path, Texture2D importedTexture, string assetName, List<string> failures)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D readableTexture = new Texture2D(importedTexture.width, importedTexture.height, TextureFormat.RGBA32, false);
            if (!readableTexture.LoadImage(bytes))
            {
                failures.Add($"- {assetName}: could not read PNG pixels for top-edge validation.");
                Object.DestroyImmediate(readableTexture);
                return;
            }

            int firstOpaqueRow = FindFirstOpaqueRow(readableTexture);
            if (firstOpaqueRow < 0)
            {
                failures.Add($"- {assetName}: sprite appears fully transparent.");
                Object.DestroyImmediate(readableTexture);
                return;
            }

            if (firstOpaqueRow > Mathf.Max(4, readableTexture.height / 32))
            {
                failures.Add($"- {assetName}: top playable edge starts too low ({firstOpaqueRow}px from top). Trim transparent padding above it.");
            }

            float topCoverage = MeasureOpaqueCoverage(readableTexture, firstOpaqueRow, 6);
            if (topCoverage < 0.65f)
            {
                failures.Add($"- {assetName}: top edge is too sparse ({topCoverage:P0} opaque coverage). Keep platform landing edge readable.");
            }

            Object.DestroyImmediate(readableTexture);
        }

        private static int FindFirstOpaqueRow(Texture2D texture)
        {
            for (int y = texture.height - 1; y >= 0; y--)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    if (texture.GetPixel(x, y).a > 0.1f)
                    {
                        return texture.height - 1 - y;
                    }
                }
            }

            return -1;
        }

        private static float MeasureOpaqueCoverage(Texture2D texture, int rowFromTop, int rowCount)
        {
            int sampleCount = 0;
            int opaqueCount = 0;
            int startY = texture.height - 1 - rowFromTop;
            int endY = Mathf.Max(0, startY - rowCount + 1);

            for (int y = startY; y >= endY; y--)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    sampleCount++;
                    if (texture.GetPixel(x, y).a > 0.1f)
                    {
                        opaqueCount++;
                    }
                }
            }

            return sampleCount == 0 ? 0f : (float)opaqueCount / sampleCount;
        }

        private readonly struct TerrainSpriteSpec
        {
            public readonly string AssetName;
            public readonly int Width;
            public readonly int Height;
            public readonly bool RequiresSolidTopEdge;

            public TerrainSpriteSpec(string assetName, int width, int height, bool requiresSolidTopEdge)
            {
                AssetName = assetName;
                Width = width;
                Height = height;
                RequiresSolidTopEdge = requiresSolidTopEdge;
            }
        }
    }
}
