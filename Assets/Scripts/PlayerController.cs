using UnityEngine;
using UnityEngine.InputSystem;

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
    
    Rigidbody body;
    Vector3 velocity, desiredVelocity;
    
    Vector3 contactNormal;
    bool desiredJump;
    int groundContactCount;
    bool OnGround => groundContactCount > 0;
    int jumpPhase;
    float minGroundDotProduct;
    int stepsSinceLastGrounded, stepsSinceLastJump;

    [SerializeField] 
    private InputActionReference movement, jump;

    [SerializeField] 
    private GameObject modelObject;

    void Awake () {
        body = GetComponent<Rigidbody>();
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
            modelObject.transform.forward = desiredVelocity;
    }

    private void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        body.velocity = velocity;
        ClearState();
    }

    private void ClearState()
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }

    private void Jump()
    {
        if (OnGround || jumpPhase < maxAirJumps)
        {
            stepsSinceLastJump = 0;
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            if (alignedSpeed > 0f) {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            velocity += contactNormal * jumpSpeed;
        }
    }

    private void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

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
        if (OnGround || SnapToGround())
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
            contactNormal = Vector3.up;
        }
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
            if (normal.y >= minGroundDotProduct) {
                groundContactCount += 1;
                contactNormal += normal;
            }
        }
    }

    bool SnapToGround () {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) {
            return false;
        }
        if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask)) {
            Debug.Log("ProbinFail");
            return false;
        }
        if (hit.normal.y < minGroundDotProduct) {
            return false;
        }
        groundContactCount = 1;
        contactNormal = hit.normal;
        float speed = velocity.magnitude;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f) {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
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
}
