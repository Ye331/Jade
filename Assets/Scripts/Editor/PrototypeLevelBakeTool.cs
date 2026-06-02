using System.Collections.Generic;
using System.Reflection;
using Jade.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jade.EditorTools
{
    public static class PrototypeLevelBakeTool
    {
        private const string BakedRootName = "__BakedGrayboxLevel";

        [MenuItem("Jade/Bake Current Graybox Level To Scene")]
        public static void BakeCurrentPrototypeLevelToScene()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Bake Graybox Level", "Exit Play Mode before baking a graybox level.", "OK");
                return;
            }

            Scene scene = SceneManager.GetActiveScene();
            MonoBehaviour builder = FindSceneBuilder(scene);
            if (builder == null)
            {
                EditorUtility.DisplayDialog("Bake Graybox Level", "No graybox scene builder was found in the active scene.", "OK");
                return;
            }

            MethodInfo buildMethod = GetBuildMethod(builder);
            if (buildMethod == null)
            {
                EditorUtility.DisplayDialog("Bake Graybox Level", "The selected scene builder does not expose a BuildScene method.", "OK");
                return;
            }

            ClearBakedPrototypeLevel(scene);

            HashSet<int> before = CollectSceneObjectIds(scene);
            buildMethod.Invoke(builder, null);

            GameObject root = new GameObject(BakedRootName);
            root.AddComponent<PrototypeBakedObjectMarker>();

            List<GameObject> createdObjects = CollectNewSceneObjects(scene, before);
            for (int i = 0; i < createdObjects.Count; i++)
            {
                GameObject created = createdObjects[i];
                if (created == null || created == root || created == builder.gameObject)
                {
                    continue;
                }

                if (created.GetComponent<PrototypeBakedObjectMarker>() == null)
                {
                    created.AddComponent<PrototypeBakedObjectMarker>();
                }

                Transform parent = created.transform.parent;
                if (parent == null)
                {
                    created.transform.SetParent(root.transform, true);
                }
            }

            builder.gameObject.SetActive(false);
            EditorUtility.SetDirty(builder.gameObject);
            EditorSceneManager.MarkSceneDirty(scene);

            EditorUtility.DisplayDialog(
                "Bake Graybox Level",
                "Baked the graybox level into the scene and disabled the runtime builder to avoid duplicate objects in Play Mode.",
                "OK");
        }

        [MenuItem("Jade/Clear Baked Graybox Level")]
        public static void ClearCurrentBakedPrototypeLevel()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Clear Baked Graybox Level", "Exit Play Mode before clearing baked graybox objects.", "OK");
                return;
            }

            Scene scene = SceneManager.GetActiveScene();
            int count = ClearBakedPrototypeLevel(scene);
            SetSceneBuildersActive(scene, true);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorUtility.DisplayDialog("Clear Baked Graybox Level", "Removed " + count + " baked graybox object(s).", "OK");
        }

        private static MonoBehaviour FindSceneBuilder(Scene scene)
        {
            GameObject selected = Selection.activeGameObject;
            if (selected != null && selected.scene == scene)
            {
                MonoBehaviour selectedBuilder = FindBuilderOnObject(selected);
                if (selectedBuilder != null)
                {
                    return selectedBuilder;
                }
            }

            MonoBehaviour[] behaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour behaviour = behaviours[i];
                if (behaviour == null || behaviour.gameObject.scene != scene)
                {
                    continue;
                }

                if (IsPrototypeSceneBuilder(behaviour))
                {
                    return behaviour;
                }
            }

            return null;
        }

        private static MonoBehaviour FindBuilderOnObject(GameObject gameObject)
        {
            MonoBehaviour[] behaviours = gameObject.GetComponents<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (IsPrototypeSceneBuilder(behaviours[i]))
                {
                    return behaviours[i];
                }
            }

            return null;
        }

        private static bool IsPrototypeSceneBuilder(MonoBehaviour behaviour)
        {
            if (behaviour == null)
            {
                return false;
            }

            System.Type type = behaviour.GetType();
            return type.Namespace == "Jade.World"
                && (type.Name.StartsWith("Prototype") || type.Name.StartsWith("ShanhaiGate"))
                && GetBuildMethod(behaviour) != null;
        }

        private static void SetSceneBuildersActive(Scene scene, bool active)
        {
            MonoBehaviour[] behaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour behaviour = behaviours[i];
                if (behaviour == null || behaviour.gameObject.scene != scene || !IsPrototypeSceneBuilder(behaviour))
                {
                    continue;
                }

                behaviour.gameObject.SetActive(active);
                EditorUtility.SetDirty(behaviour.gameObject);
            }
        }

        private static MethodInfo GetBuildMethod(MonoBehaviour builder)
        {
            return builder.GetType().GetMethod("BuildScene", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private static HashSet<int> CollectSceneObjectIds(Scene scene)
        {
            HashSet<int> ids = new HashSet<int>();
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                CollectObjectIdsRecursive(roots[i].transform, ids);
            }

            return ids;
        }

        private static void CollectObjectIdsRecursive(Transform transform, HashSet<int> ids)
        {
            ids.Add(transform.gameObject.GetInstanceID());
            for (int i = 0; i < transform.childCount; i++)
            {
                CollectObjectIdsRecursive(transform.GetChild(i), ids);
            }
        }

        private static List<GameObject> CollectNewSceneObjects(Scene scene, HashSet<int> before)
        {
            List<GameObject> createdObjects = new List<GameObject>();
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                CollectNewSceneObjectsRecursive(roots[i].transform, before, createdObjects);
            }

            return createdObjects;
        }

        private static void CollectNewSceneObjectsRecursive(Transform transform, HashSet<int> before, List<GameObject> createdObjects)
        {
            if (!before.Contains(transform.gameObject.GetInstanceID()))
            {
                createdObjects.Add(transform.gameObject);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                CollectNewSceneObjectsRecursive(transform.GetChild(i), before, createdObjects);
            }
        }

        private static int ClearBakedPrototypeLevel(Scene scene)
        {
            PrototypeBakedObjectMarker[] markers = Resources.FindObjectsOfTypeAll<PrototypeBakedObjectMarker>();
            List<GameObject> rootsToDestroy = new List<GameObject>();

            for (int i = 0; i < markers.Length; i++)
            {
                PrototypeBakedObjectMarker marker = markers[i];
                if (marker == null || marker.gameObject.scene != scene)
                {
                    continue;
                }

                if (HasMarkedAncestor(marker.transform))
                {
                    continue;
                }

                rootsToDestroy.Add(marker.gameObject);
            }

            for (int i = 0; i < rootsToDestroy.Count; i++)
            {
                Object.DestroyImmediate(rootsToDestroy[i]);
            }

            return rootsToDestroy.Count;
        }

        private static bool HasMarkedAncestor(Transform transform)
        {
            Transform parent = transform.parent;
            while (parent != null)
            {
                if (parent.GetComponent<PrototypeBakedObjectMarker>() != null)
                {
                    return true;
                }

                parent = parent.parent;
            }

            return false;
        }
    }
}
