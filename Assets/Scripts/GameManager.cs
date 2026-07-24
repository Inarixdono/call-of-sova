using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private string levelSelectorSceneName = "00SelectorNiveles";
    [SerializeField] private float endGameDelay = 3f;
    [SerializeField] private int shotsPerObjective = 1;

    private int score;
    private int currentLives = -1;
    private bool levelEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        RefreshScore();
    }

    public int ShotsPerObjective { get { return Mathf.Max(1, shotsPerObjective); } }

    public int GetRequiredShots(int totalObjectives)
    {
        return Mathf.Max(1, totalObjectives * ShotsPerObjective);
    }

    public void AddScore(int amount, string partName)
    {
        score += amount;
        RefreshScore();

        if (amount > 0)
        {
            ShowMessage("+" + amount + " " + partName);
        }
    }

public void ShowMessage(string text)
    {
        if (HUDController.Instance != null)
        {
            HUDController.Instance.ShowMessage(text);
        }
    }

    public void ShowLifeLost(string reason)
    {
        if (HUDController.Instance != null)
        {
            HUDController.Instance.ShowLifeLost(reason, Mathf.Max(currentLives, 0));
        }
    }

    public void SetAmmo(int current, int max)
    {
        if (HUDController.Instance != null)
        {
            HUDController.Instance.SetAmmo(current, max);
        }
    }

    public void SetLives(int lives)
    {
        currentLives = lives;

        if (HUDController.Instance != null)
        {
            HUDController.Instance.SetLives(lives);
        }
    }

    public void RestartScene()
    {
        CleanupBeforeSceneChange();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

public void EndGame(bool won)
    {
        if (levelEnded)
        {
            return;
        }

        levelEnded = true;
        PlayerHealth.ResetLives();

        if (HUDController.Instance != null)
        {
            if (won)
            {
                HUDController.Instance.ShowVictory(score, Mathf.Max(currentLives, 0));
            }
            else
            {
                HUDController.Instance.ShowGameOver(score);
            }
        }

        Invoke(nameof(GoToLevelSelector), endGameDelay);
    }

    private void CleanupBeforeSceneChange()
    {
        ZombieHealth[] enemies = FindObjectsOfType<ZombieHealth>();
        foreach (ZombieHealth enemy in enemies)
        {
            if (enemy != null)
            {
                DestroyImmediate(enemy.gameObject);
            }
        }
    }

    private void GoToLevelSelector()
    {
        CleanupBeforeSceneChange();
        SceneManager.LoadScene(levelSelectorSceneName);
    }

    private void RefreshScore()
    {
        if (HUDController.Instance != null)
        {
            HUDController.Instance.SetScore(score);
        }
    }
}