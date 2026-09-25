using GameConfig;
using GameFramework.Procedure;
using Godot;
using GodotGameFramework;
using GodotGameFramework.HotUpdate;
using GodotGameFramework.UI;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using GameLogic;

/// <summary>
/// 游戏流程。
///
/// 进入后把棋盘场景挂到框架根节点之下（`Framework/GameFramework.tscn` 的
/// 默认 2D 画布），于是 F5 从主场景启动就能直接进游戏。
///
/// 挂载点选在根节点而不是自身：GameFramework 根节点活到进程结束，
/// 而 ProcedureGame 自身会随流程切换被回收。棋盘必须比流程活得久，
/// 否则切换流程会连带把棋盘一起销毁。
/// </summary>
public class ProcedureGame : ProcedureBase
{
    /// <summary>棋盘场景路径。</summary>
    private const string BoardScenePath = "res://TheGame/Scenes/GameBoard.tscn";

    /// <summary>已挂载的棋盘实例。</summary>
    private static Node s_Board;

    /// <summary>
    /// 状态初始化（只调用一次）。
    /// </summary>
    protected internal override void OnInit(ProcedureOwner procedureOwner)
    {
        base.OnInit(procedureOwner);
    }

    /// <summary>
    /// 进入流程：标记启动成功、挂载棋盘、收掉加载遮罩。
    /// </summary>
    protected internal override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

        // 标记启动成功：游戏已进入可玩状态，后续崩溃不再归因于热更
        HotUpdateSafetyGuard.MarkStartupSuccess();

        MountBoard();

        // 棋盘已就绪，收掉 ProcedurePrelode 留下的加载遮罩
        LoadingForm.Current?.CloseLoading();
    }

    /// <summary>
    /// 每帧更新。
    /// </summary>
    protected internal override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
    }

    /// <summary>
    /// 离开流程。棋盘是持久内容，这里**不**销毁它；进程结束时由 Godot 统一回收。
    /// </summary>
    protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
    }

    /// <summary>
    /// 把棋盘挂载到框架根节点下。重复进入流程时复用同一实例。
    /// </summary>
    private static void MountBoard()
    {
        if (s_Board != null && GodotObject.IsInstanceValid(s_Board))
        {
            return;
        }

        var scene = GD.Load<PackedScene>(BoardScenePath);
        if (scene == null)
        {
            Log.Error("[ProcedureGame] 无法加载棋盘场景：{0}", BoardScenePath);
            return;
        }

        s_Board = scene.Instantiate();

        // 宿主 = 当前主场景根节点，即 Framework/GameFramework.tscn 的 GameEntry 节点。
        // 不用 GameEntry.GetComponent<T>()：那是**组件**查表（按类型名找
        // GameFrameworkComponent），拿不到根节点本身。
        var host = (Engine.GetMainLoop() as SceneTree)?.CurrentScene;
        if (host == null)
        {
            Log.Error("[ProcedureGame] 找不到框架根节点，棋盘无法挂载。");
            s_Board.QueueFree();
            s_Board = null;
            return;
        }

        host.AddChild(s_Board);
        Log.Info("[ProcedureGame] 棋盘已挂载。");
    }
}
