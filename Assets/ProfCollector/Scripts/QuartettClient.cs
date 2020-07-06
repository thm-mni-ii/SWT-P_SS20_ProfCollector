using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Firebase.Database;
using UnityEngine;
using Mirror;
using Object = System.Object;
using Random = System.Random;

/// <summary>
/// Different roles that a player can have.
/// </summary>
public enum PlayerRole{PLAYER_A, PLAYER_B}

/// <summary>
/// This class represents a client in the quartett game.
/// It detects input and sends it to the server for processing.
/// </summary>
public class QuartettClient : NetworkBehaviour
{
    /// <summary>
    /// Role of the player
    /// </summary>
    private PlayerRole playerRole;

    /// <summary>
    /// Information send by the framework.
    /// </summary>
    private PlayerInfo playerInfo;

    /// <summary>
    /// Current state of the client.
    /// </summary>
    private RoundState clientState;
    
    private List<int[]> tmpCardVals;
    private bool cardsCanBeLoaded;

    /// <summary>
    /// Start is called before the first frame update.
    /// Sets player role and loads inventory data.
    /// </summary>
    private void Start()
    {
        tmpCardVals = new List<int[]>();
        playerInfo = GameServer.Instance.LocalPlayerInfo;
        if (isServer)
        {
            playerRole = PlayerRole.PLAYER_A;
        }
        else
        {
            playerRole = PlayerRole.PLAYER_B;
        }

        clientState = RoundState.WAITING_FOR_CONNECTION;
        
        LoadInventoryFromDatabase();
    }
    
    /// <summary>
    /// Connects the player to the game.
    /// </summary>
    /// <param name="playerRole">Role of the player</param>
    /// <param name="cardIds">Cards that the player will use</param>
    [Command]
    private void CmdConnectToGame(PlayerRole playerRole, int[] cardIds)
    {
        RoundController.Instance.ConnectToGame(playerRole,cardIds);
    }
    /// <summary>
    /// Loads inventory data from database.
    /// </summary>
    private void LoadInventoryFromDatabase()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users/" + playerInfo.name).GetValueAsync()
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Inventory data could not be retrieved!");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot tmpInventory = task.Result.Child("inventory");

                    foreach (DataSnapshot cardTuple in tmpInventory.Children)
                    {
                        int cardId = (int.Parse(cardTuple.Key));
                        int cardAmount = int.Parse(cardTuple.Value.ToString());
                        tmpCardVals.Add(new []{cardId,cardAmount});
                    }
                    cardsCanBeLoaded = true;
                }
            });
    }

    /// <summary>
    /// Workaround: Cards somehow can't be loaded into inventory during database request (causes weird behaviour).
    /// The request instead loads the data into an temporary array and then sets a flag.
    ///
    /// Loads inventory data and connects player to game.
    /// Shuffles the cards and takes as many as allowed.
    /// </summary>
    private void LoadCardData()
    {
        List<int> tmpCardIds = new List<int>();
        foreach (int[] cardVal in tmpCardVals)
        {
            for (int i = 0; i < cardVal[1]; i++)
            {
                tmpCardIds.Add(cardVal[0]);
            }
        }
        
        tmpCardIds.Shuffle();
        tmpCardIds = tmpCardIds.GetRange(0,RoundController.CARDS_PER_DECK);

        CmdConnectToGame(playerRole,tmpCardIds.ToArray());
        cardsCanBeLoaded = false;
        
    }

    /// <summary>
    /// Updates the GUI of the cards after a player drew a card.
    /// </summary>
    /// <param name="playerRole">Player that drew a card</param>
    /// <param name="cardId">Card that was drawn</param>
    private void UpdatedCardGUI(PlayerRole playerRole, int cardId)
    {
        if (playerRole == PlayerRole.PLAYER_A)
        {
            ObjectManager.Instance.toggleHighlightCardA(false);
            ObjectManager.Instance.LoadCardAGUI(cardId);     
        }
        else
        {
            ObjectManager.Instance.toggleHighlightCardB(false);
            ObjectManager.Instance.LoadCardBGUI(cardId);  
        }
    }

    /// <summary>
    /// Sends an rpc to the client after a player drew a card.
    /// </summary>
    /// <param name="playerRole">Player that drew a card</param>
    /// <param name="cardId">Card that was drawn</param>
    [ClientRpc]
    public void RpcUpdateCardGUI(PlayerRole playerRole, int cardId)
    {
        ClientScene.localPlayer.GetComponent<QuartettClient>().UpdatedCardGUI(playerRole,cardId);
    }

    /// <summary>
    /// Updates the amount of cards a player has left.
    /// </summary>
    /// <param name="playerRole">Player that has a new card amount</param>
    /// <param name="cardsAmount">New card amount</param>
    [ClientRpc]
    public void RpcUpdateCardsAmount(PlayerRole playerRole, int cardsAmount)
    {
        ObjectManager.Instance.UpdateCardsAmount(playerRole,cardsAmount);
    }

    /// <summary>
    /// Sends the server a message to pick up the top card of the deck.
    /// </summary>
    [Command]
    public void CmdPickUpCard()
    {
        RoundController.Instance.PickUpCard();
    }
    
    /// <summary>
    /// Update is called once per frame.
    /// Processes input.
    /// </summary>
    void Update()
    {
        if (!hasAuthority) return;
        if (isLocalPlayer)
        {
            if (cardsCanBeLoaded)
            {
                LoadCardData();
            }
            
            if (Input.GetMouseButtonDown(0))
            {

                RaycastHit raycast;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out raycast, 200.0f))
                {

                    if (raycast.transform)
                    {
                        if (IsLocalPlayersCard(raycast.collider.tag))
                        {
                            if (IsLegalMove())
                            {
                                CmdPickUpCard();
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks if the card that the player clicked is his own.
    /// </summary>
    /// <param name="cardTag">Tag of the card that was clicked</param>
    /// <returns>Is this the card of the local player</returns>
    private bool IsLocalPlayersCard(string cardTag)
    {
        return playerRole.Equals(PlayerRole.PLAYER_A) && cardTag.Equals("CardA")
               || playerRole.Equals(PlayerRole.PLAYER_B) && cardTag.Equals("CardB");
    }
    
    /// <summary>
    /// Checks if the move that the player is trying to make is allowed. 
    /// </summary>
    /// <returns>Is this move legal</returns>
    private bool IsLegalMove()
    {
        return (playerRole.Equals(PlayerRole.PLAYER_A) && (clientState.Equals(RoundState.PLAYER_A_FIRST_TURN) 
                                                           || clientState.Equals(RoundState.PLAYER_A_SECOND_TURN))
                || (playerRole.Equals(PlayerRole.PLAYER_B) && (clientState.Equals(RoundState.PLAYER_B_FIRST_TURN)
                                                               || clientState.Equals(RoundState.PLAYER_B_SECOND_TURN))));
    }

    /// <summary>
    /// Chooses a property to compare.
    /// </summary>
    /// <param name="valIdx">Property to compare</param>
    public void ChooseProperty(int valIdx)
    {
        if (IsLegalMove())
        {
            CmdProcessMove(valIdx);
        }
    }

    /// <summary>
    /// Sends the move that was made by the client to the server for processing.
    /// </summary>
    /// <param name="valIdx">Idx of the value that was choosen</param>
    [Command]
    private void CmdProcessMove(int valIdx)
    {
        RoundController.Instance.ProcessMove(valIdx);
    }

    /// <summary>
    /// Sends an update about the current state of the game to all clients.
    /// </summary>
    /// <param name="updatedState">Updated state of the game</param>
    /// <param name="winStatus">Winning status</param>
    /// <param name="playerACardId">Id of player A's card</param>
    /// <param name="playerBCardId">Id of player B's card</param>
    /// <param name="valIdx">Idx of value to compare</param>
    [ClientRpc]
    public void RpcUpdateGame(RoundState updatedState, int winStatus, int playerACardId, int playerBCardId, int valIdx)
    {
        ClientScene.localPlayer.GetComponent<QuartettClient>()
            .UpdateGame(updatedState, winStatus, playerACardId, playerBCardId,valIdx);
    }

    /// <summary>
    /// Updates the waiting timer on all clients.
    /// </summary>
    /// <param name="waitTimer">Wait timer</param>
    [ClientRpc]
    public void RpcUpdateWaitTimer(string waitTimer)
    {
        ObjectManager.Instance.UpdateWaitTimer(waitTimer);
    }

    /// <summary>
    /// Updates the GUI for the current state of the game.
    /// </summary>
    /// <param name="updatedState">Updated state of the game</param>
    /// <param name="winStatus">Winning status</param>
    /// <param name="playerACardId">Id of player A's card</param>
    /// <param name="playerBCardId">Id of player B's card</param>
    /// <param name="valIdx">Idx of value to compare</param>
    private void UpdateGame(RoundState updatedState, int winStatus, int playerACardId, int playerBCardId, int valIdx)
    {
        clientState = updatedState;
        switch (clientState)
        {
            case RoundState.PLAYER_A_FIRST_TURN:
                ObjectManager.Instance.PlayerACardGui.TurnOffAllPropertyHighlights();
                ObjectManager.Instance.PlayerBCardGui.TurnOffAllPropertyHighlights();
                ObjectManager.Instance.toggleHighlightCardA(true);
                ObjectManager.Instance.DisableCardAGUI();
                ObjectManager.Instance.DisableCardBGUI();
                break;
            case RoundState.PLAYER_B_SECOND_TURN:
                ObjectManager.Instance.toggleHighlightCardB(true);
                ObjectManager.Instance.DisableCardBGUI();
                ObjectManager.Instance.LoadCardAGUI(playerACardId);
                ObjectManager.Instance.PlayerACardGui.HighlightProperty(true,valIdx);
                break;
            case RoundState.PLAYER_B_FIRST_TURN:
                ObjectManager.Instance.PlayerACardGui.TurnOffAllPropertyHighlights();
                ObjectManager.Instance.PlayerBCardGui.TurnOffAllPropertyHighlights();
                ObjectManager.Instance.toggleHighlightCardB(true);
                ObjectManager.Instance.DisableCardAGUI();
                ObjectManager.Instance.DisableCardBGUI();
                break;
            case RoundState.PLAYER_A_SECOND_TURN:
                ObjectManager.Instance.toggleHighlightCardA(true);
                ObjectManager.Instance.DisableCardAGUI();
                ObjectManager.Instance.LoadCardBGUI(playerBCardId);
                ObjectManager.Instance.PlayerBCardGui.HighlightProperty(true,valIdx);
                break;
            case RoundState.PLAYER_A_WON:
                break;
            case RoundState.PLAYER_B_WON:
                break;
            case RoundState.WAIT_FOR_NEXT_ROUND:
                ObjectManager.Instance.LoadCardAGUI(playerACardId);
                ObjectManager.Instance.LoadCardBGUI(playerBCardId);
                ObjectManager.Instance.PlayerACardGui.HighlightProperty(true,valIdx);
                ObjectManager.Instance.PlayerBCardGui.HighlightProperty(true,valIdx);
                break;
        }
    }

}
