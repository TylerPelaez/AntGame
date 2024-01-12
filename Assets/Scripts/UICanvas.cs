using System.Collections;
using TMPro;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField]
    private GameManager manager;

    [SerializeField]
    private TextMeshProUGUI textMesh;


    [SerializeField]
    private TextMeshProUGUI promptText;
    
    public static UICanvas Instance;

    private void Start()
    {
        Instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        textMesh.text = $"{manager.FoodCollected}/{manager.TotalFood}";
    }

    public void SetDisappearingPromptText(string txt)
    {
        SetPromptText(txt);
        StartCoroutine(DelayedHide());
    }

    private IEnumerator DelayedHide()
    {
        yield return new WaitForSeconds(2f);
        HidePromptText();
    }
    
    public void SetPromptText(string txt)
    {
        promptText.text = txt;
        promptText.gameObject.SetActive(true);
    }

    public void HidePromptText()
    {
        promptText.gameObject.SetActive(false);
    }
}
