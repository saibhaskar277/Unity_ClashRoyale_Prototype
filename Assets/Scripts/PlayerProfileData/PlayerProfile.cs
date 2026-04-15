using System;
using System.Collections.Generic;

[Serializable]
public class PlayerProfile
{
    public string PlayerId;
    public string DeviceId;
    public string DisplayName;

    public int Trophies;
    public int Arena;

    public List<UnitOwnershipData> OwnedUnits = new();
    public List<UnitId> SelectedDeck = new();

    public string CurrentEmote;
    public string CurrentArenaSkin;

    public long LastLoginUnix;
}


[Serializable]
public class UnitOwnershipData
{
    public UnitId UnitId;
    public int Level;
    public int CardCount;
    public bool IsUnlocked;
}

