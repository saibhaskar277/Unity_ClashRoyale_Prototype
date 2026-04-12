using System;
using UnityEngine;
using UnityEngine.UI;

public class Menu : UIScreenBase
{
    public Button settingsButton;


    public override void RegisterEvents()
    {
        settingsButton.onClick.AddListener(open);
    }

    void open()
    {
        EventManager.RaiseEvent(new OpenScreenEvent(ScreenID.Settings, true));
    }

    public override void UnRegisterEvents()
    {
        settingsButton.onClick.RemoveListener(open);

    }
}