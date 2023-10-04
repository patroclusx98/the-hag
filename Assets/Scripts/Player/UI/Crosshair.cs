using UnityEngine;

[System.Serializable]
public class Crosshair
{
    public string name;
    public Sprite sprite;
    [Range(0.01f, 10f)]
    public float scale;
}