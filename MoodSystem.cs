using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoodState {Normal, Tired, Angry, Uncomfortable, Sad}

public class MoodSystem : MonoBehaviour
{
    [Header("Configuraiton")]
    [SerializeField] private float sadBlackoutDuration = 5f;
    [SerializeField] private float angryShakeIntensity =  0.1f;
    [SerializeField] private float moodTransitionSpeed = 2f;

    [Header("Reference")]
    [SerializeField] private Animator playerAnimation;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Material screenEffectMaterial;

    [Header("Prueba Temporal")]
    [SerializeField] private bool enableTestMode = true;
    [SerializeField] private float initialDelay = 10f;
    [SerializeField] private float moodDuration = 20f;

    public MoodState currentMood;
    private Vector3 originalScale;
    private float originalSpeed;

    private void Awake()
    {
        originalScale = playerTransform.localScale;
        if (PlayerController.Instance != null)
        {
            originalSpeed = PlayerController.Instance.baseMoveSpeed;
        }
    }
    private void Start()
    {        
        if(enableTestMode)
        {
            StartCoroutine(TestMoodSequence());
        }
    }

    private void Update()
    {
        if (currentMood != MoodState.Angry && CameraPlayer.Instance != null)
        {
            CameraPlayer.Instance.StopShake(); // Paranoia 
        }
    }

    private IEnumerator TestMoodSequence()
    {
        yield return new WaitForSeconds(initialDelay);

        //secuencia c�clica
        while(true)
        {
            ChangeMood(MoodState.Tired);
            Debug.Log("Estado activado: Tired");
            yield return new WaitForSeconds(moodDuration);

            ChangeMood(MoodState.Angry);
            Debug.Log("Estado activado: Angry");
            yield return new WaitForSeconds(moodDuration);

            ChangeMood(MoodState.Uncomfortable);
            Debug.Log("Estado activado: Uncomfortable");
            yield return new WaitForSeconds(moodDuration);

            ChangeMood(MoodState.Sad);
            Debug.Log("Estado activado: Sad");
            yield return new WaitForSeconds(moodDuration);

            // Volver a normal antes de repetir
            ChangeMood(MoodState.Normal);
            Debug.Log("Estado: Normal");
            yield return new WaitForSeconds(5f);
        }
    }
    
    public void ChangeMood(MoodState newMood)
    {
        if (currentMood == newMood) return;

        //quita efectos anteriores
        ResetEffects();

        switch(newMood)
        {
            case MoodState.Tired:
                ApplyTiredEffect();
                break;
            case MoodState.Angry:
                ApplyAngryEffect();
                break;
            case MoodState.Uncomfortable:
                ApplyUncomfortableEffect();
                break;
            case MoodState.Sad:
                ApplySadEffect();
                break;
            default:
                ResetEffects();
                break;
        }
        currentMood = newMood;
    }

    private void ApplyTiredEffect()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.moveSpeed = PlayerController.Instance.baseMoveSpeed * 0.5f;
        }

        if (playerAnimation != null)
        {
            playerAnimation.Play("caminarTired");
        }
    }

    private void ApplyAngryEffect()
    {
        //efecto pantalla roja
        StartCoroutine(ApplyScreenEffect(Color.red, 0.8f));

        // Obtener la c�mara de forma segura
        if (CameraPlayer.Instance != null)
        {
            //efecto shake de c�mara
            CameraPlayer.Instance.TriggerShake(angryShakeIntensity, true);
        }
        else
        {
            Debug.LogWarning("No se encontro CameraPlayer en la escena");
        }


        if (playerAnimation != null)
        {
            playerAnimation.Play("caminarAngry");
        }
    }

    private void ApplyUncomfortableEffect()
    {
        //forma redonda
        playerTransform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        //reducir velocidad
        if (PlayerController.Instance != null)
        {
            //reducir velocidad
            originalSpeed = PlayerController.Instance.GetBaseSpeed(); // Actualiza la velocidad base
            PlayerController.Instance.SetTemporarySpeed(originalSpeed * 0.3f);
        }
    }

    private void ApplySadEffect()
    {
        StartCoroutine(SadEffectSequence());
    }

    private IEnumerator SadEffectSequence()
    {
        //Efecto Gris
        yield return StartCoroutine(ApplyScreenEffect(Color.gray, 0.8f));

        /*Transicion a negro y de vuelta
        float timer = 0f;
        while (timer < sadBlackoutDuration)
        {
            float lerpValue = Mathf.PingPong(timer, sadBlackoutDuration / 2) / (sadBlackoutDuration / 2);
            Color targetColor = Color.Lerp(Color.gray, Color.black, lerpValue);
            screenEffectMaterial.color = targetColor;

            timer += Time.deltaTime;
            yield return null;
        }*/

        //reducir velocidad
        if (PlayerController.Instance != null)
        {
            //reducir velocidad
            originalSpeed = PlayerController.Instance.GetBaseSpeed(); // Actualiza la velocidad base
            PlayerController.Instance.SetTemporarySpeed(originalSpeed * 0.5f);
        }
    }

    private IEnumerator ApplyScreenEffect(Color targetColor, float intensity)
    {
        Color startColor = screenEffectMaterial.color;
        float t = 0f;

        while (t < 1f)
        {
            screenEffectMaterial.color = Color.Lerp(startColor, targetColor * intensity, t);
            t += Time.deltaTime * moodTransitionSpeed;
            yield return null;
        }
    }

    private void ResetEffects()
    {
        //restaurar valores originales
        if (PlayerController.Instance != null)
        {
            originalSpeed = PlayerController.Instance.GetBaseSpeed(); // Actualiza antes de resetear
            PlayerController.Instance.ResetSpeed();

            // Fuerza la detenci�n del movimiento
            var rb = PlayerController.Instance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        if (playerTransform != null)
        {
            playerTransform.localScale = originalScale;
        }
        if (screenEffectMaterial != null)
        {
            screenEffectMaterial.color = Color.clear;
        }


        // Detener sacudida
        if (CameraPlayer.Instance != null)
        {
            CameraPlayer.Instance.TriggerShake(0, false);
            //CameraPlayer.Instance.transform.localPosition = new Vector3(0, 0, CameraPlayer.Instance.transform.localPosition.z);
        }

        //Animacion normal
        if (playerAnimation != null)
        {
            playerAnimation.Play("Caminar");
        }
    }
}
