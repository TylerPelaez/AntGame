using DG.Tweening;
using UnityEngine;

public class RollableCan : MonoBehaviour
{
    [SerializeField]
    private RollCanDirectionalTrigger rollLeft;

    [SerializeField]
    private RollCanDirectionalTrigger rollRight;

    [SerializeField]
    private float rollCanTimeThresholdSeconds = 1.0f;


    [SerializeField]
    private GameObject modelObject;
    
    [SerializeField]
    private float rollXRotationIncrement = 45f;

    [SerializeField]
    private float rollXTranslationIncrement = 0.1f;

    [SerializeField]
    private float rotationDurationSeconds = 0.5f;
    
    [SerializeField]
    private bool rollActive;
    private float currentRollTimeBeginSeconds;
    [SerializeField]
    private Vector3 currentRollDirection;

    [SerializeField]
    private float rollCheckRadius = 0.35f;

    [SerializeField]
    private LayerMask rollCheckMask;


    private GameObject player;
    
    public void OnRollActive(Vector3 direction)
    {
        if (!rollActive)
        {
            currentRollTimeBeginSeconds = Time.time;
            currentRollDirection = direction;
            rollActive = true;
        }
        
    }

    public void OnRollCancelled(Vector3 direction)
    {
        if (!rollActive || direction != currentRollDirection)
            return;

        rollActive = false;
    }

    private void Update()
    {
        if (rollActive && (Time.time - currentRollTimeBeginSeconds) > rollCanTimeThresholdSeconds && CanRoll())
        {
            DoRoll();
        }
    }

    private bool CanRoll()
    {
        var movementDirection = transform.TransformDirection(currentRollDirection);
        return !Physics.SphereCast(transform.position, rollCheckRadius, movementDirection, out var hitInfo, rollXTranslationIncrement, rollCheckMask);
    }
    
    private void DoRoll()
    {
        rollActive = false;
        var movementDirection = transform.TransformDirection(currentRollDirection);
        modelObject.transform.DOLocalRotate(
            modelObject.transform.localRotation.eulerAngles +
            new Vector3(0, 0, rollXRotationIncrement * -currentRollDirection.x), rotationDurationSeconds, RotateMode.FastBeyond360);

        
        transform.DOLocalMove(transform.localPosition + (movementDirection * rollXTranslationIncrement), rotationDurationSeconds);
        player.transform.DOLocalMove(player.transform.localPosition + (movementDirection * rollXTranslationIncrement), rotationDurationSeconds);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject;
            UICanvas.Instance.SetPromptText("Move Left And Right To Roll");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = null;
            UICanvas.Instance.HidePromptText();
        }
    }
}
