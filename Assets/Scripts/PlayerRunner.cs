using Fusion;
using TMPro;
using UnityEngine;

public class PlayerRunner : SimulationBehaviour, IPlayerJoined
{
    public GameObject chooserMode;
    public GameObject startGame;
    public GameObject Taoten;

    [Header("Player Setup")]
    public GameObject[] PlayerSpaw; // Danh sách prefab các nhân vật
    public int chooseChatacter = 0;

    [Header("Spawn Position")]
    public float vX = 0f;
    public float vY = 1f;
    public float vZ = 0f;

    public TMP_InputField tenNhapvao;
    public string name;

    private void Start()
    {
        chooserMode.SetActive(true);
        startGame.SetActive(false);
        Taoten.SetActive(false);
    }

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("Spawn Player");

        // ✅ Thêm check an toàn trước khi truy cập mảng
        if (chooseChatacter < 0 || chooseChatacter >= PlayerSpaw.Length)
        {
            Debug.LogError($"Invalid character index: {chooseChatacter}. PlayerSpaw length: {PlayerSpaw.Length}");
            chooseChatacter = 0; // fallback về nhân vật đầu tiên
        }

        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(
            PlayerSpaw[chooseChatacter],
            new Vector3(vX, vY, vZ),
            Quaternion.identity,
            player,
            (runner, obj) =>
            {
                // Sau khi spawn thành công
                var playerSetup = obj.GetComponent<PlayerSetUp>();
                if (playerSetup != null)
                {
                }

                var createTen = obj.GetComponent<CreateNameNetwork>();
                if (createTen != null)
                {
                    createTen.ThemTen(name);
                }
            });
            chooserMode.SetActive(false);
        }
        else
        {
            // Giữ nguyên Debug.LogError, nhưng chỉ log nếu index thật sự sai
            if (chooseChatacter < 0 || chooseChatacter >= PlayerSpaw.Length)
            {
                Debug.LogError($"Invalid character index: {chooseChatacter}. PlayerSpaw length: {PlayerSpaw.Length}");
            }
            else
            {
                // Spawn cho remote player
                Runner.Spawn(
                PlayerSpaw[chooseChatacter],
                new Vector3(vX, vY, vZ),
                Quaternion.identity,
                player,
                (runner, obj) =>
                {
                    var createTen = obj.GetComponent<CreateNameNetwork>();
                    if (createTen != null)
                    {
                        createTen.ThemTen(name);
                    }
                });
            }
        }
    }

    public void ChooseHuman()
    {
        chooseChatacter = 0;
        chooserMode.SetActive(false);
        Taoten.SetActive(true);
    }
    public void ChooseHulk()
    {
        Debug.Log("Human chosen");
        chooseChatacter = 1;
        chooserMode.SetActive(false);
        Taoten.SetActive(true);
    }
    public void CreateNameCharacter()
    {
        Debug.Log($"Name entered: {tenNhapvao.text}");
        name = tenNhapvao.text;
        Taoten.SetActive(false);
        startGame.SetActive(true);
    }
}
