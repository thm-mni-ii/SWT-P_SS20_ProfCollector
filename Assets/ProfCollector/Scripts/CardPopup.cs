using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class represents a card popup, that displays a card.
/// </summary>
public class CardPopup : MonoBehaviour
{
    /// <summary>
    /// GUI that displays the card's name
    /// </summary>
    [SerializeField] private Text cardName;
    
    /// <summary>
    /// GUI that display the cards first value
    /// </summary>
    [SerializeField] private Text cardVal1;
    
    /// <summary>
    /// GUI that display the cards second value
    /// </summary>
    [SerializeField] private Text cardVal2;
    
    /// <summary>
    /// GUI that display the cards third value
    /// </summary>
    [SerializeField] private Text cardVal3;

    /// <summary>
    /// Buttons for the card's properties
    /// </summary>
    [SerializeField] private Button[] cardProperties;

    /// <summary>
    /// Normal color of the buttons
    /// </summary>
    [SerializeField] private Color normalColor;
    
    /// <summary>
    /// Highlighted color of the buttons
    /// </summary>
    [SerializeField] private Color highlightedColor;

    /// <summary>
    /// Start is called before the first frame update.
    /// Adds listeners to the buttons.
    /// </summary>
    private void Start()
    {
        for(int i = 0; i < cardProperties.Length; i++)
        {
            cardProperties[i].interactable = true;
            int x = i + 1;
            cardProperties[i].onClick.AddListener(() =>
            {
                ClientScene.localPlayer.GetComponent<QuartettClient>().TryChooseProperty(x);
                disableButtons();
            });
        }
    }

    /// <summary>
    /// Loads card data into GUI.
    /// </summary>
    /// <param name="cardName">Name of the card</param>
    /// <param name="cardVal1">First property of the card</param>
    /// <param name="cardVal2">Second property of the card</param>
    /// <param name="cardVal3">Third property of the card</param>
    public void LoadCardPopup(string cardName, int cardVal1, int cardVal2, int cardVal3)
    {
        this.cardName.text = cardName;
        this.cardVal1.text = cardVal1.ToString();
        this.cardVal2.text = cardVal2.ToString();
        this.cardVal3.text = cardVal3.ToString();
        
        for (int i = 0; i < cardProperties.Length; i++)
        {
            cardProperties[i].interactable = true;
        }
    }

    /// <summary>
    /// Disables all buttons of the GUI.
    /// </summary>
    private void disableButtons()
    {
        foreach (Button button in cardProperties)
        {
            button.interactable = false;
        }
    }

    /// <summary>
    /// Toggles the highlight of a given property.
    /// </summary>
    /// <param name="turnOn">Should the property be highlighted</param>
    /// <param name="propertyIdx">Index of property</param>
    public void HighlightProperty(bool turnOn, int propertyIdx)
    {
        if (turnOn)
        {
            cardProperties[propertyIdx - 1].image.color = highlightedColor;
        }
        else
        {
            cardProperties[propertyIdx - 1].image.color = normalColor;
        }
    }

    /// <summary>
    /// Turns off all highlights of the properties.
    /// </summary>
    public void TurnOffAllPropertyHighlights()
    {
        for (int i = 0; i < cardProperties.Length; i++)
        {
            HighlightProperty(false,i + 1);
        }
    }

    /// <summary>
    /// Starts a blinking effect on all properties.
    /// </summary>
    public void AllPropertiesStartBlinking()
    {
        foreach (Button cardProperty in cardProperties)
        {
            cardProperty.GetComponent<BlinkingUILoop>().StartBlink();
        }
    }
    
    /// <summary>
    /// Stops the blinking effect on all properties.
    /// </summary>
    public void AllPropertiesStopBlinking()
    {
        foreach (Button cardProperty in cardProperties)
        {
            cardProperty.GetComponent<BlinkingUILoop>().StopBlinking();
        }
    }


    #region Getter

    public Text getCardName => cardName;
    
    public Text getCardVal1 => cardVal1;
    
    public Text getCardVal2 => cardVal2;

    public Text getCardVal3 => cardVal3;
    
    public Button[] getButtons => cardProperties;
    
    public Color getNormalColor => normalColor;
    
    public Color getHighlightedColor => highlightedColor;


    #endregion
    
    
    #region Setter
    public void setCardName(char[] s) => cardName.text = new String(s);
    /*
    public Text getCardVal1 => cardVal1;
    
    public Text getCardVal2 => cardVal2;

    public Text getCardVal3 => cardVal3;
    
    public Button[] getButtons => cardProperties;
    
    public Color getNormalColor => normalColor;
    
    public Color getHighlightedColor => highlightedColor;

*/
    #endregion

    
}
