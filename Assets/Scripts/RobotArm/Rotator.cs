using System.Collections;
using UnityEngine;

namespace workbook_2_1
{
    public class Rotator : MonoBehaviour
    {
        public Vector3 axis = Vector3.right;
        public float minAngle = 0;
        public float maxAngle = 0;
        public float speed = 1;

        private void Start()
        {
            transform.localRotation = Quaternion.AngleAxis(minAngle, axis);
            StartCoroutine(DoRotationBehaviour());
        }

        IEnumerator DoRotationBehaviour()
        {
            while (true)
            {
                float i = 0;

                while (i < 1)
                {
                    float angle = Mathf.Lerp(minAngle, maxAngle, i);
                    transform.localRotation = Quaternion.AngleAxis(angle, axis);
                    i += speed * Time.deltaTime;
                    yield return null;
                }

                yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));

                while (i > 0)
                {
                    float angle = Mathf.Lerp(minAngle, maxAngle, i);
                    transform.localRotation = Quaternion.AngleAxis(angle, axis);
                    i -= speed * Time.deltaTime;
                    yield return null;
                }

                yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));

            }
        }
    }
}