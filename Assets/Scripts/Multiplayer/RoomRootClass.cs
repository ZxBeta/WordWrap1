using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomRootClass : MonoBehaviour
{
    public Room room;
    public string sessionId;

}

public class Room
{
    public int clients;
    public DateTime createdAt;
    public int maxClients;
    public string name;
    public string processId;
    public string roomId;
}
