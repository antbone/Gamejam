using UnityEngine;
[ExecuteAlways]
public class DependentScaler : MonoBehaviour
{
    public RectTransform target;
    public float yScale = 1;
    [Header("false:sizeDelta=>yScale   true:yScale=>sizeDelta")]
    public bool isApplyY = false;
    [Header("false:sizeDelta=>xScale   true:xScale=>sizeDelta")]
    public float xScale = 1;
    public bool isApplyX = false;
    STField<DependentScaler, RectTransform> rt = new STField<DependentScaler, RectTransform>(x => x.GetComponent<RectTransform>());
    private void Update()
    {
        if (target)
        {
            RectTransform rect = rt.Get(this);
            rect.sizeDelta = new Vector2(isApplyX ? xScale * target.sizeDelta.x : rect.sizeDelta.x, isApplyY ? yScale * target.sizeDelta.y : rect.sizeDelta.y);
        }
    }
    void OnValidate()
    {
        if (target)
        {
            RectTransform rect = rt.Get(this);
            if (!isApplyY)
                yScale = target.sizeDelta.y == 0 ? 0 : rect.sizeDelta.y / target.sizeDelta.y;
            if (!isApplyX)
                xScale = target.sizeDelta.x == 0 ? 0 : rect.sizeDelta.x / target.sizeDelta.x;
        }

    }
}