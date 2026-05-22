# Phase 01 Basic Movement Tuning

目标手感：轻盈、响应快、有容错。当前阶段只做基础操作，不做冲刺、墙跳、二段跳或技能解锁。

## 当前实现落档

- 测试场景：`Assets/Scenes/Prototype/Phase01_BasicMovement.scene`
- 场景生成器：`PrototypeBasicMovementSceneBuilder`
- 玩家移动：`PlayerInputReader` + `PlayerMotor2D` + `PlayerMovementSettings`
- 视觉反馈：`PlayerVisualFeedback2D` + `PlayerAnimationDriver2D`
- 相机：`SimpleCameraFollow2D`
- 掉落重置：`RespawnZone2D`
- 玩家优先使用 `Assets/Resources/Prefabs/Player/JadeSpiritPlayer.prefab`；新 prefab 不可用时回退到旧 `JadeQilinPlayer.prefab`，再回退到场景生成器内的临时 sprite 或程序化角色代理。
- 当前场景重点测试长跑、短平台边缘容错、高低平台跳跃、可变跳高、相机跟随和掉落复位。

## 默认参数

- `maxRunSpeed = 8`：最大水平速度。
- `groundAcceleration = 70`：地面按方向键时的加速速度。
- `groundDeceleration = 90`：松开方向键后的减速速度。
- `turnDeceleration = 125`：反向输入时的转身减速，越高越干脆。
- `airAcceleration = 45`：空中方向控制力度。
- `airDeceleration = 30`：空中松开方向键后的减速力度。
- `jumpHeight = 3.35`：长按跳跃时的大致跳跃高度。
- `timeToJumpApex = 0.46`：到达跳跃最高点所需时间，当前略加长以增强滞空读图。
- `coyoteTime = 0.08`：离开平台后仍允许跳跃的时间，这才是通常说的“土狼时间”。
- `jumpBufferTime = 0.12`：落地前提前按跳的缓存时间。
- `jumpCutVelocityMultiplier = 0.45`：松开跳跃键时保留的上升速度比例。
- `fallGravityMultiplier = 1.55`：下落时重力倍率，当前略降低以延长空中停留。
- `lowJumpGravityMultiplier = 2.05`：松开跳跃键后的上升重力倍率。
- `maxFallSpeed = 15`：最大下落速度。

## 推荐调参顺序

1. 先调 `maxRunSpeed`，确定玩家水平移动的快慢。
2. 再调 `groundAcceleration`、`groundDeceleration`、`turnDeceleration`，确定起步、停下、转身是否顺手。
3. 调 `jumpHeight` 和 `timeToJumpApex`，确定跳跃高度和轻盈感。
4. 调 `fallGravityMultiplier`，让下落更明确，不要飘太久。
5. 最后调 `coyoteTime` 和 `jumpBufferTime`，让边缘跳和提前按跳更舒服。

## 视觉反馈参数

`PlayerVisualFeedback2D` 只负责临时角色代理的表现，不改变碰撞和移动计算。

- `runTiltDegrees`：跑动时身体前倾角度。
- `fastSpeedThreshold`：超过该水平速度后开启更明显的速度反馈。
- `squashReturnSpeed`：拉伸、压缩恢复到正常比例的速度。
落地反馈只使用角色压缩和尘土粒子，不使用镜头震动。当前方向更偏灵动轻盈，避免落地显得过重。

如果觉得角色“太软”，先降低拉伸压缩幅度或提高 `squashReturnSpeed`。如果觉得还是像方块滑动，先增加跑动前倾和拖尾可见度。

## 当前暂不包含

- 墙滑、墙跳。
- 冲刺、空中冲刺。
- 二段跳。
- 攀爬、抓墙。
- 技能解锁。
- 战斗、敌人、存档、地图 UI。

## 下一轮调参关注点

- 如果短平台太简单，优先缩短平台间距或降低 `coyoteTime`，不要先加新能力。
- 如果跳跃显得飘，优先提高 `fallGravityMultiplier` 或略缩短 `timeToJumpApex`。
- 如果角色缩小后跳得过高，优先调整 `jumpHeight`，保持 Phase01 与 Phase02 共用同一份 `LightweightMovement.asset`。
- 如果动画读法影响判断落点，先降低视觉拉伸压缩幅度，再考虑改 Animator 状态。
