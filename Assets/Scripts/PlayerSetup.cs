using Fusion;
using UnityEngine;

public class PlayerSetUp : NetworkBehaviour
{
    [System.Obsolete]
    public void SetUpCamera()
        {
            if (Object.HasInputAuthority)
            {
                CameraFollow camera = FindObjectOfType<CameraFollow>();
                if (camera != null)
                {
                    camera.AssignCamera(transform);
                }


            }

        }
    }
