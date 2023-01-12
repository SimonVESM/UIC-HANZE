using UnityEngine;

namespace InnerDriveAcademy.TownsVille
{

	public class Recoil : MonoBehaviour
	{
		private Vector3 originalPosition;
		public float recoilDistance = 0.2f;
		public float recoilSpeed = 10;

		private void Start()
		{
			originalPosition = transform.localPosition;
		}

		public void DoRecoil()
		{
			transform.localPosition = originalPosition - Vector3.forward * recoilDistance;
			enabled = true;
		}

		private void Update()
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * recoilSpeed);
			if (Vector3.Distance(transform.localPosition, originalPosition) < 0.01f) enabled = false;
		}
	}
}