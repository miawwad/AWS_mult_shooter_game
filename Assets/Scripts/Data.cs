using System;
using System.Collections.Generic;

// This data structure is returned by the client service when a game match is found
[Serializable]
public class PlayerSessionObject
{
    public string PlayerSessionId;
    public string PlayerId;
    public string GameSessionId;
    public string FleetId;
    public string CreationTime;
    public string Status;
    public string IpAddress;
    public string Port;
}

[Serializable]
public class PlayerData
{
    public int id;
    public int charId;
    public string name;
    public int life;
    public int con;
    public PlayerCoord pos;
    public PlayerCoord rot;
    public PlayerStatus status;
}

[Serializable]
public class PlayerCoord
{
    public float x;
    public float y;
    public float z;
    public float w;
}

[Serializable]
public class PlayerStatus
{
    public int move;
    public int rot;
    public int action;
}

[Serializable]
public class PlayersData
{
    public List<PlayerData> players;
}

[Serializable]
public class Nicknames
{
    public List<string> nicknames;
}
