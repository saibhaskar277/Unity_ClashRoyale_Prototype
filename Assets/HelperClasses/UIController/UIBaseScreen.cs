using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base reusable screen class.
/// Supports animation lifecycle and event registration.
/// </summary>
public abstract class UIScreenBase : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private ScreenID screenId;

    [Header("Behaviour")]
    [SerializeField] private bool isDefaultScreen;
    [SerializeField] private bool allowOverlay;

    [Header("Navigation")]
    [SerializeField] private Button backButton;

    public ScreenID ScreenId => screenId;
    public bool IsDefaultScreen => isDefaultScreen;
    public bool AllowOverlay => allowOverlay;
    public bool IsAnimating { get; private set; }

    private bool eventsRegistered;

    // ─────────────────────────────────────────────
    protected virtual void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(() =>
                EventManager.RaiseEvent(new OnBackPressEvent()));
    }

    // ─────────────────────────────────────────────
    public void Show(Action onComplete = null)
    {
        gameObject.SetActive(true);
        IsAnimating = true;

        OnShowBegin(() =>
        {
            IsAnimating = false;

            if (!eventsRegistered)
            {
                RegisterEvents();
                eventsRegistered = true;
            }

            OnScreenOpened();
            EventManager.RaiseEvent(new ScreenOpenedEvent(screenId));
            onComplete?.Invoke();
        });
    }

    protected virtual void OnShowBegin(Action onComplete)
    {
        onComplete?.Invoke();
    }

    // ─────────────────────────────────────────────
    public void Hide(Action onComplete = null)
    {
        if (eventsRegistered)
        {
            UnRegisterEvents();
            eventsRegistered = false;
        }

        IsAnimating = true;

        OnHideBegin(() =>
        {
            IsAnimating = false;
            OnScreenClosed();
            gameObject.SetActive(false);

            EventManager.RaiseEvent(new ScreenClosedEvent(screenId));
            onComplete?.Invoke();
        });
    }

    protected virtual void OnHideBegin(Action onComplete)
    {
        onComplete?.Invoke();
    }

    // ─────────────────────────────────────────────
    public bool IsVisible() => gameObject.activeSelf;

    public virtual void RegisterEvents() { }
    public virtual void UnRegisterEvents() { }

    protected virtual void OnScreenOpened() { }
    protected virtual void OnScreenClosed() { }
}

public enum ScreenID
{
    None,
    MainMenu,
    Inventory,
    Settings,
    Pause,
    Shop,
    Popup
}