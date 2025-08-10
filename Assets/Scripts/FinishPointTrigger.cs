using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCompleteTrigger : MonoBehaviour
{
    public GameObject pressFText;
    private bool isPlayerInZone = false;
    [SerializeField] private string completeSceneName = "Map1CompleteScene"; // cho phép đổi trong Inspector

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
            CompleteMap();
        }
    }

    public void CompleteMap() // Gọi được từ UI Button
    {
        SceneManager.LoadScene(completeSceneName);
    }
}
