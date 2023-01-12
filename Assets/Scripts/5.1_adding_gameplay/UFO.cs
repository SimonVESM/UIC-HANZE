using System;
using UnityEngine;

namespace InnerDriveAcademy.TownsVille
{
	/**
	 * Implements all UFO behaviour including warping in, attacking and warping out again.
	 */
	public class UFO : MonoBehaviour
	{
		//Triggers when we disappear because we despawn after destroying a house
		public static event Action<UFO> OnDespawned = delegate { };
		//Triggers when we disappear because we have been destroyed
		public static event Action<UFO> OnDestroyed = delegate { };

		//Using a public set/get instead of a public variable so that it doesn't show up in the Inspector
		public House target { get; set; }

		[Range(0, 360)]	public float rotationSpeed = 60;

		[Header("Warp speed & attack distance")]
		[Range(10, 200)] public float warpSpeed = 200;     //How fast do we warp in?
		[Range(10, 30)] public float attackDistance = 10;  //At which height do we switch from warp to attack?

		[Header("Special effect settings")]
		public GameObject weaponBeam = null;            //reference to a child representing the weapon beam
		public GameObject weaponBeamHitEffect = null;   //reference to a child representint the firebeam effect
		public GameObject destroyEffectPrefab = null;	//used to create an explosion effect when destroyed

		[Header("Attack movement settings")]
		public Vector3 moveRange = Vector3.zero;                //How far are we allowed to move on all axis, while attacking?
		public Vector3 moveSpeed = Vector3.one;                 //How fast do we move while attacking?
		private Vector3 randomPhaseOffset = Vector3.zero;       //Make sure the UFO's do not all move in synch
		private float influence = 0;                            //Used to slowly ease in into the movement pattern after warping in

		[Header("Audio settings")]
		public AudioSource audioSource;
		[Range(0, 5)] public float pitchInfluence = 0;
		private float originalPitch = 1;

		//Some fields to keep track of state
		private enum State { WARPING_IN, ATTACKING, WARPING_OUT };
		private State currentState = State.WARPING_IN;
		private Vector3 originalSpawnPosition;
		private Vector3 attackPosition;

		private void Start()
		{
			//Set up positions for warping in and out
			originalSpawnPosition = transform.localPosition;
			attackPosition = transform.localPosition;
			attackPosition.y = attackDistance;
			randomPhaseOffset = UnityEngine.Random.onUnitSphere * 2 * Mathf.PI;

			//Register for any house destroy events to check if we destroyed 'our' house
			House.OnDestroyed += House_OnDestroyed;

			//Hide any beams since we still need to warp in
			enableBeam(false);

			if (audioSource != null) originalPitch = audioSource.pitch;
		}

		private void Update()
		{
			transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);

			if (audioSource != null)
			{
				audioSource.pitch = originalPitch + Mathf.Clamp((transform.localPosition.y - attackPosition.y) * pitchInfluence, -0.2f, 0.2f);
			}

			switch (currentState)
			{
				case State.WARPING_IN: warpIn(); break;
				case State.ATTACKING: attack(); break;
				case State.WARPING_OUT: warpOut(); break;
			}
		}

		private void warpIn()
		{
			float distance = Vector3.Distance(transform.localPosition, attackPosition);

			if (distance > 0.1f)
			{
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, attackPosition, warpSpeed * Time.deltaTime);
			}
			else
			{
				enableBeam(true);
				currentState = State.ATTACKING;
			}
		}

		private void attack()
		{
			transform.localPosition =
				attackPosition +
				Vector3.Scale(
					moveRange,
					new Vector3(
						Mathf.Cos(randomPhaseOffset.x + Time.realtimeSinceStartup * moveSpeed.x),
						Mathf.Sin(randomPhaseOffset.y + Time.realtimeSinceStartup * moveSpeed.y),
						Mathf.Cos(randomPhaseOffset.z + Time.realtimeSinceStartup * moveSpeed.z)
					)
				) * influence;

			influence = Mathf.Clamp(influence + Time.deltaTime, 0, 1);

			//Every frame we do a bit of damage (based on time values)
			if (target != null) target.Damage(Time.deltaTime);

			//Do we have a fire beam effect? If so position it correctly...
			if (weaponBeamHitEffect != null)
			{
				RaycastHit hit;
				if (Physics.Raycast(transform.position, Vector3.down, out hit, attackDistance))
				{
					weaponBeamHitEffect.transform.position = hit.point + Vector3.down * 0.2f;
				}
			}
		}

		private void warpOut()
		{
			float distance = Vector3.Distance(transform.localPosition, originalSpawnPosition);

			if (distance > 0.1f)
			{
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalSpawnPosition, warpSpeed * Time.deltaTime);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void House_OnDestroyed(House pHouse)
		{
			//if we destroyed 'our' house, our job is done here ;)
			if (pHouse == target) Despawn();
		}

		private void OnCollisionEnter(Collision collision)
		{
			//make sure we only trigger once
			if (!isActiveAndEnabled || currentState != State.ATTACKING) return;

			Debug.Log(name + " hit by " + collision.gameObject.name);
			OnDestroyed(this);
			SpawnDestroyEffectPrefab();
			Destroy(gameObject);
		}

		public void Despawn()
		{
			OnDespawned(this);
			enableBeam(false);
			currentState = State.WARPING_OUT;
		}

		public void SpawnDestroyEffectPrefab()
		{
			if (gameObject.scene.isLoaded && destroyEffectPrefab != null) Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
		}


		private void OnDestroy()
		{
			enableBeam(false);
			House.OnDestroyed -= House_OnDestroyed;
		}

		private void enableBeam(bool pActive)
		{
			if (weaponBeam != null) weaponBeam.SetActive(pActive);
			if (weaponBeamHitEffect != null) weaponBeamHitEffect.SetActive(pActive);
		}

	}
}