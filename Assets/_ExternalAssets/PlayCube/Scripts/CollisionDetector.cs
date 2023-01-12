using UnityEngine;
using UnityEngine.Events;

namespace InnerDriveAcademy.TownsVille
{
    public class CollisionDetector : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnCollision;

		private void OnCollisionEnter(Collision collision)
		{
			OnCollision.Invoke();
		}
	}

}