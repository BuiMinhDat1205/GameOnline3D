using UnityEngine;
using Fusion;

public class BulletScript : NetworkBehaviour
{
    public float lifetime = 3f;
    public float speed = 15f;


    [Networked]
    public PlayerRef Owner { get; set; }

    private float timer = 0f;

    void Update()
    {
        // Tăng thời gian sống
        if (Object.HasStateAuthority)
        {
            timer += Time.deltaTime;

            if (timer >= lifetime)
            {
                Runner.Despawn(Object);
            }
        }
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        // Nếu va vào player đã bắn thì bỏ qua
        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj != null && netObj.InputAuthority == Owner)
        {
            return;
        }

        // Hủy nếu va vào bất kỳ thứ gì khác
        Runner.Despawn(Object);
    }
}
