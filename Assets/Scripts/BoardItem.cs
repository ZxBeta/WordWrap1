using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardItem : MonoBehaviour, IDropHandler
{
    public bool hasLetter;

    public int columnValue, rowValue;

    public Vector2 myPos;

    private void Awake()
    {
        hasLetter = false;
    }

    private void Update()
    {
        if (transform.childCount > 0)
            hasLetter = true;
        else
            hasLetter = false;
    }

    public void Initialize(int row, int col )
    {
        columnValue = col;
        rowValue = row;
    }

    public void OnDrop(PointerEventData eventdata)
    {
        if (Globals.isMultiplayerMode && !MultiplayerGameManager.Instance.isMyTurn)
            return;
        //  Debug.Log("Dropped");
        if (eventdata.pointerDrag != null && !eventdata.pointerDrag.GetComponent<LetterScript>().isPlaced)
        {
            MultiplayerGameManager.Instance.PushLetterInColumn(columnValue, eventdata.pointerDrag.GetComponent<LetterScript>());
          //  MultiplayerGameManager.Instance.letterToUndo = eventdata.pointerDrag.gameObject;
          //  MultiplayerGameManager.Instance.undoLttrBtn.gameObject.SetActive(true);
        }
    }

    public void SetLetter(LetterScript Letter)
    {
       // Debug.Log("setting");
        Letter.isPlaced = true;
        hasLetter = true;
        var rect = Letter.gameObject.transform.GetComponent<RectTransform>();
        Letter.gameObject.transform.SetParent(this.transform);
        Letter.transform.position = this.transform.position;
    }

  
}
