using UnityEngine;
using System.Collections;

namespace Tilify
{
    [RequireComponent(typeof(Camera))]
    public class OrbitCameraMovement : MonoBehaviour
    {
        [Header ("Rotation")]
        public KeyCode rotationActivationKey = KeyCode.LeftAlt;
        public KeyCode rotationKey = KeyCode.Mouse0;
        [Range(0, 30)]
        public float rotationSensitivity = 12;
        public float rotationOriginDistanceVoid = 10;

        [Space]
        [Header ("Zoom")]
        public KeyCode zoomKey = KeyCode.Mouse1;
        [Range (0, 20)]
        public float zoomSensitivity = 5;
        public bool zoomWithMouseWheel = true;
        public bool zoomInverse = false;

        [Space]
        [Header ("Moving")]
        public KeyCode movingKey = KeyCode.Mouse2;
        [Range(0, 20)]
        public float movingSensitivity = 5;
        public bool movingInverse = false;

        private Vector3 lastMousePosition;

        private Camera cameraComponent;

        private Vector3 rotationOrigin = Vector3.zero;
        private float lastHitDistance = 0;

        private void Awake ()
        {
            lastMousePosition = Input.mousePosition;
            cameraComponent = GetComponent<Camera> ();
        }

        private void Update ()
        {
            var newMousePosition = Input.mousePosition;
            var mousePositionDifference = cameraComponent.ScreenToViewportPoint (newMousePosition - lastMousePosition);

            if (Input.GetKey(movingKey) )
            {
                Move (mousePositionDifference);
            }

            var scrollWheelDelta = Input.GetAxis ("Mouse ScrollWheel");
            if ( scrollWheelDelta != 0)
            {
                Zoom (scrollWheelDelta);
            }

            if (Input.GetKey(rotationActivationKey) && Input.GetKey(rotationKey) )
            {
                Rotate (mousePositionDifference);
            }
            else
            {
                rotationOrigin = Vector3.zero;
            }

            lastMousePosition = Input.mousePosition;
        }

        public void Move(Vector3 amount)
        {
            if ( !movingInverse )
                amount = -amount;
            transform.Translate (amount * movingSensitivity);
        }
        public void Zoom(float amount)
        {
            if ( !zoomInverse )
                amount = -amount;
            transform.Translate (Vector3.forward * amount * zoomSensitivity);
        }


        public void Rotate(Vector2 amount)
        {
            if ( rotationOrigin == Vector3.zero )
            {
                bool isHit = Physics.Raycast (transform.position, transform.forward, out RaycastHit hit);
                
                var hitDistance = 0f;

                if ( isHit )
                    hitDistance = hit.distance;
                else
                    hitDistance = rotationOriginDistanceVoid;

                lastHitDistance = hitDistance;
                rotationOrigin = transform.position + transform.forward * hitDistance;
            }

            amount *= rotationSensitivity * 10;
            
            transform.RotateAround (rotationOrigin, transform.up, amount.x);
            transform.RotateAround (rotationOrigin, -transform.right, amount.y);
            var rotation = transform.rotation;
            transform.LookAt (rotationOrigin, Vector3.up);
        }
    }
}