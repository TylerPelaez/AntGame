using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //https://catlikecoding.com/unity/tutorials/movement/physics/ for all of this movement code 
    [SerializeField, Range(0f, 10f)]
    float maxSpeed = 6f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f, maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;

    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;

    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 40f;
    
    [SerializeField, Min(0f)]
    float probeDistance = 1f;
    
    [SerializeField]
    LayerMask probeMask = -1;
    
    [SerializeField]
    Transform playerInputSpace = default;

    [SerializeField]
    private int coyoteTime = 15;

    [SerializeField]
    private float soapFieldDurationSeconds = 2.0f;

    [SerializeField, Range(0f, 1f)]
    private float soapFieldGravityModifier = 0.5f;

    [SerializeField]
    private float modelTurnSpeed = 100f;

    Rigidbody body, connectedBody, previousConnectedBody;
    Vector3 velocity, desiredVelocity, connectionVelocity, prevConnectionVelocity;
    
    Vector3 contactNormal, steepNormal;
    bool desiredJump;
    int groundContactCount, steepContactCount;
    bool OnGround => groundContactCount > 0;
    bool OnSteep => steepContactCount > 0;
    
    int jumpPhase;
    float minGroundDotProduct;
    int stepsSinceLastGrounded, stepsSinceLastJump;

    [SerializeField] 
    private InputActionReference movement, jump;

    [SerializeField] 
    private GameObject modelObject;

    [SerializeField]
    private LayerMask antPushLayerMask;

    [SerializeField]
    private float antPushStrength;
    
    private Vector3 upAxis;

    private float lastInSoapFieldTime = -999999;
    
    Vector3 connectionWorldPosition, connectionLocalPosition;

    private int antPushCount;
    private Vector3 antPushOrigin, antPushDirection, antPushVelocity;

    [SerializeField]
    private Animator animator;

    private static readonly int Walk = Animator.StringToHash("Walk");

    void Awake () {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        OnValidate();
    }
    
    private void OnEnable()
    {
        jump.action.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        jump.action.performed -= OnJumpPerformed;
    }
    
    void OnValidate () {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }
    
    
    // Update is called once per frame
    void Update()
    {
        Vector2 playerInput = movement.action.ReadValue<Vector2>();
        if (playerInputSpace) {
            Vector3 forward = playerInputSpace.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = playerInputSpace.right;
            right.y = 0f;
            right.Normalize();
            desiredVelocity =
                (forward * playerInput.y + right * playerInput.x) * maxSpeed;
        }
        else {
            desiredVelocity =
                new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        }

        if (playerInput != Vector2.zero)
        {
            var newForward = previousConnectedBody != null ? (velocity - prevConnectionVelocity).normalized : velocity.normalized;
            newForward.y = 0;


            if (newForward == Vector3.zero)
            {
                newForward = desiredVelocity;
            }
            
            if (OnGround)
            {
                animator.SetBool(Walk, true);
                newForward = ProjectOnContactPlane(newForward).normalized;
            }
            modelObject.transform.forward = Vector3.Lerp(modelObject.transform.forward, newForward, modelTurnSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool(Walk, false);
        }
    }

    private void FixedUpdate()
    {
        upAxis = -Physics.gravity.normalized;
        UpdateState();
        
        AdjustVelocity();

        if (antPushCount > 0)
        {
            velocity += antPushVelocity;
        }
        
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && OnGround && stepsSinceLastJump > 2)
        {
            animator.Play("Idle");
        }
        
        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        if (OnGround && velocity.sqrMagnitude < 0.01f)
        {
            velocity += contactNormal * Vector3.Dot(Physics.gravity, contactNormal) * Time.deltaTime;
        }
        else
        {
            var soapMod = (Time.time - lastInSoapFieldTime) > soapFieldDurationSeconds ? 1.0f : soapFieldGravityModifier;
            
            velocity += Physics.gravity * Time.deltaTime * soapMod;
        }

        body.velocity = velocity;
        ClearState();
    }

    private void ClearState()
    {
        groundContactCount = steepContactCount = antPushCount = 0;
        prevConnectionVelocity = connectionVelocity;
        contactNormal = steepNormal = connectionVelocity = antPushOrigin = antPushVelocity = antPushDirection = Vector3.zero;
        previousConnectedBody = connectedBody;
        connectedBody = null;
    }

    private void Jump()
    {
        if (OnGround || (stepsSinceLastGrounded < coyoteTime && stepsSinceLastJump > coyoteTime)|| jumpPhase < maxAirJumps)
        {
            animator.Play("Jump");
            stepsSinceLastJump = 0;
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(2f * Physics.gravity.magnitude * jumpHeight);
            // float alignedSpeed = Vector3.Dot(velocity, Vector3.up);
            // if (alignedSpeed > 0f) {
            //     jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            // }
            velocity += Vector3.up * jumpSpeed;
        }
    }

    private void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        Vector3 relativeVelocity = (velocity - connectionVelocity) - antPushVelocity;
        float currentX = Vector3.Dot(relativeVelocity, xAxis);
        float currentZ = Vector3.Dot(relativeVelocity, zAxis);

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);
        
        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }



    private void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        velocity = body.velocity;
        if (OnGround || SnapToGround() || CheckSteepContacts())
        {
            stepsSinceLastGrounded = 0;
            jumpPhase = 0;
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = upAxis;
        }
        
        if (connectedBody) {
            if (connectedBody.isKinematic || connectedBody.mass >= body.mass)
            {
                UpdateConnectionState();
            }
        }
        
        if (antPushCount > 0)
        {
            UpdateAntPushState();
        }
    }

    void UpdateAntPushState()
    {
        var correctedOrigin = antPushOrigin / antPushCount;
        var direction = (antPushDirection / antPushCount).normalized;
        antPushVelocity = antPushStrength * Time.deltaTime * direction;
    }

    void UpdateConnectionState()
    {
        if (connectedBody == previousConnectedBody)
        {
            Vector3 connectionMovement = connectedBody.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }

        connectionWorldPosition = body.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(
            connectionWorldPosition
        );
    }
    
    void OnCollisionEnter (Collision collision) {
        EvaluateCollision(collision);
    }

    void OnCollisionStay (Collision collision) {
        EvaluateCollision(collision);
    }

    void EvaluateCollision (Collision collision) {
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(upAxis, normal);
            if (upDot >= minGroundDotProduct) {
                groundContactCount += 1;
                contactNormal += normal;
                connectedBody = collision.rigidbody;
            }
            else if (upDot > -0.01f) {
                steepContactCount += 1;
                steepNormal += normal;
                if (groundContactCount == 0) {
                    connectedBody = collision.rigidbody;
                }
            }
        }
    }

    bool CheckSteepContacts () {
        if (steepContactCount > 1) {
            steepNormal.Normalize();
            float upDot = Vector3.Dot(upAxis, steepNormal);
            if (upDot >= minGroundDotProduct) {
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }
    
    bool SnapToGround () {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) {
            return false;
        }
        if (!Physics.Raycast(body.position, -upAxis, out RaycastHit hit, probeDistance, probeMask)) {
            return false;
        }

        float upDot = Vector3.Dot(upAxis, hit.normal);
        
        if (upDot < minGroundDotProduct) {
            return false;
        }
        groundContactCount = 1;
        contactNormal = hit.normal;
        float speed = velocity.magnitude;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f) {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        connectedBody = hit.rigidbody;
        return true;
    }
    
    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        desiredJump = true;
    }
    
    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    public void ApplySoapFieldEffect()
    {
        lastInSoapFieldTime = Time.time;
    }


    private void EvaluateAntPush(Collider other)
    {
        antPushCount++;
        antPushOrigin += other.gameObject.transform.position;
        antPushDirection += other.gameObject.transform.forward;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (antPushLayerMask == (antPushLayerMask | (1 << other.gameObject.layer)))
        {
            EvaluateAntPush(other);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (antPushLayerMask == (antPushLayerMask | (1 << other.gameObject.layer)))
        {
            EvaluateAntPush(other);
        }
    }

    public void Kill()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
