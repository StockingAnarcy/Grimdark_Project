using UnityEngine;

public interface ITargetable
{
    Transform GetTargetTransform();
    float GetPriority(); // „ем выше Ч тем важнее
    Color GetHighlightColor(); // ÷вет эмиссии в зависимости от цели
    AudioClip GetPingSound();
}
