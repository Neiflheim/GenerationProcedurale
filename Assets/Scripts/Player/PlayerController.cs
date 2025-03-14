using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _lookSpeed = 100f;
        [SerializeField] private Transform _cameraTransform;
        
        private Rigidbody _rigidbody;
        private float rotationX = 0f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true; // Empêche la physique de modifier la rotation
            Cursor.lockState = CursorLockMode.Locked; // Cache et verrouille le curseur
        }

        private void Update()
        {
            // Déplacement du joueur
            float horizontalValue = Input.GetAxis("Horizontal");
            float verticalValue = Input.GetAxis("Vertical");
            Vector3 moveDirection = transform.right * horizontalValue + transform.forward * verticalValue;
            _rigidbody.linearVelocity = new Vector3(moveDirection.x * _moveSpeed, _rigidbody.linearVelocity.y, moveDirection.z * _moveSpeed);

            // Rotation de la caméra et du joueur
            float mouseX = Input.GetAxis("Mouse X") * _lookSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _lookSpeed * Time.deltaTime;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Empêche de regarder trop haut/bas

            _cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}
