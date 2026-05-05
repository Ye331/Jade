# Phase 01 Basic Movement Tuning

目标手感：轻盈、响应快、有容错。当前阶段只做基础操作，不做冲刺、墙跳、二段跳或技能解锁。

## 默认参数

- `maxRunSpeed = 8`：最大水平速度。
- `groundAcceleration = 70`：地面按方向键时的加速速度。
- `groundDeceleration = 90`：松开方向键后的减速速度。
- `turnDeceleration = 125`：反向输入时的转身减速，越高越干脆。
- `airAcceleration = 45`：空中方向控制力度。
- `airDeceleration = 30`：空中松开方向键后的减速力度。
- `jumpHeight = 3.35`：长按跳跃时的大致跳跃高度。
- `timeToJumpApex = 0.39`：到达跳跃最高点所需时间，越小越利落。
- `coyoteTime = 0.10`：离开平台后仍允许跳跃的时间。
- `jumpBufferTime = 0.12`：落地前提前按跳的缓存时间。
- `jumpCutVelocityMultiplier = 0.45`：松开跳跃键时保留的上升速度比例。
- `fallGravityMultiplier = 1.85`：下落时重力倍率。
- `lowJumpGravityMultiplier = 2.35`：松开跳跃键后的上升重力倍率。
- `maxFallSpeed = 16.5`：最大下落速度。

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
