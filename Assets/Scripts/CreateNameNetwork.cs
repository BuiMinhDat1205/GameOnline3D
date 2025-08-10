using UnityEngine;
using TMPro;
using Fusion;

public class CreateNameNetwork : NetworkBehaviour
{
    [Networked]
    public string tenNguoiChoi { get; set; }

    [SerializeField] private TextMeshProUGUI ten;

    public override void Spawned()
    {
        // When the object is spawned, update the name UI
        if (ten != null)
        {
            ten.text = tenNguoiChoi;
            Debug.Log($"Spawned with name: {tenNguoiChoi}");
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference not assigned in Inspector.");
        }
    }

    public void ThemTen(string name)
    {
        Debug.Log($"ThemTen called with name: {name}");
        tenNguoiChoi = name;

        if (ten != null)
        {
            ten.text = name;
        }
    }
}