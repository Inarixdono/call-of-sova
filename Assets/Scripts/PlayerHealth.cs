using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int startingLives = 3;
    [SerializeField] private float respawnDelay = 1.5f;

    private static int lives = -1;

    private bool isDead;

    private void Start()
    {
        if (lives < 0)
        {
            lives = startingLives;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetLives(lives);
        }
    }

    public void Kill()
    {
        LoseLife("Te atraparon");
    }

public void LoseLife(string reason)
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        DisableControl();
        lives--;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetLives(Mathf.Max(lives, 0));
        }

        if (lives <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndGame(false);
            }

            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowLifeLost(reason);
        }

        Invoke(nameof(Respawn), respawnDelay);
    }

    private void DisableControl()
    {
        PlayerShooting shooting = GetComponent<PlayerShooting>();
        if (shooting != null)
        {
            shooting.enabled = false;
        }

        FirstPersonController movement = GetComponent<FirstPersonController>();
        if (movement != null)
        {
            movement.enabled = false;
        }
    }

    private void Respawn()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartScene();
        }
    }

    public static void ResetLives()
    {
        lives = -1;
    }
}