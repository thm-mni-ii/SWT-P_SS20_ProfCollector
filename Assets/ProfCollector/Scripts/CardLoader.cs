using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

/// <summary>
/// This class represents a container for all information about cards.
/// It loads this information from the database.
/// The information of a card can be loaded by using GetCardById().
/// It is implemented as a singleton.
/// </summary>
public class CardLoader : MonoBehaviour
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    private static CardLoader instance;
    
    /// <summary>
    /// Information of all cards that exist in the database.
    /// </summary>
    private List<CardInfo> cardInfos;
    
    public static CardLoader Instance => instance;

    public static void SetInstance(CardLoader c) => instance = c;

    /// <summary>
    /// Sets singleton instance.
    /// </summary>
    /// <exception cref="Exception">Throws exception if there are more than one CardLoader</exception>
    private void Awake()
    {
        if (instance != null)
        {
            throw new Exception("Multiple CardLoaders exist!");
        }
        instance = this;
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// Loads all cards from the database.
    /// </summary>
    void Start()
    {
        // Set this before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://swt-p-ss20-profcollector.firebaseio.com/");
        
        cardInfos = new List<CardInfo>();
        FirebaseDatabase.DefaultInstance.GetReference("cards").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Game data could not be retrieved!");
            }
            else if (task.IsCompleted)
            {
                int tmpCardId;
                string tmpName;
                int tmpRarityNumber;
                string tmpPicURL;
                int tmpVal1;
                int tmpVal2;
                int tmpVal3;

                foreach (DataSnapshot card in task.Result.Children)
                {
                    tmpCardId = int.Parse(card.Key);
                    tmpName = card.Child("name").Value.ToString();
                    tmpRarityNumber = int.Parse(card.Child("rarity").Value.ToString());
                    tmpPicURL = card.Child("pic").Value.ToString();
                    tmpVal1 = int.Parse(card.Child("val1").Value.ToString());
                    tmpVal2 = int.Parse(card.Child("val2").Value.ToString());
                    tmpVal3 = int.Parse(card.Child("val3").Value.ToString());

                
                    cardInfos.Add(new CardInfo(tmpCardId,tmpName,tmpRarityNumber,tmpPicURL,tmpVal1,tmpVal2,tmpVal3));
                }
            }
        });
    }

    /// <summary>
    /// Allows access to card information over id.
    /// </summary>
    /// <param name="cardId">Id of card</param>
    /// <returns>Card with given id</returns>
    /// <exception cref="Exception">Card with given id does not exist</exception>
    public CardInfo GetCardById(int cardId)
    {
        CardInfo result = cardInfos.Find(ci => ci.CardId == cardId);
        if (result == null)
        {
            throw new Exception("Card does not exist");
        }
        
        return result;
    }

    public void loadCards()
    {
        // Set this before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://swt-p-ss20-profcollector.firebaseio.com/");
        
        cardInfos = new List<CardInfo>();
        FirebaseDatabase.DefaultInstance.GetReference("cards").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Game data could not be retrieved!");
            }
            else if (task.IsCompleted)
            {
                int tmpCardId;
                string tmpName;
                int tmpRarityNumber;
                string tmpPicURL;
                int tmpVal1;
                int tmpVal2;
                int tmpVal3;

                foreach (DataSnapshot card in task.Result.Children)
                {
                    tmpCardId = int.Parse(card.Key);
                    tmpName = card.Child("name").Value.ToString();
                    tmpRarityNumber = int.Parse(card.Child("rarity").Value.ToString());
                    tmpPicURL = card.Child("pic").Value.ToString();
                    tmpVal1 = int.Parse(card.Child("val1").Value.ToString());
                    tmpVal2 = int.Parse(card.Child("val2").Value.ToString());
                    tmpVal3 = int.Parse(card.Child("val3").Value.ToString());

                
                    cardInfos.Add(new CardInfo(tmpCardId,tmpName,tmpRarityNumber,tmpPicURL,tmpVal1,tmpVal2,tmpVal3));
                    print("loaded card :" + tmpCardId);
                }
            }
        });
    }
}
