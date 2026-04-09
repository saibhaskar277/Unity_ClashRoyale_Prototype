using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Flow")]
    [SerializeField] bool pauseOnGameOver = true;

    bool isGameOver;
    Tower[] cachedTowers;

    void Awake()
    {
        CacheTowers();
    }

    void OnEnable()
    {
        EventManager.AddListner<TowerDestroyedEvent>(OnTowerDestroyed);
    }

    void OnDisable()
    {
        EventManager.RemoveListner<TowerDestroyedEvent>(OnTowerDestroyed);
    }

    void Start()
    {
        isGameOver = false;
        if (pauseOnGameOver)
            Time.timeScale = 1f;
    }

    void Update()
    {
        if (isGameOver)
            return;

        // Safety polling to cover cases where an object gets disabled without death flow.
        ValidateMainTowersState();
    }

    public void EndGame(bool playerWon, string reason)
    {
        if (isGameOver)
            return;

        isGameOver = true;

        EventManager.RaiseEvent(new GameEndedEvent
        {
            PlayerWon = playerWon,
            Reason = reason
        });

        if (pauseOnGameOver)
            Time.timeScale = 0f;
    }

    void OnTowerDestroyed(TowerDestroyedEvent gameEvent)
    {
        if (isGameOver || !gameEvent.IsMainTower)
            return;

        bool playerWon = gameEvent.Team != UnitTeam.PlayerTeam;
        EndGame(playerWon, "Main Tower Destroyed");
    }

    void ValidateMainTowersState()
    {
        if (cachedTowers == null || cachedTowers.Length == 0)
            CacheTowers();

        for (int i = 0; i < cachedTowers.Length; i++)
        {
            Tower tower = cachedTowers[i];
            if (tower == null || !tower.IsMainTower)
                continue;

            if (!tower.gameObject.activeInHierarchy)
            {
                bool playerWon = tower.Team != UnitTeam.PlayerTeam;
                EndGame(playerWon, "Main Tower Disabled");
                return;
            }
        }
    }

    void CacheTowers()
    {
        cachedTowers = FindObjectsByType<Tower>(FindObjectsSortMode.None);
    }
}
