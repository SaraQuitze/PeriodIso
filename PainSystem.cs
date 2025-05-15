using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class PainSystem : MonoBehaviour
{
    // Configuraci�n editable en el Inspector
    [Header("Configuraci�n")]
    public float painDuration = 1800f; // 30 minutos en segundos
    public float recoveryInterval = 300f; // 5 minutos en segundos

    // Referencia al PlayerStats (asignado en el Inspector o buscado autom�ticamente)
    [Header("Referencias")]
    [SerializeField] private PlayerStats playerStats;

    private float painTimer;
    private bool isPainActive;

    private void Awake()
    {
        // Busca autom�ticamente PlayerStats si no est� asignado
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("�No se encontr� PlayerStats en la escena!");
            }
        }
    }

    private void Update()
    {
        if (isPainActive)
        {
            painTimer -= Time.deltaTime;
            if (painTimer <= 0)
            {
                playerStats.TakeDamage(1); // Usamos la referencia directa
                painTimer = painDuration;
            }
        }
    }

    public void TakePainkiller()
    {
        isPainActive = false;
        StartCoroutine(RecoverHealth());
    }

    private System.Collections.IEnumerator RecoverHealth()
    {
        while (playerStats.currentHearts < playerStats.maxHearts)
        {
            yield return new WaitForSeconds(recoveryInterval);
            playerStats.currentHearts = Mathf.Min(playerStats.currentHearts + 1, playerStats.maxHearts);
        }
    }

    // M�todo para activar el dolor (llamar desde otros sistemas)
    public void ActivatePain()
    {
        isPainActive = true;
        painTimer = painDuration;
    }
}
