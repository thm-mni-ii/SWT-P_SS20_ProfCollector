using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mirror;
using Mirror.Examples.Basic;
using UnityEngine;

/// <summary>
/// Different states that the round controller can be in.
/// </summary>
public enum RoundState {
    WAITING_FOR_CONNECTION,
    PLAYER_A_FIRST_TURN,
    PLAYER_B_SECOND_TURN,
    PLAYER_B_FIRST_TURN,
    PLAYER_A_SECOND_TURN,
    WAIT_FOR_NEXT_ROUND,
    PLAYER_A_WON,
    PLAYER_B_WON}

/// <summary>
/// This class represents a controller that manages the round system off the game.
/// This controller exists on all clients, but is only used on the server (TODO : This may be improved).
/// It processes client input and communicates the new state of the game to all clients.
/// </summary>
public class RoundController : NetworkBehaviour
{
    #region variables & setup
    /// <summary>
    /// Local player on the server
    /// </summary>
    public QuartettClient host;
    
    /// <summary>
    /// Current state that the controller is in
    /// </summary>
    private RoundState currentState;
    
    /// <summary>
    /// Stores which player has won
    ///   -1 => No one won yet
    ///    0 => It's a draw
    ///    1 => Player A won
    ///    2 => Player B won
    /// </summary>
    private int winStatus;

    /// <summary>
    /// Index of the value that will be compared
    /// </summary>
    private int valIdx;

    /// <summary>
    /// Ids of the cards that player A has for this game
    /// </summary>
    private List<int> playerACardIds;
    
    /// <summary>
    /// Ids of the cards that player B has for this game
    /// </summary>
    private List<int> playerBCardIds;
    
    /// <summary>
    /// Id of the current card of player A
    /// </summary>
    private int playerACardId;
    
    /// <summary>
    /// Id of the current card of player B
    /// </summary>
    private int playerBCardId;

    /// <summary>
    /// Amount of players connected
    /// </summary>
    private int playersConnected;

    /// <summary>
    /// Singleton instance
    /// </summary>
    private static RoundController instance;

    /// <summary>
    /// Contains the state that the controller will go to
    /// after it finished waiting
    /// </summary>
    private RoundState stateAfterWait;

    /// <summary>
    /// Time to wait between rounds
    /// </summary>
    private float waitDuration = 5.0f;

    /// <summary>
    /// Current time to wait
    /// </summary>
    private float waitTimer;

    /// <summary>
    /// Cards in a deck
    /// </summary>
    public const int CARDS_PER_DECK = 2;
    
    public static RoundController Instance => instance;
    
    
    /// <summary>
    /// Initializes singleton instance.
    /// </summary>
    /// <exception cref="Exception">If there is more than one instance</exception>
    private void Awake()
    {
        if (instance != null)
        {
            throw new Exception("Multiple RoundSystems exist!");
        }
        instance = this;
    }
    
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        currentState = RoundState.WAITING_FOR_CONNECTION;
        playersConnected = 0;
        waitTimer = 0f;
    }
    
    #endregion

    #region pre_start
    /// <summary>
    /// Connects the player to the game and loads it's cards.
    /// </summary>
    /// <param name="playerRole">Role of the player</param>
    /// <param name="cardIds">Cards that the player will have for this game</param>
    public void ConnectToGame(PlayerRole playerRole,int[] cardIds)
    {
        if (playerRole == PlayerRole.PLAYER_A)
        {
            playerACardIds = cardIds.ToList(); }
        else
        {
            playerBCardIds = cardIds.ToList();
        }
        
        playersConnected++;
        if (playersConnected == 2)
        {
            SetupGame();
        }
    }
    
    /// <summary>
    /// Sets up all values for the game
    /// and updates all clients.
    /// </summary>
    private void SetupGame()
    {
        winStatus = -1;
        playerACardId = -1;
        playerBCardId = -1;
        currentState = RoundState.PLAYER_A_FIRST_TURN;
        SentGameUpdate();
    }
    
    #endregion
    
    #region after_start

    /// <summary>
    /// Update is called once per frame.
    /// Updates the wait timer, if the controller is waiting.
    /// </summary>
    private void Update()
    {
        if (ClientScene.localPlayer && !ClientScene.localPlayer.isServer) return;

        if (currentState.Equals(RoundState.WAIT_FOR_NEXT_ROUND))
        {
            if (waitTimer > 0f)
            {
                host.RpcUpdateWaitTimer("Next Round: " + (int)waitTimer);
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    currentState = stateAfterWait;
                    winStatus = -1;
                    valIdx = -1;
                    playerACardId = -1;
                    playerBCardId = -1;
                    host.RpcUpdateWaitTimer("");
                    SentGameUpdate();
                }
            }
        }
    }

    /// <summary>
    /// Draws a card for the given player.
    /// </summary>
    /// <param name="playerRole">Player to draw a card for</param>
    public void DrawCard(PlayerRole playerRole)
    {
        if (playerRole == PlayerRole.PLAYER_A)
        {
            playerACardId = playerACardIds[0];
            playerACardIds.RemoveAt(0);
        }
        else
        {
            playerBCardId = playerBCardIds[0];
            playerBCardIds.RemoveAt(0);
        }
    }
    
    /// <summary>
    /// Initializes the waiting state of the game.
    /// </summary>
    /// <param name="stateAfterWait">State to go to after done waiting</param>
    /// <returns>Waiting for next round</returns>
    private RoundState InitializeWait(RoundState stateAfterWait)
    {
        this.stateAfterWait = stateAfterWait;
        waitTimer = waitDuration;
        return RoundState.WAIT_FOR_NEXT_ROUND;
    }

    /// <summary>
    /// Processes the input of a player.
    /// </summary>
    /// <param name="valIdx">Value that will be compared</param>
    public void ProcessMove(int valIdx)
    {
        currentState = CalcNextState(valIdx);
        SentGameUpdate();
    }

    /// <summary>
    /// Updates the clients about the state of the game.
    /// </summary>
    private void SentGameUpdate()
    {
        host.RpcUpdateGame(
            currentState,
            winStatus,
            playerACardId,
            playerBCardId,
            valIdx);
    }

    /// <summary>
    /// Calculates the state of the game after a comparison was made.
    /// </summary>
    /// <param name="nextState"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private RoundState CalcAfterComparisonState(RoundState nextState)
    {
        RoundState afterComparisonState;
        
        CompareValues();
        
        switch (winStatus)
        {
            case 1: // PLAYER A WON
                playerACardIds.Add(playerACardId);
                playerACardIds.Add(playerBCardId);
                afterComparisonState =  playerBCardIds.Count > 0 ? InitializeWait(nextState) : RoundState.PLAYER_A_WON;
                break;
            case 2: // PLAYER B WON
                playerBCardIds.Add(playerACardId);
                playerBCardIds.Add(playerBCardId);
                afterComparisonState =  playerACardIds.Count > 0 ? InitializeWait(nextState) : RoundState.PLAYER_B_WON;
                break;
            case 0: // DRAW
                playerBCardIds.Add(playerBCardId);
                playerACardIds.Add(playerACardId);
                afterComparisonState =  InitializeWait(nextState);
                break;
            default:
                throw new Exception("INVALID RESULT VALUE");
        }
        
        host.RpcUpdateCardsAmount(PlayerRole.PLAYER_A,playerACardIds.Count);
        host.RpcUpdateCardsAmount(PlayerRole.PLAYER_B,playerBCardIds.Count);

        return afterComparisonState;
    }
    
    /// <summary>
    /// Picks up a card for the player, after he clicked his card.
    /// Changes the state of the game, if it's not the turn of the player that picked up the card,
    /// since the value to compare is then already known.
    /// </summary>
    public void PickUpCard()
    {
        switch (currentState)
        {
            case RoundState.PLAYER_A_FIRST_TURN:
                DrawCard(PlayerRole.PLAYER_A);
                host.RpcUpdateCardGUI(PlayerRole.PLAYER_A,playerACardId);
                host.RpcUpdateCardsAmount(PlayerRole.PLAYER_A,playerACardIds.Count);
                break;
            case RoundState.PLAYER_A_SECOND_TURN:
                DrawCard(PlayerRole.PLAYER_A);
                host.RpcUpdateCardGUI(PlayerRole.PLAYER_A,playerACardId);
                host.RpcUpdateCardsAmount(PlayerRole.PLAYER_A,playerACardIds.Count);
                ProcessMove(valIdx);
                break;
            case RoundState.PLAYER_B_FIRST_TURN:
                DrawCard(PlayerRole.PLAYER_B);
                host.RpcUpdateCardGUI(PlayerRole.PLAYER_B,playerBCardId);
                host.RpcUpdateCardsAmount(PlayerRole.PLAYER_B,playerBCardIds.Count);
                break;
            case RoundState.PLAYER_B_SECOND_TURN:
                DrawCard(PlayerRole.PLAYER_B);
                host.RpcUpdateCardGUI(PlayerRole.PLAYER_B,playerBCardId);
                host.RpcUpdateCardsAmount(PlayerRole.PLAYER_B,playerBCardIds.Count);
                ProcessMove(valIdx);
                break;
        }
    }
   
    /// <summary>
    /// Calculates the next turn to go in for a given state and a given valIdx.
    /// </summary>
    /// <param name="valIdx">Idx of value to compare</param>
    /// <returns>New state</returns>
    /// <exception cref="Exception">CurrentState is in a state it should not be in</exception>
    private RoundState CalcNextState(int valIdx)
    {
        this.valIdx = valIdx;
        switch (currentState)
        {
            case RoundState.PLAYER_A_FIRST_TURN:
                playerBCardId = -1;
                winStatus = -1;
                return RoundState.PLAYER_B_SECOND_TURN;
            case RoundState.PLAYER_B_FIRST_TURN:
                playerACardId = -1;
                winStatus = -1;
                return RoundState.PLAYER_A_SECOND_TURN;
            case RoundState.PLAYER_A_SECOND_TURN:
                return CalcAfterComparisonState(RoundState.PLAYER_A_FIRST_TURN);
            case RoundState.PLAYER_B_SECOND_TURN:
                return CalcAfterComparisonState(RoundState.PLAYER_B_FIRST_TURN);
            default:
                throw new Exception("ILLEGAL STATE: " + currentState);
        }
    }

    /// <summary>
    /// Compares the value of player A and player B
    /// and updates the value of winStatus.
    /// </summary>
    private void CompareValues()
    {
        int playerAValue = CardLoader.Instance.GetCardById(playerACardId).valForIdx(valIdx);
        int playerBValue = CardLoader.Instance.GetCardById(playerBCardId).valForIdx(valIdx);
        
        if (playerAValue > playerBValue)
        {
            winStatus = 1;
        }
        else if (playerBValue > playerAValue)
        {
            winStatus = 2;
        }
        else
        {
            winStatus = 0;
        }
    }
    
    #endregion

    #region Getter

    public int[] getPlayerACards()
    {
        return playerACardIds.ToArray();
    }
    
    public int[] getPlayerBCards()
    {
        return playerBCardIds.ToArray();
    }

    public int getPlayerACardID()
    {
        return playerACardId;
    }

    public int getPlayerBCardID()
    {
        return playerBCardId;
    }
    
    public RoundState getCurrentState()
    {
        return currentState;
    }

    public int getPlayersConnected()
    {
        return playersConnected;
    }

    public float getWaitTimer()
    {
        return waitTimer;
    }
    
    #endregion

    #region Setter

    public void setPlayersConnected(int setter)
    {
        playersConnected = setter;
    }


    #endregion
}
