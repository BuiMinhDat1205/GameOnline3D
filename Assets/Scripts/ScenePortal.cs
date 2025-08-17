using Fusion;
using UnityEngine;

public class ScenePortal : NetworkBehaviour
{
    [Header("Scene Config")]
    public string sceneToLoad;              // Tên Scene cần load
    public Vector3 spawnPositionInNewScene; // Vị trí spawn ở scene mới

    private bool isPlayerInRange = false;
    private NetworkObject localPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var netObj = other.GetComponent<NetworkObject>();
            if (netObj.HasInputAuthority)
            {
                Debug.Log("Press F to enter " + sceneToLoad);
                isPlayerInRange = true;
                localPlayer = netObj;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var netObj = other.GetComponent<NetworkObject>();
            if (netObj.HasInputAuthority)
            {
                isPlayerInRange = false;
                localPlayer = null;
            }
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            // Lưu vị trí spawn cho scene mới
            PlayerPrefs.SetFloat("SpawnX", spawnPositionInNewScene.x);
            PlayerPrefs.SetFloat("SpawnY", spawnPositionInNewScene.y);
            PlayerPrefs.SetFloat("SpawnZ", spawnPositionInNewScene.z);

            if (Runner != null && Runner.IsServer)
            {
                if (localPlayer != null)
                {
                    Runner.Despawn(localPlayer); // despawn player cũ
                }

                // Load scene mới
                Runner.LoadScene(sceneToLoad);
            }
        }
    }
}
