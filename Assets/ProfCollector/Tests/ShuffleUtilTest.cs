using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ShuffleUtilTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void Shuffle_Util_Test_Shuffle()
        {
            // Use the Assert class to test conditions
            List<int> list = new List<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
            list.Add(5);
            
            Assert.AreEqual(new ArrayList(){1,2,3,4,5}, list);
            
            ShuffleUtil.Shuffle(list);
            
            Assert.AreNotEqual(new ArrayList(){1,2,3,4,5}, list);
            
            
            
            
        }
    }
}
