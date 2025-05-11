using UnityEngine;

public interface ITargetable
{
    Transform GetTargetTransform();
    float GetPriority(); // ��� ���� � ��� ������
    Color GetHighlightColor(); // ���� ������� � ����������� �� ����
    AudioClip GetPingSound();
}
