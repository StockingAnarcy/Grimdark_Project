using Cinemachine;
using UnityEngine;

public class PodController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player; // Ссылка на игрока
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    
    [Header("Positioning")]
    [SerializeField] private float sideOffset = 1.0f; // Смещение вбок от игрока (правое плечо)
    [SerializeField] private float heightOffset = 1.5f; // Насколько выше висит Pod
    [SerializeField] private float forwardOffset = 0.5f; // Насколько впереди (по Z) относительно игрока
    [SerializeField] private bool useLeftShoulder = false; // Переключение плеча

    [Header("Movement")]
    [SerializeField] private float followSmoothTime = 0.2f; // Задержка следования
    [SerializeField] private float rotationSpeed = 5f; // Скорость поворота
    [SerializeField] private float hoverAmplitude = 0.15f; // Парение вверх-вниз
    [SerializeField] private float hoverFrequency = 1.5f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private float avoidanceRadius = 0.5f;

    [Header("Targeting")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float loseInterestDistance = 20f;
    [SerializeField] private LayerMask detectionMask;

    [Header("Emission on target")]
    [SerializeField] private Material podMaterial;
    [SerializeField] private Color emissionColor = Color.cyan;
    [SerializeField] private float blinkSpeed = 5f;
    [SerializeField] private float emissionIntensity = 1.5f;
    private Color currentColor = Color.black;
    private AudioSource audioSource;
    private float nextPingTime = 0f;
    private AudioClip currentPingSound;


    private Vector3 velocity; // Для сглаживания движения
    private Transform lookTarget; // Для направления

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;

        podMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (!player) return;

        // Сторона плеча
        Vector3 side = useLeftShoulder ? -player.right : player.right;

        // Целевая позиция — у плеча игрока с небольшим смещением вперед
        Vector3 targetPos = player.position
                            + side * sideOffset
                            + Vector3.up * heightOffset
                            + player.forward * forwardOffset;

        // Добавляем парение
        targetPos.y += Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

        // Обход препятствий (минимальный)
        Vector3 adjustedPos = AvoidObstacles(targetPos);

        // Плавное следование
        transform.position = Vector3.SmoothDamp(transform.position, adjustedPos, ref velocity, followSmoothTime);

        HandleLookDirection();
        UpdateTarget();
        UpdateEmission();
    }

    private void HandleLookDirection()
    {
        Vector3 lookDir;

        if (lookTarget != null)
        {
            lookDir = lookTarget.position - transform.position;
        }
        else
        {
            Transform cam = virtualCamera.transform;
            lookDir = cam.forward;

           
        }

        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }
    }

    private void UpdateTarget()
    {
        if (lookTarget != null)
        {
            float dist = Vector3.Distance(transform.position, lookTarget.position);

            if (dist > loseInterestDistance)
            {
                lookTarget = null;
            }
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionMask);
        ITargetable bestTarget = null;
        float bestScore = float.MinValue;

        foreach (Collider col in colliders)
        {
            ITargetable targetable = col.GetComponentInParent<ITargetable>();
            if (targetable != null)
            {
                float score = targetable.GetPriority(); // Учитывает и дистанцию, и активность
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = targetable;
                }
            }
        }

        if (bestTarget != null)
        {
            lookTarget = bestTarget.GetTargetTransform();
            currentColor = bestTarget.GetHighlightColor();
            currentPingSound = bestTarget.GetPingSound();
        }
    }

    private void UpdateEmission()
    {
        if (podMaterial == null) return;

        float emission = 0f;

        if (lookTarget != null)
        {
            emission = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed)) * emissionIntensity;

            // Пинг-синхронизация
            if (Time.time >= nextPingTime && currentPingSound != null)
            {
                audioSource.PlayOneShot(currentPingSound);
                nextPingTime = Time.time + (Mathf.PI / blinkSpeed); // В такт синусу
            }
        }

        Color finalColor = currentColor * Mathf.LinearToGammaSpace(emission);
        
        podMaterial.SetColor("_EmissionColor", finalColor);
    }

    private Vector3 AvoidObstacles(Vector3 desiredPos)
    {
        Vector3 origin = player.position + Vector3.up * heightOffset;
        Vector3 direction = desiredPos - origin;
        float distance = direction.magnitude;

        // Если на пути стена — смещаем цель ближе к игроку
        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance, obstacleLayers))
        {
            // Смещаем чуть ближе к игроку, чтобы не было впритык
            return hit.point - direction.normalized * 0.2f;
        }

        // Дополнительно: отталкиваемся от пересечений
        Collider[] hits = Physics.OverlapSphere(desiredPos, avoidanceRadius, obstacleLayers);
        Vector3 avoidance = Vector3.zero;

        foreach (var ray in hits)
        {
            Vector3 away = desiredPos - ray.ClosestPoint(desiredPos);
            avoidance += away.normalized * (avoidanceRadius - away.magnitude);
        }

        return desiredPos + avoidance;
    }

    public void StopTargetIng()
    {
        lookTarget = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseInterestDistance);
    }
}
