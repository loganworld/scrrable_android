using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

using UnitySocketIO;
using UnitySocketIO.Events;
using System;

public class UITile : MonoBehaviour
{
    public GameObject UISlot;
    public GameObject boardTilePrefab;
    public GameObject boardTile;
    public GameObject targetSlot;
    public Vector2 lastPosition;
    public Text letterString;
    public Text letterScore;
    public bool dragging;
    public bool needSwap;
    public bool finished;

    public Transform testObj;

    public bool isSlot;

    private Transformer transformer;
    private float uiZaxis;

    private void Awake()
    {
        isSlot = false;
    }

    void Start()
    {
        
        transformer = GetComponent<Transformer>();
        uiZaxis = Camera.main.transform.position.z + FindObjectOfType<Canvas>().planeDistance;

    }

    public void MoveAction(Move move)
    {
        boardTile.SetActive(false);

        if (move.mode == 1)
        {
            GameController.data.targetBoardSlot = GameController.data.BoardSlots[move.targetBoardSlotId];

            GameController.data.OpenSelectJokerLetter(boardTile);
            gameObject.SetActive(false);

            Debug.Log("Mode 1: Board Tile Parent Name: " + boardTile.transform.name);
            Debug.Log("Mode 1: Taget Board Slot : " + GameController.data.targetBoardSlot.transform.name);
        }
        else if (move.mode == 2)
        {
            if (!isSlot)
            {
                boardTile.SetActive(true);
                boardTile.GetComponent<BoardTile>().currentslot.free = true;
                boardTile.GetComponent<BoardTile>().currentslot = null;
            }

            isSlot = false;

            GameController.data.targetBoardSlot = GameController.data.BoardSlots[move.targetBoardSlotId];

            GameController.data.PlantTile(boardTile);
            gameObject.SetActive(false);

            Debug.Log("Mode 2: canBePlant Board Tile Parent Name: " + boardTile.transform.parent.name);
            Debug.Log("Mode 2: Taget Board Slot : " + GameController.data.targetBoardSlot.transform.name);
        }
        else
        {

            if (move.mode == 3)
            {
                if (!isSlot)
                {
                    boardTile.SetActive(true);
                    boardTile.GetComponent<BoardTile>().currentslot.free = true;
                    boardTile.GetComponent<BoardTile>().currentslot = null;
                    boardTile.SetActive(false);
                }

                isSlot = false;

                targetSlot = GameController.data.players[GameController.data.currentPlayer - 1].UITileSlots[move.targetSlotId];

                GoToSlot(targetSlot);
                

                Debug.Log("Mode 3: " + targetSlot.name);
            }
            else
            {
                MoveToPos(lastPosition);

                Debug.Log("Mode 4: " + lastPosition);

            }
            GameController.data.PreApply();
        }
    }

    public void GetNewLetter()
    {
        if (Alphabet.data.LettersFeed.Count > 0)
        {
            //letterString.text = Alphabet.data.GetRandomLetter();
            letterString.text = Alphabet.data.GetLetter(0);

            int score = Alphabet.data.GetLetterScore(letterString.text);
            letterScore.text = score.ToString();
            CreateNewBoardTile(letterString.text, score);
        }
        else
        {
            finished = true;
            gameObject.SetActive(false);
        }
        
    }

    void Update()
    {
        if (GameController.data.paused || GameController.data.currentPlayer != Global.myTurn)
            return;
        if (dragging)
        {
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorPos.z = uiZaxis;
            transform.position = cursorPos;
            GameController.data.letterDragging = true;
            gameObject.transform.SetAsLastSibling();

            if (Input.GetMouseButtonUp(0))
            {
                OnMouseUp();
            }
        }
    }

    void OnMouseDrag()
    {
        if (GameController.data.paused || GameController.data.currentPlayer != Global.myTurn)
            return;
        dragging = true;

        
    }

    void OnMouseDown() {
        if (GameController.data.paused || GameController.data.currentPlayer != Global.myTurn)
            return;
        if (CheckifOverUISlot())
        {
            
            if (!Global.isSingle)
            {
                Move move = new Move();

                move.UITileName = transform.name;
                move.roomId = Global.room.id;
                move.targetSlotId = GetNumber(targetSlot.name) - 1;
                SocketIOController.instance.Emit("DownUISlot", JsonUtility.ToJson(move));
            }
                

            targetSlot.GetComponent<UISlot>().UITile = null;
            targetSlot = null;

        }
        //Debug.Log("Clicked Name : " + transform.name);
    }

    public void UpdateUISlot(Move move)
    {
        Debug.Log("Updated UI Slot : " + transform.name);
        targetSlot = GameController.data.players[GameController.data.currentPlayer - 1].UITileSlots[move.targetSlotId];
        targetSlot.GetComponent<UISlot>().UITile = null;
        targetSlot = null;

        isSlot = true;
    }

    public int GetNumber(string name)
    {
        name = name.Substring(name.Length - 3, 2);
        if (name[0] == '(')
        {
            name = name.Substring(1, 1);
        }

        return Int32.Parse(name);
    }

    void OnMouseUp()
    {
        if (GameController.data.swapMode)
        {
            SetSwapState(!needSwap);

            return;
        }

        if (GameController.data.paused || GameController.data.currentPlayer != Global.myTurn)
        {
            return;
        }

        dragging = GameController.data.letterDragging = false;

        Move move = new Move();

        string targetSlotName = GameController.data.targetBoardSlot.transform.name;
        string targetSlotParentName = GameController.data.targetBoardSlot.transform.parent.name;


        move.UITileName = transform.name;
        move.targetBoardSlotId = (GetNumber(targetSlotParentName) - 1) * 15 + GetNumber(targetSlotName) - 1;


        if (letterScore.text == "0" && GameController.data.canBePlant)
        {
            GameController.data.OpenSelectJokerLetter(boardTile);
            gameObject.SetActive(false);

            move.mode = 1;

            //Debug.Log("Mode 1: Board Tile Parent Name: " + boardTile.transform.name);
            Debug.Log("Mode 1: Taget Board Slot : " + GameController.data.targetBoardSlot.transform.name);
        }
        else if (GameController.data.canBePlant)
        {

            GameController.data.PlantTile(boardTile);
            gameObject.SetActive(false);

            move.mode = 2;

            //Debug.Log("Mode 2: canBePlant Board Tile Parent Name: " + boardTile.transform.parent.name);
            Debug.Log("Mode 2: Taget Board Slot : " + GameController.data.targetBoardSlot.transform.name);
        }
        else {

            if (CheckifOverUISlot())
            {
                Debug.Log("Mode 3: " + targetSlot.name);

                move.targetSlotId = GetNumber(targetSlot.name) - 1;
                move.mode = 3;

                GoToSlot(targetSlot);


            }
            else {
                MoveToPos(lastPosition);

                Debug.Log("Mode 4: " + lastPosition);

                move.mode = 4;
            }
            GameController.data.PreApply();
        }

        if (!Global.isSingle)
        {
            move.roomId = Global.room.id;
            SocketIOController.instance.Emit("move", JsonUtility.ToJson(move));
        }
    }


    public void ReCreateTile()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        GetNewLetter();
        GoToFreeSlot();
    }

    void CreateNewBoardTile(string letter, int score) {
        boardTile = (GameObject)Instantiate(boardTilePrefab, new Vector3(99, 0, 0), Quaternion.identity);
        boardTile.tag = "BoardTile";
        GameController.data.BoardTiles.Add(boardTile);
        boardTile.GetComponent<BoardTile>().UIclone = gameObject;
        boardTile.GetComponent<BoardTile>().letter = letter;
        boardTile.GetComponent<BoardTile>().score = score;
        TextMesh[] txts = boardTile.GetComponentsInChildren<TextMesh>();
        txts[0].text = letter;
        txts[1].text = score.ToString();

        boardTile.SetActive(false);
    }

    public void GoToSlot(GameObject slot)
    {
        MoveToPos(slot.GetComponent<RectTransform>().anchoredPosition);
        
        if (slot.GetComponent<UISlot>().UITile != null)
        {
            slot.GetComponent<UISlot>().UITile.GetComponent<UITile>().GoToFreeSlot();
        }

        slot.GetComponent<UISlot>().UITile = gameObject;
        UISlot = slot;
        lastPosition = slot.GetComponent<RectTransform>().anchoredPosition;
    }

    public void GoToFreeSlot()
    {
        GameObject freeUISlot = GameController.data.GetFreeUISlot();
        if (GameController.data.GetFreeUISlot() != null)
        {
            Debug.Log("Position : ");
            Debug.Log(freeUISlot.GetComponent<RectTransform>().anchoredPosition);

            MoveToPos(freeUISlot.GetComponent<RectTransform>().anchoredPosition);
            freeUISlot.GetComponent<UISlot>().UITile = gameObject;
            UISlot = freeUISlot;
            lastPosition = freeUISlot.GetComponent<RectTransform>().anchoredPosition;
        }
    }

    public void CancelTile() {
        Vector3 tempPos = boardTile.transform.position;
        tempPos.z = uiZaxis;
        transform.position = tempPos;
        if (UISlot.GetComponent<UISlot>().UITile == null)
        {
            MoveToPos(lastPosition);
            UISlot.GetComponent<UISlot>().UITile = gameObject;
        }
        else
            GoToFreeSlot();

        boardTile.SetActive(false);
    }

    void MoveToPos(Vector3 toPos)
    {
        gameObject.transform.parent.SetAsLastSibling();
        transformer.MoveUI(toPos, 0.25f);
    }

    public bool CheckifOverUISlot()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult res in raycastResults)
            {

                if (res.gameObject.name == "ErrorAlert")
                {
                    return false;
                }

                if (res.gameObject.tag == "UISlot")
                {
                    targetSlot = res.gameObject;
                    return true;
                }
            }
        }
        targetSlot = null;
        return false;
    }

    public void SetSwapState(bool swapState)
    {
        needSwap = swapState;

        if (needSwap)
            MoveToPos(GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 60));
        else
            MoveToPos(GetComponent<RectTransform>().anchoredPosition - new Vector2(0, 60));
    }

 }

