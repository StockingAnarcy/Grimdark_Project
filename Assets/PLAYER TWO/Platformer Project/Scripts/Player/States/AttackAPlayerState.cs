using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    [AddComponentMenu("PLAYER TWO/Platformer Project/Player/States/Attack Player State")]
    public class AttackAPlayerState : PlayerState
    {
        protected override void OnEnter(Player player)
        {
            player.velocity = Vector3.zero;

            if (player.combat.CanContinueCombo())
            {
                switch (player.combat.CurrentCombo)
                {
                    case 1: player.states.Change<AttackA2PlayerState>(); break;
                    case 2: player.states.Change<AttackA3PlayerState>(); break;
                    default: player.states.Change<AttackA1PlayerState>(); break;
                }
            }
            else
            {
                // Начинаем новую комбо-цепочку
                player.combat.ResetCombo();
                player.states.Change<AttackA1PlayerState>();
            }
        }

        protected override void OnStep(Player player)
        {
        }

        protected override void OnExit(Player player)
        {
            if (!player.combat.CanContinueCombo())
            {
                player.combat.ResetCombo();
            }
        }

        public override void OnContact(Player player, Collider other) { }
    }
}