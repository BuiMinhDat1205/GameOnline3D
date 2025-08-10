using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCompleteTrigger : MonoBehaviour
{
    public GameObject pressFText; // UI chữ "Nhấn F"
    private bool isPlayerInZone = false;

    void Start()
    {
        if (pressFText) pressFText.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            if (pressFText) pressFText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            if (pressFText) pressFText.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene("Map1CompleteScene"); // Tên scene chứa màn hình hoàn thành
        }
    }
}
