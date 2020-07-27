using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Mirror;
using Component = System.ComponentModel.Component;
using Object = System.Object;

namespace Tests
{
    public class RoundControllerTests
    {
        private RoundController controller;
        
        private RoundController freshController => new GameObject().AddComponent<RoundController>();
        
                  
        PlayerRole roleA = PlayerRole.PLAYER_A;
        int[] cardsA = {1,2,3};
                 
        PlayerRole roleB = PlayerRole.PLAYER_B;
        int[] cardsB = {3,2,1};
        
        [Test]
        public void Round_Controller_Test_Start()
        {
            controller = freshController;
            Assert.AreEqual(RoundState.WAITING_FOR_CONNECTION, controller.getCurrentState());
            
            Assert.AreEqual(0, controller.getPlayersConnected());
            
            Assert.AreEqual(0, controller.getWaitTimer());
        }
        
        // A Test behaves as an ordinary method
        [Test]
        public void Round_Controller_Test_Deck_Initialisation()
        {
            controller = freshController;
            
            controller.ConnectToGame(roleA, cardsA);

            Assert.AreEqual(cardsA, controller.getPlayerACards());
            
            // We need to reset PlayersConnected, because otherwise we would trigger
            // the Setup method --> not part of this test.
            controller.setPlayersConnected(0);
            
            controller.ConnectToGame(roleB, cardsB);
            
            Assert.AreEqual(cardsB, controller.getPlayerBCards());
            
        }
        
        [Test]
        public void Round_Controller_Test_Draw_Card()
        {
            // GameObject.Instantiate("GameManager");

            controller = freshController;
           
            controller.ConnectToGame(roleA, cardsA);
            controller.setPlayersConnected(0);
            controller.ConnectToGame(roleB, cardsB);
           
            controller.DrawCard(roleA);
            controller.DrawCard(roleB);

            Assert.AreEqual(1, controller.getPlayerACardID());
            Assert.AreEqual(3, controller.getPlayerBCardID());
            Assert.AreEqual( new []{2,3}, controller.getPlayerACards());
            Assert.AreEqual(new []{2,1}, controller.getPlayerBCards());
           
        }
    }
}
