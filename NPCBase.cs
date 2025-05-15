using UnityEngine;

public abstract class NPCBase : MonoBehaviour
{
    //public NPCType npcType;
    //public Dialogue[] dialogues;

    [Tooltip("Tiempo entre interacciones")]
    public float interactionCooldown = 1f;
    private float lastInteractionTime;

    public virtual void Interact()
    {
        // Verifica cooldown
        if (Time.time - lastInteractionTime < interactionCooldown) return;

        // Verifica singleton y di�logos
        if (DialogueSystem.Instance == null)
        {
            Debug.LogError("Sistema de di�logo no disponible");
            return;
        }

        /*if (dialogues == null || dialogues.Length == 0)
        {
            Debug.LogWarning($"No hay di�logos asignados a {gameObject.name}");
            return;
        }

        DialogueSystem.Instance.StartDialogue(dialogues);
        lastInteractionTime = Time.time;*/
    }

}

public class BossNPC : NPCBase
{
    private int instructionsGiven;

}