# Character: Jade Qilin Spirit

> Historical note: 当前主角已迁移到 `Assets/Design/Character_JadeSpirit.md` 和 `JadeSpirit` 动画链路。本文件仅保留旧 `JadeQilin` 资源背景与迁移记录。

## 角色定位

当前主角从四足灵兽调整为弱人形的中式童话玉灵精灵。它保留玉麒麟的青玉、祥瑞和云气意象，但不再是动物本体，而是一个由玉光、云布和小型灵体四肢组成的轻盈平台跳跃主角。

## 视觉关键词

- 中式童话玉灵精灵。
- 弱人形骨架，但脸、头发和衣服都不写实，避免像普通人类小孩。
- 圆润玉面、发光双眼、胸口玉芯、短云布层、短飘带、脚底微光。
- 小型玉叶冠和低矮麒麟角结，只作为祥瑞装饰，不做大兽角。
- 轮廓简洁、颜色明亮、动作轻巧，适合小尺寸平台跳跃。

## 当前实现

- 最终概念参考图：`Assets/Art/Concept/Characters/JadeSpirit_FinalConcept.png`
- 完整体动作参考图：`Assets/Art/Concept/Characters/JadeSpirit_FullSpriteSheetReference.png`
- 简化跑跳测试参考图：`Assets/Art/Concept/Characters/JadeSpirit_SimplifiedRunJumpSheetReference.png`
- 历史旧概念图：`Assets/Resources/Characters/JadeQilin.png`，仅保留归档，不再作为主角生成标准。
- 历史旧剪影图：`Assets/Resources/Characters/JadeQilin_Silhouette.png`，仅保留归档，不再作为主角生成标准。
- 当前游戏动画帧：`Assets/Resources/Characters/JadeQilinFrames/`
- 美术预览图：`Assets/Art/PrototypeArt/Characters/JadeQilinFrames_Preview.png`
- 玩家 prefab：`Assets/Resources/Prefabs/Player/JadeQilinPlayer.prefab`
- Animator Controller：`Assets/Animation/Characters/JadeQilin/JadeQilinPlayer.controller`
- 动画片段目录：`Assets/Animation/Characters/JadeQilin/`
- 当前游戏内使用 8 张透明 PNG 关键帧，内容已替换为更适合平台跳跃读图的简化玉灵跑跳测试版，保留原文件名和 GUID 以维持 Animator/prefab 引用。
- 游戏帧应贴近最终玉灵精灵概念图右侧小预览：保留白玉面、深色眼眶、发光大眼、高玉叶冠、短金白小角、云纹宽袖、胸口玉芯和短灵尾，不再退回旧四足动物或极简符号版。
- Phase01 和 Phase02 优先实例化玩家 prefab，并通过 Animator 播放 Idle、Run、Turn、JumpStart、JumpRise、JumpApex、Fall、Land。
- 当前高分辨率正式帧尚未补齐时，构建器会用 8 帧临时 sprite 兜底：`Idle01`、`Idle02`、`Run01`、`Run02`、`Run03`、`Run04`、`Jump`、`Fall`。
- 如果 prefab 或帧图加载失败，场景生成器会回退到程序化角色代理。
- 视觉对象保留在玩家物理根节点下，只影响显示、拉伸和压缩，不改变 `PlayerMotor2D` 与 `BoxCollider2D` 的碰撞。

## 动画管线落档

- 正式帧投放目录：`Assets/Resources/Characters/JadeQilinHighResFrames/`
- 规格文档：`Assets/Design/JadeQilin_AnimationImportSpec.md`
- 构建入口：Unity/Tuanjie 菜单 `Jade > Build Jade Qilin Player Assets`
- 导入设置由 `JadeQilinSpriteImportPostprocessor` 自动处理，重点是透明 PNG、Sprite、底部中心 pivot、512 PPU、无压缩、无 mipmap。
- `PlayerAnimationDriver2D` 负责把移动状态送入 Animator：`Speed01`、`VerticalSpeed`、`Grounded`、`Jumped`、`Landed`、`Turned`。
- `PlayerVisualFeedback2D` 仍然保留落地压缩、跑动前倾和尘土反馈，和 Animator 互补，不负责状态切换。

## 动作表现规则

- 物理碰撞仍由 `PlayerMotor2D` 和 `BoxCollider2D` 控制。
- 视觉只挂在 `VisualRoot` 下，不改变玩家碰撞体。
- 保留拉伸、压缩、落地尘土和跑动脚尘。
- 不使用角色拖影，避免精细 sprite 在移动中糊成一团。
- 不使用落地镜头震动，避免动作变硬、变重。

## 后续方向

- 先用当前临时 sprite 验证手感和关卡尺度。
- 如果比例、颜色或动作读法仍不满意，优先重画这 8 个关键姿态，而不是增加复杂系统。
- 正式动画前优先做清楚的轮廓、明亮的玉色识别点和顺滑的跑跳姿态。
- 正式帧替换时，先只替换 Idle/Run/Jump/Fall 核心状态并跑通 Phase01，再补 Turn、JumpStart、JumpApex、Land 的细节。
