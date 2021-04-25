using UnityEngine;
using System.Collections;
using UnitySocketIO;
using UnitySocketIO.Events;
using System;

public class BoardTile : MonoBehaviour {

    public bool completed;
    public GameObject UIclone;
    public BoardSlot currentslot;
    public string letter;
    public int score;

    public int GetNumber(string name)
    {
        name = name.Substring(name.Length - 3, 2);
        if (name[0] == '(')
        {
            name = name.Substring(1, 1);
        }

        return Int32.Parse(name);
    }

    void OnMouseDown()
    {
        if(currentslot != null && !completed)
        {
            currentslot.free = true;
            currentslot = null;


            //string colName = transform.parent.name;
            //string rowName = transform.parent.parent.name;

            //Move move = new Move();
            //move.roomId = Global.room.id;
            //move.targetBoardSlotId = (GetNumber(rowName) - 1) * 15 + GetNumber(colName) - 1;

            //Debug.Log("BoardTile : (" + rowName + ", " + colName + ")");
            //SocketIOController.instance.Emit("ClickBoardTile", JsonUtility.ToJson(move));
        }
            
    }

    public void OnMouseDrag()
    {
        if (completed)
            return;
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPos.z = 0;
        UIclone.SetActive(true);
        UIclone.GetComponent<UITile>().dragging = true;
        UIclone.transform.position = cursorPos;
        GameController.data.letterDragging = true;
        gameObject.SetActive(false);
    }
}
