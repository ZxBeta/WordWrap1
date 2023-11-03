using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayers : MonoBehaviour
{
    public Transform spawnPos;
    public GameObject playerDetailsObj;
    List<Player> players;
    int index = 0;

    private void OnEnable()
    {
        SpawnList();
    }

    void SpawnList()
    {
        new DestroyGameObjInContainer(spawnPos.gameObject);

        players = APIDataContainer.instance.onlinePlayersList;
        index = 0;

        for(int i = 0; i< players.Count; i++)
        {
            if (players[i].dbId.ToString() != Globals.dbID)
                index++;
            else
                continue;
               

            GameObject p = Instantiate(playerDetailsObj, spawnPos);

            OnlinePlayerButton ob = p.GetComponent<OnlinePlayerButton>();
            ob.serialNOText.text = index.ToString();
            ob.nameText.text = players[i].userName;
            ob.score_GameWonText.text = players[i].points.ToString();
            ob.RankText.text =players[i].rank.ToString();
            ob.dbid = (int)players[i].dbId;

        }

    }
}
