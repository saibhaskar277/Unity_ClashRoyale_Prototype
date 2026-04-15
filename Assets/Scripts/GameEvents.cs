
public class UnitSpawnData
{
    public UnitTeam currentUnitType;
    public UnitData unitData;
    public int level;
    public float damage;
    public float health;
}

public struct ElixirChangedEvent : IGameEvent
{
    public float CurrentElixir;
    public float MaxElixir;
    public float RegenPerSecond;
}

public struct MatchTimerUpdatedEvent : IGameEvent
{
    public float RemainingSeconds;
    public float TotalSeconds;
}

public struct ElixirMultiplierChangedEvent : IGameEvent
{
    public float Multiplier;
}

public struct TowerDestroyedEvent : IGameEvent
{
    public Tower Tower;
    public UnitTeam Team;
    public bool IsMainTower;
}

public struct GameEndedEvent : IGameEvent
{
    public bool PlayerWon;
    public string Reason;
}