

using UnityEngine;

namespace RhythmPlayer
{
    public class PlayerVisuals : MonoBehaviour
    {
        [SerializeField] private float visualRotationSpeed = 5;
        [SerializeField] private float visualMovementSpeed = 5;
        [SerializeField] private GameObject visuals;

        void Awake()
        {
            visuals = GetComponentInChildren<SpriteRenderer>().gameObject;
        }


        private void RotateVisualsTowardsCollider()
        {
            // visuals.transform.rotation = Quaternion.Lerp(visuals.transform.rotation, playerCollider.transform.rotation, visualRotationSpeed * Time.deltaTime);
        }

        private void MoveVisualsTowardsCollider()
        {
            // visuals.transform.localPosition = Vector3.Lerp(visuals.transform.localPosition, playerCollider.transform.localPosition, visualMovementSpeed * Time.deltaTime);
        }
    }
}