// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using Meta.XR.Samples;
using UnityEngine;

[MetaCodeSample("StarterSample-Passthrough")]
public class PassthroughStyler : MonoBehaviour
{
    private const float FadeDuration = 0.2f;

    [SerializeField]
    private OVRPassthroughLayer _passthroughLayer;

    [SerializeField]
    private RectTransform _colorWheel;

    [SerializeField]
    private Texture2D _colorTexture;

    [SerializeField]
    private Texture2D _colorLutTexture;

    [SerializeField]
    private CanvasGroup _mainCanvas;

    [SerializeField]
    private GameObject[] _compactObjects;

    [SerializeField]
    private GameObject[] _objectsToHideForBlackAndWhitePassthrough;

    private LaserPointer _laserPointer;

    private Vector3 _cursorPosition = Vector3.zero;
    private bool _settingColor = false;
    private Color _savedColor = Color.white;
    private float _savedBrightness = 0.0f;
    private float _savedContrast = 0.0f;
    private float _savedSaturation = 0.0f;

    private OVRPassthroughLayer.ColorMapEditorType _currentStyle =
        OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment;

    private float _savedBlend = 1;
    private OVRPassthroughColorLut _passthroughColorLut;
    private IEnumerator _fade;

    private void Start()
    {
        _savedColor = new Color(1, 1, 1, 0);
        _passthroughColorLut = new OVRPassthroughColorLut(_colorLutTexture);
        _laserPointer = FindAnyObjectByType<LaserPointer>();
        if (_laserPointer != null)
        {
            _laserPointer.SetCursorPosition += Cursor;
        }
    }

    private void OnDestroy()
    {
        if (_laserPointer != null)
        {
            _laserPointer.SetCursorPosition -= Cursor;
        }
    }

    private void Update()
    {
        if (_settingColor)
        {
            GetColorFromWheel();
        }
    }

    /// <summary>
    /// Called from editor
    /// </summary>
    public void OnBrightnessChanged(float newValue)
    {
        _savedBrightness = newValue;
        UpdateBrighnessContrastSaturation();
    }

    /// <summary>
    /// Called from editor
    /// </summary>
    public void OnContrastChanged(float newValue)
    {
        _savedContrast = newValue;
        UpdateBrighnessContrastSaturation();
    }

    /// <summary>
    /// Called from editor
    /// </summary>
    public void OnSaturationChanged(float newValue)
    {
        _savedSaturation = newValue;
        UpdateBrighnessContrastSaturation();
    }

    /// <summary>
    /// Called from editor
    /// </summary>
    public void OnAlphaChanged(float newValue)
    {
        _passthroughLayer.edgeRenderingEnabled = newValue > float.Epsilon;
        _savedColor = new Color(_savedColor.r, _savedColor.g, _savedColor.b, newValue);
        _passthroughLayer.edgeColor = _savedColor;
    }

    /// <summary>
    /// Called from editor
    /// </summary>
    public void OnBlendChange(float newValue)
    {
        _savedBlend = newValue;
        _passthroughLayer.SetColorLut(_passthroughColorLut, _savedBlend);
    }

    /// <summary>
    /// Called from editor
    /// </summary>
    public void DoColorDrag(bool doDrag)
    {
        _settingColor = doDrag;
    }

    /// <summary>
    /// Called from editor
    /// </summary>
    public void SetPassthroughStyleToColorAdjustment(bool isOn)
    {
        if (isOn)
        {
            SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment);
        }
    }

    /// <summary>
    /// Called from editor
    /// </summary>
    public void SetPassthroughStyleToColorLut(bool isOn)
    {
        if (isOn)
        {
            SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType.ColorLut);
        }
    }

    private void UpdateBrighnessContrastSaturation()
    {
        _passthroughLayer.SetBrightnessContrastSaturation(_savedBrightness, _savedContrast, _savedSaturation);
    }

    private void Cursor(Vector3 cP)
    {
        _cursorPosition = cP;
    }

    private void GetColorFromWheel()
    {
        // convert cursor world position to UV
        var localPos = _colorWheel.transform.InverseTransformPoint(_cursorPosition);
        var toImg = new Vector2(localPos.x / _colorWheel.sizeDelta.x + 0.5f,
            localPos.y / _colorWheel.sizeDelta.y + 0.5f);
        Color sampledColor = Color.black;
        if (toImg.x < 1.0 && toImg.x > 0.0f && toImg.y < 1.0 && toImg.y > 0.0f)
        {
            int Upos = Mathf.RoundToInt(toImg.x * _colorTexture.width);
            int Vpos = Mathf.RoundToInt(toImg.y * _colorTexture.height);
            sampledColor = _colorTexture.GetPixel(Upos, Vpos);
            // For some reason, perhaps it's the compression of the color wheel image, the very edges will incorrectly
            // have rgb values of 1,1,1. Alpha is still 0 which is correct and the image shows that it has a fully transparent
            // background but our assumption that the color values there are 0 are incorrect. If alpha is 0, the output needs to be
            // 0,0,0 as well. This fixes that
            sampledColor *= sampledColor.a;
        }

        _savedColor = new Color(sampledColor.r, sampledColor.g, sampledColor.b, _savedColor.a);
        _passthroughLayer.edgeColor = _savedColor;
    }

    private void SetPassthroughStyle(OVRPassthroughLayer.ColorMapEditorType passthroughStyle)
    {
        _currentStyle = passthroughStyle;
        if (_currentStyle == OVRPassthroughLayer.ColorMapEditorType.ColorLut)
        {
            _passthroughLayer.SetColorLut(_passthroughColorLut, _savedBlend);
        }
        else
        {
            UpdateBrighnessContrastSaturation();
        }
    }
}
