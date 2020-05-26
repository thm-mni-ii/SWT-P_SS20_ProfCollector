using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Mirror;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// This class represents a way to start a game.
/// It executes a set game and stores player data.
/// </summary>    
public class GameStarter : MonoBehaviour
{
    private const string FILE_NAME = "MyFile.txt";

    private Process gameProcess;
    
    /// <summary>
    /// Necessary for getting active window.
    /// </summary>
    /// <returns>Active window</returns>
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
 
    /// <summary>
    /// Necessary for toggling window.
    /// </summary>
    /// <param name="hWnd">Window to toggle</param>
    /// <param name="nCmdShow">on/off</param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


    /// <summary>
    /// Used to hide window.
    /// </summary>
    const int HINDOW_HIDE = 0;
    
    /// <summary>
    /// Used to show window.
    /// </summary>
    private const int WINDOW_SHOW = 1;

    private IntPtr window;

    /// <summary>
    /// Gets active window at start of game.
    /// </summary>
    private void Start()
    {
        window = GetActiveWindow();
    }

    /// <summary>
    /// Basic GUI for starting a game.
    /// This has to be replaced by a room menu at some point.
    /// </summary>
    private void OnGUI()
    {
        PlayerInfo playerInfo;
        if (GUI.Button(new Rect(500, 0, 120, 20), "Host Game"))
        {
            playerInfo = new PlayerInfo("Peter",true);
            startGame(playerInfo);
        }
        if (GUI.Button(new Rect(620, 0, 120, 20), "Join Game"))
        {
            playerInfo = new PlayerInfo("Peter",false);
            startGame(playerInfo);
        }
    }

    /// <summary>
    /// Checks if game is still running and opens window if it is not.
    /// </summary>
    private void Update()
    {
        if (gameProcess != null && gameProcess.HasExited)
        {
            ShowWindow(window, WINDOW_SHOW);
        }
    }

    /// <summary>
    /// Stores player information, starts game and hides the framework window.
    /// </summary>
    /// <param name="playerInfo">Player information to store</param>
    private void startGame(PlayerInfo playerInfo)
    {
        StorePlayerData(playerInfo);
        gameProcess = Process.Start(Environment.CurrentDirectory + @"\Games\Test\Test.exe");
        ShowWindow(window, HINDOW_HIDE);
    }
    
    /// <summary>
    /// Stores player data into a .txt file so the other game can load data.
    /// </summary>
    /// <param name="playerInfo">Player information to store</param>
    private void StorePlayerData(PlayerInfo playerInfo)
    {
        
        if (File.Exists(FILE_NAME))
        {
           File.Delete(FILE_NAME);
        }
        var sr = File.CreateText(FILE_NAME);
        sr.WriteLine(JsonUtility.ToJson(playerInfo));
        sr.Close();
    }
}
