using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VirtualCamHandler : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera vCam;

    [SerializeField]
    private int vCamInitialPriority = 10;

    [SerializeField]
    private int vCamBoostPriority = 10;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            vCam.Priority = vCamInitialPriority + vCamBoostPriority;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && vCam.Priority > vCamInitialPriority)
        {
            vCam.Priority = vCamInitialPriority;
        }
    }
}
