using UnityEngine;
using UnityEngine.UI;

public class InGameAvatarPanel : MonoBehaviour
{
    public Image currentAvatar;
    GameObject pressedBtn;
    public Button[] avbtns;
    public int tempIndex;
    bool changedPicTemp;


    public void OnClickApply()//for saving
    {
       
        pressedBtn.transform.GetChild(1).gameObject.SetActive(false);

        UserProfileManager.eventUpdateDP.Invoke(currentAvatar.sprite);
        tempIndex = pressedBtn.transform.GetComponent<AvtarButtons>().index;
        Globals.avatar = tempIndex.ToString();

        if (Globals.isLoggedIn)
        {
            APIManager.instance.UpdateAvatar(int.Parse(Globals.dbID), Globals.avatar);
        }
          
        else
            SaveLoadSystem.SaveGuestDetails();
    }


    public void APIChangeAvtar(int index)//by invoking btn from api call
    {

        pressedBtn = avbtns[index].gameObject;
       
        tempIndex = pressedBtn.transform.GetComponent<AvtarButtons>().index;
        Sprite pressedS = pressedBtn.transform.GetComponent<Image>().sprite;
        pressedBtn.transform.GetComponent<Image>().sprite = currentAvatar.sprite;
        currentAvatar.sprite = pressedS;
   
    }


    public void SetProfilePicture(GameObject t)//on button clicked
    {
        if (pressedBtn != null)
            pressedBtn.transform.GetChild(1).gameObject.SetActive(false);

        pressedBtn = t;

        t.transform.GetChild(1).gameObject.SetActive(true);

        Sprite pressedS = pressedBtn.transform.GetComponent<Image>().sprite;
        pressedBtn.transform.GetComponent<Image>().sprite = currentAvatar.sprite;
        currentAvatar.sprite = pressedS;
        changedPicTemp = true;

    }


    public void OnCloseReverse()//when panel closed without save
    {
        if (!changedPicTemp)
            return;

        Sprite pressedS = pressedBtn.transform.GetComponent<Image>().sprite;
        pressedBtn.transform.GetComponent<Image>().sprite = currentAvatar.sprite;
        currentAvatar.sprite = pressedS;
        pressedBtn.transform.GetChild(1).gameObject.SetActive(false);
        changedPicTemp = false;
    }

}

