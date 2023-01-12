using System;
using UnityEngine;
using UnityEngine.Events;

namespace InnerDriveAcademy.TownsVille
{

	/**
	* Attach this script to a GameObject to allow it to shoot snowballs on mouse click.
	* (Attach it to two different GameObjects to have a double barreled snowball shooter ;))
*/
	public class SnowballThrower : MonoBehaviour
	{
		public UnityEvent OnSnowballLaunched;

		public Rigidbody snowBallPrefab;
		public Transform snowBallSpawnPoint;
		public float snowBallThrowSpeed = 0;

		public AudioClip throwAudio;
		[Range(0, 1)] public float throwAudioVolume = 0.5f;

		private void Awake()
		{
			//by default if no spawnpoint has been set, use our own transform as spawn location
			if (snowBallSpawnPoint == null) snowBallSpawnPoint = transform;
		}

		private void Update()
		{
			//left mouse button click? Launch it!
			if (Input.GetMouseButtonDown(0))
			{
				shootSnowBall();
				OnSnowballLaunched.Invoke();
			}
		}

		private void shootSnowBall()
		{
			//Some sanity checks to avoid crashes and nullreferenceexceptions
			if (snowBallPrefab == null)
			{
				Debug.Log("Please assign a value to the snowBallPrefab field of the SnowballThrower Script");
				return;
			}

			//Instantiate our snowball prefab at the spawn point position
			Rigidbody snowBallInstance = Instantiate(snowBallPrefab, snowBallSpawnPoint.position, snowBallSpawnPoint.rotation);
			//Set the snow velocity (once)
			snowBallInstance.velocity = snowBallInstance.transform.forward * snowBallThrowSpeed;
			//If some audio has been set, play it
			if (throwAudio != null) AudioSource.PlayClipAtPoint(throwAudio, transform.position, throwAudioVolume + UnityEngine.Random.value * 0.3f);
		}
	}
}