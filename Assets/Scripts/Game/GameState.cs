using System;

public enum GameState
{
    None,
    Story,           // 剧情界面
    CardSelection,   // 抽牌选择
    Game,            // 游戏主阶段（处理+展示）
    Result,          // 结果判定
    End             // 游戏结束
}

public interface IGamePhase
{
    void Enter();
    void Update();
    void Exit();
}

public interface IGameFlow
{
    void StartGame();
    void EndGame();
    void TransitionTo(GameState newState);
    bool IsGameOver { get; }
    int CurrentRound { get; }
}