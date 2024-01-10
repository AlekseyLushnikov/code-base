using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtility 
{
    public static void Crop(this Image image)
    {
        var parent = image.rectTransform.parent;
        if (parent == null)
        {
            Debug.LogError("Can't crop image without container!");
            return;
        }
        var parentRectTransform = parent.GetComponent<RectTransform>();
        var parentSize = parentRectTransform.rect;
        var parentAspect = parentSize.width / (float) parentSize.height;
        var mainTexture = image.mainTexture;
        var aspect = mainTexture.width / (float) mainTexture.height;
        var mainSize = parentAspect > 1 ? parentSize.width : parentSize.height;
        if (parentAspect > aspect)
        {
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mainSize);
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mainSize / aspect);
        }
        else
        {
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mainSize * aspect);
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mainSize);
        }
    }

    public static void Fit(this Image image)
    {
        var parent = image.rectTransform.parent;
        if (parent == null)
        {
            Debug.LogError("Can't fit image without container!");
            return;
        }
        var parentRectTransform = parent.GetComponent<RectTransform>();
        var parentSize = parentRectTransform.rect;
        var parentAspect = parentSize.width / parentSize.height;
        var mainTexture = image.mainTexture;
        var aspect = mainTexture.width / mainTexture.height;
        if (parentAspect > aspect)
        {
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentSize.width  * aspect);
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentSize.width);
        }
        else
        {
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentSize.height );
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentSize.height / aspect);
        }
    }

    public static Vector2 ScaleWithAspect(this Image image, float targetFrameSize)
    {
        var size = image.sprite.rect;
        var aspect = size.width / size.height;
        var resultSize = aspect < 1 
            ? new Vector2(targetFrameSize * aspect, targetFrameSize ) 
            : new Vector2(targetFrameSize , targetFrameSize * aspect);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, resultSize.x);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,  resultSize.y);
        return resultSize;
    }
    public static void Crop(this RawImage image)
    {
        var texture = image.texture;
        var parent = image.rectTransform.parent;
        if (parent == null)
        {
            Debug.LogError("Can't crop image without container!");
            return;
        }
        var parentRectTransform = parent.GetComponent<RectTransform>();
        var parentSize = parentRectTransform.rect;
        var parentAspect = parentSize.width / parentSize.height;
        var aspect =  (float) texture.width / texture.height;
        if (parentAspect > aspect)
        {
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentSize.width);
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentSize.width / aspect);
        }
        else
        {
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentSize.height * aspect);
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentSize.height);
        }
    }

    public static void Fit(this RawImage image)
    {
        var texture = image.texture;;
        var parent = image.rectTransform.parent;
        if (parent == null)
        {
            Debug.LogError("Can't fit image without container!");
            return;
        }
        var parentRectTransform = parent.GetComponent<RectTransform>();
        var parentSize = parentRectTransform.rect;
        var parentAspect = parentSize.width / parentSize.height;
        var aspect =  (float) texture.width / texture.height;
        if (parentAspect > aspect)
        {
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentSize.width  * aspect);
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentSize.width);
        }
        else
        {
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentSize.height );
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentSize.height / aspect);
        }
    }

    public static Vector2 ScaleWithAspect(this RawImage image, float targetFrameSize)
    {
        var texture = image.texture;
        var aspect =  (float) texture.width / texture.height;
        var resultSize = aspect < 1 
            ? new Vector2(targetFrameSize * aspect, targetFrameSize ) 
            : new Vector2(targetFrameSize , targetFrameSize * aspect);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, resultSize.x);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,  resultSize.y);
        return resultSize;
    }
}
