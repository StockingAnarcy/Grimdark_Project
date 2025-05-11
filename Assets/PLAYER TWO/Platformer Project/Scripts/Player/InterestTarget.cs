using PLAYERTWO.PlatformerProject;
using UnityEngine;

public enum InterestType
{
    LoreObject,
    Npc,
    Enemy,
    Puzzle,
    PointOfInterest
}

public class InterestTarget : MonoBehaviour, ITargetable
{
    [Header("Base")]
    public InterestType type = InterestType.PointOfInterest;
    public float basePriority = 5f;
    public float distanceWeight = 1f;
    public bool isAvailable = true;

    [Header("Pod Visual/Audio")]
    public Color highlightColor = Color.cyan;
    public AudioClip pingSound;


    public Transform GetTargetTransform() => transform;

    public float GetPriority()
    {
        if (!isAvailable) return -100f; // Низкий приоритет если неактивен

        float distance = Vector3.Distance(Player.Instance.transform.position, transform.position);
        return basePriority / (1 + distance * distanceWeight);
    }
    public Color GetHighlightColor() => highlightColor;
    public AudioClip GetPingSound() => pingSound;
}
