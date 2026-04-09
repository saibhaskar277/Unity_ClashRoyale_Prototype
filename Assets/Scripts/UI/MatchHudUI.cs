using TMPro;
using UnityEngine;

public class MatchHudUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI elixirMultiplierText;
    
    float remainingSeconds;
    float currentMultiplier = 1f;

    void OnEnable()
    {
        EventManager.AddListner<MatchTimerUpdatedEvent>(OnMatchTimerUpdated);
        EventManager.AddListner<ElixirMultiplierChangedEvent>(OnElixirMultiplierChanged);
        RefreshUI();
    }

    void OnDisable()
    {
        EventManager.RemoveListner<MatchTimerUpdatedEvent>(OnMatchTimerUpdated);
        EventManager.RemoveListner<ElixirMultiplierChangedEvent>(OnElixirMultiplierChanged);
    }

    void Update()
    {
        RefreshUI();
    }

    string FormatTime(float totalSeconds)
    {
        int seconds = Mathf.Max(0, Mathf.CeilToInt(totalSeconds));
        int minutesPart = seconds / 60;
        int secondsPart = seconds % 60;
        return $"{minutesPart:00}:{secondsPart:00}";
    }

    void OnMatchTimerUpdated(MatchTimerUpdatedEvent gameEvent)
    {
        remainingSeconds = gameEvent.RemainingSeconds;
    }

    void OnElixirMultiplierChanged(ElixirMultiplierChangedEvent gameEvent)
    {
        currentMultiplier = gameEvent.Multiplier;
    }

    void RefreshUI()
    {
        if (timerText != null)
            timerText.text = FormatTime(remainingSeconds);

        if (elixirMultiplierText != null)
            elixirMultiplierText.text = $"Elixir x{currentMultiplier:0.#}";
    }
}
