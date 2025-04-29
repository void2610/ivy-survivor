using UnityEngine;

public class IvyObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyBase>(out var enemy))
        {
            var speedDown = IvyManager.Instance.GetIvySpeedDown();
            enemy.SpeedDown(speedDown).Forget();
            enemy.TakeDamage(IvyManager.Instance.GetIvyDamage());
            
            if(enemy.GetCollideIvyCount() >= 2) EventManager.OnEnemyInDoubleIvy.Trigger(enemy);
        }
    }
}
