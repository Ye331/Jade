using UnityEditor;
using UnityEngine;

namespace Jade.EditorTools
{
    public class JadeSpiritSpriteImportPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            string normalizedPath = assetPath.Replace('\\', '/');
            if (!IsJadeSpiritFramePath(normalizedPath))
            {
                return;
            }

            TextureImporter importer = (TextureImporter)assetImporter;
            JadeSpiritPlayerAssetBuilder.ApplyCharacterTextureSettings(importer);
        }

        private static bool IsJadeSpiritFramePath(string path)
        {
            return path.Contains("/Resources/Characters/JadeQilinFrames/")
                || path.Contains("/Resources/Characters/JadeSpiritHighResFrames/")
                || path.Contains("/Resources/Characters/JadeQilinHighResFrames/")
                || path.Contains("/Art/PrototypeArt/Characters/JadeQilinFrames/");
        }
    }
}
