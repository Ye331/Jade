# JadeSpirit Animation Import Spec

当前玩家使用 `JadeSpiritPlayer.prefab` 和 `JadeSpiritPlayer.controller`。正式透明 PNG 帧准备好后，放入 JadeSpirit 高分辨率帧目录，再运行 `Jade > Build Jade Spirit Player Assets` 重建 clips、controller 和 prefab。

## 帧图要求

- 透明 PNG。
- 推荐源画布：`1024x1024`。
- 导入 PPU：`512`。
- Pivot：底部中心。
- 同一动作组内保持相同画布尺寸、脚底基线和默认朝向。
- 角色主体建议占画布高度约 70%-78%，避免进游戏后过小或被裁切。

## 当前必需命名

当前基础状态机只要求这些帧组：

- `JadeSpirit_Idle_01`
- `JadeSpirit_Run_01` 到 `JadeSpirit_Run_08`
- `JadeSpirit_JumpRise_01` 到 `JadeSpirit_JumpRise_03`
- `JadeSpirit_JumpApex_01` 到 `JadeSpirit_JumpApex_02`
- `JadeSpirit_Fall_01` 到 `JadeSpirit_Fall_03`

`JumpStart`、`Land`、`Turn` 帧可以继续作为历史或备用资源留在项目里，但它们不是当前 active controller 的必需帧，也不是当前基础状态。

## 导入设置

`Assets/Resources/Characters/JadeSpiritHighResFrames/` 下的帧会由 editor postprocessor 自动套用这些设置：

- Sprite 2D，Single Sprite。
- Filter Mode: Bilinear。
- Compression: None。
- Mip Maps: Off。
- Max Size: 2048。
- Alpha Is Transparency: On。
- Pivot: Bottom Center。

## 构建步骤

1. 把正式帧放入 `Assets/Resources/Characters/JadeSpiritHighResFrames/`。
2. 打开 Unity/Tuanjie。
3. 运行 `Jade > Build Jade Spirit Player Assets`。
4. 优先检查 Phase03，再检查 Phase01 和 Phase02。

## 期望运行表现

- `Idle` 使用 1 帧，方便调试体型和碰撞。
- `Run` 使用 8 帧循环。
- `Jumped` 直接进入 `JumpRise`。
- `JumpRise`、`JumpApex`、`Fall` 根据垂直速度切换。
- `Landed` 立即回到 `Idle` 或 `Run`，不播放落地动画。
- 空中视觉反馈不拉伸角色轮廓。

## 兜底规则

如果高分辨率帧缺失，builder 可以使用当前原型 `JadeQilinFrames` sprite 作为临时兜底，保证 prefab 和 controller 仍能重建。兜底资源只用于链路连续性，不是当前视觉目标。
