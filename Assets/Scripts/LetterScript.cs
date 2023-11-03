using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class LetterScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public bool isPlaced { get; set; }
    public bool isSelected { get; set; }
    public bool canSelect;
    public bool isWildLetter { get; set; }
    GameObject attachedBoard;
    Vector2 DefaultPosition;
    public TextMeshProUGUI Textbox;
    public TextMeshProUGUI MuxBox;
    public string value { get; private set; }
    public int multiplier { get; private set; }
    public Image background;

    public void initializeLetter(string st, int mux, bool isMultiplayer, bool isWild)
    {
        if (isMultiplayer == false)
            isPlaced = true;
        else
            isPlaced = false;
        isSelected = false;
        value = st;
        canSelect = true;
        attachedBoard = null;
        multiplier = mux;
        Textbox.text = st;
        if(multiplier == 1)
        {
            Textbox.color = Color.black;
        }
        if(multiplier == 2)
        {
            Color clr;
            ColorUtility.TryParseHtmlString("#79D01B", out clr);
            background.color = clr;
            Textbox.color = Color.white;
            MuxBox.text = "2x";
        }
        if(multiplier == 3)
        {
            Color clr;
            ColorUtility.TryParseHtmlString("#02AFFF", out clr);
            background.color = clr;
            Textbox.color = Color.white;
            MuxBox.text = "3x";
        }
        isWildLetter = isWild;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Globals.isMultiplayerMode && !MultiplayerGameManager.Instance.isMyTurn)
            return;


        if (!isPlaced)
        {
            DefaultPosition = transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (Globals.isMultiplayerMode && !MultiplayerGameManager.Instance.isMyTurn)
            return;

        if (!isPlaced)
            gameObject.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Globals.isMultiplayerMode && !MultiplayerGameManager.Instance.isMyTurn)
            return;


        if (!isPlaced)
        {
            gameObject.transform.position = DefaultPosition;
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //  Debug.Log("selected");
        AudioHandler.instance.ButtonPressed();

        if (!canSelect)
        {
            AudioHandler.instance.NotValidMove();
            ShowMessage.instance.Show("Not a valid selection!");
            return;
        }
           

        if (isPlaced && !isSelected && Globals.isMultiplayerMode && MultiplayerGameManager.Instance.isMyTurn)
        {
            if(!isWildLetter)
            {
                isSelected = true;
                MultiplayerGameManager.Instance.InsertLetter(value);
                MultiplayerGameManager.Instance.LastPlaced = gameObject;
                MultiplayerGameManager.Instance.AddToSelectedLetters(gameObject);
                MultiplayerGameManager.Instance.SetSelectable((gameObject.GetComponentInParent(typeof(BoardItem), true) as BoardItem).columnValue);
            }
            else
            {
                WildLetterHandler.instance.FitWildLetter(value, MultiplayerGameManager.Instance.WordTextBox);
                WildLetterHandler.instance.CloseWildLetterPanel();
                MultiplayerGameManager.Instance.CheckWord();
            }

            if (transform.GetChild(3).gameObject.activeInHierarchy)
            {
                MultiplayerGameManager.Instance.addedLetterFromRack = true;
            }
        }
        if (isPlaced && !isSelected && Globals.isSingleplayerMode)
        {
            if(!isWildLetter)
            {
                isSelected = true;
                SingleplayerGameManager.Instance.InsertLetter(value);
                SingleplayerGameManager.Instance.LastPlaced = gameObject;
                SingleplayerGameManager.Instance.AddToSelectedLetters(gameObject);
                SingleplayerGameManager.Instance.SetSelectable((gameObject.GetComponentInParent(typeof(BoardItem), true) as BoardItem).columnValue);
            }
            else
            {
                Debug.Log("here");
                WildLetterHandler.instance.FitWildLetter(value, SingleplayerGameManager.Instance.WordTextBox);
                WildLetterHandler.instance.CloseWildLetterPanel();
                SingleplayerGameManager.Instance.CheckCorrectWord();
            }

            Pressed();
        }
    }

    public void HighLight()
    {
        GetComponent<Animator>().SetTrigger("HighLight");
        gameObject.transform.GetChild(3).gameObject.SetActive(false);
    }
    
    void Pressed()
    {
        transform.GetChild(3).gameObject.SetActive(true);
    }
    public void UnPressed()
    {
        transform.GetChild(3).gameObject.SetActive(false);
    }
}
