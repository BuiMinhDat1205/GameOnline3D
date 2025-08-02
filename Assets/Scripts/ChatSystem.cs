using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;
public class ChatSystem : NetworkBehaviour
{
    public TextMeshProUGUI textMessage;
    public TMP_InputField inputFieldMessage;
    public Button buttonSend;
    public GameObject chat;
    public GameObject open;
    public GameObject close;

    //Chạy ngay sau khi nhân vật được spawn trong network
    public override void Spawned()
    {
        textMessage = GameObject.Find("TextMessage").GetComponent<TextMeshProUGUI>();
        inputFieldMessage = GameObject.Find("InputFieldMessage").GetComponent<TMP_InputField>();
        buttonSend = GameObject.Find("ButtonSend").GetComponent<Button>();
        buttonSend.onClick.AddListener(SendMessageChat);
    }

    public void SendMessageChat()
    {
        var message = inputFieldMessage.text;
        if (string.IsNullOrWhiteSpace(message)) return;
        var id = Runner.LocalPlayer.PlayerId;
        var text = $"<b>Player {id}:</b> {message}";
        RpcChat(text);
        inputFieldMessage.text = "";
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcChat(string message)
    {
        textMessage.text += message + "\n";
    }
    public void OpenChat()
    {
        open.SetActive(false);
        close.SetActive(true);
        chat.SetActive(true);
    }
    public void CloseChat()
    {
        open.SetActive(true);
        close.SetActive(false);
        chat.SetActive(false);
    }
}
