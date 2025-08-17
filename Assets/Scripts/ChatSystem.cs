using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;

public class ChatSystem : NetworkBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textMessage;
    public TMP_InputField inputFieldMessage;
    public Button buttonSend;
    public GameObject chat;
    public GameObject open;
    public GameObject close;

    private bool isChatOpen = false; // trạng thái chat

    public override void Spawned()
    {
        // Nếu chưa gán trên Inspector thì thử tìm bằng tên
        if (textMessage == null)
        {
            var goText = GameObject.Find("TextMessage");
            if (goText != null)
                textMessage = goText.GetComponent<TextMeshProUGUI>();
            else
                Debug.LogError("[ChatSystem] Không tìm thấy TextMessage trong scene!");
        }

        if (inputFieldMessage == null)
        {
            var goInput = GameObject.Find("InputFieldMessage");
            if (goInput != null)
                inputFieldMessage = goInput.GetComponent<TMP_InputField>();
            else
                Debug.LogError("[ChatSystem] Không tìm thấy InputFieldMessage trong scene!");
        }

        if (buttonSend == null)
        {
            var goButton = GameObject.Find("ButtonSend");
            if (goButton != null)
                buttonSend = goButton.GetComponent<Button>();
            else
                Debug.LogError("[ChatSystem] Không tìm thấy ButtonSend trong scene!");
        }

        // Nếu đã có buttonSend thì gán sự kiện click
        if (buttonSend != null)
        {
            buttonSend.onClick.RemoveListener(SendMessageChat);
            buttonSend.onClick.AddListener(SendMessageChat);
        }
    }

    void Update()
    {
        // Tab để bật/tắt chat
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isChatOpen)
                CloseChat();
            else
                OpenChat();
        }

        // Enter để gửi
        if (isChatOpen && Input.GetKeyDown(KeyCode.Return))
        {
            SendMessageChat();
            // Focus lại input để tiếp tục gõ
            if (inputFieldMessage != null)
                inputFieldMessage.ActivateInputField();
        }
    }

    public void SendMessageChat()
    {
        if (inputFieldMessage == null || textMessage == null) return;

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
        if (textMessage != null)
            textMessage.text += message + "\n";
    }

    public void OpenChat()
    {
        if (open != null) open.SetActive(false);
        if (close != null) close.SetActive(true);
        if (chat != null) chat.SetActive(true);
        isChatOpen = true;

        // Tự động focus vào input
        if (inputFieldMessage != null)
            inputFieldMessage.ActivateInputField();
    }

    public void CloseChat()
    {
        if (open != null) open.SetActive(true);
        if (close != null) close.SetActive(false);
        if (chat != null) chat.SetActive(false);
        isChatOpen = false;
    }
}
