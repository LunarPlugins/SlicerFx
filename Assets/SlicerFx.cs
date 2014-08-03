//
// Slicer FX
//
// Copyright (C) 2013, 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SlicerFx : MonoBehaviour
{
    // Slicer mode (directional or spherical)
    public enum SlicerMode { Directional, Spherical }
    [SerializeField] SlicerMode _mode = SlicerMode.Directional;
    public SlicerMode mode { get { return _mode; } set { _mode = value; } }

    // Slicer direction (used only in the directional mode)
    [SerializeField] Vector3 _direction = Vector3.forward;
    public Vector3 direction { get { return _direction; } set { _direction = value; } }

    // Slicer origin (used only in the spherical mode)
    [SerializeField] Vector3 _origin = Vector3.zero;
    public Vector3 origin { get { return _origin; } set { _origin = value; } }

    // Albedo color
    [SerializeField] Color _albedo = new Color(0.5f, 0.5f, 0.5f, 0);
    public Color albedo { get { return _albedo; } set { _albedo = value; } }

    // Emission color
    [SerializeField] Color _emission = new Color(0.2f, 0.2f, 0.2f, 0);
    public Color emission { get { return _emission; } set { _emission = value; } }

    // Interval between stripes
    [SerializeField] float _interval = 1.0f;
    public float interval { get { return _interval; } set { _interval = value; } }

    // Threshold for clipping
    [SerializeField] float _threshold = 0.5f;
    public float threshold { get { return _threshold; } set { _threshold = value; } }

    // Scroll speed
    [SerializeField] float _scrollSpeed = 1.0f;
    public float scrollSpeed { get { return _scrollSpeed; } set { _scrollSpeed = value; } }

    // Private shader variables
    Shader shader;
    int albedoID;
    int emissionID;
    int paramsID;
    int vectorID;

    void Awake()
    {
        shader = Shader.Find("Hidden/SlicerFX");

        albedoID   = Shader.PropertyToID("_SlicerAlbedo");
        emissionID = Shader.PropertyToID("_SlicerEmission");
        paramsID   = Shader.PropertyToID("_SlicerParams");
        vectorID   = Shader.PropertyToID("_SlicerVector");
    }

    void OnEnable()
    {
        camera.SetReplacementShader(shader, null);
        Update();
    }

    void OnDisable()
    {
        camera.ResetReplacementShader();
    }

    void Update()
    {
        Shader.SetGlobalColor(albedoID, _albedo);
        Shader.SetGlobalColor(emissionID, _emission);

        var param = new Vector4(_scrollSpeed, _interval, _threshold, 0);
        Shader.SetGlobalVector(paramsID, param);

        if (_mode == SlicerMode.Directional)
        {
            Shader.DisableKeyword("SLICER_SPHERICAL");
            Shader.SetGlobalVector(vectorID, _direction.normalized);
        }
        else
        {
            Shader.EnableKeyword("SLICER_SPHERICAL");
            Shader.SetGlobalVector(vectorID, _origin);
        }
    }
}
