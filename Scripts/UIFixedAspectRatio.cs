using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class FitUIToAspectRatio : MonoBehaviour
{
    public AutoFitSpriteToCamera background;
    public float targetWidth = 16f;
    public float targetHeight = 9f;

    private RectTransform rectTransform;
    private Vector2 lastScreenSize;

    private void OnEnable()
    {
        if (background == null)
        {
            Debug.LogError($"{this.GetType().Name}: Background is not set in {name}");
        }
    }
    
#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying && background != null)
        {
            if (background.fitStyle == AutoFitSpriteToCamera.FitStyle.FitToWidth)
            {
                Vector2 current = new Vector2(Screen.width, Screen.height);
                if (current != lastScreenSize)
                {
                    FixAspectRatio();
                }
            }
        }
    }
#endif

    void FixAspectRatio()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        
        float screenAspect = (float)Screen.width / Screen.height;
        float targetAspect = targetWidth / targetHeight;
            
        if (screenAspect > targetAspect)
        {
            // Wider screen: match height, pad width (pillarbox)
            float normalizedWidth = targetAspect / screenAspect;
            rectTransform.anchorMin = new Vector2(0.5f - normalizedWidth / 2f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f + normalizedWidth / 2f, 1f);
        }
        else
        {
            // Taller screen: match width, pad height (letterbox)
            float normalizedHeight = screenAspect / targetAspect;
            rectTransform.anchorMin = new Vector2(0f, 0.5f - normalizedHeight / 2f);
            rectTransform.anchorMax = new Vector2(1f, 0.5f + normalizedHeight / 2f);
        }

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}