using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NativeSharing 
{

    public static void ShareText(string text)
    {

        NativeShare nativeShare = new NativeShare();
        nativeShare.SetText(text);
        nativeShare.Share();

    }

}
