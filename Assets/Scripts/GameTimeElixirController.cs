using UnityEngine;

public class GameTimeElixirController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] UnitSpawnSystem playerSpawnSystem;
    [SerializeField] MatchTimeElixirConfig config;

    [Header("Defaults")]
    [SerializeField] float defaultElixirMultiplier = 1f;

    float remainingTimeSeconds;
    bool isRunning;

    public float RemainingTimeSeconds => remainingTimeSeconds;
    public float TotalTimeSeconds => config != null ? config.TotalMatchTimeSeconds : 0f;
    public float CurrentElixirMultiplier { get; private set; } = 1f;

    void Start()
    {
        if (config == null)
        {
            Debug.LogWarning("GameTimeElixirController has no MatchTimeElixirConfig assigned.", this);
            return;
        }

        remainingTimeSeconds = config.TotalMatchTimeSeconds;
        isRunning = true;
        ApplyCurrentMultiplier();
        RaiseTimerUpdated();
    }

    void OnEnable()
    {
        EventManager.AddListner<GameEndedEvent>(OnGameEnded);
    }

    void OnDisable()
    {
        EventManager.RemoveListner<GameEndedEvent>(OnGameEnded);
    }

    void Update()
    {
        if (!isRunning || config == null)
            return;

        remainingTimeSeconds = Mathf.Max(0f, remainingTimeSeconds - Time.deltaTime);
        RaiseTimerUpdated();
        ApplyCurrentMultiplier();

        if (remainingTimeSeconds <= 0f)
        {
            isRunning = false;
            EventManager.RaiseEvent(new GameEndedEvent
            {
                PlayerWon = false,
                Reason = "Time Up"
            });
        }
    }

    void ApplyCurrentMultiplier()
    {
        float nextMultiplier = ResolveMultiplier(remainingTimeSeconds);
        if (Mathf.Approximately(nextMultiplier, CurrentElixirMultiplier))
            return;

        CurrentElixirMultiplier = nextMultiplier;
        EnsureSpawnSystem();
        if (playerSpawnSystem != null)
            playerSpawnSystem.SetElixirRegenMultiplier(CurrentElixirMultiplier);

        EventManager.RaiseEvent(new ElixirMultiplierChangedEvent
        {
            Multiplier = CurrentElixirMultiplier
        });
    }

    float ResolveMultiplier(float remainingSeconds)
    {
        if (config == null)
            return defaultElixirMultiplier;

        float multiplier = defaultElixirMultiplier;
        var stages = config.ElixirStages;
        if (stages == null || stages.Count == 0)
            return multiplier;

        for (int i = 0; i < stages.Count; i++)
        {
            var stage = stages[i];
            if (remainingSeconds <= stage.RemainingTimeSeconds)
            {
                multiplier = Mathf.Max(0f, stage.Multiplier);
            }
        }

        return multiplier;
    }

    void RaiseTimerUpdated()
    {
        EventManager.RaiseEvent(new MatchTimerUpdatedEvent
        {
            RemainingSeconds = remainingTimeSeconds,
            TotalSeconds = TotalTimeSeconds
        });
    }

    void EnsureSpawnSystem()
    {
        if (playerSpawnSystem == null)
            playerSpawnSystem = FindAnyObjectByType<UnitSpawnSystem>();
    }

    void OnGameEnded(GameEndedEvent gameEndedEvent)
    {
        isRunning = false;
    }
}
