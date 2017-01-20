using UnityEngine;
using System;

public class CameraChangePoint : MonoBehaviour
{
    public Transform m_NewPosition;
    public Vector3 m_NewOffset;
    public float m_LerpSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            GameController.Get().SetCameraPosition(m_NewPosition, m_NewOffset, m_LerpSpeed);
        }

        gameObject.SetActive(false);
    }
}
