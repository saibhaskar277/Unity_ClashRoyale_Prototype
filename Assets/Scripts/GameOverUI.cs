using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject rootPanel;
    [SerializeField] TextMeshProUGUI titleText;

    void Awake()
    {
        Hide();
    }

    public void Show(string title)
    {
        if (rootPanel != null)
        {
            rootPanel.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }

        if (titleText != null)
        {
            titleText.text = title;
        }
    }

    public void Hide()
    {
        if (titleText != null)
        {
            titleText.text = string.Empty;
        }

        if (rootPanel != null)
        {
            rootPanel.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
