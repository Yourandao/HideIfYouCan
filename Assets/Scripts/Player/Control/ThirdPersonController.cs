using UnityEngine;

namespace Assets.Scripts.PlayerScripts.Control
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : MonoBehaviour
    {
        [SerializeField] private CharacterController controller = default;

        [SerializeField] private float movementSpeed = 2.5f;
        [SerializeField] private float jumpSpeed     = 2.5f;

        [SerializeField] private float rotationSpeed = 10f;

        private Vector3 input;

        private void Update()
        {
            if (!controller.isGrounded)
                return;

            input.x = Input.GetAxisRaw("Horizontal");
            input.z = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Jump"))
                input.y = jumpSpeed;
        }

        private void FixedUpdate()
        {
            if (input.y > Physics.gravity.y / movementSpeed)
                input.y += Physics.gravity.y * Time.fixedDeltaTime;

            controller.Move(input * movementSpeed * Time.fixedDeltaTime);

            var rotation = Vector3.RotateTowards(transform.forward,
                                                 new Vector3(input.x, 0f, input.z),
                                                 rotationSpeed * Time.fixedDeltaTime,
                                                 0f);

            transform.rotation = Quaternion.LookRotation(rotation);
        }
    }
}