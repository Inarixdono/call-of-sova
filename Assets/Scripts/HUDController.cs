using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [SerializeField] private Text scoreText;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text objectivesText;
    [SerializeField] private Text messageText;
    [SerializeField] private Text endGameText;
    [SerializeField] private float messageDuration = 0.8f;

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
        if (messageText != null)
        {
            messageText.text = string.Empty;
        }

        if (endGameText != null)
        {
            endGameText.text = string.Empty;
        }
    }

    public void SetScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntuacion: " + score;
        }
    }

    public void SetAmmo(int current, int max)
    {
        if (ammoText != null)
        {
            ammoText.text = current + " / " + max;
        }
    }

    public void SetLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "Vidas: " + lives;
        }
    }

private static readonly Color VictoryColor = new Color(0.35f, 0.9f, 0.4f);
    private static readonly Color GameOverColor = new Color(0.95f, 0.25f, 0.25f);
    private static readonly Color LifeLostColor = new Color(1f, 0.75f, 0.15f);

    public void ShowVictory(int score, int lives)
    {
        if (endGameText != null)
        {
            endGameText.color = VictoryColor;
            endGameText.text = "Nivel superado\nPuntuacion: " + score + "\nVidas: " + lives;
        }
    }

    public void ShowGameOver(int score)
    {
        if (endGameText != null)
        {
            endGameText.color = GameOverColor;
            endGameText.text = "Game Over\nPuntuacion: " + score;
        }
    }

    public void ShowLifeLost(string reason, int lives)
    {
        if (endGameText != null)
        {
            endGameText.color = LifeLostColor;
            endGameText.text = reason + "\nVidas restantes: " + lives;
        }
    }

    public void SetObjectives(int remaining, int total)
    {
        if (objectivesText != null)
        {
            objectivesText.text = "Objetivos: " + remaining + " / " + total;
        }
    }

    public void ShowMessage(string text)
    {
        if (messageText == null)
        {
            return;
        }

        messageText.text = text;

        CancelInvoke(nameof(ClearMessage));
        if (!string.IsNullOrEmpty(text))
        {
            Invoke(nameof(ClearMessage), messageDuration);
        }
    }

    private void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = string.Empty;
        }
    }
}
