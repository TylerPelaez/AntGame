using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PopupToaster : MonoBehaviour
{

    [SerializeField]
    private Animation waffle1Animation, waffle2Animation;

    [SerializeField]
    private GameObject slider;
    
    [SerializeField]
    private float sliderMinY = 0.45f;

    [SerializeField]
    private float popupDelaySeconds = 1f;
    
    private Vector3 sliderOriginalPosition;
    
    private Rigidbody sliderRigidBody;

    private bool poppingUp;
    
    private void Start()
    {
        sliderRigidBody = slider.GetComponent<Rigidbody>();
        sliderOriginalPosition = slider.transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (slider.transform.localPosition.y < sliderMinY && !poppingUp)
        {
            StartPopup();
        }
    }

    void StartPopup()
    {
        //TODO: start playing sound
        sliderRigidBody.isKinematic = true;
        poppingUp = true;
        StartCoroutine(DoPopup());
    }

    IEnumerator DoPopup()
    {
        yield return new WaitForSeconds(popupDelaySeconds);
        if (waffle1Animation)
            waffle1Animation.Play();
        if (waffle2Animation)
            waffle2Animation.Play();
        var tween = slider.transform.DOLocalMove(sliderOriginalPosition, waffle1Animation.GetClip("Waffle").length);
        tween.OnComplete(() =>
        {
            sliderRigidBody.isKinematic = false;
            poppingUp = false;
        });
    }
}
