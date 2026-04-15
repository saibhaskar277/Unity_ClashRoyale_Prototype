using System.Collections.Generic;
using UnityEngine;

public class PlayerSessionData : MonoBehaviour
{


    public static PlayerSessionData instance;

    public PlayerProfile Profile { get; private set; }

    public string PlayerId => Profile.PlayerId;
    public string DeviceId => Profile.DeviceId;

    public IReadOnlyList<UnitId> Deck => Profile.SelectedDeck;
    public IReadOnlyList<UnitOwnershipData> OwnedUnits => Profile.OwnedUnits;

    public int Trophies => Profile.Trophies;
    public int Arena => Profile.Arena;

    public PlayerSessionData(PlayerProfile profile)
    {
        Profile = profile;
    }


    private void Awake()
    {
        if(instance == null)
           instance = this;
    }


    public int GetUnitLevel(UnitId unitId)
    {
        var unit = Profile.OwnedUnits.Find(x => x.UnitId == unitId);
        return unit != null ? unit.Level : 1;
    }

    public bool HasUnit(UnitId unitId)
    {
        return Profile.OwnedUnits.Exists(x => x.UnitId == unitId && x.IsUnlocked);
    }

    public bool IsUnitInDeck(UnitId unitId)
    {
        return Profile.SelectedDeck.Contains(unitId);
    }
}