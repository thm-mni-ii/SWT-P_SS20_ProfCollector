using System.Collections;
using System.Collections.Generic;
using Mirror;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = System.Object;

namespace Tests
{
    public class ObjectManagerTest
    {
        private GameServer server;
        [SetUp]
        public void initializeTestScene()
        {
            server = new GameObject().AddComponent<GameServer>();
            server.playerPrefab = PrefabUtility.LoadPrefabContents(
                "Assets/Mirror/Examples/Room/Prefabs/GamePlayer.prefab");
            Debug.Log(server.playerPrefab);
            
            //GameObject.Instantiate(server);
        }
        
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestAwake()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.

            //GameServer server = GameObject.Instantiate(new GameObject().AddComponent<GameServer>());
            
            GameObject player = GameObject.Instantiate(server.playerPrefab);
            
            
            
            RoundController roundController = new GameObject().AddComponent<RoundController>();
            
            RoundController.setController(roundController);
            
            Debug.Log(RoundController.Instance);
            
            QuartettClient host = player.GetComponent<QuartettClient>();

            host.setPlayerRole(PlayerRole.PLAYER_B);
            List<int[]> cards = new List<int[]>();
            int[] array = {1, 2, 3};
            cards.Add(array);

            host.setCards(cards);
            roundController.ConnectToGame(host.getPlayerRole(), array);

            Assert.AreEqual(ClientScene.localPlayer, roundController.getHost());
            Assert.NotNull(roundController.getHost());
            
            yield return new WaitForSeconds(10);
        }
    }
}
