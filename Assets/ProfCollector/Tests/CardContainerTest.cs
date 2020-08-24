using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CardContainerTest
    {
        
        CardContainer cardContainer;
            
        // A Test behaves as an ordinary method
        [Test]
        public void Card_Container_Test_Highlight()
        {
            // Use the Assert class to test conditions
            cardContainer = new GameObject().AddComponent<CardContainer>();
            cardContainer.setHighlight(new GameObject().AddComponent<SpriteRenderer>());
            cardContainer.toggleHighlight(true);
            
            Assert.AreEqual(true, cardContainer.getHighlightEnabled());
            
            cardContainer.toggleHighlight(false);
            
            Assert.AreEqual(false, cardContainer.getHighlightEnabled());
            
        }
    }
}
