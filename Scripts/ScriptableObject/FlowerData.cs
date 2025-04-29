using UnityEngine;

[CreateAssetMenu(fileName = "FlowerData", menuName = "Scriptable Objects/FlowerData")]
public class FlowerData : ScriptableObject
{
    [Header("Flower Info")]
    public string className;
    public string displayName;
    [TextArea(1, 5)]
    public string description;
    public Sprite itemImage;
    public Sprite image;
    
    [Header("Flower Stats")]
    public int cost;
    public float range;
    public float interval;
    public int maxHealth;
    [Header("Bullet Stats")]
    public int bulletDamage;
    public float bulletSpeed;
    public float bulletLifeTime;
}
