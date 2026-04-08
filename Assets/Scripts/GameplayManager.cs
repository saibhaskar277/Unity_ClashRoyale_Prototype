using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Main Towers")]
    [SerializeField] Tower playerMainTower;
    [SerializeField] Tower enemyMainTower;

    [Header("UI")]
    [SerializeField] GameOverUI gameOverUI;
    [SerializeField] string winTitle = "You Win";
    [SerializeField] string loseTitle = "You Lose";

    [Header("Flow")]
    [SerializeField] bool pauseOnGameOver = true;

    bool isGameOver;

    void Start()
    {
        isGameOver = false;
        if (pauseOnGameOver)
        {
            Time.timeScale = 1f;
        }

        if (gameOverUI != null)
        {
            gameOverUI.Hide();
        }
    }

    void Update()
    {
        if (isGameOver)
            return;

        if (playerMainTower == null || enemyMainTower == null)
            return;

        if (!playerMainTower.gameObject.activeInHierarchy)
        {
            EndGame(false);
            return;
        }

        if (!enemyMainTower.gameObject.activeInHierarchy)
        {
            EndGame(true);
        }
    }

    public void EndGame(bool playerWon)
    {
        if (isGameOver)
            return;

        isGameOver = true;

        if (gameOverUI != null)
        {
            gameOverUI.Show(playerWon ? winTitle : loseTitle);
        }

        if (pauseOnGameOver)
        {
            Time.timeScale = 0f;
        }
    }
}
