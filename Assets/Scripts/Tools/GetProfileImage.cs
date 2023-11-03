using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetProfileImage 
{
    public static Sprite Get(string imageString)
    {
        if(ImageUtility.IsBase64(imageString))
        {
            return ImageUtility.Base64ToSprite(imageString);
        }
        else
        {
            try
            {
                var imageIndex = int.Parse(imageString);
                return APIDataContainer.instance.AvatarImagelist[imageIndex];
            }

            catch
            {
                return APIDataContainer.instance.AvatarImagelist[0];
            }
          
        }

    }
}
