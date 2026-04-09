using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject rootPanel;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] string winTitle = "You Win";
    [SerializeField] string loseTitle = "You Lose";

    void Awake()
    {
        Hide();
    }

    void OnEnable()
    {
        EventManager.AddListner<GameEndedEvent>(OnGameEnded);
    }

    void OnDisable()
    {
        EventManager.RemoveListner<GameEndedEvent>(OnGameEnded);
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

    void OnGameEnded(GameEndedEvent gameEvent)
    {
        Show(gameEvent.PlayerWon ? winTitle : loseTitle);
    }
}
