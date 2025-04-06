using UnityEngine;
using GameConfig;

public abstract class BaseGamePhase : IGamePhase
{
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

public class StoryPhase : BaseGamePhase
{
    public override void Enter()
    {
        int id = VController.Ins.ShowST("StoryPanel");
        TM.SetTimer(this.Hash("StoryDelay"), 3, null, (s) =>
        {
            GameFlowManager.Instance.TransitionTo(GameState.CardSelection);
            VController.Ins.HideST(id);
        });
    }
}

public class CardSelectionPhase : BaseGamePhase
{
    public override void Enter()
    {
        CardSelectModel.Ins.card1.D = CardProbability();
        CardSelectModel.Ins.card2.D = CardProbability();
        CardSelectModel.Ins.card3.D = CardProbability();
        Debug.Log(CardSelectModel.Ins.card1);

        int id = VController.Ins.ShowST("SelectCardPanel");
        VPanel panel = VController.Ins.GetSTUI(id);
        panel.UI.WaitMessage("SelectedCard", (success) =>
        {
            if (success)
            {
                CardInventory.Ins.cards.Add(11);
                GameFlowManager.Instance.TransitionTo(GameState.Game);
                VController.Ins.HideST(id);
            }
        });
    }

    private int CardProbability()
    {
        int probability = Random.Range(10001, 10004);


        return probability;
    }

}

public class GamePhase : BaseGamePhase
{
    private bool isProcessingComplete = false;
    private Table table;

    public override void Enter()
    {
        Debug.Log("进入游戏主阶段");
        isProcessingComplete = false;
        table = GameManager.Ins.table;

        // 重置并开始牌圈移动
        table.ResetCom();
        table.StartCardCircleMovement();

        // 开始处理选中卡牌的效果
        ProcessCardEffect();
    }

    public override void Update()
    {
        if (!isProcessingComplete)
        {
            // 处理卡牌效果
            if (IsProcessingComplete())
            {
                isProcessingComplete = true;
                ShowNumberEffect();
            }
        }
        else
        {
            // 展示数字效果
            if (IsDisplayComplete())
            {
                // 停止牌圈移动
                table.StopCardCircleMovement();
                GameFlowManager.Instance.TransitionTo(GameState.Result);
            }
        }
    }

    public override void Exit()
    {
        // 确保退出时停止牌圈移动
        if (table != null)
        {
            table.StopCardCircleMovement();
        }
    }

    private float CardProbability()
    {
        float probability = 0.0f;


        return probability;
    }

    private void ProcessCardEffect()
    {
        // 实现卡牌效果处理逻辑
        Debug.Log("处理卡牌效果");
    }

    private void ShowNumberEffect()
    {
        // 实现数字效果展示逻辑
        Debug.Log("展示数字效果");
    }

    private bool IsProcessingComplete()
    {
        // 实现处理完成检测逻辑
        return false;
    }

    private bool IsDisplayComplete()
    {
        // 实现展示完成检测逻辑
        return false;
    }
}

public class ResultPhase : BaseGamePhase
{
    public override void Enter()
    {
        Debug.Log("进入结果判定阶段");
        // 判定是否达成目标
    }

    public override void Update()
    {
        // 判定完成后，如果游戏未结束则进入下一回合的剧情阶段
        if (IsResultComplete() && !GameFlowManager.Instance.IsGameOver)
        {
            GameFlowManager.Instance.TransitionTo(GameState.Story);
        }
    }

    private bool IsResultComplete()
    {
        // 实现结果判定完成检测逻辑
        return false;
    }
}