using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles UI navigation with support for fullscreen + overlay screens.
/// </summary>
public class UIManager : MonoBehaviour
{
    private readonly Dictionary<ScreenID, UIScreenBase> screenMap = new();
    private readonly Stack<ScreenID> history = new();

    private bool isTransitioning;
    private Action pendingTransition;

    public ScreenID CurrentScreen =>
        history.Count > 0 ? history.Peek() : ScreenID.None;

    // ─────────────────────────────────────────────
    private void Awake()
    {
        var screens = GetComponentsInChildren<UIScreenBase>(true);

        EventManager.AddListner<OpenScreenEvent>(OnOpenScreen);
        EventManager.AddListner<OnBackPressEvent>(OnBackPressed);
        EventManager.AddListner<CloseScreenEvent>(OnCloseScreen);

        UIScreenBase defaultScreen = null;

        foreach (var screen in screens)
        {
            if (screen == null) continue;

            screenMap[screen.ScreenId] = screen;
            screen.gameObject.SetActive(false);

            if (screen.IsDefaultScreen)
                defaultScreen = screen;
        }

        if (defaultScreen != null)
            OpenScreen(defaultScreen.ScreenId, false, true);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListner<OpenScreenEvent>(OnOpenScreen);
        EventManager.RemoveListner<OnBackPressEvent>(OnBackPressed);
        EventManager.RemoveListner<CloseScreenEvent>(OnCloseScreen);
    }

    // ─────────────────────────────────────────────
    private void OnOpenScreen(OpenScreenEvent e)
    {
        OpenScreen(e.screenID, e.overlay, e.remember);
    }

    private void OnBackPressed(OnBackPressEvent e)
    {
        Back();
    }

    private void OnCloseScreen(CloseScreenEvent e)
    {
        CloseScreen(e.screenID);
    }

    // ─────────────────────────────────────────────
    public void OpenScreen(
        ScreenID id,
        bool overlay = false,
        bool remember = true,
        Action onComplete = null)
    {
        if (isTransitioning)
        {
            pendingTransition = () => OpenScreen(id, overlay, remember, onComplete);
            return;
        }

        if (!screenMap.TryGetValue(id, out var nextScreen))
        {
            Debug.LogWarning($"Screen not found: {id}");
            return;
        }

        if (nextScreen.IsVisible())
        {
            onComplete?.Invoke();
            return;
        }

        isTransitioning = true;

        UIScreenBase current = null;
        if (history.Count > 0)
            screenMap.TryGetValue(history.Peek(), out current);

        void ShowIncoming()
        {
            if (remember)
                PushToHistory(id);

            nextScreen.Show(() =>
            {
                FinishTransition(onComplete);
            });
        }

        // overlay = keep current open
        if (overlay)
        {
            ShowIncoming();
            return;
        }

        // fullscreen = hide currently visible screens
        var visibleScreens = screenMap.Values
            .Where(s => s.IsVisible())
            .ToList();

        HideScreens(visibleScreens, ShowIncoming);
    }

    // ─────────────────────────────────────────────
    public void Back(Action onComplete = null)
    {
        if (isTransitioning)
        {
            pendingTransition = () => Back(onComplete);
            return;
        }

        if (history.Count <= 1)
            return;

        isTransitioning = true;

        var currentId = history.Pop();
        var previousId = history.Peek();

        var current = screenMap[currentId];
        var previous = screenMap[previousId];

        current.Hide(() =>
        {
            if (previous.IsVisible())
            {
                FinishTransition(onComplete);
            }
            else
            {
                previous.Show(() =>
                {
                    FinishTransition(onComplete);
                });
            }
        });
    }

    // ─────────────────────────────────────────────
    public void CloseScreen(ScreenID id, Action onComplete = null)
    {
        if (!screenMap.TryGetValue(id, out var screen))
            return;

        RemoveFromHistory(id);

        if (!screen.IsVisible())
        {
            onComplete?.Invoke();
            return;
        }

        screen.Hide(onComplete);
    }

    public void CloseCurrent(Action onComplete = null)
    {
        if (history.Count == 0)
            return;

        CloseScreen(history.Peek(), onComplete);
    }

    public void HideAll(Action onComplete = null)
    {
        var visible = screenMap.Values.Where(s => s.IsVisible()).ToList();
        HideScreens(visible, onComplete);
    }

    // ─────────────────────────────────────────────
    public bool IsScreenOpen(ScreenID id)
    {
        return screenMap.TryGetValue(id, out var screen) && screen.IsVisible();
    }

    // ─────────────────────────────────────────────
    private void HideScreens(List<UIScreenBase> screens, Action onComplete)
    {
        if (screens == null || screens.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        int remaining = screens.Count;

        foreach (var screen in screens)
        {
            screen.Hide(() =>
            {
                remaining--;
                if (remaining <= 0)
                    onComplete?.Invoke();
            });
        }
    }

    private void PushToHistory(ScreenID id)
    {
        RemoveFromHistory(id);
        history.Push(id);
    }

    private void RemoveFromHistory(ScreenID id)
    {
        if (!history.Contains(id)) return;

        var temp = new Stack<ScreenID>();

        while (history.Count > 0)
        {
            var top = history.Pop();
            if (top != id)
                temp.Push(top);
        }

        while (temp.Count > 0)
            history.Push(temp.Pop());
    }

    private void FinishTransition(Action onComplete)
    {
        isTransitioning = false;
        onComplete?.Invoke();

        if (pendingTransition != null)
        {
            var next = pendingTransition;
            pendingTransition = null;
            next?.Invoke();
        }
    }
}