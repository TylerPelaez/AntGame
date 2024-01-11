using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private WinArea winArea;

    public int FoodCollected { get; private set; }
    public int TotalFood { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        var foodItems = FindObjectsOfType<FoodItem>();
        TotalFood = foodItems.Length;
        foreach (var foodItem in foodItems)
        {
            foodItem.OnCollect.AddListener(OnFoodItemCollected);
        }
    }

    void OnFoodItemCollected()
    {
        FoodCollected++;

        if (FoodCollected == TotalFood)
        {
            winArea.canWin = true;
        }
    }
}
