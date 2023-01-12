using UnityEngine;

namespace InnerDriveAcademy.TownsVille
{

	/**
	 * Should be attached to the Snowball prefab to make sure it responds to hitting things,
	 * and destroys itself over time to prevent fps/memory issues.
	 */
	public class Snowball : MonoBehaviour
	{
		public float lifeTime = 5;                  //will destroy itself after this amount of seconds
		public GameObject impactEffectPrefab;       //will instantiate this object at the time and point of impact

		private void Awake()
		{
			//Set up the destroy timer, the moment we are created
			Destroy(gameObject, lifeTime);
		}

		private void OnCollisionEnter(Collision collision)
		{
			//Provide some debug info, which can be removed later if you want to ...
			Debug.Log("Snowball hit " + collision.gameObject.name + " " + collision.collider);

			//Spawn an impact effect if any has been set...
			if (impactEffectPrefab != null) Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);

			//Destroy the gameObject (at the end of the frame, that is how unity works...)
			Destroy(gameObject);
		}

	}

}