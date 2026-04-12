using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : UIScreenBase
{
    public Button menuButton;


    public override void RegisterEvents()
    {
        menuButton.onClick.AddListener(Open);
    }

    void Open()
    {
        EventManager.RaiseEvent(new OpenScreenEvent(ScreenID.MainMenu, false));
    }




    public override void UnRegisterEvents()
    {
        menuButton.onClick.RemoveListener(Open);

    }

    // Optional: Override animations if using DoTween or similar
    // protected override void OnShowBegin(Action onComplete) { ... }
    // protected override void OnHideBegin(Action onComplete) { ... }
}