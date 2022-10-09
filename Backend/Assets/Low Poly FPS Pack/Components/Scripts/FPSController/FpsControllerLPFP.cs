using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace FPSControllerLPFP
{
    /// Manages a first person character
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(AudioSource))]
    public class FpsControllerLPFP : MonoBehaviour
    {
        public float totalSprintTimeAllowed = 5.0f;
        public float currentSprintTime = 5.0f;

        public Image mainSprintDisplay;
        public Image parentSprintDisplay;

        private int cyclesWaited;

        public bool isCrouching;
        public float finalSpeed = 0.0f;
#pragma warning disable 649
        [Header("Arms")]
        [Tooltip("The transform component that holds the gun camera."), SerializeField]
        private Transform arms;

        [Tooltip("The position of the arms and gun camera relative to the fps controller GameObject."), SerializeField]
        private Vector3 armPosition;

        [Header("Audio Clips")]
        [Tooltip("The audio clip that is played while walking."), SerializeField]
        private AudioClip walkingSound;

        [Tooltip("The audio clip that is played while running."), SerializeField]
        private AudioClip runningSound;

        [Header("Movement Settings")]
        [Tooltip("How fast the player moves while walking and strafing."), SerializeField]
        private float walkingSpeed = 5f;

        [Tooltip("How fast the player moves while running."), SerializeField]
        private float runningSpeed = 9f;

        [Tooltip("Approximately the amount of time it will take for the player to reach maximum running or walking speed."), SerializeField]
        private float movementSmoothness = 0.125f;

        [Tooltip("Amount of force applied to the player when jumping."), SerializeField]
        private float jumpForce = 35f;

        [Header("Look Settings")]
        [Tooltip("Rotation speed of the fps controller."), SerializeField]
        private float mouseSensitivity = 7f;

        [Tooltip("Approximately the amount of time it will take for the fps controller to reach maximum rotation speed."), SerializeField]
        private float rotationSmoothness = 0.05f;

        [Tooltip("Minimum rotation of the arms and camera on the x axis."),
         SerializeField]
        private float minVerticalAngle = -90f;

        [Tooltip("Maximum rotation of the arms and camera on the axis."),
         SerializeField]
        private float maxVerticalAngle = 90f;

        [Tooltip("The names of the axes and buttons for Unity's Input Manager."), SerializeField]
        private FpsInput input;
#pragma warning restore 649

        public Rigidbody _rigidbody;
        private CapsuleCollider _collider;
        private AudioSource _audioSource;
        private SmoothRotation _rotationX;
        private SmoothRotation _rotationY;
        private SmoothVelocity _velocityX;
        private SmoothVelocity _velocityZ;
        public bool _isGrounded;

        private readonly RaycastHit[] _groundCastResults = new RaycastHit[8];
        private readonly RaycastHit[] _wallCastResults = new RaycastHit[8];

        private bool canCrouch = true;

        public List<Transform> crouchTests = new List<Transform>();

        [HideInInspector] public bool mouseOut = false;

        public AudioClip current;
        public AudioClip target;

        public bool canMove;
        public float targetVolume;

        public bool cheats;
        public bool noclip;

        public WalkingMaterialData currentWalkingMat;
        public GameObject baseFootstepObj;
        public Transform footstepSoundOrigin;

        public float i_timeSinceLastStep;

        public bool moveVarLocked;

        IEnumerator RechargeSprint()
        {
            if (input.Run && _isGrounded && _rigidbody.velocity.sqrMagnitude > 0.1f && !isCrouching)
            {
                currentSprintTime -= 0.01f;
                mainSprintDisplay.color = new Color(mainSprintDisplay.color.r, mainSprintDisplay.color.g, mainSprintDisplay.color.b, 1);
                parentSprintDisplay.color = new Color(parentSprintDisplay.color.r, parentSprintDisplay.color.g, parentSprintDisplay.color.b, 1);
                cyclesWaited = 0;
            }
            else
            {
                if (cyclesWaited >= 100)
                {
                    currentSprintTime += 0.025f;
                    mainSprintDisplay.color = new Color(mainSprintDisplay.color.r, mainSprintDisplay.color.g, mainSprintDisplay.color.b, mainSprintDisplay.color.a - 0.01f);
                    parentSprintDisplay.color = new Color(parentSprintDisplay.color.r, parentSprintDisplay.color.g, parentSprintDisplay.color.b, parentSprintDisplay.color.a - 0.01f);
                }
            }

            if (currentSprintTime <= 0)
            {
                currentSprintTime = 0;
            }
            else if (currentSprintTime > totalSprintTimeAllowed)
            {
                currentSprintTime = totalSprintTimeAllowed;
            }

            mainSprintDisplay.rectTransform.localScale = new Vector3(currentSprintTime / totalSprintTimeAllowed, 1, 1);

            cyclesWaited += 1;

            yield return new WaitForSeconds(0.01f);
            StartCoroutine(RechargeSprint());
        }

        private void Start()
        {
            StartCoroutine(RechargeSprint());
            StartCoroutine(Check_StepCheck());
            StartCoroutine(BlendAmbience());

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _collider = GetComponent<CapsuleCollider>();
            _audioSource = GetComponent<AudioSource>();
            arms = AssignCharactersCamera();
            _audioSource.clip = walkingSound;
            _audioSource.loop = true;
            _rotationX = new SmoothRotation(RotationXRaw);
            _rotationY = new SmoothRotation(RotationYRaw);
            _velocityX = new SmoothVelocity();
            _velocityZ = new SmoothVelocity();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ValidateRotationRestriction();
        }

        public AudioSource ambience;
        public List<AmbienceSetData> ambiences = new List<AmbienceSetData>();

        public void SetAmbience(int id)
        {
            if (id == -2)
            {
                ambience.Stop();
                target = null;
                current = null;
                ambience.volume = 0;
                ambience.clip = null;
            }
            else if (id != -1)
            {
                target = ambiences[id].ambientTrack;
                targetVolume = ambiences[id].volumeToPlayAt;
            }
            else
            {
                target = null;
            }
        }

        IEnumerator BlendAmbience()
        {
            yield return new WaitForSeconds(0.025f);

            if (target == null)
            {
                if (current != null)
                {
                    //We must lower the volume then stop the track
                    if (ambience.volume >= 0.01f)
                    {
                        ambience.volume -= targetVolume / 100f;
                    }
                    else
                    {
                        ambience.Stop();
                    }
                }
            }
            else
            {
                if (!ambience.isPlaying)
                {
                    ambience.Play();
                }

                if (current != target)
                {
                    //We need to turn down current until 0, then change to target and turn taget up to targetVolume

                    if (ambience.volume < 0.01f)
                    {
                        ambience.clip = target;
                        current = target;
                    }
                    else if (ambience.volume > 0.01f)
                    {
                        ambience.volume -= targetVolume / 100f;
                    }
                }

                if (current == target)
                {
                    //We have the clip currently set to the target clip
                    if (ambience.volume >= targetVolume - targetVolume / 100f && ambience.volume <= targetVolume + targetVolume / 100f)
                    {
                        //Volume is good
                    }
                    else
                    {
                        //Volume needs to be changed
                        if (ambience.volume < targetVolume)
                        {
                            ambience.volume += targetVolume / 100f;
                        }
                        else if (ambience.volume < targetVolume)
                        {
                            ambience.volume -= targetVolume / 100f;
                        }
                    }
                }
            }

            StartCoroutine(BlendAmbience());
        }

        private Transform AssignCharactersCamera()
        {
            var t = transform;
            arms.SetPositionAndRotation(t.position, t.rotation);
            return arms;
        }

        /// Clamps <see cref="minVerticalAngle"/> and <see cref="maxVerticalAngle"/> to valid values and
        /// ensures that <see cref="minVerticalAngle"/> is less than <see cref="maxVerticalAngle"/>.
        private void ValidateRotationRestriction()
        {
            minVerticalAngle = ClampRotationRestriction(minVerticalAngle, -90, 90);
            maxVerticalAngle = ClampRotationRestriction(maxVerticalAngle, -90, 90);
            if (maxVerticalAngle >= minVerticalAngle) return;
            Debug.LogWarning("maxVerticalAngle should be greater than minVerticalAngle.");
            var min = minVerticalAngle;
            minVerticalAngle = maxVerticalAngle;
            maxVerticalAngle = min;
        }

        private static float ClampRotationRestriction(float rotationRestriction, float min, float max)
        {
            if (rotationRestriction >= min && rotationRestriction <= max) return rotationRestriction;
            var message = string.Format("Rotation restrictions should be between {0} and {1} degrees.", min, max);
            Debug.LogWarning(message);
            return Mathf.Clamp(rotationRestriction, min, max);
        }

        /// Checks if the character is on the ground.
        private void OnCollisionStay()
        {
            var bounds = _collider.bounds;
            var extents = bounds.extents;
            var radius = extents.x - 0.01f;
            Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
                _groundCastResults, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);
            if (!_groundCastResults.Any(hit => hit.collider != null && hit.collider != _collider)) return;
            for (var i = 0; i < _groundCastResults.Length; i++)
            {
                _groundCastResults[i] = new RaycastHit();
            }

            _isGrounded = true;
        }

        /// Processes the character movement and the camera rotation every fixed framerate frame.
        private void FixedUpdate()
        {
            // FixedUpdate is used instead of Update because this code is dealing with physics and smoothing.
            RotateCameraAndCharacter();
            MoveCharacter();
            _isGrounded = false;
        }

        /// Moves the camera to the character, processes jumping and plays sounds every frame.
        /// 

        IEnumerator Check_StepCheck()
        {
            yield return new WaitForSeconds(.05f);
            i_timeSinceLastStep += .05f;

            StartCoroutine(Check_StepCheck());
        }

        private void Update()
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.

            layerMask = ~layerMask;
            RaycastHit hitMaterial;
            if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(-Vector3.up), out hitMaterial, Mathf.Infinity, layerMask))
            {
                if (hitMaterial.collider.gameObject.GetComponent<ObjectMat>() != null)
                {
                    Debug.DrawRay(this.transform.position, this.transform.TransformDirection(-Vector3.up) * 1000, Color.green);
                    currentWalkingMat = hitMaterial.collider.gameObject.GetComponent<ObjectMat>().material;
                }
                else
                {
                    Debug.DrawRay(this.transform.position, this.transform.TransformDirection(-Vector3.up) * 1000, Color.red);
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                mouseOut = false;

                if (!moveVarLocked)
                {
                    canMove = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                mouseOut = true;
            }

            if (cheats)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    if (noclip)
                    {
                        noclip = false;
                    }
                    else
                    {
                        noclip = true;
                    }
                }
            }

            if (mouseOut)
            {
                canMove = false;
            }

            bool passed = true;
            foreach (Transform place in crouchTests)
            {
                RaycastHit hit;
                if (Physics.Raycast(place.transform.position, place.transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
                {
                    if (hit.collider.tag == "Player")
                    {
                        //Ignore
                    }
                    else
                    {
                        if (hit.distance <= .35f)
                        {
                            passed = false;
                            Debug.DrawRay(place.transform.position, place.transform.TransformDirection(Vector3.up) * 1000, Color.red);
                        }
                        else
                        {
                            Debug.DrawRay(place.transform.position, place.transform.TransformDirection(Vector3.up) * 1000, Color.green);
                        }
                    }
                }

                if (!passed)
                {
                    canCrouch = false;
                }
                else
                {
                    canCrouch = true;
                }
            }

            //PlayFootstepSounds();

            if (!canMove || mouseOut)
            {
                return;
            }

            finalSpeed = 1.5f;

            if (!isCrouching)
            {
                if (input.Run && currentSprintTime > 0)
                {
                    camera.GetComponent<ViewBobbing>().walkingBobbingSpeed = 17;
                    finalSpeed = runningSpeed;
                    Mathf.Pow(runningSpeed, -1.25f);
                    finalSpeed /= 15;
                }
                else
                {
                    camera.GetComponent<ViewBobbing>().walkingBobbingSpeed = 14;
                    finalSpeed = walkingSpeed;
                    Mathf.Pow(walkingSpeed, -1.25f);
                    finalSpeed /= 7.5f;
                }
            }
            else
            {
                camera.GetComponent<ViewBobbing>().walkingBobbingSpeed = 10;
                finalSpeed = walkingSpeed;
                Mathf.Pow(walkingSpeed, -1.25f);
                finalSpeed /= 2.35f;
            }

            if (_isGrounded && _rigidbody.velocity.sqrMagnitude > 0.1f && i_timeSinceLastStep >= finalSpeed)
            {
                if (currentWalkingMat != null)
                {
                    int random = UnityEngine.Random.Range(0, currentWalkingMat.footstepSounds.Count);
                    if (currentWalkingMat.footstepSounds.Count > 0)
                    {
                        AudioSource step = Instantiate(baseFootstepObj, footstepSoundOrigin.transform.position, Quaternion.identity).GetComponent<AudioSource>();
                        step.clip = currentWalkingMat.footstepSounds[random];
                        step.volume = currentWalkingMat.footstepSoundVolume[random];
                        step.Play();
                    }

                    i_timeSinceLastStep = 0;
                }
            }

            arms.position = transform.position + transform.TransformVector(armPosition);
            Jump();

            if (canCrouch)
            {
                if (Input.GetKey(KeyCode.C))
                {
                    //Crouch
                    isCrouching = true;
                    _collider.height = .6f;
                    walkingSpeed = 1.5f;
                    runningSpeed = 1.5f;
                }
                else
                {
                    isCrouching = false;
                    _collider.height = 1.8f;
                    walkingSpeed = 3;
                    runningSpeed = 5;
                }
            }
        }

        private void RotateCameraAndCharacter()
        {
            if (!canMove || mouseOut)
            {
                return;
            }
            var rotationX = _rotationX.Update(RotationXRaw, rotationSmoothness);
            var rotationY = _rotationY.Update(RotationYRaw, rotationSmoothness);
            var clampedY = RestrictVerticalRotation(rotationY);
            _rotationY.Current = clampedY;
            var worldUp = arms.InverseTransformDirection(Vector3.up);
            var rotation = arms.rotation *
                           Quaternion.AngleAxis(rotationX, worldUp) *
                           Quaternion.AngleAxis(clampedY, Vector3.left);
            transform.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
            arms.rotation = rotation;
        }

        /// Returns the target rotation of the camera around the y axis with no smoothing.
        private float RotationXRaw
        {
            get { return input.RotateX * mouseSensitivity; }
        }

        /// Returns the target rotation of the camera around the x axis with no smoothing.
        private float RotationYRaw
        {
            get { return input.RotateY * mouseSensitivity; }
        }

        /// Clamps the rotation of the camera around the x axis
        /// between the <see cref="minVerticalAngle"/> and <see cref="maxVerticalAngle"/> values.
        private float RestrictVerticalRotation(float mouseY)
        {
            var currentAngle = NormalizeAngle(arms.eulerAngles.x);
            var minY = minVerticalAngle + currentAngle;
            var maxY = maxVerticalAngle + currentAngle;
            return Mathf.Clamp(mouseY, minY + 0.01f, maxY - 0.01f);
        }

        /// Normalize an angle between -180 and 180 degrees.
        /// <param name="angleDegrees">angle to normalize</param>
        /// <returns>normalized angle</returns>
        private static float NormalizeAngle(float angleDegrees)
        {
            while (angleDegrees > 180f)
            {
                angleDegrees -= 360f;
            }

            while (angleDegrees <= -180f)
            {
                angleDegrees += 360f;
            }

            return angleDegrees;
        }

        private bool W;
        private bool A;
        private bool S;
        private bool D;
        private bool Sh;
        public Transform camera;

        private void MoveCharacter()
        {
            if (!canMove || mouseOut)
            {
                return;
            }

            if (noclip)
            {
                this._rigidbody.isKinematic = true;
                this._rigidbody.useGravity = false;
                this._collider.enabled = false;

                if (Input.GetKeyDown(KeyCode.W))
                {
                    W = true;
                }
                else if (Input.GetKeyUp(KeyCode.W))
                {
                    W = false;
                }                
                
                if (Input.GetKeyDown(KeyCode.A))
                {
                    A = true;
                }
                else if (Input.GetKeyUp(KeyCode.A))
                {
                    A = false;
                }                
                
                if (Input.GetKeyDown(KeyCode.S))
                {
                    S = true;
                }
                else if (Input.GetKeyUp(KeyCode.S))
                {
                    S = false;
                }               
                
                if (Input.GetKeyDown(KeyCode.D))
                {
                    D = true;
                }
                else if (Input.GetKeyUp(KeyCode.D))
                {
                    D = false;
                }            
                
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    Sh = true;
                }
                else if (Input.GetKeyUp(KeyCode.W))
                {
                    Sh = false;
                }

                if (!Sh)
                {
                    float speed = 0.075f;
                    if (W)
                    {
                        this.transform.position += camera.transform.forward.normalized * speed;
                    }

                    if (A)
                    {
                        this.transform.position += -camera.transform.right.normalized * speed;
                    }

                    if (S)
                    {
                        this.transform.position += -camera.transform.forward.normalized * speed;
                    }

                    if (D)
                    {
                        this.transform.position += camera.transform.right.normalized * speed;
                    }
                }
                else
                {
                    float speed = 0.5f;
                    if (W)
                    {
                        this.transform.position += camera.transform.forward.normalized * speed;
                    }

                    if (A)
                    {
                        this.transform.position += -camera.transform.right.normalized * speed;
                    }

                    if (S)
                    {
                        this.transform.position += -camera.transform.forward.normalized * speed;
                    }

                    if (D)
                    {
                        this.transform.position += camera.transform.right.normalized * speed;
                    }
                }

                return;
            }
            else
            {
                this._rigidbody.isKinematic = false;
                this._rigidbody.useGravity = true;
                this._collider.enabled = true;
            }

            var direction = new Vector3(input.Move, 0f, input.Strafe).normalized;
            var worldDirection = transform.TransformDirection(direction);

            var velocity = new Vector3();
            if (input.Run && currentSprintTime > 0)
            {
                velocity = worldDirection * runningSpeed;
            }
            else
            {
                velocity = worldDirection * walkingSpeed;
            }
            //Checks for collisions so that the character does not stuck when jumping against walls.
            var intersectsWall = CheckCollisionsWithWalls(velocity);
            if (intersectsWall)
            {
                _velocityX.Current = _velocityZ.Current = 0f;
                return;
            }

            var smoothX = _velocityX.Update(velocity.x, movementSmoothness);
            var smoothZ = _velocityZ.Update(velocity.z, movementSmoothness);
            var rigidbodyVelocity = _rigidbody.velocity;
            var force = new Vector3(smoothX - rigidbodyVelocity.x, 0f, smoothZ - rigidbodyVelocity.z);
            _rigidbody.AddForce(force, ForceMode.VelocityChange);
        }

        private bool CheckCollisionsWithWalls(Vector3 velocity)
        {
            if (_isGrounded) return false;
            var bounds = _collider.bounds;
            var radius = _collider.radius;
            var halfHeight = _collider.height * 0.5f - radius * 1.0f;
            var point1 = bounds.center;
            point1.y += halfHeight;
            var point2 = bounds.center;
            point2.y -= halfHeight;
            Physics.CapsuleCastNonAlloc(point1, point2, radius, velocity.normalized, _wallCastResults,
                radius * 0.04f, ~0, QueryTriggerInteraction.Ignore);
            var collides = _wallCastResults.Any(hit => hit.collider != null && hit.collider != _collider);
            if (!collides) return false;
            for (var i = 0; i < _wallCastResults.Length; i++)
            {
                _wallCastResults[i] = new RaycastHit();
            }

            return true;
        }

        private void Jump()
        {
            if (jumpForce == 0)
            {
                return;
            }
            if (!_isGrounded || !input.Jump) return;
            _isGrounded = false;
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private void PlayFootstepSounds()
        {
            if (_isGrounded && _rigidbody.velocity.sqrMagnitude > 0.1f)
            {
                _audioSource.clip = input.Run ? runningSound : walkingSound;
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
            else
            {
                if (_audioSource.isPlaying)
                {
                    _audioSource.Pause();
                }
            }
        }
			
        /// A helper for assistance with smoothing the camera rotation.
        private class SmoothRotation
        {
            private float _current;
            private float _currentVelocity;

            public SmoothRotation(float startAngle)
            {
                _current = startAngle;
            }
				
            /// Returns the smoothed rotation.
            public float Update(float target, float smoothTime)
            {
                return _current = Mathf.SmoothDampAngle(_current, target, ref _currentVelocity, smoothTime);
            }

            public float Current
            {
                set { _current = value; }
            }
        }
			
        /// A helper for assistance with smoothing the movement.
        private class SmoothVelocity
        {
            private float _current;
            private float _currentVelocity;

            /// Returns the smoothed velocity.
            public float Update(float target, float smoothTime)
            {
                return _current = Mathf.SmoothDamp(_current, target, ref _currentVelocity, smoothTime);
            }

            public float Current
            {
                set { _current = value; }
            }
        }
			
        /// Input mappings
        [Serializable]
        private class FpsInput
        {
            [Tooltip("The name of the virtual axis mapped to rotate the camera around the y axis."),
             SerializeField]
            private string rotateX = "Mouse X";

            [Tooltip("The name of the virtual axis mapped to rotate the camera around the x axis."),
             SerializeField]
            private string rotateY = "Mouse Y";

            [Tooltip("The name of the virtual axis mapped to move the character back and forth."),
             SerializeField]
            private string move = "Horizontal";

            [Tooltip("The name of the virtual axis mapped to move the character left and right."),
             SerializeField]
            private string strafe = "Vertical";

            [Tooltip("The name of the virtual button mapped to run."),
             SerializeField]
            private string run = "Fire3";

            [Tooltip("The name of the virtual button mapped to jump."),
             SerializeField]
            private string jump = "Jump";

            /// Returns the value of the virtual axis mapped to rotate the camera around the y axis.
            public float RotateX
            {
                get { return Input.GetAxisRaw(rotateX); }
            }
				         
            /// Returns the value of the virtual axis mapped to rotate the camera around the x axis.        
            public float RotateY
            {
                get { return Input.GetAxisRaw(rotateY); }
            }
				        
            /// Returns the value of the virtual axis mapped to move the character back and forth.        
            public float Move
            {
                get { return Input.GetAxisRaw(move); }
            }
				       
            /// Returns the value of the virtual axis mapped to move the character left and right.         
            public float Strafe
            {
                get { return Input.GetAxisRaw(strafe); }
            }
				    
            /// Returns true while the virtual button mapped to run is held down.          
            public bool Run
            {
                get { return Input.GetButton(run); }
            }
				     
            /// Returns true during the frame the user pressed down the virtual button mapped to jump.          
            public bool Jump
            {
                get { return Input.GetButtonDown(jump); }
            }
        }
    }
}