using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{ 
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Combo Settings")]
        public float comboWindow = 0.5f; // Время между атаками для комбо
        public int maxComboSteps = 3; // Максимальное количество атак в комбо

        private float _lastAttackTime;
        private bool _inputBuffered;
        private int _currentCombo;

        public int CurrentCombo => _currentCombo;

        public void RegisterAttackInput()
        {
            _inputBuffered = true;
        }

        public bool HasBufferedInput()
        {
            return _inputBuffered;
        }

        public bool CanContinueCombo()
        {
            return Time.time - _lastAttackTime <= comboWindow &&
                   _currentCombo < maxComboSteps;
        }

        public void RecordAttack()
        {
            _lastAttackTime = Time.time;
            _inputBuffered = false;

            if (!CanContinueCombo())
            {
                _currentCombo = 0;
            }

            _currentCombo++;
        }

        public void ResetCombo()
        {
            _currentCombo = 0;
            _inputBuffered = false;
        }
    
    }
}
