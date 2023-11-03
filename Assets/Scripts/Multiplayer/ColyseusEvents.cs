using Colyseus;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq;
using UnityEngine;
using static ColyseusRoomBase;

public static class ColyseusEvents
{
    public static void GetTotalPlayerInWorld(ColyseusRoom<MyRoomState> worldRoom)
    {
        worldRoom.OnMessage<LobbyRoomPlayerInfo>("ONLINEPLAYER", (message) =>
        {
            
            Debug.Log("Total players in world = " + message.gamePlayerList.Length);
            APIDataContainer.instance.onlinePlayersList = message.gamePlayerList.ToList();
        });
    }

    public static void GetTotalPlayerInLobby(ColyseusRoom<MyRoomState> room)
    {      
        room.OnMessage<LobbyRoomPlayerInfo>("JOINFINAL", (message) => 
        {
            Debug.Log("Player added in lobby = " + message.gamePlayerList.Length);
            if(Globals.isRandomLobby)
            _ColyseusClient.instance.totalPlayersCount = message.gamePlayerList.Length;
            _ColyseusClient.instance.LobbyRoomInfo = message;        
            WaitingLobby.updatePlayersInLobby.Invoke();

        });
    }




    public static void PlayerSubmitted(ColyseusRoom<MyRoomState> room)
    {
        room.OnMessage<PlayerSumbitData>("playersubmitted", (message) =>
        {
         
            Debug.Log("Submition from dbId " + message.dbID + " userIndex " + message.userIndex);
            if (message.dbID != _ColyseusClient.instance.dbid)
            {
           
                for (int i = 0; i < MultiplayerGameManager.Instance.gameRoomPlayers.Count; i++)
                {
                    if (MultiplayerGameManager.Instance.gameRoomPlayers[i].dbId == message.dbID)
                    {
                        string name = MultiplayerGameManager.Instance.gameRoomPlayers[i].userName;
                        int score = (MultiplayerGameManager.Instance.playersCurrentRoundScoreDict[i] - message.score) * -1;

                       
                        if (message.addedWord)
                        {
                            Globals.msgTime = 5;

                            ShowMessage.instance.Show(name + " made "
                                + "'" + message.wordMade + "'" + " and scored " + "'" + score + "'");
                        }

                        else
                        {
                            ShowMessage.instance.Show(MultiplayerGameManager.Instance.gameRoomPlayers[i].userName + " pass the turn"
                              );
                        }

                        break;
                    }
                }
            }
           

            if (message.addedWord)
            {
                MultiplayerGameManager.Instance.UpdateAddedWordFromSubmit(message.wordMade, message.addedLettersList);
       
            }
          
            MultiPlayerUI.ScoreUpdateaction.Invoke(message.userIndex, message.score);
            MultiplayerGameManager.Instance.CheckNextState(message.userIndex);
        });
    }

    public static void ChallengeRecivedEvent(ColyseusRoom<MyRoomState> room)
    {
        room.OnMessage<ChallengeRecived>("ROOM_CONNECT_PRIVATE", (message) =>
        {
           // Debug.Log(message.senderdbId.ToString()+" "+ Globals.dbID);
            ChallengeManager.challengeRecived = message;
            
            if(message.senderdbId.ToString() != Globals.dbID)
            ChallengeManager.showChallenge.Invoke();
        });
    }

    public static void RandomLobbyTimer(ColyseusRoom<MyRoomState> room)
    {
        room.OnMessage<Object>("start_game", (message) =>
        {
            Debug.Log("Lobby closed ");

            if (_ColyseusClient.instance.LobbyRoomInfo.gamePlayerList.Length < 2)
            {
                ShowMessage.instance.Show("Unable to find any player try again later");
                _ColyseusClient.instance.LeaveLobby();
                WaitingLobby.CloseMe.Invoke();
                return; 
            }

            WaitingLobby.startCountDown.Invoke();

        });

    }

             

    public static void RoundEnded(ColyseusRoom<MyRoomState> room)
    {
        room.OnMessage<RoundWinner>("roundWinner", (message) =>
        {
            Debug.Log("Round winner " + message.userName);

        });

    }


    public static void GameEnded(ColyseusRoom<MyRoomState> room)
    {    
        room.OnMessage<GameWinner>("gameWinner", (message) =>
        {
            MultiplayerGameManager.Instance.FindWinner();
            MultiplayerGameManager.Instance.GameEnded();

            Debug.Log("Round winner " + message.userName);
        });
    }

    public static void PlayerLeftLobbyRoom(ColyseusRoom<MyRoomState> room)
    {
        room.OnMessage<PlayerLeftLobby>("PLAYERLEFT", (message) =>
        {
            Debug.Log("Player left " + message.userName);
          
            if(_ColyseusClient.instance.lobbyRoomJoined)
            WaitingLobby.playerLeft.Invoke(message);  
            

        });
    }

    public static void PlayerLeftGameRoom(ColyseusRoom<MyRoomState> room)
    {
        room.OnMessage<PlayerLeftLobby>("PLAYERLEFT", (message) =>
        {
            if (message.consented)
            {
                _ColyseusClient.instance.totalPlayersCount--;
       
                if(Globals.gameEnded==true)
                    ShowMessage.instance.Show(message.userName + " Left");

                for (int i =0; i < MultiplayerGameManager.Instance.gameRoomPlayers.Count; i++)
                {
                    if (MultiplayerGameManager.Instance.gameRoomPlayers[i].dbId == message.dbId)
                    {
                        Debug.Log("removed " + MultiplayerGameManager.Instance.gameRoomPlayers[i].userName);
                        MultiplayerGameManager.Instance.gameRoomPlayers.RemoveAt(i);                       
                    }
                }

                if (_ColyseusClient.instance.gameroomJoined && _ColyseusClient.instance.totalPlayersCount < 2)
                {
                    MultiplayerGameManager.Instance.FindWinner();
                    MultiplayerGameManager.Instance.GameEnded();
                }
            }
            else
            {
               
                MultiplayerGameManager.Instance.OpponenntReconnecting(message.userName,true);
            }

        });
    }

    public static void OpponentReconnected(ColyseusRoom<MyRoomState> room)
    {
        room.OnMessage<PlayerLeftLobby>("player_Reconnect", (message) =>
        {
            Debug.Log("Player reconnected " + message.userName);
            MultiplayerGameManager.Instance.OpponenntReconnecting(message.userName, false);

        });

    }

    public static void LettersRecived(ColyseusRoom<MyRoomState> room)
    {
        room.OnMessage<IDictionary>("DICTIONARY", (message) =>
        {
            //Debug.Log("recived letters " + message[]);


      

            foreach(var d in message)
            {
               // Debug.Log("ddd " + jo.Keys);

                
            }
            

        });

    }


    public static void OnStateChange(MyRoomState state, bool isFirstState)
    {
        if (isFirstState)
        {
            // First setup of your client state
            //   Debug.Log(state.status);
            Debug.Log(state.turnIndex);
        }
        else
        {
            // Further updates on your client state
          //  Debug.Log(state);
            Debug.Log("current turn " +state.turnIndex);
          
            Globals.currentTurnIndex = (int)state.turnIndex;

            if (state.turnIndex == _ColyseusClient.instance.myIndex && !MultiplayerGameManager.Instance.isMyTurn)
            {
               MultiplayerGameManager.Instance.CheckIsMyTurn(true);
            }
            else
                MultiplayerGameManager.Instance.CheckIsMyTurn(false);

            MultiPlayerUI.i.UpdatePlayerIndicator(Globals.currentTurnIndex);

        }
    }

    public static void RoomStateOnChange(ColyseusRoom<MyRoomState> room)
    {
        room.State.OnChange += (changes) =>
        {
            changes.ForEach((obj) =>
            {
               // Debug.Log("Room State change " + obj.ToString());
      
            });
        };
    }



    public static void OnPlayerAdd(int key, Player player)
    {
         Debug.Log("Player added! = " + player.userName +" dbid "+ player.dbId);
    }

    public static void OnPlayerChange(int key, Player player)
    {
        Debug.Log("Player changed! " + player.userName + " dbid " + player.dbId);
    }

    public static void OnPlayerRemove(int key, Player player)
    {
      
        Debug.Log("Player removed! " + player.userName + " dbid " + player.dbId);
    }

}
