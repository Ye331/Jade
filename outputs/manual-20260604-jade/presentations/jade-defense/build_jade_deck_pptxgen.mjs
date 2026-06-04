import pptxgen from "file:///C:/Users/11979/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/pptxgenjs/dist/pptxgen.es.js";
import fs from "node:fs";
import path from "node:path";

const ROOT = "D:/unity/Jade";
const OUT = `${ROOT}/outputs/manual-20260604-jade/presentations/jade-defense/output/Jade_course_defense_minimal_tech.pptx`;

const C = {
  ink: "061F1D",
  ink2: "0B302D",
  jade: "45E6C8",
  gold: "D5BA6B",
  paper: "F4F1E8",
  muted: "A8C9BD",
  white: "F8FFF9",
  code: "071815",
  line: "1C5D52",
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

const pptx = new pptxgen();
pptx.layout = "LAYOUT_WIDE";
pptx.author = "Codex";
pptx.subject = "Jade course defense deck";
pptx.title = "Jade 课程答辩 简约技术展示版";
pptx.company = "Jade";
pptx.lang = "zh-CN";
pptx.theme = {
  headFontFace: "Microsoft YaHei",
  bodyFontFace: "Microsoft YaHei",
  lang: "zh-CN",
};
pptx.defineLayout({ name: "JADE_WIDE", width: 13.333, height: 7.5 });
pptx.layout = "JADE_WIDE";

function bg(slide, color = C.ink) {
  slide.background = { color };
}

function tx(slide, text, x, y, w, h, opts = {}) {
  slide.addText(text, {
    x, y, w, h,
    fontFace: opts.mono ? "Consolas" : "Microsoft YaHei",
    fontSize: opts.size ?? 18,
    color: opts.color ?? C.white,
    bold: opts.bold ?? false,
    breakLine: false,
    fit: "shrink",
    margin: opts.margin ?? 0.02,
    valign: opts.valign ?? "mid",
  });
}

function box(slide, x, y, w, h, fill, line = fill, radius = 0.08, transparency = 0) {
  slide.addShape(pptx.ShapeType.roundRect, {
    x, y, w, h,
    rectRadius: radius,
    fill: { color: fill, transparency },
    line: { color: line, transparency: line === fill ? 100 : 0, width: 1 },
  });
}

function line(slide, x1, y1, x2, y2, color = C.jade, width = 1.5) {
  slide.addShape(pptx.ShapeType.line, { x: x1, y: y1, w: x2 - x1, h: y2 - y1, line: { color, width } });
}

function img(slide, src, x, y, w, h) {
  const file = fs.existsSync(src) ? src : assets.bg;
  slide.addImage({ path: file, x, y, w, h });
}

function title(slide, titleText, kicker = "") {
  tx(slide, kicker, 0.7, 0.35, 5.5, 0.28, { size: 10.5, color: C.jade, bold: true });
  tx(slide, titleText, 0.7, 0.62, 8.4, 0.55, { size: 24, bold: true });
  slide.addShape(pptx.ShapeType.rect, { x: 0.7, y: 1.18, w: 0.72, h: 0.035, fill: { color: C.jade }, line: { transparency: 100 } });
}

function tag(slide, label, x, y, w = 1.35) {
  box(slide, x, y, w, 0.38, "0E3833", C.line);
  tx(slide, label, x + 0.12, y + 0.07, w - 0.24, 0.2, { size: 12, bold: true });
}

function code(slide, body, x, y, w, h, cap, size = 9.5) {
  box(slide, x, y, w, h, C.code, C.line);
  slide.addShape(pptx.ShapeType.roundRect, { x, y, w, h: 0.34, rectRadius: 0.08, fill: { color: "0B2925" }, line: { color: C.line, width: 1 } });
  tx(slide, cap, x + 0.16, y + 0.07, w - 0.3, 0.17, { size: 8.5, color: C.jade, mono: true, bold: true });
  tx(slide, body, x + 0.17, y + 0.45, w - 0.34, h - 0.55, { size, color: "D7FFF2", mono: true, valign: "top" });
}

function slide() {
  const s = pptx.addSlide();
  bg(s);
  return s;
}

{
  const s = slide();
  img(s, assets.bg, 0, 0, 13.333, 7.5);
  s.addShape(pptx.ShapeType.rect, { x: 0, y: 0, w: 13.333, h: 7.5, fill: { color: C.ink, transparency: 18 }, line: { transparency: 100 } });
  tx(s, "JADE", 0.75, 4.7, 5.8, 0.78, { size: 47, bold: true });
  tx(s, "中式神话 2D 平台跳跃原型", 0.78, 5.48, 5.3, 0.35, { size: 18, color: C.paper });
  tx(s, "Unity / 课程答辩 / 简约技术展示版", 0.8, 5.95, 4.6, 0.24, { size: 11, color: C.muted });
}

{
  const s = slide();
  img(s, assets.bg, 0, 0, 8.15, 7.5);
  s.addShape(pptx.ShapeType.rect, { x: 7.65, y: 0, w: 5.7, h: 7.5, fill: { color: C.ink }, line: { transparency: 100 } });
  img(s, assets.spirit, 8.1, 0.55, 4.4, 3.1);
  tx(s, "Jade 是一款中式神话题材的 Unity 2D 平台跳跃原型。", 8.25, 4.05, 4.15, 0.9, { size: 21, bold: true });
  tag(s, "山海", 8.25, 5.42, 0.95); tag(s, "灵玉", 9.35, 5.42, 0.95); tag(s, "水墨剪影", 10.45, 5.42, 1.45); tag(s, "可玩原型", 8.25, 5.96, 1.45);
}

{
  const s = slide();
  title(s, "开发路线", "FROM FEEL TO PLAYABLE LEVEL");
  line(s, 1.55, 3.55, 11.8, 3.55, C.line, 2);
  const items = [["Phase01", "手感原型", "跑、跳、镜头、复位"], ["Phase02", "灰盒关卡", "路线节奏与检查点"], ["Phase04", "可玩关卡", "Dash / Double Jump 教学"], ["ShanhaiGate", "场景落地", "山海门与洞穴串联"]];
  [1.0, 3.95, 6.9, 9.85].forEach((x, i) => {
    box(s, x, 2.35, 2.35, 1.95, i === 2 ? "123D37" : C.ink2, C.line);
    tx(s, items[i][0], x + 0.22, 2.62, 1.8, 0.18, { size: 10.5, color: C.jade, bold: true });
    tx(s, items[i][1], x + 0.22, 2.9, 1.8, 0.36, { size: 20, bold: true });
    tx(s, items[i][2], x + 0.22, 3.76, 1.9, 0.28, { size: 11.5, color: C.muted });
  });
}

{
  const s = slide();
  title(s, "核心玩法展示", "PLAYABLE LOOP");
  img(s, assets.cave, 0.7, 1.52, 5.75, 3.68);
  img(s, assets.terrain, 6.83, 1.52, 5.1, 1.83);
  img(s, assets.caveArt, 6.83, 3.55, 5.1, 1.65);
  tag(s, "移动", 1.35, 5.75); tag(s, "跳跃容错", 3.05, 5.75, 1.5); tag(s, "检查点", 4.75, 5.75); tag(s, "掉落复位", 6.45, 5.75, 1.5);
  tx(s, "用小规模可玩关卡先验证操作节奏，再替换正式美术。", 1.35, 6.55, 7.5, 0.3, { size: 16, color: C.paper });
}

{
  const s = slide();
  title(s, "玩家移动系统", "PlayerMotor2D.cs");
  code(s, snippets.motor, 0.7, 1.48, 7.75, 5.12, "PlayerMotor2D.cs / jump buffer + coyote time", 9);
  img(s, assets.spirit, 8.82, 1.4, 3.1, 2.3);
  tag(s, "响应快", 9.18, 4.05); tag(s, "可调参", 9.18, 4.6); tag(s, "容错手感", 9.18, 5.15, 1.5);
  tx(s, "土狼时间和跳跃缓存让平台边缘操作更友好。", 9.18, 6.05, 3.1, 0.55, { size: 17, bold: true, color: C.paper });
}

{
  const s = slide();
  title(s, "移动参数配置", "PlayerMovementSettings.cs");
  code(s, snippets.settings, 0.7, 1.45, 5.88, 5.18, "ScriptableObject / tunable movement profile", 9.3);
  box(s, 7.1, 1.62, 4.35, 4.2, "0D342F", C.line);
  tx(s, "关键参数", 7.45, 1.95, 2.0, 0.35, { size: 20, bold: true });
  tx(s, "maxRunSpeed        8\njumpHeight         4.2\ncoyoteTime         0.08\njumpBufferTime     0.12\ndashSpeed          15", 7.55, 2.58, 3.65, 1.75, { size: 17, color: "D7FFF2", mono: true, valign: "top" });
  tx(s, "把手感数据集中到配置资产，方便反复调参。", 7.45, 5.05, 3.55, 0.42, { size: 15, color: C.paper });
}

{
  const s = slide();
  title(s, "关卡生成与灰盒设计", "PrototypePhase04FirstPlayableLevelSceneBuilder.cs");
  code(s, snippets.level, 0.7, 1.48, 6.35, 5.08, "BuildScene() / runtime graybox route", 9.2);
  box(s, 7.5, 1.92, 4.35, 2.75, "0D342F", C.line);
  tx(s, "起点  →  短跳  →  分支收集", 7.85, 2.78, 3.7, 0.3, { size: 17, bold: true });
  tx(s, "检查点  →  上升平台  →  终点", 8.3, 3.55, 3.3, 0.3, { size: 17, bold: true, color: C.gold });
  tx(s, "灰盒先验证路线节奏：主路、可选路线、失败复位都能被快速调整。", 7.68, 5.2, 3.9, 0.6, { size: 15, color: C.paper });
}

{
  const s = slide();
  title(s, "能力与交互系统", "PlayerAbilityInventory2D.cs");
  code(s, snippets.ability, 0.7, 1.48, 6.35, 5.08, "Unlock ability / notify HUD + route logic", 9.3);
  img(s, assets.dash, 8.1, 1.48, 1.08, 1.08);
  img(s, assets.doubleJump, 9.62, 1.48, 1.08, 1.08);
  img(s, assets.frame, 11.1, 1.48, 1.08, 1.08);
  tx(s, "能力解锁让关卡出现新的通过方式。", 8.1, 3.05, 3.8, 0.62, { size: 21, bold: true });
  tag(s, "Dash", 8.1, 4.1, 1.0); tag(s, "Double Jump", 9.3, 4.1, 1.65); tag(s, "Shard Gate", 11.15, 4.1, 1.45);
}

{
  const s = slide();
  title(s, "HUD 与反馈", "GameplayHud2D.cs");
  img(s, assets.hud, 0.58, 1.35, 6.35, 2.55);
  img(s, assets.health, 7.42, 1.36, 1.25, 1.25);
  img(s, assets.dash, 9.05, 1.38, 1.1, 1.1);
  img(s, assets.doubleJump, 10.45, 1.38, 1.1, 1.1);
  code(s, snippets.hud, 0.7, 4.1, 7.68, 2.52, "GameplayHud2D.cs / sprite resources + refresh events", 8.8);
  tx(s, "UI 负责把生命、能力、暂停和提示反馈可视化。", 8.85, 4.8, 3.25, 0.88, { size: 20, bold: true });
}

{
  const s = slide();
  title(s, "敌人与机关", "MonsterLobber2D.cs / WideFallingStoneTrap2D.cs");
  img(s, assets.monster, 0.65, 1.45, 5.25, 2.84);
  code(s, snippets.monster, 6.18, 1.45, 3.18, 4.95, "MonsterLobber2D.cs / lob attack", 8.0);
  code(s, snippets.trap, 9.58, 1.45, 2.92, 4.95, "WideFallingStoneTrap2D.cs / state machine", 7.8);
  tag(s, "范围检测", 0.9, 5.38, 1.45); tag(s, "抛物线攻击", 2.55, 5.38, 1.7); tag(s, "陷阱触发", 4.45, 5.38, 1.45);
}

{
  const s = slide();
  img(s, assets.bg, 0, 0, 13.333, 7.5);
  s.addShape(pptx.ShapeType.rect, { x: 0, y: 0, w: 13.333, h: 7.5, fill: { color: C.ink, transparency: 35 }, line: { transparency: 100 } });
  tx(s, "美术资产与氛围", 0.72, 0.55, 5.3, 0.6, { size: 29, bold: true });
  tx(s, "用玉色、遗迹、森林和水墨雾感建立中式神话氛围。", 0.76, 1.2, 6.2, 0.3, { size: 15.5, color: C.paper });
  img(s, assets.spirit, 0.88, 4.5, 2.18, 1.5);
  img(s, assets.monster, 3.58, 4.43, 2.58, 1.5);
  img(s, assets.hud, 6.65, 4.55, 3.0, 1.28);
  img(s, assets.cave, 10.08, 4.5, 2.42, 1.35);
}

{
  const s = slide();
  title(s, "总结与下一步", "DELIVERABLES + NEXT");
  tx(s, "已完成", 1.45, 1.82, 1.5, 0.35, { size: 22, bold: true, color: C.jade });
  tag(s, "可玩场景", 1.45, 2.58, 1.55); tag(s, "移动手感", 1.45, 3.22, 1.55); tag(s, "能力交互", 1.45, 3.86, 1.55); tag(s, "HUD / 美术 / 音频", 1.45, 4.5, 2.25);
  line(s, 6.35, 1.6, 6.35, 5.95, C.line, 1.5);
  tx(s, "下一步", 7.35, 1.82, 1.5, 0.35, { size: 22, bold: true, color: C.gold });
  tag(s, "正式地图 UI", 7.35, 2.58, 1.8); tag(s, "存档系统", 7.35, 3.22, 1.55); tag(s, "更多敌人与能力回访", 7.35, 3.86, 2.55);
  img(s, assets.spirit, 9.42, 4.42, 2.5, 1.75);
}

fs.mkdirSync(path.dirname(OUT), { recursive: true });
await pptx.writeFile({ fileName: OUT });
console.log(OUT);
