﻿using System.Collections;
using System;

using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class UserInfo : MonoBehaviour
{

	public string userN;
    public string localId;

    public UserInfo()
    {
        userN = Databasemanagment.playerName;
        localId = Databasemanagment.localId;
    }

}
