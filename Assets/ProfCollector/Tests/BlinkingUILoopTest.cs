using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
    public class BlinkingUILoopTest
    {
        BlinkingUILoop blinkingUiLoop = new GameObject().AddComponent<BlinkingUILoop>();
        // A Test behaves as an ordinary method
        [Test]
        public void BlinkingUILoopTestStart()
        {
            // Use the Assert class to test conditions
            
            
            blinkingUiLoop.setBlinkRate(0.25f);
            blinkingUiLoop.StartBlink();

            Assert.AreEqual(true, blinkingUiLoop.getFading());
            Assert.AreEqual(0.25f, blinkingUiLoop.getblinkTimer());
            Assert.AreEqual(true, blinkingUiLoop.getActiveStatus());

        }
        
        [Test]
        public void BlinkingUILoopTestStop()
        {
            // Use the Assert class to test conditions
            blinkingUiLoop.setUIImage(new GameObject().AddComponent<Image>());
            blinkingUiLoop.StopBlinking();

            Assert.AreEqual(false, blinkingUiLoop.getFading());
            Assert.AreEqual(0f, blinkingUiLoop.getblinkTimer());
            Assert.AreEqual(false, blinkingUiLoop.getActiveStatus());

        }
        
    }
}
