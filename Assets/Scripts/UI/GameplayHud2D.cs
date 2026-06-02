using Jade.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jade.UI
{
    public class GameplayHud2D : MonoBehaviour
    {
        private const string DashIconResourcePath = "UI/AbilityOrb_Dash";
        private const string DoubleJumpIconResourcePath = "UI/AbilityOrb_DoubleJump";
        private const string HudPanelResourcePath = "UI/HudPanel";
        private const string AbilityFrameResourcePath = "UI/AbilityFrame";
        private const string HealthLotusResourcePath = "UI/TopLeftLifeLotus";
        private const string TopLeftStatusResourcePath = "UI/TopLeftStatusHudFrame";
        private const string PlayerAvatarResourcePath = "UI/PlayerAvatar";
        private const string PauseButtonResourcePath = "UI/PauseButton";
        private const string SettingsButtonResourcePath = "UI/SettingsButton";
        private const string PausePanelResourcePath = "UI/PausePanel";
        private const string AbilitySlotUnlockedResourcePath = "UI/AbilitySlotUnlocked";
        private const string AbilitySlotLockedResourcePath = "UI/AbilitySlotLocked";

        private static readonly Color PanelColor = new Color(0.02f, 0.18f, 0.16f, 0.88f);
        private static readonly Color BorderColor = new Color(0.82f, 0.72f, 0.45f, 0.95f);
        private static readonly Color JadeColor = new Color(0.26f, 0.92f, 0.82f, 0.95f);
        private static readonly Color DisabledColor = new Color(0.22f, 0.3f, 0.3f, 0.72f);

        private PlayerAbilityInventory2D abilities;
        private PlayerHealth2D health;
        private Transform healthRoot;
        private GameObject promptPanel;
        private Text promptTitleText;
        private Text promptHintText;
        private AbilitySlot dashSlot;
        private AbilitySlot doubleJumpSlot;
        private Font uiFont;
        private Sprite hudPanelSprite;
        private Sprite abilityFrameSprite;
        private Sprite healthLotusSprite;
        private Sprite topLeftStatusSprite;
        private Sprite playerAvatarSprite;
        private Sprite pauseButtonSprite;
        private Sprite settingsButtonSprite;
        private Sprite pausePanelSprite;
        private Sprite abilitySlotUnlockedSprite;
        private Sprite abilitySlotLockedSprite;
        private GameObject pauseOverlay;
        private bool isPaused;
        private float previousTimeScale = 1f;
        private readonly List<Image> panelImages = new List<Image>();
        private readonly List<Image> frameImages = new List<Image>();
        private readonly List<Image> healthImages = new List<Image>();
        private readonly List<Image> topLeftStatusImages = new List<Image>();
        private readonly List<Image> playerAvatarImages = new List<Image>();
        private readonly List<Image> pauseButtonImages = new List<Image>();
        private readonly List<Image> settingsButtonImages = new List<Image>();
        private readonly List<Image> pausePanelImages = new List<Image>();

        public static GameplayHud2D Active { get; private set; }

        public void Configure(PlayerAbilityInventory2D abilityInventory, PlayerHealth2D playerHealth)
        {
            if (abilities != null)
            {
                abilities.AbilitiesChanged -= RefreshAbilities;
            }

            if (health != null)
            {
                health.HealthChanged -= HandleHealthChanged;
            }

            abilities = abilityInventory;
            health = playerHealth;

            if (abilities != null)
            {
                abilities.AbilitiesChanged += RefreshAbilities;
            }

            if (health != null)
            {
                health.HealthChanged += HandleHealthChanged;
            }

            RefreshHealth();
            RefreshAbilities();
        }

        public void ShowAbilityPrompt(string title, string hint)
        {
            if (promptPanel == null)
            {
                return;
            }

            promptTitleText.text = title;
            promptHintText.text = hint;
            promptPanel.SetActive(true);
        }

        public void HideAbilityPrompt()
        {
            if (promptPanel != null)
            {
                promptPanel.SetActive(false);
            }
        }

        private void Awake()
        {
            Active = this;
            uiFont = Font.CreateDynamicFontFromOSFont(new[] { "Microsoft YaHei", "SimHei", "Arial" }, 28);
            hudPanelSprite = LoadSprite(HudPanelResourcePath);
            abilityFrameSprite = LoadSprite(AbilityFrameResourcePath);
            healthLotusSprite = LoadSprite(HealthLotusResourcePath);
            topLeftStatusSprite = LoadSprite(TopLeftStatusResourcePath);
            playerAvatarSprite = LoadSprite(PlayerAvatarResourcePath);
            pauseButtonSprite = LoadSprite(PauseButtonResourcePath);
            settingsButtonSprite = LoadSprite(SettingsButtonResourcePath);
            pausePanelSprite = LoadSprite(PausePanelResourcePath);
            abilitySlotUnlockedSprite = LoadSprite(AbilitySlotUnlockedResourcePath);
            abilitySlotLockedSprite = LoadSprite(AbilitySlotLockedResourcePath);
            BuildHud();
        }


        private void Update()
        {
            if (hudPanelSprite != null && abilityFrameSprite != null && healthLotusSprite != null && topLeftStatusSprite != null && playerAvatarSprite != null && pauseButtonSprite != null && settingsButtonSprite != null && pausePanelSprite != null && abilitySlotUnlockedSprite != null && abilitySlotLockedSprite != null)
            {
                return;
            }

            bool loadedAny = false;
            if (hudPanelSprite == null)
            {
                hudPanelSprite = LoadSprite(HudPanelResourcePath);
                loadedAny = loadedAny || hudPanelSprite != null;
            }

            if (abilityFrameSprite == null)
            {
                abilityFrameSprite = LoadSprite(AbilityFrameResourcePath);
                loadedAny = loadedAny || abilityFrameSprite != null;
            }

            if (healthLotusSprite == null)
            {
                healthLotusSprite = LoadSprite(HealthLotusResourcePath);
                loadedAny = loadedAny || healthLotusSprite != null;
            }

            if (topLeftStatusSprite == null)
            {
                topLeftStatusSprite = LoadSprite(TopLeftStatusResourcePath);
                loadedAny = loadedAny || topLeftStatusSprite != null;
            }

            if (playerAvatarSprite == null)
            {
                playerAvatarSprite = LoadSprite(PlayerAvatarResourcePath);
                loadedAny = loadedAny || playerAvatarSprite != null;
            }

            if (pauseButtonSprite == null)
            {
                pauseButtonSprite = LoadSprite(PauseButtonResourcePath);
                loadedAny = loadedAny || pauseButtonSprite != null;
            }

            if (settingsButtonSprite == null)
            {
                settingsButtonSprite = LoadSprite(SettingsButtonResourcePath);
                loadedAny = loadedAny || settingsButtonSprite != null;
            }

            if (pausePanelSprite == null)
            {
                pausePanelSprite = LoadSprite(PausePanelResourcePath);
                loadedAny = loadedAny || pausePanelSprite != null;
            }

            if (abilitySlotUnlockedSprite == null)
            {
                abilitySlotUnlockedSprite = LoadSprite(AbilitySlotUnlockedResourcePath);
                loadedAny = loadedAny || abilitySlotUnlockedSprite != null;
            }

            if (abilitySlotLockedSprite == null)
            {
                abilitySlotLockedSprite = LoadSprite(AbilitySlotLockedResourcePath);
                loadedAny = loadedAny || abilitySlotLockedSprite != null;
            }

            if (loadedAny)
            {
                ApplyLoadedSprites();
            }
        }
        private void OnDestroy()
        {
            if (Active == this)
            {
                Active = null;
            }

            if (isPaused)
            {
                Time.timeScale = previousTimeScale;
            }

            if (abilities != null)
            {
                abilities.AbilitiesChanged -= RefreshAbilities;
            }

            if (health != null)
            {
                health.HealthChanged -= HandleHealthChanged;
            }
        }

        private void BuildHud()
        {
            Canvas canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;

            CanvasScaler scaler = gameObject.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            if (gameObject.GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }

            EnsureEventSystem();
            BuildTopLeft();
            BuildTopRight();
            BuildPromptPanel();
            BuildAbilityBar();
        }

        private void BuildTopLeft()
        {
            GameObject panel = CreateRect("PlayerStatus", transform, new Vector2(660f, 228f), new Vector2(24f, -18f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            Image art = CreateImage("TopLeftStatusArt", panel.transform, new Vector2(660f, 228f), Vector2.zero, new Vector2(0f, 1f), new Vector2(0f, 1f), Color.white);
            art.sprite = topLeftStatusSprite != null ? topLeftStatusSprite : CreateRoundedSprite();
            art.preserveAspect = true;
            topLeftStatusImages.Add(art);

            GameObject avatarClip = CreateRect("AvatarClip", panel.transform, new Vector2(150f, 122f), new Vector2(32f, -36f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            avatarClip.AddComponent<RectMask2D>();

            Image avatar = CreateImage("PlayerAvatar", avatarClip.transform, new Vector2(132f, 132f), new Vector2(44f, 14f), new Vector2(0f, 1f), new Vector2(0f, 1f), Color.white);
            avatar.sprite = playerAvatarSprite != null ? playerAvatarSprite : CreateCircleSprite();
            avatar.preserveAspect = true;
            playerAvatarImages.Add(avatar);

            healthRoot = CreateRect("HealthRoot", panel.transform, new Vector2(660f, 228f), Vector2.zero, new Vector2(0f, 1f), new Vector2(0f, 1f)).transform;
        }

        private void BuildTopRight()
        {
            CreateHudImage("PauseButton", pauseButtonSprite, new Vector2(104f, 112f), new Vector2(-36f, -24f), pauseButtonImages);
            CreateHudImage("SettingsButton", settingsButtonSprite, new Vector2(104f, 112f), new Vector2(-36f, -142f), settingsButtonImages);
        }

        private void BuildPausePanel()
        {
            pauseOverlay = CreateRect("PauseOverlay", transform, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
            RectTransform overlayRect = pauseOverlay.GetComponent<RectTransform>();
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            Image dim = pauseOverlay.AddComponent<Image>();
            dim.color = new Color(0f, 0.03f, 0.04f, 0.48f);

            GameObject panel = CreateRect("PausePanel", pauseOverlay.transform, new Vector2(720f, 500f), Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            Image panelImage = panel.AddComponent<Image>();
            panelImage.sprite = pausePanelSprite != null ? pausePanelSprite : CreateRoundedSprite();
            panelImage.color = pausePanelSprite != null ? Color.white : PanelColor;
            panelImage.preserveAspect = true;
            pausePanelImages.Add(panelImage);

            CreateText("PauseTitle", panel.transform, "\u6682\u505c", 42, TextAnchor.MiddleCenter, new Vector2(220f, 52f), new Vector2(360f, -88f), new Vector2(0f, 1f), new Vector2(0f, 1f), Color.white);
            CreateText("MusicLabel", panel.transform, "\u97f3\u4e50\u97f3\u91cf", 25, TextAnchor.MiddleLeft, new Vector2(180f, 42f), new Vector2(112f, -188f), new Vector2(0f, 1f), new Vector2(0f, 1f), Color.white);
            CreateText("SfxLabel", panel.transform, "\u97f3\u6548\u97f3\u91cf", 25, TextAnchor.MiddleLeft, new Vector2(180f, 42f), new Vector2(112f, -258f), new Vector2(0f, 1f), new Vector2(0f, 1f), Color.white);
            CreateText("BrightnessLabel", panel.transform, "\u753b\u9762\u4eae\u5ea6", 25, TextAnchor.MiddleLeft, new Vector2(180f, 42f), new Vector2(112f, -328f), new Vector2(0f, 1f), new Vector2(0f, 1f), Color.white);

            CreatePanelButton("ResumeButton", panel.transform, "\u7ee7\u7eed", new Vector2(210f, 70f), new Vector2(176f, -402f), HidePause);
            CreatePanelButton("BackButton", panel.transform, "\u8fd4\u56de", new Vector2(210f, 70f), new Vector2(420f, -402f), HidePause);

            pauseOverlay.SetActive(false);
        }

        private void BuildPromptPanel()
        {
            promptPanel = CreatePanel("AbilityPrompt", transform, new Vector2(680f, 150f), new Vector2(34f, 30f), new Vector2(0f, 0f), new Vector2(0f, 0f));
            CreateText("PromptKey", promptPanel.transform, "E", 38, TextAnchor.MiddleCenter, new Vector2(82f, 82f), new Vector2(38f, 36f), new Vector2(0f, 0f), new Vector2(0f, 0f), Color.white);
            promptTitleText = CreateText("PromptTitle", promptPanel.transform, string.Empty, 30, TextAnchor.MiddleLeft, new Vector2(500f, 42f), new Vector2(136f, 84f), new Vector2(0f, 0f), new Vector2(0f, 0f), Color.white);
            promptHintText = CreateText("PromptHint", promptPanel.transform, string.Empty, 25, TextAnchor.MiddleLeft, new Vector2(540f, 42f), new Vector2(136f, 42f), new Vector2(0f, 0f), new Vector2(0f, 0f), Color.white);
            promptPanel.SetActive(false);
        }

        private void BuildAbilityBar()
        {
            GameObject root = CreateRect("AbilityBar", transform, new Vector2(560f, 270f), new Vector2(-38f, 42f), new Vector2(1f, 0f), new Vector2(1f, 0f));
            CreateText("AbilityTitle", root.transform, "\u80fd\u529b", 34, TextAnchor.MiddleCenter, new Vector2(260f, 42f), new Vector2(276f, 232f), new Vector2(0f, 0f), new Vector2(0f, 0f), BorderColor);

            dashSlot = CreateAbilitySlot(root.transform, "DashSlot", "\u51b2\u523a", LoadSprite(DashIconResourcePath), new Vector2(156f, 0f));
            doubleJumpSlot = CreateAbilitySlot(root.transform, "DoubleJumpSlot", "\u4e8c\u6bb5\u8df3", LoadSprite(DoubleJumpIconResourcePath), new Vector2(394f, 0f));
        }

        private void RefreshHealth()
        {
            if (healthRoot == null)
            {
                return;
            }

            healthImages.Clear();

            for (int i = healthRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(healthRoot.GetChild(i).gameObject);
            }

            int max = health != null ? Mathf.Max(1, health.MaxHealth) : 3;
            int current = health != null ? Mathf.Clamp(health.CurrentHealth, 0, max) : max;
            int visibleSlots = Mathf.Min(max, 3);
            float[] lotusCentersX = { 328f, 439f, 550f };

            for (int i = 0; i < visibleSlots; i++)
            {
                if (i >= current)
                {
                    continue;
                }

                Image lotus = CreateImage("HealthLotus_" + (i + 1), healthRoot, new Vector2(104f, 88f), new Vector2(lotusCentersX[i] - 52f, -103f), new Vector2(0f, 1f), new Vector2(0f, 1f), Color.white);
                lotus.sprite = healthLotusSprite != null ? healthLotusSprite : CreateCircleSprite();
                lotus.preserveAspect = true;
                healthImages.Add(lotus);
            }
        }

        private void HandleHealthChanged(int currentHealth, int maxHealth)
        {
            RefreshHealth();
        }

        private void RefreshAbilities()
        {
            if (dashSlot != null)
            {
                dashSlot.SetUnlocked(abilities != null && abilities.DashUnlocked, abilitySlotUnlockedSprite, abilitySlotLockedSprite);
            }

            if (doubleJumpSlot != null)
            {
                doubleJumpSlot.SetUnlocked(abilities != null && abilities.DoubleJumpUnlocked, abilitySlotUnlockedSprite, abilitySlotLockedSprite);
            }
        }

        private AbilitySlot CreateAbilitySlot(Transform parent, string name, string label, Sprite icon, Vector2 anchoredPosition)
        {
            GameObject slot = CreateRect(name, parent, new Vector2(220f, 258f), anchoredPosition, new Vector2(0f, 0f), new Vector2(0f, 0f));
            Image ring = CreateImage("Ring", slot.transform, new Vector2(210f, 210f), new Vector2(0f, 58f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), JadeColor);
            ring.sprite = abilitySlotLockedSprite != null ? abilitySlotLockedSprite : CreateCircleSprite();
            ring.preserveAspect = true;

            Image iconImage = CreateImage("Icon", slot.transform, new Vector2(118f, 118f), new Vector2(0f, 112f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), Color.white);
            iconImage.sprite = icon != null ? icon : CreateCircleSprite();
            iconImage.preserveAspect = true;

            Text labelText = CreateText("Label", slot.transform, label, 32, TextAnchor.MiddleCenter, new Vector2(220f, 50f), new Vector2(0f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), Color.white);

            return new AbilitySlot(iconImage, ring, labelText);
        }

        private void CreateRoundButton(string name, string text, Vector2 anchoredPosition)
        {
            GameObject button = CreateRect(name, transform, new Vector2(86f, 86f), anchoredPosition, new Vector2(1f, 1f), new Vector2(1f, 1f));
            Image background = button.AddComponent<Image>();
            background.sprite = abilityFrameSprite != null ? abilityFrameSprite : CreateCircleSprite();
            background.color = Color.white;
            background.preserveAspect = true;
            frameImages.Add(background);
            CreateText("Text", button.transform, text, 31, TextAnchor.MiddleCenter, new Vector2(78f, 78f), Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Color.white);
        }

        private void CreateImageButton(string name, Sprite sprite, Vector2 size, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick, List<Image> trackingList)
        {
            GameObject buttonObject = CreateRect(name, transform, size, anchoredPosition, new Vector2(1f, 1f), new Vector2(1f, 1f));
            Image image = buttonObject.AddComponent<Image>();
            image.sprite = sprite != null ? sprite : CreateCircleSprite();
            image.color = Color.white;
            image.preserveAspect = true;
            trackingList.Add(image);

            Button button = buttonObject.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = image;
            button.onClick.AddListener(onClick);
        }

        private void CreateHudImage(string name, Sprite sprite, Vector2 size, Vector2 anchoredPosition, List<Image> trackingList)
        {
            GameObject imageObject = CreateRect(name, transform, size, anchoredPosition, new Vector2(1f, 1f), new Vector2(1f, 1f));
            Image image = imageObject.AddComponent<Image>();
            image.sprite = sprite != null ? sprite : CreateCircleSprite();
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;
            trackingList.Add(image);
        }

        private void CreatePanelButton(string name, Transform parent, string text, Vector2 size, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
        {
            GameObject buttonObject = CreateRect(name, parent, size, anchoredPosition, new Vector2(0f, 1f), new Vector2(0f, 1f));
            Image image = buttonObject.AddComponent<Image>();
            image.sprite = CreateSolidSprite(Color.white);
            image.color = new Color(1f, 1f, 1f, 0.01f);
            image.raycastTarget = true;

            Button button = buttonObject.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = image;
            button.onClick.AddListener(onClick);

            CreateText("Label", buttonObject.transform, text, 28, TextAnchor.MiddleCenter, size, Vector2.zero, new Vector2(0f, 1f), new Vector2(0f, 1f), Color.white);
        }

        private void TogglePause()
        {
            if (isPaused)
            {
                HidePause();
            }
            else
            {
                ShowPause();
            }
        }

        private void ShowPause()
        {
            if (isPaused)
            {
                return;
            }

            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            isPaused = true;

            if (pauseOverlay != null)
            {
                pauseOverlay.SetActive(true);
            }
        }

        private void HidePause()
        {
            if (!isPaused)
            {
                return;
            }

            Time.timeScale = previousTimeScale;
            isPaused = false;

            if (pauseOverlay != null)
            {
                pauseOverlay.SetActive(false);
            }
        }

        private void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            DontDestroyOnLoad(eventSystem);
        }

        private GameObject CreatePanel(string name, Transform parent, Vector2 size, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject panel = CreateRect(name, parent, size, anchoredPosition, anchorMin, anchorMax);
            Image fill = panel.AddComponent<Image>();
            fill.sprite = hudPanelSprite != null ? hudPanelSprite : CreateRoundedSprite();
            fill.color = hudPanelSprite != null ? Color.white : PanelColor;
            panelImages.Add(fill);

            if (hudPanelSprite == null)
            {
                Outline outline = panel.AddComponent<Outline>();
                outline.effectColor = BorderColor;
                outline.effectDistance = new Vector2(2f, -2f);
            }
            return panel;
        }


        private void ApplyLoadedSprites()
        {
            if (hudPanelSprite != null)
            {
                for (int i = 0; i < panelImages.Count; i++)
                {
                    if (panelImages[i] == null)
                    {
                        continue;
                    }

                    panelImages[i].sprite = hudPanelSprite;
                    panelImages[i].color = Color.white;
                }
            }

            if (abilityFrameSprite != null)
            {
                for (int i = 0; i < frameImages.Count; i++)
                {
                    if (frameImages[i] == null)
                    {
                        continue;
                    }

                    frameImages[i].sprite = abilityFrameSprite;
                    frameImages[i].color = Color.white;
                    frameImages[i].preserveAspect = true;
                }
            }

            if (healthLotusSprite != null)
            {
                for (int i = 0; i < healthImages.Count; i++)
                {
                    if (healthImages[i] == null)
                    {
                        continue;
                    }

                    healthImages[i].sprite = healthLotusSprite;
                    healthImages[i].preserveAspect = true;
                }
            }

            if (topLeftStatusSprite != null)
            {
                for (int i = 0; i < topLeftStatusImages.Count; i++)
                {
                    if (topLeftStatusImages[i] == null)
                    {
                        continue;
                    }

                    topLeftStatusImages[i].sprite = topLeftStatusSprite;
                    topLeftStatusImages[i].color = Color.white;
                    topLeftStatusImages[i].preserveAspect = true;
                }
            }

            if (playerAvatarSprite != null)
            {
                for (int i = 0; i < playerAvatarImages.Count; i++)
                {
                    if (playerAvatarImages[i] == null)
                    {
                        continue;
                    }

                    playerAvatarImages[i].sprite = playerAvatarSprite;
                    playerAvatarImages[i].color = Color.white;
                    playerAvatarImages[i].preserveAspect = true;
                }
            }

            if (pauseButtonSprite != null)
            {
                for (int i = 0; i < pauseButtonImages.Count; i++)
                {
                    if (pauseButtonImages[i] == null)
                    {
                        continue;
                    }

                    pauseButtonImages[i].sprite = pauseButtonSprite;
                    pauseButtonImages[i].color = Color.white;
                    pauseButtonImages[i].preserveAspect = true;
                }
            }

            if (settingsButtonSprite != null)
            {
                for (int i = 0; i < settingsButtonImages.Count; i++)
                {
                    if (settingsButtonImages[i] == null)
                    {
                        continue;
                    }

                    settingsButtonImages[i].sprite = settingsButtonSprite;
                    settingsButtonImages[i].color = Color.white;
                    settingsButtonImages[i].preserveAspect = true;
                }
            }

            if (pausePanelSprite != null)
            {
                for (int i = 0; i < pausePanelImages.Count; i++)
                {
                    if (pausePanelImages[i] == null)
                    {
                        continue;
                    }

                    pausePanelImages[i].sprite = pausePanelSprite;
                    pausePanelImages[i].color = Color.white;
                    pausePanelImages[i].preserveAspect = true;
                }
            }

            RefreshAbilities();
        }
        private GameObject CreateRect(string name, Transform parent, Vector2 size, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = anchorMin;
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            return go;
        }

        private Image CreateImage(string name, Transform parent, Vector2 size, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            GameObject go = CreateRect(name, parent, size, anchoredPosition, anchorMin, anchorMax);
            Image image = go.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private Text CreateText(string name, Transform parent, string text, int size, TextAnchor anchor, Vector2 rectSize, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            GameObject go = CreateRect(name, parent, rectSize, anchoredPosition, anchorMin, anchorMax);
            Text label = go.AddComponent<Text>();
            label.font = uiFont;
            label.text = text;
            label.fontSize = size;
            label.alignment = anchor;
            label.color = color;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            return label;
        }

        private static Sprite LoadSprite(string resourcePath)
        {
            Sprite sprite = Resources.Load<Sprite>(resourcePath);
            if (sprite != null)
            {
                return sprite;
            }

            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null)
            {
                return null;
            }

            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        }

        private static Sprite whiteSprite;
        private static Sprite circleSprite;
        private static Sprite roundedSprite;

        private static Sprite CreateRoundedSprite()
        {
            return roundedSprite != null ? roundedSprite : (roundedSprite = CreateSolidSprite(new Color(1f, 1f, 1f, 1f)));
        }

        private static Sprite CreateCircleSprite()
        {
            if (circleSprite != null)
            {
                return circleSprite;
            }

            int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.47f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float alpha = Vector2.Distance(new Vector2(x, y), center) <= radius ? 1f : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            circleSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
            return circleSprite;
        }

        private static Sprite CreateSolidSprite(Color color)
        {
            if (whiteSprite != null)
            {
                return whiteSprite;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            whiteSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
            return whiteSprite;
        }

        private sealed class AbilitySlot
        {
            private readonly Image icon;
            private readonly Image ring;
            private readonly Text label;

            public AbilitySlot(Image icon, Image ring, Text label)
            {
                this.icon = icon;
                this.ring = ring;
                this.label = label;
            }

            public void SetUnlocked(bool unlocked, Sprite unlockedFrame, Sprite lockedFrame)
            {
                Sprite targetFrame = unlocked ? unlockedFrame : lockedFrame;
                if (targetFrame != null)
                {
                    ring.sprite = targetFrame;
                }

                ring.color = Color.white;
                icon.color = unlocked ? Color.white : new Color(0.35f, 0.45f, 0.45f, 0.38f);
                label.color = unlocked ? Color.white : new Color(0.78f, 0.85f, 0.82f, 0.75f);
            }
        }
    }
}


