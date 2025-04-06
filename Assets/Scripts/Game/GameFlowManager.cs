using System;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : IGameFlow
{
    private static GameFlowManager instance;
    public static GameFlowManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameFlowManager();
            return instance;
        }
    }

    private Dictionary<GameState, IGamePhase> phases;
    private GameState currentState = GameState.None;
    private IGamePhase currentPhase;

    public bool IsGameOver { get; private set; }
    public int CurrentRound { get; private set; }
    public int CurrentStage { get; private set; }
    private const int MAX_ROUNDS = 5;
    private const int MAX_STAGE = 4;

    public int CardNumbers = 10;

    private GameFlowManager()
    {
        InitializePhases();
    }

    private void InitializePhases()
    {
        phases = new Dictionary<GameState, IGamePhase>
        {
            { GameState.Story, new StoryPhase() },
            { GameState.CardSelection, new CardSelectionPhase() },
            { GameState.Game, new GamePhase() },
            { GameState.Result, new ResultPhase() }
        };
    }

    public void StartGame()
    {
        //CurrentStage = 1;
        CurrentRound = 1;
        IsGameOver = false;
        TransitionTo(GameState.Story);
    }

    public void EndGame()
    {
        IsGameOver = true;
        currentPhase?.Exit();
        currentState = GameState.End;
    }

    public void TransitionTo(GameState newState)
    {
        if (currentState == newState) return;

        currentPhase?.Exit();
        currentState = newState;

        if (phases.TryGetValue(newState, out IGamePhase phase))
        {
            currentPhase = phase;
            currentPhase.Enter();
        }

        // 检查回合结束
        if (newState == GameState.Result)
        {
            if (CurrentRound >= MAX_ROUNDS)
            {
                EndGame();
                //CurrentStage++;
            }
            else
            {
                CurrentRound++;
            }
        }
    }

    public void Update()
    {
        if (!IsGameOver && currentPhase != null)
        {
            currentPhase.Update();
        }
    }
}