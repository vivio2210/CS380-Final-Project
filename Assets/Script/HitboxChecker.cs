using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxChecker : MonoBehaviour
{
    public bool immortal = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                if (!immortal)
                {
                    Debug.Log("Player Died");
                    if (DataRecorder.Instance)
                    {
                        DataRecorder.Instance.EndSession(true);
                    }
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
