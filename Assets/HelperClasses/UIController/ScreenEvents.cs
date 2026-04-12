// ── Screen Events ────────────────────────────────────────────────────────────

public class OpenScreenEvent : IGameEvent
{
    public ScreenID screenID;
    public bool     overlay;
    public bool     remember;

    public OpenScreenEvent(ScreenID screenID, bool overlay = false, bool remember = true)
    {
        this.screenID = screenID;
        this.overlay  = overlay;
        this.remember = remember;
    }
}

public class CloseScreenEvent : IGameEvent
{
    public ScreenID screenID;

    public CloseScreenEvent(ScreenID screenID)
    {
        this.screenID = screenID;
    }
}

public class ScreenOpenedEvent : IGameEvent
{
    public ScreenID screenID;
    public ScreenOpenedEvent(ScreenID id) => screenID = id;
}

public class ScreenClosedEvent : IGameEvent
{
    public ScreenID screenID;
    public ScreenClosedEvent(ScreenID id) => screenID = id;
}

public class OnBackPressEvent : IGameEvent { }
