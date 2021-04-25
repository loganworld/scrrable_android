using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnitySocketIO;
using UnitySocketIO.Events;

public class Alphabet : MonoBehaviour
{

    [System.Serializable]
    public class Letters
    {
        public string letter;
        public int qty;
        public int score;
    }
    public List<Letters> LettersList;
    public List<string> LettersFeed;

    public static Alphabet data;

	void Awake () {
        data = this;
    }

    public void SetLettersFeed(List<string> letters)
    {
        if (letters == null)
        {
            return;
        }

        LettersFeed.Clear();
        foreach (string letter in letters)
        {
            LettersFeed.Add(letter);
        }
    }

    public void ShuffleLettersFeed()
    {

        int n = LettersFeed.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n);
            string value = LettersFeed[k];
            LettersFeed[k] = LettersFeed[n];
            LettersFeed[n] = value;
        }


    }

    void FillLettersFeed() {
        LettersFeed.Clear();
        string str = "";
        foreach (Letters letterItem in LettersList)
        {
            for (int i = 1; i <= letterItem.qty; i++)
            {
                LettersFeed.Add(letterItem.letter);
                str += letterItem.letter;
            }
        }

        ShuffleLettersFeed();

        //if (Global.myTurn == 1)
        //{
        //    // SocketIOController.instance.Emit("LettersFeed", JsonUtility.ToJson(new LettersData(LettersFeed)));
        //    Debug.Log("------------ LettersFeed ------------");
        //    Debug.Log(LettersFeed.Count);
        //    Debug.Log(str.Length);
        //}
    }

    public string GetRandomLetter() {
        int rand = UnityEngine.Random.Range(0,LettersFeed.Count);
        string result = LettersFeed[rand]; 
        LettersFeed.RemoveAt(rand);
        return result;
    }

    public string GetLetter(int no)
    {
        
        string result = LettersFeed[no];
        LettersFeed.RemoveAt(no);
        return result;
    }

    public int GetLetterScore(string letter)
    {
        foreach(Letters letterItem in LettersList)
        {
            if (letterItem.letter == letter)
                return letterItem.score;
        }
        return 0;
    }

    public void ResetFeed()
    {
        LettersFeed.Clear();
        FillLettersFeed();
    }
}
