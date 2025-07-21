using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public void UpdateHealth(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + Vector3.up * 2f; // điều chỉnh vị trí trên đầu
            transform.LookAt(Camera.main.transform); // luôn quay về camera
        }
    }
}
