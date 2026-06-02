using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Jade.EditorTools
{
    public static class ShanhaiGateGroundedTerrainTool
    {
        private const string RootName = "Art_GroundedTerrain";
        private const string TerrainPath = "Assets/Art/Environments/ShanhaiGate/FinalTerrain/";
        private const string WallPath = TerrainPath + "DropWallComponents/";
        private const float UpperCameraBottomY = -3.85f;
        private const float LowerCameraBottomY = -7.25f;
        private const float MinimumLandDepth = 1.25f;
        private const float MaximumLandDepth = 4.25f;

        [MenuItem("Jade/Create Grounded Terrain Supports")]
        private static void CreateGroundedTerrainSupports()
        {
            Sprite landFillSprite = LoadSprite(TerrainPath + "Block_Thick.png");
            Sprite seamSprite = LoadSprite(WallPath + "DropWall_Seam_Trim.png");
            Sprite rootSprite = LoadSprite(WallPath + "DropWall_Bottom_Cap_Rooted.png");

            if (landFillSprite == null)
            {
                Debug.LogWarning("Missing grounded terrain sprite: " + TerrainPath + "Block_Thick.png");
                return;
            }

            GameObject root = GameObject.Find(RootName);
            if (root == null)
            {
                root = new GameObject(RootName);
                Undo.RegisterCreatedObjectUndo(root, "Create grounded terrain root");
            }
            else
            {
                Undo.RecordObject(root, "Refresh grounded terrain");
                ClearChildren(root.transform);
            }

            root.transform.position = Vector3.zero;

            int count = 0;
            foreach (BoxCollider2D collider in Object.FindObjectsByType<BoxCollider2D>(FindObjectsSortMode.None))
            {
                GameObject terrain = collider.gameObject;
                if (!ShouldGround(terrain, collider))
                {
                    continue;
                }

                Bounds bounds = GetWorldBounds(terrain.transform, collider);
                float visibleBottomY = bounds.center.x >= 100f || bounds.min.y < -2f ? LowerCameraBottomY : UpperCameraBottomY;
                float landDepth = Mathf.Clamp(bounds.min.y - visibleBottomY + 0.45f, MinimumLandDepth, MaximumLandDepth);
                if (landDepth < MinimumLandDepth)
                {
                    continue;
                }

                GameObject support = CreateChild(root.transform, "Art_GroundSupport_" + terrain.name);
                float width = Mathf.Max(bounds.size.x * 0.96f, 2.2f);
                float topY = bounds.min.y + 0.12f;
                float centerY = topY - landDepth * 0.5f;
                CreateTiledRenderer(support.transform, "LandBody", landFillSprite, 1, new Vector2(width, landDepth), new Vector3(bounds.center.x, centerY, 0f), new Color(0.94f, 0.98f, 0.94f, 0.92f));

                if (seamSprite != null)
                {
                    CreateRenderer(support.transform, "SoftTopJoin", seamSprite, 6, new Vector2(width * 1.02f, 0.28f), new Vector3(bounds.center.x, bounds.min.y - 0.08f, 0f), new Color(1f, 1f, 1f, 0.82f));
                }

                if (rootSprite != null && bounds.size.x >= 5f)
                {
                    float rootWidth = Mathf.Min(width * 0.72f, 8f);
                    CreateRenderer(support.transform, "SubtleRootBase", rootSprite, 3, new Vector2(rootWidth, 0.9f), new Vector3(bounds.center.x, topY - landDepth + 0.32f, 0f), new Color(1f, 1f, 1f, 0.72f));
                }

                count++;
            }

            EditorUtility.SetDirty(root);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("Created grounded terrain supports: " + count);
        }

        private static bool ShouldGround(GameObject terrain, BoxCollider2D collider)
        {
            string name = terrain.name.ToLowerInvariant();
            if (name.Contains("wall") || name.Contains("trap") || name.Contains("player") || name.Contains("monster") || name.Contains("respawn") || name.Contains("invisible"))
            {
                return false;
            }

            if (!(name.Contains("land") || name.Contains("gap") || name.Contains("start") || name.Contains("breakableplatform")))
            {
                return false;
            }

            Bounds bounds = GetWorldBounds(terrain.transform, collider);
            return bounds.size.x >= 2f && bounds.size.y <= 4.2f && bounds.min.y > -8f;
        }

        private static Bounds GetWorldBounds(Transform transform, BoxCollider2D collider)
        {
            Vector3 center = transform.TransformPoint(collider.offset);
            Vector3 size = Vector2.Scale(collider.size, new Vector2(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y)));
            return new Bounds(center, size);
        }

        private static GameObject CreateChild(Transform parent, string objectName)
        {
            GameObject child = new GameObject(objectName);
            Undo.RegisterCreatedObjectUndo(child, "Create grounded terrain piece");
            child.transform.SetParent(parent);
            child.transform.position = Vector3.zero;
            return child;
        }

        private static void CreateRenderer(Transform parent, string objectName, Sprite sprite, int sortingOrder, Vector2 worldSize, Vector3 worldPosition, Color color)
        {
            GameObject child = CreateChild(parent, objectName);
            child.transform.position = worldPosition;

            SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;

            Vector2 spriteSize = sprite.bounds.size;
            child.transform.localScale = new Vector3(
                worldSize.x / Mathf.Max(spriteSize.x, 0.01f),
                worldSize.y / Mathf.Max(spriteSize.y, 0.01f),
                1f);

            EditorUtility.SetDirty(child);
            EditorUtility.SetDirty(renderer);
        }

        private static void CreateTiledRenderer(Transform parent, string objectName, Sprite sprite, int sortingOrder, Vector2 worldSize, Vector3 worldPosition, Color color)
        {
            GameObject child = CreateChild(parent, objectName);
            child.transform.position = worldPosition;
            child.transform.localScale = Vector3.one;

            SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            renderer.drawMode = SpriteDrawMode.Tiled;
            renderer.tileMode = SpriteTileMode.Continuous;
            renderer.size = worldSize;

            EditorUtility.SetDirty(child);
            EditorUtility.SetDirty(renderer);
        }

        private static Sprite LoadSprite(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static void ClearChildren(Transform root)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in root)
            {
                children.Add(child.gameObject);
            }

            foreach (GameObject child in children)
            {
                Undo.DestroyObjectImmediate(child);
            }
        }
    }
}
