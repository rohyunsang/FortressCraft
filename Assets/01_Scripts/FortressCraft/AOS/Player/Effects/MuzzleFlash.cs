using UnityEngine;

namespace Agit.FortressCraft
{
	public class MuzzleFlash : AutoReleasedFx
	{
		[SerializeField] private AudioEmitter _audioEmitter;
		[SerializeField] private ParticleSystem _particleEmitter;
		[SerializeField] private float _timeToFade;

		protected override float Duration => _timeToFade;
		
		public virtual void OnFire(ShotState state)
		{
			_audioEmitter.PlayOneShot();
			_particleEmitter.Play();
		}
	}
}