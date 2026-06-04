import fs from "node:fs";
import path from "node:path";

const ROOT = "D:/unity/Jade";
const WORK = `${ROOT}/outputs/manual-20260604-jade/presentations/jade-defense/ooxml-build-${Date.now()}`;
const OUT = `${ROOT}/outputs/manual-20260604-jade/presentations/jade-defense/output/Jade_course_defense_minimal_tech.pptx`;
const EMU = 914400;
const SW = 12192000;
const SH = 6858000;

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

function esc(s) {
  return String(s).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
}

function emu(v) {
  return Math.round(v * EMU);
}

function hexAlpha(transparency) {
  return Math.round((100 - transparency) * 1000);
}

function solid(color, transparency = 0) {
  return `<a:solidFill><a:srgbClr val="${color}">${transparency ? `<a:alpha val="${hexAlpha(transparency)}"/>` : ""}</a:srgbClr></a:solidFill>`;
}

function shape(id, x, y, w, h, fill, line = null, transparency = 0, geom = "rect") {
  const ln = line ? `<a:ln w="9525">${solid(line)}</a:ln>` : `<a:ln><a:noFill/></a:ln>`;
  return `<p:sp><p:nvSpPr><p:cNvPr id="${id}" name="Shape ${id}"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm><a:off x="${emu(x)}" y="${emu(y)}"/><a:ext cx="${emu(w)}" cy="${emu(h)}"/></a:xfrm><a:prstGeom prst="${geom}"><a:avLst/></a:prstGeom>${solid(fill, transparency)}${ln}</p:spPr><p:txBody><a:bodyPr/><a:lstStyle/><a:p/></p:txBody></p:sp>`;
}

function textBox(id, text, x, y, w, h, opts = {}) {
  const size = Math.round((opts.size ?? 18) * 100);
  const font = opts.mono ? "Consolas" : "Microsoft YaHei";
  const bold = opts.bold ? ` b="1"` : "";
  const color = opts.color ?? C.white;
  const paras = String(text).split("\n").map(line => `<a:p><a:r><a:rPr lang="zh-CN" sz="${size}"${bold}><a:solidFill><a:srgbClr val="${color}"/></a:solidFill><a:latin typeface="${font}"/><a:ea typeface="${font}"/></a:rPr><a:t>${esc(line)}</a:t></a:r></a:p>`).join("");
  return `<p:sp><p:nvSpPr><p:cNvPr id="${id}" name="Text ${id}"/><p:cNvSpPr txBox="1"/><p:nvPr/></p:nvSpPr><p:spPr><a:xfrm><a:off x="${emu(x)}" y="${emu(y)}"/><a:ext cx="${emu(w)}" cy="${emu(h)}"/></a:xfrm><a:prstGeom prst="rect"><a:avLst/></a:prstGeom><a:noFill/><a:ln><a:noFill/></a:ln></p:spPr><p:txBody><a:bodyPr wrap="square" anchor="${opts.anchor ?? "mid"}" lIns="45720" tIns="22860" rIns="45720" bIns="22860"><a:normAutofit/></a:bodyPr><a:lstStyle/>${paras}</p:txBody></p:sp>`;
}

function pic(id, rId, x, y, w, h) {
  return `<p:pic><p:nvPicPr><p:cNvPr id="${id}" name="Picture ${id}"/><p:cNvPicPr><a:picLocks noChangeAspect="0"/></p:cNvPicPr><p:nvPr/></p:nvPicPr><p:blipFill><a:blip r:embed="${rId}"/><a:stretch><a:fillRect/></a:stretch></p:blipFill><p:spPr><a:xfrm><a:off x="${emu(x)}" y="${emu(y)}"/><a:ext cx="${emu(w)}" cy="${emu(h)}"/></a:xfrm><a:prstGeom prst="rect"><a:avLst/></a:prstGeom></p:spPr></p:pic>`;
}

function tag(id, label, x, y, w = 1.35) {
  return shape(id, x, y, w, 0.38, "0E3833", C.line, 0, "roundRect") + textBox(id + 1, label, x + 0.08, y + 0.05, w - 0.16, 0.25, { size: 12, bold: true });
}

function codeBlock(id, body, x, y, w, h, cap, size = 9) {
  return shape(id, x, y, w, h, C.code, C.line, 0, "roundRect")
    + shape(id + 1, x, y, w, 0.34, "0B2925", C.line, 0, "roundRect")
    + textBox(id + 2, cap, x + 0.12, y + 0.04, w - 0.2, 0.22, { size: 8.5, color: C.jade, bold: true, mono: true })
    + textBox(id + 3, body, x + 0.12, y + 0.42, w - 0.22, h - 0.48, { size, color: "D7FFF2", mono: true, anchor: "t" });
}

function title(id, main, kicker = "") {
  return textBox(id, kicker, 0.7, 0.35, 5.5, 0.26, { size: 10.5, color: C.jade, bold: true })
    + textBox(id + 1, main, 0.7, 0.62, 8.3, 0.52, { size: 24, bold: true })
    + shape(id + 2, 0.7, 1.18, 0.72, 0.035, C.jade);
}

function buildSlides() {
  const slides = [];
  const media = [];
  const addMedia = (src) => {
    const id = media.length + 1;
    const ext = path.extname(src).toLowerCase() === ".jpg" ? "jpg" : "png";
    media.push({ src: fs.existsSync(src) ? src : assets.bg, name: `image${id}.${ext}` });
    return { rId: `rId${id + 1}`, target: `../media/image${id}.${ext}` };
  };
  const slide = (nodes, bg = C.ink) => {
    slides.push({ bg, nodes });
  };
  let m;

  m = addMedia(assets.bg);
  slide(pic(2, m.rId, 0, 0, 13.333, 7.5) + shape(3, 0, 0, 13.333, 7.5, C.ink, null, 18) + textBox(4, "JADE", 0.75, 4.7, 5.8, 0.78, { size: 47, bold: true }) + textBox(5, "中式神话 2D 平台跳跃原型", 0.78, 5.48, 5.3, 0.35, { size: 18, color: C.paper }) + textBox(6, "Unity / 课程答辩 / 简约技术展示版", 0.8, 5.95, 4.6, 0.24, { size: 11, color: C.muted }));

  const bg2 = addMedia(assets.bg), sp2 = addMedia(assets.spirit);
  slide(pic(2, bg2.rId, 0, 0, 8.15, 7.5) + shape(3, 7.65, 0, 5.7, 7.5, C.ink) + pic(4, sp2.rId, 8.1, 0.55, 4.4, 3.1) + textBox(5, "Jade 是一款中式神话题材的 Unity 2D 平台跳跃原型。", 8.25, 4.05, 4.15, 0.9, { size: 21, bold: true }) + tag(10, "山海", 8.25, 5.42, 0.95) + tag(20, "灵玉", 9.35, 5.42, 0.95) + tag(30, "水墨剪影", 10.45, 5.42, 1.45) + tag(40, "可玩原型", 8.25, 5.96, 1.45));

  slide(title(2, "开发路线", "FROM FEEL TO PLAYABLE LEVEL") + shape(10, 1.55, 3.54, 10.25, 0.035, C.line) + [
    ["Phase01", "手感原型", "跑、跳、镜头、复位", 1.0], ["Phase02", "灰盒关卡", "路线节奏与检查点", 3.95], ["Phase04", "可玩关卡", "Dash / Double Jump 教学", 6.9], ["ShanhaiGate", "场景落地", "山海门与洞穴串联", 9.85],
  ].map((it, i) => shape(20 + i * 10, it[3], 2.35, 2.35, 1.95, i === 2 ? "123D37" : C.ink2, C.line, 0, "roundRect") + textBox(21 + i * 10, it[0], it[3] + 0.18, 2.6, 1.8, 0.22, { size: 10.5, color: C.jade, bold: true }) + textBox(22 + i * 10, it[1], it[3] + 0.18, 2.88, 1.85, 0.36, { size: 20, bold: true }) + textBox(23 + i * 10, it[2], it[3] + 0.18, 3.74, 1.95, 0.28, { size: 11.5, color: C.muted })).join(""));

  const cv = addMedia(assets.cave), tr = addMedia(assets.terrain), ca = addMedia(assets.caveArt);
  slide(title(2, "核心玩法展示", "PLAYABLE LOOP") + pic(10, cv.rId, 0.7, 1.52, 5.75, 3.68) + pic(11, tr.rId, 6.83, 1.52, 5.1, 1.83) + pic(12, ca.rId, 6.83, 3.55, 5.1, 1.65) + tag(20, "移动", 1.35, 5.75) + tag(30, "跳跃容错", 3.05, 5.75, 1.5) + tag(40, "检查点", 4.75, 5.75) + tag(50, "掉落复位", 6.45, 5.75, 1.5) + textBox(60, "用小规模可玩关卡先验证操作节奏，再替换正式美术。", 1.35, 6.55, 7.5, 0.3, { size: 16, color: C.paper }));

  const sp = addMedia(assets.spirit);
  slide(title(2, "玩家移动系统", "PlayerMotor2D.cs") + codeBlock(10, snippets.motor, 0.7, 1.48, 7.75, 5.12, "PlayerMotor2D.cs / jump buffer + coyote time", 9) + pic(30, sp.rId, 8.82, 1.4, 3.1, 2.3) + tag(40, "响应快", 9.18, 4.05) + tag(50, "可调参", 9.18, 4.6) + tag(60, "容错手感", 9.18, 5.15, 1.5) + textBox(70, "土狼时间和跳跃缓存让平台边缘操作更友好。", 9.18, 6.05, 3.1, 0.55, { size: 17, bold: true, color: C.paper }));

  slide(title(2, "移动参数配置", "PlayerMovementSettings.cs") + codeBlock(10, snippets.settings, 0.7, 1.45, 5.88, 5.18, "ScriptableObject / tunable movement profile", 9.3) + shape(30, 7.1, 1.62, 4.35, 4.2, "0D342F", C.line, 0, "roundRect") + textBox(31, "关键参数", 7.45, 1.95, 2.0, 0.35, { size: 20, bold: true }) + textBox(32, "maxRunSpeed        8\njumpHeight         4.2\ncoyoteTime         0.08\njumpBufferTime     0.12\ndashSpeed          15", 7.55, 2.58, 3.65, 1.75, { size: 17, color: "D7FFF2", mono: true, anchor: "t" }) + textBox(33, "把手感数据集中到配置资产，方便反复调参。", 7.45, 5.05, 3.55, 0.42, { size: 15, color: C.paper }));

  slide(title(2, "关卡生成与灰盒设计", "PrototypePhase04FirstPlayableLevelSceneBuilder.cs") + codeBlock(10, snippets.level, 0.7, 1.48, 6.35, 5.08, "BuildScene() / runtime graybox route", 9.2) + shape(30, 7.5, 1.92, 4.35, 2.75, "0D342F", C.line, 0, "roundRect") + textBox(31, "起点  →  短跳  →  分支收集", 7.85, 2.78, 3.7, 0.3, { size: 17, bold: true }) + textBox(32, "检查点  →  上升平台  →  终点", 8.3, 3.55, 3.3, 0.3, { size: 17, bold: true, color: C.gold }) + textBox(33, "灰盒先验证路线节奏：主路、可选路线、失败复位都能被快速调整。", 7.68, 5.2, 3.9, 0.6, { size: 15, color: C.paper }));

  const dash = addMedia(assets.dash), dj = addMedia(assets.doubleJump), fr = addMedia(assets.frame);
  slide(title(2, "能力与交互系统", "PlayerAbilityInventory2D.cs") + codeBlock(10, snippets.ability, 0.7, 1.48, 6.35, 5.08, "Unlock ability / notify HUD + route logic", 9.3) + pic(30, dash.rId, 8.1, 1.48, 1.08, 1.08) + pic(31, dj.rId, 9.62, 1.48, 1.08, 1.08) + pic(32, fr.rId, 11.1, 1.48, 1.08, 1.08) + textBox(33, "能力解锁让关卡出现新的通过方式。", 8.1, 3.05, 3.8, 0.62, { size: 21, bold: true }) + tag(40, "Dash", 8.1, 4.1, 1.0) + tag(50, "Double Jump", 9.3, 4.1, 1.65) + tag(60, "Shard Gate", 11.15, 4.1, 1.45));

  const hud = addMedia(assets.hud), health = addMedia(assets.health), dash2 = addMedia(assets.dash), dj2 = addMedia(assets.doubleJump);
  slide(title(2, "HUD 与反馈", "GameplayHud2D.cs") + pic(10, hud.rId, 0.58, 1.35, 6.35, 2.55) + pic(11, health.rId, 7.42, 1.36, 1.25, 1.25) + pic(12, dash2.rId, 9.05, 1.38, 1.1, 1.1) + pic(13, dj2.rId, 10.45, 1.38, 1.1, 1.1) + codeBlock(20, snippets.hud, 0.7, 4.1, 7.68, 2.52, "GameplayHud2D.cs / sprite resources + refresh events", 8.8) + textBox(40, "UI 负责把生命、能力、暂停和提示反馈可视化。", 8.85, 4.8, 3.25, 0.88, { size: 20, bold: true }));

  const mon = addMedia(assets.monster);
  slide(title(2, "敌人与机关", "MonsterLobber2D.cs / WideFallingStoneTrap2D.cs") + pic(10, mon.rId, 0.65, 1.45, 5.25, 2.84) + codeBlock(20, snippets.monster, 6.18, 1.45, 3.18, 4.95, "MonsterLobber2D.cs / lob attack", 8) + codeBlock(40, snippets.trap, 9.58, 1.45, 2.92, 4.95, "WideFallingStoneTrap2D.cs / state machine", 7.8) + tag(60, "范围检测", 0.9, 5.38, 1.45) + tag(70, "抛物线攻击", 2.55, 5.38, 1.7) + tag(80, "陷阱触发", 4.45, 5.38, 1.45));

  const bg11 = addMedia(assets.bg), sp11 = addMedia(assets.spirit), mon11 = addMedia(assets.monster), hud11 = addMedia(assets.hud), cv11 = addMedia(assets.cave);
  slide(pic(2, bg11.rId, 0, 0, 13.333, 7.5) + shape(3, 0, 0, 13.333, 7.5, C.ink, null, 35) + textBox(4, "美术资产与氛围", 0.72, 0.55, 5.3, 0.6, { size: 29, bold: true }) + textBox(5, "用玉色、遗迹、森林和水墨雾感建立中式神话氛围。", 0.76, 1.2, 6.2, 0.3, { size: 15.5, color: C.paper }) + pic(10, sp11.rId, 0.88, 4.5, 2.18, 1.5) + pic(11, mon11.rId, 3.58, 4.43, 2.58, 1.5) + pic(12, hud11.rId, 6.65, 4.55, 3.0, 1.28) + pic(13, cv11.rId, 10.08, 4.5, 2.42, 1.35));

  const sp12 = addMedia(assets.spirit);
  slide(title(2, "总结与下一步", "DELIVERABLES + NEXT") + textBox(10, "已完成", 1.45, 1.82, 1.5, 0.35, { size: 22, bold: true, color: C.jade }) + tag(20, "可玩场景", 1.45, 2.58, 1.55) + tag(30, "移动手感", 1.45, 3.22, 1.55) + tag(40, "能力交互", 1.45, 3.86, 1.55) + tag(50, "HUD / 美术 / 音频", 1.45, 4.5, 2.25) + shape(60, 6.35, 1.6, 0.02, 4.35, C.line) + textBox(70, "下一步", 7.35, 1.82, 1.5, 0.35, { size: 22, bold: true, color: C.gold }) + tag(80, "正式地图 UI", 7.35, 2.58, 1.8) + tag(90, "存档系统", 7.35, 3.22, 1.55) + tag(100, "更多敌人与能力回访", 7.35, 3.86, 2.55) + pic(110, sp12.rId, 9.42, 4.42, 2.5, 1.75));

  return { slides, media };
}

function writeFile(rel, data) {
  const file = path.join(WORK, rel);
  fs.mkdirSync(path.dirname(file), { recursive: true });
  fs.writeFileSync(file, data);
}

fs.mkdirSync(WORK, { recursive: true });
const { slides, media } = buildSlides();

writeFile("[Content_Types].xml", `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Default Extension="png" ContentType="image/png"/><Default Extension="jpg" ContentType="image/jpeg"/><Override PartName="/ppt/presentation.xml" ContentType="application/vnd.openxmlformats-officedocument.presentationml.presentation.main+xml"/><Override PartName="/ppt/theme/theme1.xml" ContentType="application/vnd.openxmlformats-officedocument.theme+xml"/><Override PartName="/ppt/slideMasters/slideMaster1.xml" ContentType="application/vnd.openxmlformats-officedocument.presentationml.slideMaster+xml"/><Override PartName="/ppt/slideLayouts/slideLayout1.xml" ContentType="application/vnd.openxmlformats-officedocument.presentationml.slideLayout+xml"/>${slides.map((_, i) => `<Override PartName="/ppt/slides/slide${i + 1}.xml" ContentType="application/vnd.openxmlformats-officedocument.presentationml.slide+xml"/>`).join("")}</Types>`);
writeFile("_rels/.rels", `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="ppt/presentation.xml"/></Relationships>`);
writeFile("ppt/presentation.xml", `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><p:presentation xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main"><p:sldMasterIdLst><p:sldMasterId id="2147483648" r:id="rId1"/></p:sldMasterIdLst><p:sldIdLst>${slides.map((_, i) => `<p:sldId id="${256 + i}" r:id="rId${i + 2}"/>`).join("")}</p:sldIdLst><p:sldSz cx="${SW}" cy="${SH}" type="wide"/><p:notesSz cx="6858000" cy="9144000"/></p:presentation>`);
writeFile("ppt/_rels/presentation.xml.rels", `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideMaster" Target="slideMasters/slideMaster1.xml"/>${slides.map((_, i) => `<Relationship Id="rId${i + 2}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide" Target="slides/slide${i + 1}.xml"/>`).join("")}<Relationship Id="rId${slides.length + 2}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme" Target="theme/theme1.xml"/></Relationships>`);
writeFile("ppt/theme/theme1.xml", `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><a:theme xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" name="Jade"><a:themeElements><a:clrScheme name="Jade"><a:dk1><a:srgbClr val="${C.ink}"/></a:dk1><a:lt1><a:srgbClr val="${C.white}"/></a:lt1><a:dk2><a:srgbClr val="${C.ink2}"/></a:dk2><a:lt2><a:srgbClr val="${C.paper}"/></a:lt2><a:accent1><a:srgbClr val="${C.jade}"/></a:accent1><a:accent2><a:srgbClr val="${C.gold}"/></a:accent2><a:accent3><a:srgbClr val="${C.line}"/></a:accent3><a:accent4><a:srgbClr val="${C.muted}"/></a:accent4><a:accent5><a:srgbClr val="FFFFFF"/></a:accent5><a:accent6><a:srgbClr val="000000"/></a:accent6><a:hlink><a:srgbClr val="${C.jade}"/></a:hlink><a:folHlink><a:srgbClr val="${C.gold}"/></a:folHlink></a:clrScheme><a:fontScheme name="Jade"><a:majorFont><a:latin typeface="Microsoft YaHei"/><a:ea typeface="Microsoft YaHei"/></a:majorFont><a:minorFont><a:latin typeface="Microsoft YaHei"/><a:ea typeface="Microsoft YaHei"/></a:minorFont></a:fontScheme><a:fmtScheme name="Jade"><a:fillStyleLst><a:solidFill><a:schemeClr val="accent1"/></a:solidFill></a:fillStyleLst><a:lnStyleLst><a:ln w="9525"><a:solidFill><a:schemeClr val="accent1"/></a:solidFill></a:ln></a:lnStyleLst><a:effectStyleLst><a:effectStyle><a:effectLst/></a:effectStyle></a:effectStyleLst><a:bgFillStyleLst><a:solidFill><a:schemeClr val="dk1"/></a:solidFill></a:bgFillStyleLst></a:fmtScheme></a:themeElements></a:theme>`);
writeFile("ppt/slideMasters/slideMaster1.xml", `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><p:sldMaster xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main"><p:cSld><p:spTree><p:nvGrpSpPr><p:cNvPr id="1" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr><p:grpSpPr/></p:spTree></p:cSld><p:sldLayoutIdLst><p:sldLayoutId id="2147483649" r:id="rId1"/></p:sldLayoutIdLst><p:txStyles><p:titleStyle/><p:bodyStyle/><p:otherStyle/></p:txStyles></p:sldMaster>`);
writeFile("ppt/slideMasters/_rels/slideMaster1.xml.rels", `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideLayout" Target="../slideLayouts/slideLayout1.xml"/><Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme" Target="../theme/theme1.xml"/></Relationships>`);
writeFile("ppt/slideLayouts/slideLayout1.xml", `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><p:sldLayout xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main" type="blank" preserve="1"><p:cSld name="Blank"><p:spTree><p:nvGrpSpPr><p:cNvPr id="1" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr><p:grpSpPr/></p:spTree></p:cSld></p:sldLayout>`);
writeFile("ppt/slideLayouts/_rels/slideLayout1.xml.rels", `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideMaster" Target="../slideMasters/slideMaster1.xml"/></Relationships>`);

let mediaIndex = 0;
for (let i = 0; i < slides.length; i++) {
  const slide = slides[i];
  const mediaCount = (slide.nodes.match(/r:embed="rId\d+"/g) || []).length;
  const rels = [`<Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideLayout" Target="../slideLayouts/slideLayout1.xml"/>`];
  for (let j = 0; j < mediaCount; j++) {
    const med = media[mediaIndex + j];
    rels.push(`<Relationship Id="rId${j + 2}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/image" Target="../media/${med.name}"/>`);
  }
  mediaIndex += mediaCount;
  writeFile(`ppt/slides/slide${i + 1}.xml`, `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><p:sld xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main"><p:cSld><p:bg><p:bgPr>${solid(slide.bg)}</p:bgPr></p:bg><p:spTree><p:nvGrpSpPr><p:cNvPr id="1" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr><p:grpSpPr/>${slide.nodes}</p:spTree></p:cSld><p:clrMapOvr><a:masterClrMapping/></p:clrMapOvr></p:sld>`);
  writeFile(`ppt/slides/_rels/slide${i + 1}.xml.rels`, `<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">${rels.join("")}</Relationships>`);
}

for (const med of media) {
  fs.mkdirSync(path.join(WORK, "ppt/media"), { recursive: true });
  fs.copyFileSync(med.src, path.join(WORK, "ppt/media", med.name));
}

fs.writeFileSync(`${WORK}/manifest.txt`, `slides=${slides.length}\nmedia=${media.length}\n`);
fs.writeFileSync(`${WORK}/out_path.txt`, OUT);
console.log(WORK);
