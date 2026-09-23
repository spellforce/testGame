# Stacklands UI 反编译导出

此目录由 `GameScripts.dll` 的托管代码生成，只保留名称和职责明显属于 UI、通用视觉、动效、粒子或物理辅助的类型。导出时间：2026-09-24。

## 来源

- DLL：`D:\tools\Platform\Steam\steamapps\common\Stacklands\Stacklands_Data\Managed\GameScripts.dll`
- SHA-256：`34CEF5CC7ED9F3AE95760F5DB2164E3CF2CBCA1B3FE22D2B525E3732EA69058C`
- 文件数：66 个 `.decompiled.cs`
- 工具：ILSpy command-line decompiler 9.1.0.7988

## 导出范围

包含菜单、设置、存档选择、卡牌图鉴、提示框、按钮、滚动文本、状态栏、UI 转场、UI 画布和相关显示组件，例如 `MainMenu`、`OptionsScreen`、`CardopediaScreen`、`Tooltip`、`CustomButton`、`GameCanvas` 和 `Statusbar`。另外包含通用的视觉、动效、粒子和物理辅助类型：`ParticleCollider`、`PhysicsExtensions`、`CardAnimation`、`QueuedAnimation`、`ShapeDrawer`、`MaterialChanger`、`ImageEffect`、`FRILerp`、`ColorManager`、`FontManager`、`SpriteManager` 等。

## 有意排除

没有导出攻击与战斗专属类型（例如 `AttackAnimation*`、`Projectile`、`RangedProjectile`、`MagicProjectile`）、卡牌/资源生产、事件流程、世界管理、存档数据模型以及其他玩法机制类型。`GameScreen`、`DebugScreen`、`CutsceneScreen`、`Draggable`、`Hoverable`、`Interactable` 等同时包含明显玩法或输入逻辑的类型也被排除；本次保留的是通用碰撞/物理和视觉效果基础，而不是战斗碰撞规则。

## 使用说明

这些文件是供阅读和迁移 UI 结构的反编译结果，不是可直接编译的独立工程；它们仍引用 Unity、TextMeshPro、DOTween 以及游戏中的其他程序集。若要在新项目中使用，应以此目录为参考，逐个移植 UI 所需的字段、事件和资源引用，并重新实现被排除的游戏逻辑接口。

反编译不会恢复原始注释、项目文件、资源或完整的变量命名；若源 DLL 更新，应重新导出并核对 SHA-256。
