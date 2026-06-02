using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Jade.EditorTools
{
    public static class ShanhaiGateTerrainSkinTool
    {
        [MenuItem("Jade/Apply Selected Sprite As Terrain Cover")]
        private static void ApplySelectedSpriteAsTerrainCover()
        {
            Sprite sprite = FindSelectedSprite();
            if (sprite == null)
            {
                Debug.LogWarning("Select one Sprite asset and one or more terrain GameObjects before applying terrain cover.");
                return;
            }

            foreach (GameObject selected in Selection.gameObjects)
            {
                ApplySpriteAsCover(selected, sprite);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("Jade/Apply Selected Sprite As Terrain Cover", true)]
        private static bool CanApplySelectedSpriteAsTerrainCover()
        {
            return FindSelectedSprite() != null && Selection.gameObjects.Length > 0;
        }

        public static bool ApplySpriteAsCover(GameObject terrain, Sprite sprite, TerrainCoverOptions options = null)
        {
            BoxCollider2D collider = terrain.GetComponent<BoxCollider2D>();
            SpriteRenderer sourceRenderer = terrain.GetComponent<SpriteRenderer>();
            if (collider == null || sourceRenderer == null || sprite == null)
            {
                return false;
            }

            options ??= TerrainCoverOptions.Default;

            Transform skinTransform = terrain.transform.Find("Art_TerrainSkin");
            GameObject skinObject;
            if (skinTransform == null)
            {
                skinObject = new GameObject("Art_TerrainSkin");
                skinTransform = skinObject.transform;
                Undo.RegisterCreatedObjectUndo(skinObject, "Create terrain cover");
                skinTransform.SetParent(terrain.transform);
            }
            else
            {
                skinObject = skinTransform.gameObject;
                Undo.RecordObject(skinObject, "Update terrain cover");
            }

            SpriteRenderer skinRenderer = skinObject.GetComponent<SpriteRenderer>();
            if (skinRenderer == null)
            {
                skinRenderer = skinObject.AddComponent<SpriteRenderer>();
            }

            Undo.RecordObject(sourceRenderer, "Hide graybox renderer");
            Undo.RecordObject(skinRenderer, "Update terrain cover renderer");

            sourceRenderer.enabled = !options.HideGrayboxRenderer;
            skinRenderer.sprite = sprite;
            skinRenderer.color = Color.white;
            skinRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
            skinRenderer.sortingOrder = sourceRenderer.sortingOrder + options.SortingOrderOffset;

            Bounds spriteBounds = sprite.bounds;
            Vector2 worldSize = Vector2.Scale(collider.size, Abs(terrain.transform.lossyScale));
            bool isWall = worldSize.y > worldSize.x * 1.6f || terrain.name.ToLowerInvariant().Contains("wall");
            float targetWidth = Mathf.Max(worldSize.x * options.WidthMultiplier, 0.01f);
            float targetHeight = isWall
                ? Mathf.Max(worldSize.y, 0.01f)
                : targetWidth * spriteBounds.size.y / Mathf.Max(spriteBounds.size.x, 0.01f);
            targetHeight *= options.HeightMultiplier;

            Vector3 parentScale = terrain.transform.lossyScale;
            skinTransform.localScale = new Vector3(
                targetWidth / Mathf.Max(spriteBounds.size.x, 0.01f) / Mathf.Max(Mathf.Abs(parentScale.x), 0.0001f),
                targetHeight / Mathf.Max(spriteBounds.size.y, 0.01f) / Mathf.Max(Mathf.Abs(parentScale.y), 0.0001f),
                1f);

            Vector3 worldCenter = terrain.transform.TransformPoint(collider.offset);
            Vector3 skinWorldPosition = worldCenter;
            if (!isWall)
            {
                float colliderTop = worldCenter.y + worldSize.y * 0.5f;
                skinWorldPosition.y = colliderTop - targetHeight * 0.5f + options.TopEdgeYOffset;
            }
            else
            {
                skinWorldPosition.x += options.WallXOffset;
            }

            skinTransform.position = skinWorldPosition;
            skinTransform.localRotation = Quaternion.identity;

            EditorUtility.SetDirty(terrain);
            EditorUtility.SetDirty(skinObject);
            return true;
        }

        private static Vector2 Abs(Vector3 value)
        {
            return new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y));
        }

        private static Sprite FindSelectedSprite()
        {
            foreach (Object selected in Selection.objects)
            {
                if (selected is Sprite sprite)
                {
                    return sprite;
                }
            }

            return null;
        }

        public sealed class TerrainCoverOptions
        {
            public static readonly TerrainCoverOptions Default = new TerrainCoverOptions();

            public bool HideGrayboxRenderer = true;
            public int SortingOrderOffset = 1;
            public float WidthMultiplier = 1f;
            public float HeightMultiplier = 1f;
            public float TopEdgeYOffset;
            public float WallXOffset;
        }
    }
}
