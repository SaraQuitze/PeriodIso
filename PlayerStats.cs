using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Health")]
    public int maxHearts = 5;
    public int currentHearts;

    private void Awake()
    {
        // Implementación Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: si quieres que persista entre escenas
        }
        else
        {
            Destroy(gameObject);
        }

        currentHearts = maxHearts;
    }

    public void TakeDamage(int amount)
    {
        currentHearts = Mathf.Clamp(currentHearts - amount, 0, maxHearts);
        if (currentHearts <= 0) GameOver();
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        // Tu lógica de reinicio aquí
    }
}