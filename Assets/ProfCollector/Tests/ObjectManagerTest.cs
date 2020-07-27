using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ObjectManagerTest
    {
        private ObjectManager manager = new GameObject().AddComponent<ObjectManager>();
        private ObjectManager freshManager => new GameObject().AddComponent<ObjectManager>();
        
        // A Test behaves as an ordinary method
        [Test]
        public void Object_Manager_Test_Load_Card_A_GUI()
        {
            // Use the Assert class to test conditions

            int cardID = 1;

            manager = freshManager;
            CardLoader loader = new GameObject().AddComponent<CardLoader>();
            loader.loadCards();
            CardLoader.SetInstance(loader);
            manager.LoadCardAGUI(cardID);
            
            Assert.AreEqual(true, manager.PlayerACardGui.enabled);

        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ObjectManagerTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
