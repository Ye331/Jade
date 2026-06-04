# Background: Jade Myth Forest

## 用途

`JadeMythForest_Background` 是当前灰盒阶段的临时远景背景，用来让角色和关卡进入“中式神话 + 玉色灵境”的方向。

## 设计原则

- 作为远景氛围层，不参与碰撞和关卡路径判断。
- 低对比、低饱和，避免抢角色、平台和收集物。
- 主题元素包括远山、幽林、残庙、天河、玉色灵光。
- 当前使用单张相机跟随背景，不做横向重复铺片，避免出现贴图拼接感。

## 当前资源

- 源图：`Assets/Art/PrototypeArt/Backgrounds/JadeMythForest_Background.png`
- 运行时加载：`Assets/Resources/Backgrounds/JadeMythForest_Background.png`
- 使用场景：Phase01 基础移动测试、Phase02 灰盒小关卡。
- 实现方式：背景挂在主相机下，自动按正交相机尺寸放大并留出少量 overscan。

## 后续方向

- 如果背景太抢眼，优先降低 `SpriteRenderer.color` 的 alpha 或亮度。
- 如果需要更像正式游戏，再拆成远山、云雾、残庙、前景剪影等多层视差。
- 正式美术阶段再替换为分层背景资产，不影响当前玩家碰撞和关卡结构。
