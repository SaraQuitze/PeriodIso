using UnityEngine;

public class DialogueSystem : MonoBehaviour
{

    private static DialogueSystem _instance;
    public static DialogueSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindFirstObjectByType<DialogueSystem>();

                if (_instance == null)
                {
                    Debug.LogError("No existe DialogueSystem en la escena");

                    GameObject go = new GameObject("DialogueSystem (Autocreated)");
                    _instance = go.AddComponent<DialogueSystem>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
    }

    /*public void StartDialogue(Dialogue[] dialogues)
    {
        Debug.Log("Diálogo iniciado");
    }*/
}
