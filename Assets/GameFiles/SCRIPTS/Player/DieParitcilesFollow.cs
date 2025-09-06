using UnityEngine;

public class DieParticlesFollow : MonoBehaviour
{
    public Transform Player;
    public Transform Particles;

    void Update()
    {
        if (Mathf.Abs(Particles.position.z - Player.position.z) > 0.01f)
        {
            Vector3 pos = Particles.position;
            pos.z = Player.position.z;
            Particles.position = pos;
        }
        if (Mathf.Abs(Particles.position.x - Player.position.x) > 0.01f)
        {
            Vector3 pos = Particles.position;
            pos.x = Player.position.x;
            Particles.position = pos;
        }
        if (Mathf.Abs(Particles.position.y - Player.position.y) > 0.01f)
        {
            Vector3 pos = Particles.position;
            pos.y = Player.position.y;
            Particles.position = pos;
        }
    }
}