using UnityEngine;

public interface IEnemy
{
    void Initialize();
}

public interface IBullet
{
    void Launch(Vector3 direction, float speed);
}