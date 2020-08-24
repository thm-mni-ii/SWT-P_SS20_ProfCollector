using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CardLoaderTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void CardLoaderTestSimplePasses()
        {
            CardLoader cardLoader = new GameObject().AddComponent<CardLoader>();
            cardLoader.loadCards();
            
            foreach (var cardInfo in cardLoader.CardInfos)
            {
                Debug.Log(cardInfo.CardId);
            }
            //CardInfo getCard = cardLoader.GetCardById(1);
            
            //Assert.AreEqual(1, getCard.CardId);
            // Use the Assert class to test conditions
        }
    }
}
