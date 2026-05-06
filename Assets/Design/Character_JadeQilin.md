# Character: Jade Qilin Spirit

## 角色定位

第一版主角采用中式神话小灵兽方向，不做人形。当前名字暂定为“玉麒麟灵兽”，用于验证平台跳跃中的灵动感、速度感和童话神话气质。

## 视觉关键词

- 中式神话灵兽。
- 玉麒麟、九色鹿、灵狐、玉兔的混合意象。
- 墨色身体，但不能在小尺寸下糊成黑色一坨。
- 青玉发光角、胸口玉光、尾部流光、脚步微光。
- 轻盈、漂亮、灵动，不厚重。

## 当前实现

- 概念参考图：`Assets/Resources/Characters/JadeQilin.png`
- 剪影参考图：`Assets/Resources/Characters/JadeQilin_Silhouette.png`
- 临时游戏动画帧：`Assets/Resources/Characters/JadeQilinFrames/`
- 美术预览图：`Assets/Art/PrototypeArt/Characters/JadeQilinFrames_Preview.png`
- 当前游戏内不直接使用整张生成图。整图缩小后在深色背景里会读成黑块，不适合验证移动手感。
- Phase01 和 Phase02 优先使用 8 帧临时 sprite：`Idle01`、`Idle02`、`Run01`、`Run02`、`Run03`、`Run04`、`Jump`、`Fall`。
- 如果帧图加载失败，场景生成器会回退到程序化灵兽代理。
- 临时 sprite 保留在 `VisualRoot_TemporaryCharacter` 下，只影响显示、拉伸和压缩，不改变物理碰撞。

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
