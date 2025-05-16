using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class AttackA2PlayerState : PlayerState
    {
        private bool _hasHit; // Было ли попадание

        protected override void OnEnter(Player player)
        {
            player.combat.RecordAttack();
            player.playerEvents.OnAttack?.Invoke(2); // Уведомляем о атаке

            if (player.TryGetComponent<PlayerAnimator>(out var animator))
            {
                animator.TriggerAttack(2);
            }

            player.inputs.LockMovementDirection();
            // Включаем хитбокс атаки
            //if (player.TryGetComponent<AttackHitbox>(out var hitbox))
            //{
            //    hitbox.Enable();
            //    hitbox.SetDamage(10f); // Урон первой атаки
            //}

            _hasHit = false;
        }

        protected override void OnExit(Player player)
        {
            // Выключаем хитбокс при выходе из состояния
            //if (player.TryGetComponent<AttackHitbox>(out var hitbox))
            //{
            //    hitbox.Disable();
            //}

            // Сбрасываем комбо, если атака не продолжилась
            if (!player.combat.CanContinueCombo())
            {
                player.combat.ResetCombo();
            }
        }

        protected override void OnStep(Player player)
        {
            // Проверка буфера ввода (50%-90% анимации)
            if (player.inputs.GetAttackADown() && timeSinceEntered >= player.stats.current.attackA2 * 0.5f &&
               timeSinceEntered <= player.stats.current.attackA2 * 0.9f)
            {
                player.combat.RegisterAttackInput();

                if (player.combat.HasBufferedInput())
                {
                    player.states.Change<AttackAPlayerState>();
                    return;
                }
            }

            // Завершение атаки
            if (timeSinceEntered >= player.stats.current.attackA2)
            {
                if (player.isGrounded)
                    player.states.Change<IdlePlayerState>();
                else
                    player.states.Change<FallPlayerState>();
            }
        }

        public override void OnContact(Player player, Collider other)
        {
            // Обработка попадания (если не используем хитбоксы)
            //if (!_hasHit && other.CompareTag(GameTags.Enemy))
            //{
            //    _hasHit = true;
            //    if (other.TryGetComponent<Health>(out var health))
            //    {
            //        health.Damage(10); // Урон
            //                           // Эффект отбрасывания
            //        var direction = (other.transform.position - player.position).normalized;
            //        health.Knockback(direction * 5f);
            //    }
            //}
        }
    } 
}
