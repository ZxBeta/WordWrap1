using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ColyseusRoomBase;
using static UserDetailsBase;

public class ChallengePlayers : MonoBehaviour
{
    public Sprite disabledSprite;
    int challengedCount;

   public void SendRequest(OnlinePlayerButton ob)
   {

        ChallengeRequestDetails challengeRequest = new ChallengeRequestDetails();

        challengeRequest.senderdbId = int.Parse(Globals.dbID);
        //print(challengeRequest.senderdbId);
        challengeRequest.senderUserName = Globals.UserName;
        challengeRequest.recieverdbId = ob.dbid;
        challengeRequest.reciverUserName = ob.name;
        challengeRequest.noOfLevels = _ColyseusClient.instance.totalRounds;
        Globals.totalPlyrsInChalangeLobby++;
        challengeRequest.minplayersToMatch = Globals.totalPlyrsInChalangeLobby;
        challengeRequest.totalTime = _ColyseusClient.instance.totalTime;
        transform.GetComponent<Image>().sprite = disabledSprite;
        transform.GetComponent<Button>().enabled = false;
        _ColyseusClient.instance.ChallengePlayers(challengeRequest);


   }

 

  


}
