using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UnitySocketIO;
using UnitySocketIO.Events;
using LitJson;

public class JokerLetter : MonoBehaviour , IPointerClickHandler{

    private string _letter;
	void Start () {
        _letter = GetComponentInChildren<Text>().text;
    }


    public void OnPointerClick(PointerEventData data)
    {
        //if (Global.myTurn != GameController.data.currentPlayer)
        //{
        //    return;
        //}

        //if (Global.socketConnected)
        //{
        //    Room room = new Room(_letter, Global.room.id);
        //    SocketIOController.instance.Emit("ApplyJokerTile", JsonUtility.ToJson(room));
        //}
        
        GameController.data.ApplyJokerTile(_letter);
    }
}
