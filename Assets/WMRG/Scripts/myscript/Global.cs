using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
    /*********** WegGL *********************/
    //public static string DOMAIN = "scrabble.multiplays.net";
    //public static int PORT = 0;
    //public static bool SSL_ENALBLED = false;
    /***************************************/

    /*********** Android *********************/
    public static string DOMAIN = "93.188.164.208";
    public static int PORT = 3003;
    public static bool SSL_ENALBLED = false;
    /***************************************/

    public static bool isTesting = false;

    public static int testingPort = 3003;
    public static string testingDomain = "localhost";

    public static bool isSingle = false;

    public static string currentDomain = "";

    public static float balance = 0;

    public static int limitedMinutes = 5;

    public static bool socketConnected = false;
    public static bool mainPlayer;
    public static bool isMultiplayer;
    public static Room room;
    public static int myTurn = 1;

    public static User m_user;
    public static SaveData savedData = new SaveData();
    public static bool isLoading = false;
    public static bool nextLoad;
    public static string myname = "";
    public static List<string> othernames = new List<string>();

    public static void GetDomain()
    {
        currentDomain = DOMAIN;

        if (SSL_ENALBLED)
        {
            currentDomain = "https://" + currentDomain;
        }
        else
        {
            currentDomain = "http://" + currentDomain;
        }

        if (PORT != 0)
        {
            currentDomain += ":" + PORT;
        }

        if (isTesting == true)
        {
            currentDomain = "http://" + testingDomain + ":" + testingPort;
        }
    }

}

[Serializable]
public class User
{
    public long id;
    public string name;
    public long score;

    public string address;

    public User(long id = -1, string name = "", long score = 0, string address = "")
    {
        this.id = id;
        this.name = name;
        this.score = score;
        this.address = address;
    }
}

[Serializable]
public class SaveData
{
    public int cntPlayers;
    public int turn;
    public bool initial;
    public List<int> positions;

    public static SaveData CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<SaveData>(data);
    }

    public SaveData()
    {
        cntPlayers = -1;
        turn = -1;
        initial = false;
        positions = new List<int>();
    }
}

[Serializable] 
public class SetWinnerData
{
    public string roomId, myName;
    public List<string> otherPlayers;

    public SetWinnerData()
    {
        if (Global.room != null)
        {
            roomId = Global.room.id;
        }
        if (Global.othernames.Count >= Global.myTurn)
        {
            myName = Global.othernames[Global.myTurn - 1];
            otherPlayers = new List<string>(Global.othernames);
        }
    }

    public static SetWinnerData CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<SetWinnerData>(data);
    }
}