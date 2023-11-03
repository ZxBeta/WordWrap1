using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public static class GetDateTime
{
    public static DateTime Get()
    {
        var client = new TcpClient("time.nist.gov", 13);
        DateTime dateTime;

        using (var streamReader = new StreamReader(client.GetStream()))
        {
            var response = streamReader.ReadToEnd();
            var utcDateTimeString = response.Substring(7, 17);
            dateTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }

        return dateTime;

    }

    public static string GetDate()
    {
        var client = new TcpClient("time.nist.gov", 13);
        DateTime dateTime;

        using (var streamReader = new StreamReader(client.GetStream()))
        {
            var response = streamReader.ReadToEnd();
            try 
            { var utcDateTimeString = response.Substring(7, 17);
                dateTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", 
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                return dateTime.ToString("yyyy/M/dd");
            }
            catch
            {
                return null;
            }
           
           
            
        }

      

    }
}
