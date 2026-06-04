using UnityEngine;

namespace Jade.World
{
    public class PrototypePhase04EnvironmentArtPass2D : MonoBehaviour
    {
        private const int MidgroundOrder = -16;
        private const int PlatformTrimOrder = 3;
        private const int ForegroundOrder = 21;

        private Transform artRoot;
        private Sprite squareSprite;

        public void Build(Sprite sprite)
        {
            if (sprite == null)
            {
                return;
            }

            squareSprite = sprite;
            artRoot = new GameObject("Art_Phase04_EnvironmentPass").transform;
            artRoot.SetParent(transform);
            artRoot.localPosition = Vector3.zero;
            artRoot.localRotation = Quaternion.identity;
            artRoot.localScale = Vector3.one;

            CreateMidground();
            CreatePlatformTrims();
            CreateForeground();
            CreateGoalGateDress();
        }

        private void CreateMidground()
        {
            CreateRect("Art_Midground_MistBand_Start", new Vector2(-4f, -0.7f), new Vector2(22f, 0.18f), new Color(0.45f, 0.75f, 0.68f, 0.18f), MidgroundOrder);
            CreateRect("Art_Midground_MistBand_DashGap", new Vector2(13f, -0.4f), new Vector2(18f, 0.14f), new Color(0.52f, 0.9f, 0.8f, 0.13f), MidgroundOrder);
            CreateRect("Art_Midground_MistBand_ShortJumps", new Vector2(27f, 0.55f), new Vector2(17f, 0.12f), new Color(0.62f, 0.95f, 0.88f, 0.11f), MidgroundOrder);

            CreateMountain("Art_Midground_InkMountain_Left", new Vector2(-13f, -1.4f), new Vector2(4.6f, 4.2f), 0f);
            CreateMountain("Art_Midground_InkMountain_Center", new Vector2(5f, -1.2f), new Vector2(6.5f, 5.1f), 0.08f);
            CreateMountain("Art_Midground_InkMountain_DashGap", new Vector2(17f, -1.55f), new Vector2(5.4f, 4.4f), -0.06f);
            CreateMountain("Art_Midground_InkMountain_ShortJumps", new Vector2(29f, -0.95f), new Vector2(6f, 4.8f), 0.04f);

            CreateRuinPillar("Art_Midground_RuinPillar_Start_A", new Vector2(-11.6f, -2.15f), 2.4f, 0.26f);
            CreateRuinPillar("Art_Midground_RuinPillar_Start_B", new Vector2(-1.4f, -2.25f), 1.85f, -0.18f);
            CreateRuinPillar("Art_Midground_RuinPillar_DashGap", new Vector2(15.6f, -1.95f), 2.15f, 0.14f);
            CreateRect("Art_Midground_BeastSilhouette_Back", new Vector2(23.5f, -0.9f), new Vector2(2.2f, 0.6f), new Color(0.04f, 0.1f, 0.11f, 0.34f), MidgroundOrder + 1);
            CreateRect("Art_Midground_BeastSilhouette_Horn", new Vector2(24.35f, -0.34f), new Vector2(0.16f, 0.7f), new Color(0.08f, 0.22f, 0.2f, 0.42f), MidgroundOrder + 1, 24f);
        }

        private void CreatePlatformTrims()
        {
            CreatePlatformDress("Art_PlatformTrim_Start", new Vector2(-9f, -3f), new Vector2(13f, 1f));
            CreatePlatformDress("Art_PlatformTrim_RunRead", new Vector2(1f, -3f), new Vector2(7.5f, 0.55f));
            CreatePlatformDress("Art_PlatformTrim_DashTakeoff", new Vector2(7.5f, -2.7f), new Vector2(2.2f, 0.45f));
            CreatePlatformDress("Art_PlatformTrim_DashLanding", new Vector2(18.2f, -2.25f), new Vector2(3f, 0.5f));
            CreatePlatformDress("Art_PlatformTrim_ShortJump_01", new Vector2(21f, -1.95f), new Vector2(2.6f, 0.45f));
            CreatePlatformDress("Art_PlatformTrim_ShortJump_02", new Vector2(26f, -1.45f), new Vector2(2.35f, 0.45f));
            CreatePlatformDress("Art_PlatformTrim_ShortJump_03", new Vector2(31f, -1.15f), new Vector2(2.5f, 0.45f));

            CreateJadeEdge("Art_PlatformTrim_JadeEdge_Start", new Vector2(-9f, -2.42f), 12.4f);
            CreateJadeEdge("Art_PlatformTrim_JadeEdge_RunRead", new Vector2(1f, -2.68f), 7.1f);
            CreateJadeEdge("Art_PlatformTrim_JadeEdge_DashTakeoff", new Vector2(7.5f, -2.43f), 1.8f);
            CreateJadeEdge("Art_PlatformTrim_JadeEdge_DashLanding", new Vector2(18.2f, -1.94f), 2.6f);
            CreateJadeEdge("Art_PlatformTrim_JadeEdge_ShortJump_01", new Vector2(21f, -1.67f), 2.1f);
            CreateJadeEdge("Art_PlatformTrim_JadeEdge_ShortJump_02", new Vector2(26f, -1.17f), 1.9f);
            CreateJadeEdge("Art_PlatformTrim_JadeEdge_ShortJump_03", new Vector2(31f, -0.87f), 2.0f);

            CreatePebbleCluster("Art_PlatformTrim_Pebbles_Start", new Vector2(-4.5f, -2.35f));
            CreatePebbleCluster("Art_PlatformTrim_Pebbles_Dash", new Vector2(17.6f, -1.86f));
            CreatePebbleCluster("Art_PlatformTrim_Pebbles_ShortJumps", new Vector2(30.5f, -0.79f));
        }

        private void CreateForeground()
        {
            CreateBambooCluster("Art_Foreground_Bamboo_Start_A", new Vector2(-14.2f, -3.6f), 2.9f);
            CreateBambooCluster("Art_Foreground_Bamboo_Start_B", new Vector2(-3.1f, -3.65f), 2.2f);
            CreateBambooCluster("Art_Foreground_Bamboo_DashGap", new Vector2(11.8f, -3.35f), 2.6f);

            CreateRect("Art_Foreground_BrokenBridge_DashGap_A", new Vector2(11.7f, -2.1f), new Vector2(2.9f, 0.14f), new Color(0.03f, 0.05f, 0.05f, 0.72f), ForegroundOrder, -13f);
            CreateRect("Art_Foreground_BrokenBridge_DashGap_B", new Vector2(13.7f, -2.05f), new Vector2(1.9f, 0.12f), new Color(0.05f, 0.09f, 0.08f, 0.62f), ForegroundOrder, 10f);

            CreateVine("Art_Foreground_HangingVine_DashGap_A", new Vector2(14.4f, 0.2f), 1.6f);
            CreateVine("Art_Foreground_HangingVine_ShortJump_A", new Vector2(24.2f, 1.25f), 1.25f);

            CreateRect("Art_Foreground_RuinedTablet_Start", new Vector2(-6.2f, -2.18f), new Vector2(0.42f, 1.15f), new Color(0.04f, 0.07f, 0.07f, 0.68f), ForegroundOrder, -8f);
            CreateRect("Art_Foreground_RuinedTablet_JumpRead", new Vector2(28.7f, -0.34f), new Vector2(0.34f, 0.9f), new Color(0.04f, 0.08f, 0.07f, 0.58f), ForegroundOrder, 7f);
        }

        private void CreateGoalGateDress()
        {
            CreateRect("Art_GoalGate_LeftInkPillar", new Vector2(87.15f, 3.85f), new Vector2(0.34f, 2.6f), new Color(0.05f, 0.14f, 0.13f, 0.86f), 14);
            CreateRect("Art_GoalGate_RightInkPillar", new Vector2(88.85f, 3.85f), new Vector2(0.34f, 2.6f), new Color(0.05f, 0.14f, 0.13f, 0.86f), 14);
            CreateRect("Art_GoalGate_TopBeam", new Vector2(88f, 5.15f), new Vector2(2.25f, 0.28f), new Color(0.08f, 0.22f, 0.2f, 0.82f), 14);
            CreateRect("Art_GoalGate_JadeGlow", new Vector2(88f, 4.15f), new Vector2(1.25f, 1.8f), new Color(0.38f, 1f, 0.82f, 0.24f), 13);
            CreateRect("Art_GoalGate_InnerLine", new Vector2(88f, 4.15f), new Vector2(0.14f, 1.65f), new Color(0.72f, 1f, 0.9f, 0.62f), 15);
        }

        private void CreatePlatformDress(string name, Vector2 position, Vector2 size)
        {
            CreateRect(name + "_InkMass", position + new Vector2(0f, -0.04f), size + new Vector2(0.22f, 0.12f), new Color(0.05f, 0.08f, 0.085f, 0.66f), PlatformTrimOrder);
            CreateRect(name + "_Underside", position + new Vector2(0f, -size.y * 0.52f - 0.12f), new Vector2(size.x * 0.92f, 0.2f), new Color(0.025f, 0.035f, 0.04f, 0.8f), PlatformTrimOrder + 1);
        }

        private void CreateJadeEdge(string name, Vector2 position, float width)
        {
            CreateRect(name, position, new Vector2(width, 0.055f), new Color(0.44f, 1f, 0.84f, 0.42f), PlatformTrimOrder + 2);
        }

        private void CreatePebbleCluster(string name, Vector2 origin)
        {
            CreateRect(name + "_A", origin, new Vector2(0.18f, 0.09f), new Color(0.09f, 0.15f, 0.14f, 0.72f), PlatformTrimOrder + 3, -9f);
            CreateRect(name + "_B", origin + new Vector2(0.24f, 0.03f), new Vector2(0.12f, 0.07f), new Color(0.12f, 0.22f, 0.2f, 0.58f), PlatformTrimOrder + 3, 14f);
            CreateRect(name + "_C", origin + new Vector2(-0.22f, 0.02f), new Vector2(0.15f, 0.08f), new Color(0.06f, 0.1f, 0.1f, 0.68f), PlatformTrimOrder + 3, 6f);
        }

        private void CreateMountain(string name, Vector2 position, Vector2 size, float lean)
        {
            CreateRect(name + "_Peak", position, size, new Color(0.035f, 0.07f, 0.075f, 0.36f), MidgroundOrder, lean * 35f);
            CreateRect(name + "_JadeRidge", position + new Vector2(size.x * 0.1f, size.y * 0.06f), new Vector2(size.x * 0.09f, size.y * 0.58f), new Color(0.28f, 0.82f, 0.72f, 0.16f), MidgroundOrder + 1, lean * 35f);
        }

        private void CreateRuinPillar(string name, Vector2 position, float height, float lean)
        {
            CreateRect(name + "_Body", position, new Vector2(0.32f, height), new Color(0.04f, 0.08f, 0.08f, 0.48f), MidgroundOrder + 2, lean * 35f);
            CreateRect(name + "_Cap", position + new Vector2(0f, height * 0.5f + 0.08f), new Vector2(0.62f, 0.16f), new Color(0.05f, 0.12f, 0.11f, 0.42f), MidgroundOrder + 2, lean * 35f);
        }

        private void CreateBambooCluster(string name, Vector2 basePosition, float height)
        {
            CreateBambooStem(name + "_Stem_A", basePosition, height, -6f);
            CreateBambooStem(name + "_Stem_B", basePosition + new Vector2(0.3f, -0.05f), height * 0.85f, 4f);
            CreateBambooStem(name + "_Stem_C", basePosition + new Vector2(-0.28f, -0.03f), height * 0.72f, 9f);
        }

        private void CreateBambooStem(string name, Vector2 basePosition, float height, float rotation)
        {
            CreateRect(name, basePosition + new Vector2(0f, height * 0.5f), new Vector2(0.08f, height), new Color(0.02f, 0.045f, 0.04f, 0.72f), ForegroundOrder, rotation);
            CreateRect(name + "_Leaf_A", basePosition + new Vector2(0.22f, height * 0.82f), new Vector2(0.6f, 0.08f), new Color(0.03f, 0.08f, 0.07f, 0.58f), ForegroundOrder, rotation + 22f);
            CreateRect(name + "_Leaf_B", basePosition + new Vector2(-0.2f, height * 0.68f), new Vector2(0.5f, 0.07f), new Color(0.03f, 0.08f, 0.07f, 0.52f), ForegroundOrder, rotation - 28f);
        }

        private void CreateVine(string name, Vector2 topPosition, float length)
        {
            CreateRect(name + "_Line", topPosition + new Vector2(0f, -length * 0.5f), new Vector2(0.06f, length), new Color(0.03f, 0.1f, 0.08f, 0.56f), ForegroundOrder - 1, 7f);
            CreateRect(name + "_GlowTip", topPosition + new Vector2(0.07f, -length), new Vector2(0.16f, 0.08f), new Color(0.45f, 1f, 0.82f, 0.36f), ForegroundOrder - 1, -12f);
        }

        private SpriteRenderer CreateRect(string name, Vector2 position, Vector2 size, Color color, int sortingOrder, float rotationDegrees = 0f)
        {
            GameObject rect = new GameObject(name);
            rect.transform.SetParent(artRoot);
            rect.transform.position = position;
            rect.transform.localRotation = Quaternion.Euler(0f, 0f, rotationDegrees);
            rect.transform.localScale = new Vector3(size.x, size.y, 1f);

            SpriteRenderer renderer = rect.AddComponent<SpriteRenderer>();
            renderer.sprite = squareSprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }
    }
}
