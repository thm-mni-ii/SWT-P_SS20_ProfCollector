using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class represents a manager for all objects in the scene
/// that other scripts want to interact with.
/// These are mainly GUI elements that can be updated.
/// </summary>
public class ObjectManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    private static ObjectManager instance;

    public static ObjectManager Instance => instance;

    /// <summary>
    /// Initializes singleton instance.
    /// </summary>
    /// <exception cref="Exception">If there is more than one instance</exception>
    private void Awake()
    {
        if (instance != null)
        {
            throw new Exception("Multiple ObjectMangers exist!");
        }
        instance = this;
    }

    /// <summary>
    /// Initializes GUI.
    /// </summary>
    private void Start()
    {
        DisableCardAGUI();
        DisableCardBGUI();
        this.playerACardsAmountText.text = RoundController.CARDS_PER_DECK.ToString();
        this.playerBCardsAmountText.text = RoundController.CARDS_PER_DECK.ToString();
        toggleHighlightCardA(false);
        toggleHighlightCardB(false);
        HideHint();
    }

    /// <summary>
    /// Top card of player A's deck
    /// </summary>
    [SerializeField] private CardContainer cardA;
    
    /// <summary>
    /// Top card of player B's deck
    /// </summary>
    [SerializeField] private CardContainer cardB;
    
    /// <summary>
    /// Top light in the scene
    /// </summary>
    [SerializeField] private LightController sceneLight;

    /// <summary>
    /// GUI for showing player A's card
    /// </summary>
    [SerializeField] private CardPopup playerACardGUI;
    
    /// <summary>
    /// GUI for player B's card
    /// </summary>
    [SerializeField] private CardPopup playerBCardGUI;
    
    /// <summary>
    /// GUI for player A card amount
    /// </summary>
    [SerializeField] private TextMesh playerACardsAmountText;
    
    /// <summary>
    /// GUI for player B card amount
    /// </summary>
    [SerializeField] private TextMesh playerBCardsAmountText;

    /// <summary>
    /// Hint text for giving instructions
    /// </summary>
    [SerializeField] private Text hintText;
    
    public CardPopup PlayerACardGui => playerACardGUI;

    public CardPopup PlayerBCardGui => playerBCardGUI;

    public LightController SceneLight => sceneLight;

    /// <summary>
    /// Loads the GUI for player A's card.
    /// </summary>
    /// <param name="cardId">Id of card to load</param>
    public void LoadCardAGUI(int cardId)
    {
        CardInfo cardInfo = CardLoader.Instance.GetCardById(cardId);
        playerACardGUI.gameObject.SetActive(true);
        playerACardGUI.GetComponent<CardPopup>().LoadCardPopup(cardInfo.Name,cardInfo.Val1,cardInfo.Val2,cardInfo.Val3);
    }
    
    /// <summary>
    /// Loads the GUI for player B's card.
    /// </summary>
    /// <param name="cardId">Id of card to load</param>
    public void LoadCardBGUI(int cardId)
    {
        CardInfo cardInfo = CardLoader.Instance.GetCardById(cardId);
        playerBCardGUI.gameObject.SetActive(true);
        playerBCardGUI.GetComponent<CardPopup>().LoadCardPopup(cardInfo.Name,cardInfo.Val1,cardInfo.Val2,cardInfo.Val3);
    }

    /// <summary>
    /// Disables the card GUI for card A.
    /// </summary>
    public void DisableCardAGUI()
    {
        playerACardGUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// Disables the card GUI for card B.
    /// </summary>
    public void DisableCardBGUI()
    {
        playerBCardGUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the card amount in a deck for a given player.
    /// </summary>
    /// <param name="playerRole">Player with new card amount</param>
    /// <param name="cardsAmount">New card amount</param>
    public void UpdateCardsAmount(PlayerRole playerRole, int cardsAmount)
    {
        if (playerRole == PlayerRole.PLAYER_A)
        {
            playerACardsAmountText.text = cardsAmount.ToString();
        }
        else
        { 
            playerBCardsAmountText.text = cardsAmount.ToString();
        }
    }

    /// <summary>
    /// Toggles the highlight of player A's card.
    /// </summary>
    /// <param name="turnOn">Should the highlight be turned on</param>
    public void toggleHighlightCardA(bool turnOn)
    {
        cardA.toggleHighlight(turnOn);
    }

    /// <summary>
    /// Toggles the highlight of player B's card.
    /// </summary>
    /// <param name="turnOn">Should the highlight be turned on</param>
    public void toggleHighlightCardB(bool turnOn)
    {
        cardB.toggleHighlight(turnOn);
    }

    /// <summary>
    /// Shows text in hint text.
    /// </summary>
    /// <param name="text">Text to show</param>
    public void ShowHint(string text)
    {
        hintText.enabled = true;
        hintText.text = text;
    }

    /// <summary>
    /// Clears the hint text.
    /// </summary>
    public void HideHint()
    {
        hintText.text = "";
        hintText.enabled = false;
    }
}
