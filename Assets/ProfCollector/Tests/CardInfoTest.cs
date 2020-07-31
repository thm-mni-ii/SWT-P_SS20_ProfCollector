using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CardInfoTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void CardInfoTestConstruction()
        {
            // Use the Assert class to test conditions
            CardInfo cardInfo = new CardInfo(1, "TestCard", 1, "TestUrl", 1,1,1);
            
            Assert.AreEqual(1, cardInfo.CardId);
            
            Assert.AreEqual("TestCard", cardInfo.Name);
            
            Assert.AreEqual(CardInfo.Rarity.UNCOMMON, cardInfo.CardRarity);
            
            Assert.AreEqual("TestUrl", cardInfo.PicUrl);
            
            Assert.AreEqual(1, cardInfo.Val1);
            
            Assert.AreEqual(1, cardInfo.Val2);
            
            Assert.AreEqual(1, cardInfo.Val3);

        }

        [Test]
        public void CardInfoTestValForIdx()
        {
            CardInfo cardInfo = new CardInfo(1, "TestCard", 1, "TestUrl", 1,2,3);
        
            Assert.AreEqual(1, cardInfo.valForIdx(1));
            
            Assert.AreEqual(2, cardInfo.valForIdx(2));
            
            Assert.AreEqual(3, cardInfo.valForIdx(3));


            Assert.Throws<Exception>(() => Assert.AreEqual(4, cardInfo.valForIdx(4)));
            
        }
    }
}
