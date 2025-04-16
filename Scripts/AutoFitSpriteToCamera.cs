using System;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class AutoFitSpriteToCamera : MonoBehaviour
{
    public enum FitStyle
    {
        FitToWidth,
        FitToHeight
    }
    
    public FitStyle fitStyle = FitStyle.FitToWidth;
    private SpriteRenderer sr;

    private void Start()
    {
        FitToCamera();
    }

    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        FitToCamera();
    }

    private void OnValidate()
    { 
        FitToCamera();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
        {
            FitToCamera();
        }
    }
#endif

    void FitToCamera()
    {
        switch (fitStyle)
        {
            case FitStyle.FitToWidth:
                FitToCameraWidth();
                break;
            case FitStyle.FitToHeight:
                FitToCameraHeight();
                break; 
        }
    }
    
    void FitToCameraWidth()
        {
            if (Camera.main == null || sr == null || sr.sprite == null)
                return;
    
            float cameraHeight = Camera.main.orthographicSize * 2f;
            float cameraWidth = cameraHeight * Camera.main.aspect;
    
            float spriteWidth = sr.sprite.bounds.size.x;
            float scaleX = cameraWidth / spriteWidth;
    
            // Scale uniformly based on width
            transform.localScale = new Vector3(scaleX, scaleX, 1f);
        }

    void FitToCameraHeight()
    {
        if (Camera.main == null || sr == null || sr.sprite == null)
            return;

        float cameraHeight = Camera.main.orthographicSize * 2f;

        float spriteHeight = sr.sprite.bounds.size.y;
        float scaleY = cameraHeight / spriteHeight;

        // Scale uniformly based on height
        transform.localScale = new Vector3(scaleY, scaleY, 1f);
    }
}