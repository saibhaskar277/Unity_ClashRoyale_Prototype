using UnityEngine;

public class Timer
{
    private float duration;
    private float time;

    public bool IsFinished => time <= 0;

    public Timer(float duration)
    {
        this.duration = duration;
        time = duration;
    }

    public void Tick()
    {
        time -= Time.deltaTime;
    }

    public void Reset()
    {
        time = duration;
    }
}


