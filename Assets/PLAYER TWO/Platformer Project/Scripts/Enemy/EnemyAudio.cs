using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[RequireComponent(typeof(Enemy))]
	[AddComponentMenu("PLAYER TWO/Platformer Project/Enemy/Enemy Audio")]
	public class EnemyAudio : MonoBehaviour
	{
		[Header("Effects")]
		public AudioClip death;
		[Range(0f, 1f)]
		public float volume;
		protected Enemy m_enemy;
		protected AudioSource m_audio;

		protected virtual void InitializeEnemy() => m_enemy = GetComponent<Enemy>();

		protected virtual void InitializeAudio()
		{
			if (!TryGetComponent(out m_audio))
			{
				m_audio = gameObject.AddComponent<AudioSource>();
			}
			m_audio.volume = volume;
		}

		protected virtual void InitializeCallbacks()
		{
			m_enemy.enemyEvents.OnDie.AddListener(() => m_audio.PlayOneShot(death));
		}

		protected virtual void Start()
		{
			InitializeEnemy();
			InitializeAudio();
			InitializeCallbacks();
		}
	}
}
