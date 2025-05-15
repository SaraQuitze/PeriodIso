using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerController player;
    public TaskSystem taskSystem;
    public MoodSystem moodSystem;
    public UIManager uiManager;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartNewDay()
    {
        /*resetear variables diarias*/
    }

    public void EndDay()
    {
        /*Evaluar variables diarias*/
    }
}
