using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;
    private float speed;
    private Vector3 direction;

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Update()
    {
        // Example of how you might use the speed and direction
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
