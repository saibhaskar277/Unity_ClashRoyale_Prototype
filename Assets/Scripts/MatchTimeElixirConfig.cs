using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Match Time Elixir Config")]
public class MatchTimeElixirConfig : ScriptableObject
{
    [Header("Match Duration")]
    [Min(1f)] public float TotalMatchTimeSeconds = 180f;

    [Header("Elixir Multipliers By Remaining Time")]
    [Tooltip("Applied when remaining time is <= RemainingTimeSeconds. Add in descending order.")]
    public List<ElixirMultiplierStage> ElixirStages = new List<ElixirMultiplierStage>
    {
        new ElixirMultiplierStage { RemainingTimeSeconds = 120f, Multiplier = 2f },
        new ElixirMultiplierStage { RemainingTimeSeconds = 60f, Multiplier = 3f }
    };
}

[Serializable]
public struct ElixirMultiplierStage
{
    [Min(0f)] public float RemainingTimeSeconds;
    [Min(0f)] public float Multiplier;
}
