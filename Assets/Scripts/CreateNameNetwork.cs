using UnityEngine;
using TMPro;
using Fusion;

public class CreateNameNetwork : NetworkBehaviour
{
    [Networked]
    public string tenNguoiChoi { get; set; }

    [SerializeField] private TextMeshProUGUI ten;

    // 🆕 Thêm biến để hiện text local (chỉ local player thấy)
    [SerializeField] private TextMeshProUGUI localNote;

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

        // 🆕 Chỉ hiện text này cho local player
        if (localNote != null)
        {
            if (Object.HasInputAuthority)
            {
                localNote.gameObject.SetActive(true);
                // 🆕 Hiển thị theo tên network
                localNote.text = $"{tenNguoiChoi}";
            }
            else
            {
                localNote.gameObject.SetActive(false);
            }
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

        // 🆕 Nếu là local player thì update luôn localNote
        if (localNote != null && Object.HasInputAuthority)
        {
            localNote.text = $"{name} (Bạn)";
        }
    }
}
