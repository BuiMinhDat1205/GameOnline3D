using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public static event Action<GameObject> OnPlayerAttack;

    public List<Transform> attackTargets = new List<Transform>(); // danh sách mục tiêu

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (attackTargets != null && attackTargets.Count > 0)
            {
                foreach (Transform target in attackTargets)
                {
                    if (target != null)
                    {
                        Debug.Log("Player tấn công " + target.name);
                        OnPlayerAttack?.Invoke(target.gameObject); // Gửi tới Pet
                    }
                }
            }
        }
    }
}
