using DG.Tweening;
using UnityEngine;

public class Roomba : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 10f;

    [SerializeField]
    private float rotationSpeed = 10f;
    
    [SerializeField]
    private int currentPatrolPoint;
    
    
    [SerializeField]
    private Vector3[] patrolPoints;
    
    private LineRenderer lineRenderer;
    private Rigidbody body;
    
    [SerializeField]
    private State state = State.Uninitialized;
    
    
    
    private enum State
    {
        Uninitialized,
        Idle,
        Translating,
        Rotating
    }
    
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer != null && lineRenderer.enabled)
        {
            patrolPoints = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(patrolPoints);
            lineRenderer.enabled = false;
            currentPatrolPoint = 0;

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                patrolPoints[i].y = transform.position.y;
            }

            transform.position = patrolPoints[0];
            transform.forward = (patrolPoints[1] - patrolPoints[0]).normalized;
            StartTranslate();
        }
    }

    private void StartTranslate()
    {
        var nextPatrolPoint = currentPatrolPoint + 1 == patrolPoints.Length ? 0 : currentPatrolPoint + 1;
        var travelDistance = Vector3.Distance(patrolPoints[currentPatrolPoint], patrolPoints[nextPatrolPoint]);        
        
        var moveTween = body.DOMove(patrolPoints[nextPatrolPoint], travelDistance / movementSpeed);
        moveTween.SetUpdate(UpdateType.Fixed);
        moveTween.OnComplete(StartRotate);
        
        state = State.Translating;
    }

    private void StartRotate()
    {
        currentPatrolPoint = currentPatrolPoint + 1 == patrolPoints.Length ? 0 : currentPatrolPoint + 1;
        var nextPatrolPoint = currentPatrolPoint + 1 == patrolPoints.Length ? 0 : currentPatrolPoint + 1;
        
        var toNextPointDirection = (patrolPoints[nextPatrolPoint] - patrolPoints[currentPatrolPoint]).normalized;
        var angles = Vector3.Angle(transform.forward, toNextPointDirection);

        var rotateTween = body.DORotate(new Vector3(0, transform.rotation.eulerAngles.y + angles, 0), angles / rotationSpeed);
        rotateTween.SetUpdate(UpdateType.Fixed);

        rotateTween.OnComplete(StartTranslate);

        state = State.Rotating;
    }
}
