using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if CINEMACHINE
using Unity.Cinemachine; // Para Unity 6
#endif

public class CameraPlayer : MonoBehaviour
{
    [Header("Cinemachine settings")]
#if CINEMACHINE
    [SerializeField]
    private CinemachineCamera virtualCam;
    private CinemachineImpulseSource impulseSource;
#endif

    public static CameraPlayer Instance { get; private set; } // Singleton

    [Header("Configuration")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float minX, maxX, minY, maxY;//limites del mapa
    [SerializeField] private float defaultShakeIntensity = 0.5f;
    [SerializeField] private bool usingCinemachine;
    [SerializeField] private Transform target; //asignar layer manualmente desde el inspector

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;
    private bool isAngryState = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            originalPosition = transform.localPosition;

//#if CINEMACHINE
            // Mueve la inicialización de virtualCam aquí
            virtualCam = GetComponent<CinemachineCamera>();
            if (virtualCam == null)
            {
                // Busca en todo el GameObject si no está en el mismo
                virtualCam = FindAnyObjectByType<CinemachineCamera>();
                if (virtualCam == null)
                {
                    Debug.LogError("CinemachineCamera no encontrado. Asegúrate de:");
                    Debug.LogError("1. Tener un GameObject con CinemachineCamera");
                    Debug.LogError("2. Tener el paquete Cinemachine instalado");
                }
            }
//#endif

            InitializeCamera();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeCamera()
    {

//#if CINEMACHINE
        impulseSource = GetComponent<CinemachineImpulseSource>();

        if (usingCinemachine)
        {
            virtualCam = GetComponent<CinemachineCamera>();

            if (virtualCam != null)
            {
                if (impulseSource == null)
                {
                    Debug.LogError("No se encontró el componente CinemachineImpulseSource");
                }
            }
        }
//#endif
    }

    private void VerifyCameraSetup()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera no encontrada. Asegúrate de:");
            Debug.LogError("1. Tener una cámara con tag 'MainCamera'");
            Debug.LogError("2. La cámara está activa");
            return;
        }

//#if CINEMACHINE
        if (usingCinemachine)
        {
            if (virtualCam == null)
            {
                Debug.LogError("Virtual Camera no asignada");
                return;
            }

            // Forzar actualización de la cámara
            StartCoroutine(ForceCameraUpdate());
        }
//#endif
    }

    private IEnumerator ForceCameraUpdate()
    {
        yield return new WaitForEndOfFrame();

        if (virtualCam != null && target != null)
        {
            virtualCam.Follow = target;
            virtualCam.LookAt = target;
            virtualCam.gameObject.SetActive(false);
            virtualCam.gameObject.SetActive(true);
        }

        if (Camera.main != null)
        {
            Camera.main.gameObject.SetActive(false);
            Camera.main.gameObject.SetActive(true);
        }
    }
    private void Start()
    {
        //asegurarse que la main camera está activa
        if (Camera.main == null)
        {
            VerifyCameraSetup();
            enabled = false;//desactiva el script
            return;
        }

        // Asignación robusta del target
        TryAssignPlayerTarget();

        // Configuración Cinemachine
#if CINEMACHINE
        if (usingCinemachine)
        {
            InitializeImpulseSource();
            StartCoroutine(DelayedCameraSetup());
        }
#endif
    }

    private IEnumerator DelayedCameraSetup()
    {
        // Espera un frame para asegurar que todo se ha inicializado
        yield return null;

        TryAssignVirtualCamera();
    }

    private void TryAssignPlayerTarget()
    {
        if (target == null)
        {
            if (PlayerController.Instance != null)
            {
                target = PlayerController.Instance.transform;
                Debug.Log("Target asignado desde PlayerController: " + target.name);
            }
            else
            {
                Debug.LogWarning("PlayerController.Instance no encontrado, intentando FindObjectOfType...");
                var player = FindFirstObjectByType<PlayerController>();
                if (player != null)
                {
                    target = player.transform;
                    Debug.Log("Target asignado mediante FindObjectOfType: " + target.name);
                }
                else
                {
                    Debug.LogError("No se encontró ningún PlayerController en la escena");
                    enabled = false;
                }
            }
        }
    }

    private void TryAssignVirtualCamera()
    {
//#if CINEMACHINE
        if (virtualCam == null)
        {
            virtualCam = GetComponent<CinemachineCamera>();
            if (virtualCam == null)
            {
                Debug.LogError("No se encontró CinemachineCamera en el GameObject");
                enabled = false;
                return;
            }
        }

        if (target != null && virtualCam != null)
        {
            virtualCam.Follow = target;
            virtualCam.LookAt = target;
            Debug.Log($"Cinemachine configurado correctamente. Follow: {target.name}");

            // Verificación adicional
            if (virtualCam.Follow == null)
            {
                Debug.LogError("¡Advertencia! virtualCam.Follow sigue siendo null después de la asignación");
            }
        }
        else
        {
            Debug.LogError($"No se pudo configurar Cinemachine. Target: {target}, VirtualCam: {virtualCam}");
        }
//#endif
    }

    private void LateUpdate()
    {
        // Fuerza el seguimiento si Cinemachine falla
        if (virtualCam != null && virtualCam.Follow != null &&
            Vector3.Distance(virtualCam.transform.position, virtualCam.Follow.position) > 2f)
        {
            virtualCam.ForceCameraPosition(virtualCam.Follow.position, Quaternion.identity);
        }
    }


    //sacudida de cámara

//#if CINEMACHINE
    private void InitializeImpulseSource()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (impulseSource == null)
        {
            impulseSource = gameObject.AddComponent<CinemachineImpulseSource>();
            Debug.Log("CinemachineImpulseSource añadido automáticamente");
        }

        // Configuración básica del impulso
        impulseSource.ImpulseDefinition.TimeEnvelope = new CinemachineImpulseManager.EnvelopeDefinition
        {
            AttackTime = 0.1f,
            SustainTime = 0.5f,
            DecayTime = 0.3f
        };
    }
//#endif

    public void TriggerShake(float magnitude, bool isAngry = true)
    {
        if (!isAngry)
        {
            StopShake();
            return;
        }

/*#if CINEMACHINE
        if (impulseSource == null)
        {
            impulseSource = gameObject.AddComponent<CinemachineImpulseSource>();
        }

        // Configuración dinámica
        impulseSource.ImpulseDefinition.AmplitudeGain = magnitude;
        impulseSource.ImpulseDefinition.FrequencyGain = magnitude;
        impulseSource.GenerateImpulse();
#else*/
    // Shake manual alternativo
    if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
    shakeCoroutine = StartCoroutine(manualShake(0.5f, magnitude));
//#endif
    }

    public IEnumerator manualShake(float duration, float magnitude)
     {
         //fallback a manual shake si cinemachine no funciona
         Vector3 originalPos = transform.localPosition;
         float elapsed = 0f;

         while (elapsed < duration)
         {
             Vector3 shakeOffset = Random.insideUnitSphere * magnitude;
             transform.localPosition = originalPos + shakeOffset;
             elapsed += Time.deltaTime;
             yield return null;
         }

         transform.localPosition = originalPos;
     }

     public void StopShake()
     {
         if (shakeCoroutine != null)
         {
             StopCoroutine(shakeCoroutine);
             shakeCoroutine = null;

             transform.localPosition = originalPosition;
         }
     }
}
