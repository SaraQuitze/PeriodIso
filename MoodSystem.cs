using System;
using System.Collections.Generic;
using UnityEngine;

public enum MoodState {Normal, Tired, Angry, Uncomfortable, Sad}

public class MoodSystem : MonoBehaviour
{
    public MoodState currentMood;
    private Dictionary<MoodState, Action> moodEffects;

    private void Awake()
    {
        InitializeEffects();
    }

    private void InitializeEffects()
    {
        moodEffects = new Dictionary<MoodState, Action> {
            {MoodState.Tired, ApplyTiredEffect},
            {MoodState.Normal, ResetEffects},
            {MoodState.Angry, ApplyAngryEffect},
            {MoodState.Uncomfortable, ApplyUncomfortableEffect},
            {MoodState.Sad, ApplySadEffect}
        };
    }

    private void ApplyTiredEffect()
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogWarning("PlayerController.Instance no está disponible");
            return;
        }

            PlayerController.Instance.moveSpeed = Mathf.Max(
            PlayerController.Instance.moveSpeed - 3f, 1f // Velocidad mínima para que no quede en 0
            );
    }

    private void ApplyAngryEffect()
    {
        // Implementa el efecto de ira
    }

    private void ApplyUncomfortableEffect()
    {
        // Implementa el efecto de incomodidad
    }

    private void ApplySadEffect()
    {
        // Implementa el efecto de tristeza
    }

    private void ResetEffects()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.moveSpeed = 5.0f;
        }
    }

    public void ChangeMood(MoodState newMood)
    {
        // Primero: Limpiar efectos del estado anterior
        // Luego: Aplicar nuevo estado
        if (moodEffects.TryGetValue(newMood, out Action effect))
        {
            effect?.Invoke();
            currentMood = newMood;
        }
    }
}
