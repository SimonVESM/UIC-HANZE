using System;
using UnityEngine;

namespace InnerDriveAcademy.TownsVille
{
	/**
	 * Represent the behaviour of a single House.
	 */
	public class House : MonoBehaviour
	{
		//triggered when a house is destroyed
		public static event Action<House> OnDestroyed = delegate { };

		public int timeToLive = 1;          //the amount of 'burning' time a house can withstand before exploding
		private float health;               //the current time to live that is left for the house

		public GameObject explosionPrefab;  //the prefab to spawn when we explode

		private void Awake()
		{
			ResetHouse();
		}

		public void ResetHouse(float healthMultiplier = 1)
		{
			health = timeToLive * healthMultiplier;
			gameObject.SetActive(true);
		}

		public void Damage(float pDamage)
		{
			if (!isActiveAndEnabled) return;

			health -= pDamage;

			//did we die? Note that we don't actually destroy the house, like we do with the UFOs
			//we just flag this house as inactive, so we can reuse it when we play the game again
			if (health <= 0)
			{
				Debug.Log("House 'destroyed'.");
				gameObject.SetActive(false);
				if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
				OnDestroyed(this);
			}
		}
	}
}