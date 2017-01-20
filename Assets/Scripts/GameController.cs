using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    class CameraSetting
    {
        public bool bLerping;
        public float flLerpSpeed;
        public float flLerpStartTime;
        public float flLerpJourneyLength;

        public Transform trmStartMarker;
        public Transform trmEndMarker;
    }

    private static GameController m_pController;
    public static GameController Get()
    {
        return m_pController;
    }

    // Objects
    private GameObject m_pCamera;
    private GameObject m_pPlayer;

    // Settings
    public bool Camera_FollowPlayer { get; set; }
    public bool Camera_LookAtPlayer { get; set; }

    // Variable
    public Transform m_FirstPosition;
    public Vector3 m_CameraOffset;

    // Camera control
    private CameraSetting m_CamSetting = new CameraSetting();

    public void SetCameraPosition(Transform trmValue, Vector3 vecOffset, float flSpeed)
    {
        if (m_CamSetting.bLerping)
            return;

        m_CameraOffset = vecOffset;
        m_CamSetting.bLerping = true;
        m_CamSetting.flLerpSpeed = flSpeed;
        m_CamSetting.flLerpStartTime = Time.time;

        m_CamSetting.trmStartMarker = m_pCamera.transform;
        m_CamSetting.trmEndMarker = trmValue;

        m_CamSetting.flLerpJourneyLength = Vector3.Distance(m_CamSetting.trmStartMarker.position, m_CamSetting.trmEndMarker.position);
    }

    void Start()
    {
        m_pCamera = Camera.main.gameObject;
        m_pPlayer = GameObject.FindGameObjectWithTag("Player");

        SetCameraPosition(m_FirstPosition, m_CameraOffset, 1.0f);
        Camera_FollowPlayer = true;
        //Camera_LookAtPlayer = true;
    }

    private void Awake()
    {
        if (m_pController == null)
            m_pController = this;
        else
        {
            Debug.LogError("GameController already exist, removing...");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        UpdateCamera();
    }

    void UpdateCamera()
    {
        if (m_CamSetting.bLerping)
        {
            float flDistCovered = (Time.time - m_CamSetting.flLerpStartTime) * m_CamSetting.flLerpSpeed;
            float flFracJourney = flDistCovered / m_CamSetting.flLerpJourneyLength;

            // Position
            Vector3 vecPlayerPosition = m_CamSetting.trmEndMarker.position + new Vector3(m_pPlayer.transform.position.x, 0.0f) + m_CameraOffset;
            m_pCamera.transform.position = Vector3.Lerp(m_CamSetting.trmStartMarker.position, vecPlayerPosition, flFracJourney);

            // Rotation
            Transform trmPlayerLookAt = m_CamSetting.trmEndMarker;
            trmPlayerLookAt.LookAt(m_pPlayer.transform);
            m_pCamera.transform.rotation = Quaternion.SlerpUnclamped(m_CamSetting.trmStartMarker.rotation, trmPlayerLookAt.rotation, flFracJourney);

            // Lerp doesn't stop exactly at stop marker
            if (Vector3.Distance(m_pCamera.transform.position, vecPlayerPosition) < 0.05f)
            {
                m_CamSetting.bLerping = false;
                m_pCamera.transform.position = vecPlayerPosition;
                m_pCamera.transform.rotation = trmPlayerLookAt.rotation;
            }
        }
        else
        {
            if (Camera_FollowPlayer)
                m_pCamera.transform.position = m_CamSetting.trmEndMarker.position + new Vector3(m_pPlayer.transform.position.x, 0.0f) + m_CameraOffset;

            if (Camera_LookAtPlayer)
                m_pCamera.transform.LookAt(m_pPlayer.transform);
        }
    }
}
