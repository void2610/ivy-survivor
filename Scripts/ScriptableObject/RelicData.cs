using UnityEngine;

[CreateAssetMenu(fileName = "RelicData", menuName = "Scriptable Objects/RelicData")]
public class RelicData : ScriptableObject
{
    public string displayName;
    public string className;
    [TextArea(1, 5)]
    public string description;
    public Sprite image;
}
