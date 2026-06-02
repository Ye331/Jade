using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Jade.EditorTools
{
    public sealed class ShanhaiGateTerrainCoverWindow : EditorWindow
    {
        private Sprite terrainSprite;
        private Sprite platformLongSprite;
        private Sprite platformShortSprite;
        private Sprite blockThickSprite;
        private Sprite wallVerticalSprite;
        private Sprite pitRimSprite;
        private bool hideGrayboxRenderer = true;
        private int sortingOrderOffset = 1;
        private float widthMultiplier = 1f;
        private float heightMultiplier = 1f;
        private float topEdgeYOffset;
        private float wallXOffset;

        [MenuItem("Jade/ShanhaiGate Terrain Cover Window")]
        private static void Open()
        {
            GetWindow<ShanhaiGateTerrainCoverWindow>("Terrain Cover");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("ShanhaiGate Terrain Cover", EditorStyles.boldLabel);
            EditorGUILayout.Space(4f);

            DrawBatchSpriteFields();
            EditorGUILayout.Space(8f);

            terrainSprite = (Sprite)EditorGUILayout.ObjectField("Terrain Sprite", terrainSprite, typeof(Sprite), false);

            EditorGUILayout.Space(6f);
            hideGrayboxRenderer = EditorGUILayout.Toggle("Hide Graybox Renderer", hideGrayboxRenderer);
            sortingOrderOffset = EditorGUILayout.IntField("Sorting Order Offset", sortingOrderOffset);
            widthMultiplier = EditorGUILayout.Slider("Width Multiplier", widthMultiplier, 0.5f, 1.5f);
            heightMultiplier = EditorGUILayout.Slider("Height Multiplier", heightMultiplier, 0.5f, 2f);
            topEdgeYOffset = EditorGUILayout.Slider("Top Edge Y Offset", topEdgeYOffset, -0.5f, 0.5f);
            wallXOffset = EditorGUILayout.Slider("Wall X Offset", wallXOffset, -0.5f, 0.5f);

            EditorGUILayout.Space(8f);
            using (new EditorGUI.DisabledScope(terrainSprite == null || Selection.gameObjects.Length == 0))
            {
                if (GUILayout.Button("Apply To Selected Terrain"))
                {
                    ApplyToSelection();
                }
            }

            using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
            {
                if (GUILayout.Button("Apply Typed Covers To Selected Terrain"))
                {
                    ApplyTypedCoversToSelection();
                }
            }

            EditorGUILayout.HelpBox(
                "Select graybox terrain GameObjects in the Hierarchy, assign a final transparent terrain Sprite here, then apply. Platforms align the sprite top edge to the BoxCollider2D top and let the art extend downward.",
                MessageType.Info);
        }

        private void DrawBatchSpriteFields()
        {
            EditorGUILayout.LabelField("Typed Final Terrain Sprites", EditorStyles.boldLabel);
            platformLongSprite = (Sprite)EditorGUILayout.ObjectField("Platform Long", platformLongSprite, typeof(Sprite), false);
            platformShortSprite = (Sprite)EditorGUILayout.ObjectField("Platform Short", platformShortSprite, typeof(Sprite), false);
            blockThickSprite = (Sprite)EditorGUILayout.ObjectField("Block Thick", blockThickSprite, typeof(Sprite), false);
            wallVerticalSprite = (Sprite)EditorGUILayout.ObjectField("Wall Vertical", wallVerticalSprite, typeof(Sprite), false);
            pitRimSprite = (Sprite)EditorGUILayout.ObjectField("Pit Rim", pitRimSprite, typeof(Sprite), false);
        }

        private void ApplyToSelection()
        {
            var options = CreateOptions();
            int appliedCount = 0;
            foreach (GameObject selected in Selection.gameObjects)
            {
                if (ShanhaiGateTerrainSkinTool.ApplySpriteAsCover(selected, terrainSprite, options))
                {
                    appliedCount++;
                }
            }

            if (appliedCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            Debug.Log($"Applied terrain cover to {appliedCount} selected object(s).");
        }

        private void ApplyTypedCoversToSelection()
        {
            var options = CreateOptions();
            int appliedCount = 0;
            int skippedCount = 0;

            foreach (GameObject selected in Selection.gameObjects)
            {
                Sprite sprite = PickSpriteForTerrain(selected);
                if (sprite == null)
                {
                    skippedCount++;
                    continue;
                }

                if (ShanhaiGateTerrainSkinTool.ApplySpriteAsCover(selected, sprite, options))
                {
                    appliedCount++;
                }
                else
                {
                    skippedCount++;
                }
            }

            if (appliedCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            Debug.Log($"Applied typed terrain covers to {appliedCount} selected object(s). Skipped {skippedCount} object(s).");
        }

        private ShanhaiGateTerrainSkinTool.TerrainCoverOptions CreateOptions()
        {
            return new ShanhaiGateTerrainSkinTool.TerrainCoverOptions
            {
                HideGrayboxRenderer = hideGrayboxRenderer,
                SortingOrderOffset = sortingOrderOffset,
                WidthMultiplier = widthMultiplier,
                HeightMultiplier = heightMultiplier,
                TopEdgeYOffset = topEdgeYOffset,
                WallXOffset = wallXOffset
            };
        }

        private Sprite PickSpriteForTerrain(GameObject terrain)
        {
            BoxCollider2D collider = terrain.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                return null;
            }

            string lowerName = terrain.name.ToLowerInvariant();
            if (lowerName.Contains("wall"))
            {
                return wallVerticalSprite;
            }

            if (lowerName.Contains("gap") || lowerName.Contains("trap"))
            {
                return pitRimSprite;
            }

            Vector2 worldSize = Vector2.Scale(collider.size, Abs(terrain.transform.lossyScale));
            if (worldSize.y >= 1.5f)
            {
                return blockThickSprite;
            }

            return worldSize.x >= 8f ? platformLongSprite : platformShortSprite;
        }

        private static Vector2 Abs(Vector3 value)
        {
            return new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y));
        }
    }
}
