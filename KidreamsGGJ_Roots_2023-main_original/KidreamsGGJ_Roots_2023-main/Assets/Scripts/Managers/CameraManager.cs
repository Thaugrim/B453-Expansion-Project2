using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraStates { FollowPlayer, FollowVampireLord }

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance => _instance;

    private delegate void CameraState();
    private CameraState _cameraState;


    [SerializeField] private Camera _mainCam;
    public Camera MainCam => _mainCam;

    [SerializeField] private Transform _mainCamTransform;
    public Transform MainCamTransform => _mainCamTransform;

    [SerializeField] private float _size = 14f;

    public AudioSource _cameraAudioSource;
    public AudioClip _graveEmerge, _heartBeat, _moveToUnderworld, _bite, _mouseDeath, _rabbitDeath, _boarDeath, _villigerDeath;
    public bool IsPlayingSounds => _cameraAudioSource.isPlaying;

    private void Awake()
    {
        _instance = this;
        _mainCam.orthographicSize = _size;
        _mainCamTransform = _mainCam.transform;
    }
    private void Start()
    {
        _cameraState = FollowPlayer;
    }
    private void Update()
    {
        _cameraState.Invoke();
    }
    
    private void FollowPlayer()
    {
        if (!GameManager.Instance.PlayerController)
            return;

        float playerX = GameManager.Instance.PlayerController.transform.position.x;
        float playerY = GameManager.Instance.PlayerController.transform.position.y;
        float cameraZ = _mainCamTransform.position.z;

        Vector3 newCamPos = new Vector3(playerX, playerY, cameraZ);
        _mainCamTransform.position = newCamPos;
    }
    private void FollowVampireLord()
    {
        if (!GameManager.Instance.VampireLordController)
            return;

        float playerX = GameManager.Instance.VampireLordController.transform.position.x;
        float playerY = GameManager.Instance.VampireLordController.transform.position.y;
        float cameraZ = _mainCamTransform.position.z;

        Vector3 newCamPos = new Vector3(playerX, playerY, cameraZ);
        _mainCamTransform.position = newCamPos;
    }
    public void ChangeState(CameraStates newState)
    {
        switch (newState)
        {
            case CameraStates.FollowPlayer:
                _cameraState = FollowPlayer;
                break;
            case CameraStates.FollowVampireLord:
                _cameraState = FollowVampireLord;
                break;
        }
    }
    public bool IsFollowingPlayer()
    {
        if (_cameraState == FollowPlayer)
            return true;
        else
            return false;
    }
    public void ChangeAudioSource(AudioClip ac)
    {
        _cameraAudioSource.clip = ac;
    }
}
