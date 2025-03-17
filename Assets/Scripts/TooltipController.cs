using TMPro;
using UnityEngine;

public class TooltipController : MonoBehaviour
{
    public TextMeshProUGUI tooltipTitle;
    public TextMeshProUGUI tooltipDescription;
    public TextMeshProUGUI tooltipPowerDescription;
    
    private RectTransform _rectTransform;

    private Canvas _canvas;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
        _canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform, 
            Input.mousePosition, 
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out var mousePos);

        _rectTransform.anchoredPosition = mousePos + new Vector2(15f, 80f);
    }

    public void ShowTooltip(string title, string desc, string powerDesc)
    {
        tooltipTitle.text = title;
        tooltipDescription.text = desc;
        tooltipPowerDescription.text = powerDesc;
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
