using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerInfoTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void PlayerInfoTestConstruction()
        {
            // Use the Assert class to test conditions
            PlayerInfo playerInfoHost = new PlayerInfo("Host", true);

            Assert.AreEqual(true, playerInfoHost.isHost);
            Assert.AreEqual("Host", playerInfoHost.name);


            PlayerInfo playerInfoClient = new PlayerInfo("Client", false);

            Assert.AreEqual(false, playerInfoClient.isHost);
            Assert.AreEqual("Client", playerInfoClient.name);

        }
    }
}
