using Fusion;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class OnlineGame : SimulationBehaviour, IPlayerJoined
{
    public GameObject[] PlayerSpaw;
    public GameObject chooserMode;
    public GameObject startGame;
    public GameObject Taoten;
    public int chooseChatacter;
    public float vX;
    public float vY;
    public float vZ;
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
        Debug.Log("spawn");
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(PlayerSpaw[chooseChatacter], new Vector3(vX, vY, vZ), Quaternion.identity, player,
            (runner, obj) =>
            {
                // Sau khi spawn xong, bạn có thể setup camera hoặc logic khác
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

