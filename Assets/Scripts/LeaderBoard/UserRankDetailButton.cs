using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserRankDetailButton : MonoBehaviour
{
    public Color defaultColor;
    public Color pressedColor;

    [Header ("RankingBased")]
    public Color IstRankColor;
    public Color IIRankColor;
    public Color IIIRankColor;

    [Header("Texts")]
    public TMP_Text percentText;
    public TMP_Text nameText;
    public TMP_Text totalGamePlayedText;
    public TMP_Text gameWonText;
    public Color textDefaultColor;
    public Color textPressedColor;


    Image thisImage;

    private void Awake()
    {
        thisImage = GetComponent<Image>();
    }


    public void OnClick()
    {
        if (thisImage.color == defaultColor) ChangeState(pressedColor,textPressedColor); else ChangeState(defaultColor,textDefaultColor);      
    }

    public void ChangeDefaultColorByRank(int rank)
    {
        if (rank > 2) return;

        if (rank == 0) defaultColor = IstRankColor;
        else if (rank == 1) defaultColor = IIRankColor;
        else if (rank == 2) defaultColor = IIIRankColor;

        ChangeState(defaultColor,textPressedColor);
        textDefaultColor = textPressedColor;
    }

    void ChangeState(Color imgColor, Color btnTextColor)
    {
        thisImage.color = imgColor;
        percentText.color = btnTextColor;
        nameText.color = btnTextColor;
        totalGamePlayedText.color = btnTextColor;
        gameWonText.color = btnTextColor;
    }
}
