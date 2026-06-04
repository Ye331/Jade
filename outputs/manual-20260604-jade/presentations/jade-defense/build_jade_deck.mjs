import fs from "node:fs";
import path from "node:path";
import {
  Presentation,
  PresentationFile,
  image,
  layers,
  shape,
  stroke,
  text,
} from "file:///C:/Users/11979/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/@oai/artifact-tool/dist/artifact_tool.mjs";

const ROOT = "D:/unity/Jade";
const OUT = `${ROOT}/outputs/manual-20260604-jade/presentations/jade-defense/output/Jade_course_defense_minimal_tech.pptx`;

const W = 1600;
const H = 900;
const C = {
  ink: "#061F1D",
  ink2: "#0B302D",
  jade: "#45E6C8",
  gold: "#D5BA6B",
  paper: "#F4F1E8",
  muted: "#A8C9BD",
  white: "#F8FFF9",
  code: "#071815",
  line: "#1C5D52",
};

const assets = {
  bg: `${ROOT}/Assets/Art/Environments/ShanhaiGate/Backgrounds/BG_Composite_NoForegroundVines_Preview.png`,
  spirit: `${ROOT}/Assets/Art/Concept/Characters/JadeSpirit_FinalConcept.png`,
  monster: `${ROOT}/Assets/Art/Enemies/MonsterLobber/MonsterLobber_Preview.png`,
  hud: `${ROOT}/Assets/Resources/UI/TopLeftStatusHud.png`,
  health: `${ROOT}/Assets/Resources/UI/HealthLotus.png`,
  dash: `${ROOT}/Assets/Resources/UI/AbilityOrb_Dash.png`,
  doubleJump: `${ROOT}/Assets/Resources/UI/AbilityOrb_DoubleJump.png`,
  frame: `${ROOT}/Assets/Resources/UI/AbilityFrame.png`,
  cave: `${ROOT}/Assets/Art/Environments/ShanhaiGate/CaveTerrain/Drafts/CaveBlockout_FINAL_REFERENCE.png`,
  caveArt: `${ROOT}/Assets/Art/Environments/ShanhaiGate/CaveTerrain/Drafts/CaveDashSection_ArtDraft_15_RemoveRedNoVines.png`,
  terrain: `${ROOT}/Assets/Art/Environments/ShanhaiGate/BrightRuinTerrain/ShanhaiGate_BrightRuinTerrain_Transparent.png`,
};

const snippets = {
  motor: `private void UpdateTimers()
{
    if (!isGrounded) coyoteCounter -= Time.fixedDeltaTime;
    if (jumpBufferCounter > 0f) jumpBufferCounter -= Time.fixedDeltaTime;

    if (input.ConsumeJumpPressed())
        jumpBufferCounter = settings.jumpBufferTime;
}

private void HandleJumpInput()
{
    if (jumpBufferCounter > 0f && coyoteCounter > 0f)
    {
        body.velocity = new Vector2(body.velocity.x, jumpVelocity);
        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
        jumpedThisFrame = true;
    }
}`,
  settings: `[CreateAssetMenu(menuName = "Jade/Player Movement Settings")]
public class PlayerMovementSettings : ScriptableObject
{
    public float maxRunSpeed = 8f;
    public float jumpHeight = 4.2f;
    public float coyoteTime = 0.08f;
    public float jumpBufferTime = 0.12f;

    public float dashSpeed = 15f;
    public float dashDuration = 0.26f;
    public int airDashCount = 1;
}`,
  level: `private void BuildScene()
{
    GameObject player = CreatePlayer();
    Camera cameraToUse = CreateCamera(player.transform);
    CreateRoute(sprite);
    CreateDashAbilityPickup(sprite, new Vector2(2.5f, -2.25f));
    CreateDoubleJumpAbilityPickup(sprite, new Vector2(31f, -0.45f));
    CreateCheckpoint(sprite, "Checkpoint_FirstShrine", new Vector2(46f, -0.75f));
    CreateGoal(sprite, new Vector2(88f, 3f));
    CreateRespawnZone();
}`,
  ability: `public void UnlockDash()
{
    if (dashUnlocked) return;

    dashUnlocked = true;
    savedDashUnlocked = true;
    Debug.Log("Ability unlocked: Air Dash");
    NotifyAbilitiesChanged();
}

public void UnlockDoubleJump()
{
    if (doubleJumpUnlocked) return;
    doubleJumpUnlocked = true;
    savedDoubleJumpUnlocked = true;
    NotifyAbilitiesChanged();
}`,
  hud: `private void Awake()
{
    hudPanelSprite = LoadSprite("UI/HudPanel");
    healthLotusSprite = LoadSprite("UI/TopLeftLifeLotus");
    abilitySlotUnlockedSprite = LoadSprite("UI/AbilitySlotUnlocked");
    abilitySlotLockedSprite = LoadSprite("UI/AbilitySlotLocked");
    BuildHud();
}

public void Configure(PlayerAbilityInventory2D abilityInventory,
    PlayerHealth2D playerHealth)
{
    abilities.AbilitiesChanged += RefreshAbilities;
    health.HealthChanged += HandleHealthChanged;
}`,
  monster: `private void Update()
{
    if (Mathf.Abs(target.position.x - transform.position.x) > attackRangeX)
        return;

    fireTimer -= Time.deltaTime;
    if (fireTimer > 0f) return;

    fireTimer = fireInterval;
    FireAtTarget();
}

private Vector2 CalculateLobVelocity(Vector2 start, Vector2 end)
{
    float apexY = Mathf.Max(start.y, end.y) + arcHeight;
    float totalTime = upTime + downTime;
    return new Vector2((end.x - start.x) / totalTime, gravity * upTime);
}`,
  trap: `private void Update()
{
    switch (state)
    {
        case TrapState.Armed:
            if (PlayerInTriggerArea()) state = TrapState.Warning;
            break;
        case TrapState.Dropping:
            MoveToward(DroppedPosition(), fallSpeed);
            break;
        case TrapState.Resetting:
            MoveToward(startPosition, resetSpeed);
            break;
    }
}`,
};

function safeImage(src) {
  return fs.existsSync(src) ? src : assets.bg;
}

function imageUri(src) {
  const actual = safeImage(src);
  const ext = path.extname(actual).toLowerCase();
  const mime = ext === ".jpg" || ext === ".jpeg" ? "image/jpeg" : "image/png";
  return `data:${mime};base64,${fs.readFileSync(actual).toString("base64")}`;
}

function t(value, x, y, w, h, opts = {}) {
  return text(value, {
    position: { x, y },
    width: w,
    height: h,
    style: {
      fontFace: opts.mono ? "Consolas" : "Microsoft YaHei",
      fontSize: opts.size ?? 28,
      color: opts.color ?? C.white,
      bold: opts.bold ?? false,
      lineSpacing: opts.lineSpacing ?? 1.08,
    },
  });
}

function rect(x, y, w, h, fill, line = "transparent", radius = 0) {
  const config = {
    geometry: "rect",
    position: { x, y },
    width: w,
    height: h,
    fill,
    borderRadius: radius,
  };
  if (line && line !== "transparent") {
    config.line = stroke(line);
  }
  return shape(config);
}

function img(src, x, y, w, h, fit = "cover") {
  return image({
    uri: imageUri(src),
    position: { x, y },
    width: w,
    height: h,
    fit,
  });
}

function title(slide, titleText, kicker = "") {
  add(slide, [
    t(kicker, 84, 44, 600, 32, { size: 18, color: C.jade, bold: true }),
    t(titleText, 84, 76, 950, 62, { size: 38, bold: true }),
    rect(84, 142, 86, 4, C.jade),
  ]);
}

function tag(label, x, y, w = 150) {
  return [
    rect(x, y, w, 46, "#0E3833", C.line, 8),
    t(label, x + 18, y + 10, w - 36, 24, { size: 20, color: C.white, bold: true }),
  ];
}

function codeBlock(code, x, y, w, h, caption) {
  return [
    rect(x, y, w, h, C.code, C.line, 8),
    rect(x, y, w, 38, "#0B2925", C.line, 8),
    t(caption, x + 18, y + 8, w - 36, 20, { size: 15, color: C.jade, mono: true, bold: true }),
    t(code, x + 22, y + 54, w - 44, h - 68, { size: 16, color: "#D7FFF2", mono: true, lineSpacing: 1.02 }),
  ];
}

function add(slide, nodes) {
  const children = Array.isArray(nodes) ? nodes.flat() : [nodes];
  slide.compose(layers({ width: W, height: H, position: { x: 0, y: 0 } }, children));
}

const deck = Presentation.create();

function newSlide(bg = C.ink) {
  const slide = deck.slides.add({ width: W, height: H });
  add(slide, rect(0, 0, W, H, bg));
  return slide;
}

// 1 Cover
{
  const s = newSlide();
  add(s, [
    img(assets.bg, 0, 0, W, H, "cover"),
    rect(0, 0, W, H, "rgba(0, 28, 24, 0.55)"),
    t("JADE", 88, 570, 680, 92, { size: 76, bold: true }),
    t("中式神话 2D 平台跳跃原型", 94, 666, 620, 42, { size: 28, color: C.paper }),
    t("Unity / 课程答辩 / 简约技术展示版", 96, 724, 560, 28, { size: 18, color: C.muted }),
    rect(96, 758, 156, 5, C.jade),
  ]);
}

// 2 Positioning
{
  const s = newSlide();
  add(s, [
    img(assets.bg, 0, 0, 980, H, "cover"),
    rect(0, 0, 980, H, "rgba(3, 27, 24, 0.18)"),
    rect(920, 0, 680, H, C.ink),
    img(assets.spirit, 970, 84, 520, 350, "contain"),
    t("Jade 是一款中式神话题材的 Unity 2D 平台跳跃原型。", 970, 494, 500, 102, { size: 32, bold: true }),
    ...tag("山海", 970, 648, 112),
    ...tag("灵玉", 1100, 648, 112),
    ...tag("水墨剪影", 1230, 648, 160),
    ...tag("可玩原型", 970, 712, 160),
  ]);
}

// 3 Route
{
  const s = newSlide();
  title(s, "开发路线", "FROM FEEL TO PLAYABLE LEVEL");
  const xs = [126, 480, 834, 1188];
  const labels = [
    ["Phase01", "手感原型", "跑、跳、镜头、复位"],
    ["Phase02", "灰盒关卡", "路线节奏与检查点"],
    ["Phase04", "可玩关卡", "Dash / Double Jump 教学"],
    ["ShanhaiGate", "场景落地", "山海门与洞穴串联"],
  ];
  add(s, rect(190, 430, 1220, 4, C.line));
  for (let i = 0; i < xs.length; i++) {
    add(s, [
      rect(xs[i], 300, 280, 230, i === 2 ? "#123D37" : "#0B302D", C.line, 8),
      rect(xs[i] + 28, 408, 46, 46, C.jade, C.jade, 23),
      t(labels[i][0], xs[i] + 28, 330, 210, 24, { size: 18, color: C.jade, bold: true }),
      t(labels[i][1], xs[i] + 28, 362, 220, 36, { size: 30, bold: true }),
      t(labels[i][2], xs[i] + 28, 474, 220, 32, { size: 18, color: C.muted }),
    ]);
  }
}

// 4 Gameplay
{
  const s = newSlide();
  title(s, "核心玩法展示", "PLAYABLE LOOP");
  add(s, [
    img(assets.cave, 84, 182, 690, 440, "cover"),
    img(assets.terrain, 820, 182, 610, 220, "contain"),
    img(assets.caveArt, 820, 428, 610, 194, "cover"),
    ...tag("移动", 170, 680),
    ...tag("跳跃容错", 370, 680, 170),
    ...tag("检查点", 570, 680),
    ...tag("掉落复位", 770, 680, 170),
    t("用小规模可玩关卡先验证操作节奏，再替换正式美术。", 170, 760, 900, 34, { size: 24, color: C.paper }),
  ]);
}

// 5 Player Motor
{
  const s = newSlide();
  title(s, "玩家移动系统", "PlayerMotor2D.cs");
  add(s, [
    ...codeBlock(snippets.motor, 84, 178, 930, 610, "PlayerMotor2D.cs / jump buffer + coyote time"),
    img(assets.spirit, 1060, 168, 370, 260, "contain"),
    ...tag("响应快", 1100, 482, 148),
    ...tag("可调参", 1100, 548, 148),
    ...tag("容错手感", 1100, 614, 168),
    t("土狼时间和跳跃缓存让平台边缘操作更友好。", 1100, 710, 360, 56, { size: 24, color: C.paper, bold: true }),
  ]);
}

// 6 Settings
{
  const s = newSlide();
  title(s, "移动参数配置", "PlayerMovementSettings.cs");
  add(s, [
    ...codeBlock(snippets.settings, 84, 174, 706, 620, "ScriptableObject / tunable movement profile"),
    rect(850, 194, 520, 500, "#0D342F", C.line, 8),
    t("关键参数", 890, 232, 240, 38, { size: 30, bold: true }),
    t("maxRunSpeed        8\njumpHeight         4.2\ncoyoteTime         0.08\njumpBufferTime     0.12\ndashSpeed          15", 900, 308, 440, 230, { size: 28, color: "#D7FFF2", mono: true }),
    t("把手感数据集中到配置资产，方便反复调参。", 890, 604, 420, 40, { size: 24, color: C.paper }),
  ]);
}

// 7 Level
{
  const s = newSlide();
  title(s, "关卡生成与灰盒设计", "PrototypePhase04FirstPlayableLevelSceneBuilder.cs");
  add(s, [
    ...codeBlock(snippets.level, 84, 178, 760, 610, "BuildScene() / runtime graybox route"),
    rect(900, 232, 520, 320, "#0D342F", C.line, 8),
    t("起点", 940, 354, 60, 30, { size: 22, bold: true }),
    rect(1012, 369, 88, 4, C.jade),
    t("短跳", 1120, 354, 72, 30, { size: 22, bold: true }),
    rect(1200, 369, 88, 4, C.jade),
    t("分支收集", 1305, 354, 110, 30, { size: 22, bold: true }),
    t("检查点", 998, 448, 90, 30, { size: 22, color: C.gold, bold: true }),
    rect(1094, 463, 88, 4, C.gold),
    t("上升平台", 1200, 448, 110, 30, { size: 22, color: C.gold, bold: true }),
    rect(1312, 463, 58, 4, C.gold),
    t("终点", 1380, 448, 60, 30, { size: 22, color: C.gold, bold: true }),
    t("灰盒先验证路线节奏：主路、可选路线、失败复位都能被快速调整。", 920, 620, 470, 70, { size: 24, color: C.paper }),
  ]);
}

// 8 Ability
{
  const s = newSlide();
  title(s, "能力与交互系统", "PlayerAbilityInventory2D.cs");
  add(s, [
    img(assets.dash, 980, 170, 130, 130, "contain"),
    img(assets.doubleJump, 1160, 170, 130, 130, "contain"),
    img(assets.frame, 1340, 170, 130, 130, "contain"),
    ...codeBlock(snippets.ability, 84, 178, 760, 610, "Unlock ability / notify HUD + route logic"),
    t("能力解锁让关卡出现新的通过方式。", 970, 370, 450, 64, { size: 32, bold: true }),
    ...tag("Dash", 970, 492, 118),
    ...tag("Double Jump", 1110, 492, 188),
    ...tag("Shard Gate", 1320, 492, 170),
  ]);
}

// 9 HUD
{
  const s = newSlide();
  title(s, "HUD 与反馈", "GameplayHud2D.cs");
  add(s, [
    img(assets.hud, 70, 164, 760, 300, "contain"),
    img(assets.health, 890, 165, 150, 150, "contain"),
    img(assets.dash, 1080, 165, 130, 130, "contain"),
    img(assets.doubleJump, 1240, 165, 130, 130, "contain"),
    ...codeBlock(snippets.hud, 84, 496, 920, 300, "GameplayHud2D.cs / sprite resources + refresh events"),
    t("UI 负责把生命、能力、暂停和提示反馈可视化。", 1060, 560, 390, 96, { size: 30, bold: true }),
  ]);
}

// 10 Monster and trap
{
  const s = newSlide();
  title(s, "敌人与机关", "MonsterLobber2D.cs / WideFallingStoneTrap2D.cs");
  add(s, [
    img(assets.monster, 76, 174, 630, 340, "contain"),
    ...codeBlock(snippets.monster, 742, 174, 380, 590, "MonsterLobber2D.cs / lob attack"),
    ...codeBlock(snippets.trap, 1150, 174, 350, 590, "WideFallingStoneTrap2D.cs / state machine"),
    ...tag("范围检测", 108, 646, 160),
    ...tag("抛物线攻击", 292, 646, 190),
    ...tag("陷阱触发", 506, 646, 160),
  ]);
}

// 11 Art
{
  const s = newSlide();
  add(s, [
    img(assets.bg, 0, 0, W, H, "cover"),
    rect(0, 0, W, H, "rgba(4, 28, 25, 0.34)"),
    t("美术资产与氛围", 88, 70, 620, 64, { size: 44, bold: true }),
    t("用玉色、遗迹、森林和水墨雾感建立中式神话氛围。", 92, 145, 720, 36, { size: 24, color: C.paper }),
    img(assets.spirit, 106, 535, 260, 180, "contain"),
    img(assets.monster, 430, 528, 310, 180, "contain"),
    img(assets.hud, 800, 545, 360, 160, "contain"),
    img(assets.cave, 1210, 540, 290, 160, "cover"),
  ]);
}

// 12 Summary
{
  const s = newSlide();
  title(s, "总结与下一步", "DELIVERABLES + NEXT");
  add(s, [
    t("已完成", 178, 220, 180, 40, { size: 34, bold: true, color: C.jade }),
    ...tag("可玩场景", 178, 306, 180),
    ...tag("移动手感", 178, 382, 180),
    ...tag("能力交互", 178, 458, 180),
    ...tag("HUD / 美术 / 音频", 178, 534, 260),
    rect(760, 190, 2, 520, C.line),
    t("下一步", 880, 220, 180, 40, { size: 34, bold: true, color: C.gold }),
    ...tag("正式地图 UI", 880, 306, 210),
    ...tag("存档系统", 880, 382, 170),
    ...tag("更多敌人与能力回访", 880, 458, 280),
    img(assets.spirit, 1130, 520, 300, 210, "contain"),
  ]);
}

const blob = await PresentationFile.exportPptx(deck);
fs.mkdirSync(path.dirname(OUT), { recursive: true });
fs.writeFileSync(OUT, Buffer.from(blob.data));
console.log(OUT);
