using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RanNameByDateTime 
{
   public static string Get()
   {

        string name = "";
        DateTime d = System.DateTime.Now;
        var dateTimeString = d.ToFileTimeUtc().ToString();
       // Debug.Log(d);
      //  Debug.Log(dateTimeString.Length);
        name += dateTimeString.Substring(0,1);
        name += dateTimeString.Substring(6,1);
        name += dateTimeString.Substring(9,1);
        name += dateTimeString.Substring(13,1);
        name += dateTimeString.Substring(16,1);
        return name;

   }
}
