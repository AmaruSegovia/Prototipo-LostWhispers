using System;
using UnityEngine;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] bool m_IsWalking;
        [SerializeField] float m_WalkSpeed;
        [SerializeField] float m_RunSpeed;
        [SerializeField][Range(0f, 1f)] float m_RunstepLenghten;
        [SerializeField] float m_JumpSpeed;
        [SerializeField] float m_StickToGroundForce;
        [SerializeField] float m_GravityMultiplier;
        [SerializeField] MouseLook m_MouseLook;
        [SerializeField] bool m_UseFovKick;
        [SerializeField] FOVKick m_FovKick = new FOVKick();
        [SerializeField] bool m_UseHeadBob;
        [SerializeField] CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] float m_StepInterval;
        [SerializeField] AudioClip[] m_FootstepSounds;    
        [SerializeField] AudioClip m_JumpSound;           
        [SerializeField] AudioClip m_LandSound;


        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private Vector3 explosionForce = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            m_MouseLook.Init(transform, m_Camera.transform);
        }

        // Update is called once per frame
        private void Update()
        {
            RotateView();

            // Handle walking effect
            float speed;
            GetInput(out speed);

            Vector3 move = Vector3.zero;

            if (explosionForce != Vector3.zero)
            {
                // Apply explosion force and gradually reduce it
                m_CharacterController.Move(explosionForce * Time.fixedDeltaTime);
                explosionForce = Vector3.Lerp(explosionForce, Vector3.zero, Time.fixedDeltaTime * 1.5f); // Faster reduction
            }

            // Update movement direction and allow jumping
            UpdateMoveDirection(speed);
            move = m_MoveDir * Time.fixedDeltaTime;

            // Apply gravity always
            if (!m_CharacterController.isGrounded || explosionForce != Vector3.zero)
            {
                move.y += Physics.gravity.y * Time.fixedDeltaTime;
            }

            m_CollisionFlags = m_CharacterController.Move(move);

            // If hitting a ceiling, adjust vertical movement
            if ((m_CollisionFlags & CollisionFlags.Above) != 0)
            {
                m_MoveDir.y = 0;
            }

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                PlayLandingSound();
            }
            m_MouseLook.UpdateCursorLock();
            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);

            Vector3 move = Vector3.zero;

            if (explosionForce != Vector3.zero)
            {
                // Aplica la fuerza de explosión y la reduce gradualmente
                m_CharacterController.Move(explosionForce * Time.fixedDeltaTime);
                explosionForce = Vector3.Lerp(explosionForce, Vector3.zero, Time.fixedDeltaTime * 1.5f); // Reducción más rápida
            }

            // Actualiza la dirección de movimiento y permite saltar
            UpdateMoveDirection(speed);
            move = m_MoveDir * Time.fixedDeltaTime;

            // Aplica la gravedad siempre
            if (!m_CharacterController.isGrounded || explosionForce != Vector3.zero)
            {
                move.y += Physics.gravity.y * Time.fixedDeltaTime;
            }

            m_CollisionFlags = m_CharacterController.Move(move);

            // Si golpea un techo, ajusta el movimiento vertical
            if ((m_CollisionFlags & CollisionFlags.Above) != 0)
            {
                m_MoveDir.y = 0;
            }

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }


        private void UpdateMoveDirection(float speed)
        {
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;

            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }
        }

        public void ApplyExplosionForce(Vector3 direction, float force)
        {
            direction.y = 10f;  // Controla la altura de la explosión
            explosionForce = direction.normalized * force;
        }

        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }

        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }

        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }

        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT

            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            // Si el jugador está sobre un cuerpo rígido, no lo mueve
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                explosionForce = Vector3.zero; // Reinicia la fuerza de explosión al tocar el suelo
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }

    }
}