using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class PlayerIK : MonoBehaviour
    {
        [Header("IK Settings")]
        public Transform leftHandTarget;  // Цель для левой руки
        public Transform rightHandTarget; // Цель для правой руки
        public float ikWeight = 1f;       // Сила влияния IK

        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null) return;
            if (layerIndex != 1) return;

            // Применяем IK для рук, только если цели заданы
            if (leftHandTarget != null)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
                _animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                _animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }

            if (rightHandTarget != null)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);
                _animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }
        }
    }
}