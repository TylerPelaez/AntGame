using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Buildable : MonoBehaviour
{
    [SerializeField]
    private GameObject unbuiltObject, builtObject;

    [SerializeField]
    private InputActionReference interact;

    [SerializeField]
    private Animator interactAnimator;

    [SerializeField]
    private CinemachineVirtualCamera cm;
    
    enum State
    {
        Unbuilt,
        Building,
        Built,
        Interacted
    }
    
    private bool canBuild, playerIn;
    private int componentsCollected;
    private int totalComponents;

    public UnityEvent onInteractComplete;

    private ParticleSystem buildParticles;

    [SerializeField]
    private State state;
    
    private void Awake()
    {
        buildParticles = GetComponent<ParticleSystem>();
        var components = FindObjectsOfType<Collectible>();
        totalComponents = components.Length;
        foreach (var component in components)
        {
            component.OnCollect.AddListener(OnComponentCollect);
        }
    }


    private void OnEnable()
    {
        interact.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        interact.action.performed -= OnInteract;
    }


    private void OnComponentCollect()
    {
        componentsCollected++;
        if (componentsCollected == totalComponents)
            canBuild = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIn = true;
            switch (state)
            {
                case State.Unbuilt:
                    UICanvas.Instance.SetPromptText( canBuild ? "Press E To Build" : $"{componentsCollected} / {totalComponents} Components Collected");
                    break;
                case State.Built:
                    UICanvas.Instance.SetPromptText("Press E To Interact");
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIn = false;
            UICanvas.Instance.HidePromptText();
        }
    }

    private void Build()
    {
        state = State.Building;
        cm.Priority += 11;
        UICanvas.Instance.HidePromptText();
        StartCoroutine(MidBuild());
    }

    private IEnumerator MidBuild()
    {
        yield return new WaitForSeconds(.5f);
        buildParticles.Play();
        yield return new WaitForSeconds(1);
        unbuiltObject.SetActive(false);
        builtObject.SetActive(true);
    }

    public void OnParticleSystemStopped()
    {
        state = State.Built;
        cm.Priority -= 11;
        UICanvas.Instance.SetPromptText("Press E To Interact");
    }

    private void DoAction()
    {
        state = State.Interacted;
        cm.Priority += 11;
        interactAnimator.Play("Interact");
        UICanvas.Instance.HidePromptText();
    }

    public void OnAnimationComplete()
    {
        cm.Priority -= 11;
        state = State.Interacted;
        onInteractComplete?.Invoke();
    }
    
    
    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (canBuild && playerIn)
        {
            switch (state)
            {
                case State.Unbuilt:
                    Build();
                    break;
                case State.Built:
                    DoAction();
                    break;    
                default:
                    return;
            }
        }
    }
}
