using UnityEngine;

public class BulletFactory : SingletonMonoBehaviour<BulletFactory>
{
    [SerializeField] private SerializableDictionary<string, GameObject> bulletPrefabs;
    [SerializeField] private Transform bulletContainer;
    
    private float _bulletScale = 1f;
    
    public void EnlargeBullet()
    {
        _bulletScale = 1.5f;
    }
    
    public void DestroyAllBullets()
    {
        foreach (Transform child in bulletContainer)
        {
            Destroy(child.gameObject);
        }
    }
    
    public void CreateBullet(string bulletType, Vector2 position, float rotation, int damage, float speed, float lifeTime)
    {
        if (bulletPrefabs.TryGetValue(bulletType, out var prefab))
        {
            var bullet = Instantiate(prefab, position, Quaternion.Euler(0, 0, rotation), bulletContainer);
            bullet.GetComponent<BulletBase>().Init(damage, speed, lifeTime);
            
            bullet.transform.localScale *= _bulletScale;
        }
    }
}
