using UnityEngine;

namespace InnerDriveAcademy.TownsVille
{
	public class Cooldown : MonoBehaviour
	{
		public float cooldownTime = 0.01f;
		public MonoBehaviour objectToDisable;
		
		private float cooldownStart = 0;

		private void Awake()
		{
			enabled = false;
		}

		public void DoCooldown()
		{
			objectToDisable.enabled = false;
			enabled = true;
			cooldownStart = Time.realtimeSinceStartup;
		}

		private void Update()
		{
			if (Time.realtimeSinceStartup - cooldownStart > cooldownTime)
			{
				objectToDisable.enabled = true;
				enabled = false;
			}
		}

	}
}