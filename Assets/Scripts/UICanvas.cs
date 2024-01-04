using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField]
    private GameManager manager;

    [SerializeField]
    private TextMeshProUGUI textMesh;

    // Update is called once per frame
    void Update()
    {
        textMesh.text = $"{manager.FoodCollected}/{manager.TotalFood}";
    }
}
