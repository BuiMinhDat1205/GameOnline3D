using Fusion;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public float moveSpeed = 5f;

    private void Update()
    {
        if (!HasStateAuthority) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
