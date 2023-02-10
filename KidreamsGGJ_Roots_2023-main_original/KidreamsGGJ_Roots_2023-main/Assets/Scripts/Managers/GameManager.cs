using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TextCore.Text;

public enum GameStates { PlayerLoop, VampireLordLoop }

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private delegate void GameState();
    private GameState _gameState;

    public event Action OnResurrectPlayer;

    private float _tombInstansiationOffsetY = Screen.height;

    public GameObject NewGraveDirt { get; set; }

    private Dictionary<GameObject, GameObject> _allGraves; 

    // win/lose condition: blood empty = perma-death, blood full = vampireLord resurrection, value = 0-100.
    [SerializeField] private int _bloodAmount = 25;
    public int BloodAmount => _bloodAmount;

    [SerializeField] private int _deathCount = 0;
    public int DeathCount { get => _deathCount; set => _deathCount = value; }

    [SerializeField] private GameObject _graveDirt, _graveTomb;
    public GameObject GraveDirt => _graveDirt;
    public GameObject GraveTomb => _graveTomb;

    [SerializeField] private GameObject _playerPrefab, _currentVampireLord;
    public GameObject PlayerPrefab => _playerPrefab;
    public GameObject CurrentVampireLord => _currentVampireLord;

    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerController { get => _playerController; set => _playerController = value; }
    
    [SerializeField] private VampireLordController _vampireLordController;
    public VampireLordController VampireLordController => _vampireLordController;

    [SerializeField] private Transform _playerSpawn, _vampireLordSpawn;
    public Transform PlayerSpawn => _playerSpawn;
    public Transform VampireLordSpawn => _vampireLordSpawn;

    [SerializeField] private List<EntityData> _engraved;
    public List<EntityData> Engraved { get => _engraved; set => _engraved = value ; }

    [SerializeField] private EntityData _chosenEngraved;
    public EntityData ChosenEngraved { get => _chosenEngraved; set => _chosenEngraved = value; }

    [SerializeField] private List<Entity> _allEntities;
    public List<Entity> AllEntities { get => _allEntities; set => _allEntities = value; }

    [SerializeField] private UnderworldOverlay _underworldOverlay;
    public UnderworldOverlay UnderworldOverlay => _underworldOverlay;

    [SerializeField] private bool _debugPlayerLoop = false;

    private void Awake()
    {
        _instance = this;
        _allEntities = new();
        _allGraves = new();
        _engraved = new();
        _vampireLordController = _currentVampireLord.GetComponent<VampireLordController>();
        _underworldOverlay.SetRegularMode();
        OnResurrectPlayer += TransitionToOverworld;
        _gameState = PlayerLoop;
    }
    private void Update()
    {
        _gameState.Invoke();
    }
    private void OnDisable()
    {
        OnResurrectPlayer -= TransitionToOverworld;
    }

    private void PlayerLoop()
    {
        if (_debugPlayerLoop)
            Debug.Log($"GameState is PlayerLoop");

        if (!CameraManager.Instance.IsFollowingPlayer())
            CameraManager.Instance.ChangeState(CameraStates.FollowPlayer);
    }
    private void VampireLordLoop()
    {
        if (_debugPlayerLoop)
            Debug.Log($"GameState is VampireLordLoop");

        if (CameraManager.Instance.IsFollowingPlayer())
            CameraManager.Instance.ChangeState(CameraStates.FollowVampireLord);
    }

    [Button("Test TransitionToUnderworld")]
    public async void TransitionToUnderworld()
    {
        foreach (Entity entity in AllEntities)
        {
            if (entity)
                Destroy(entity.gameObject);
        }

        AllEntities.Clear();

        if (!CameraManager.Instance.IsPlayingSounds)
        {
            CameraManager.Instance.ChangeAudioSource(CameraManager.Instance._moveToUnderworld);
            CameraManager.Instance._cameraAudioSource.Play();
        }

        await _underworldOverlay.StartUnderworldAnim();

        _vampireLordController.gameObject.SetActive(true);
        ChangeState(GameStates.VampireLordLoop);
    }

    public void TransitionToOverworld()
    {
        _vampireLordController.gameObject.SetActive(false);

        _underworldOverlay.SetRegularMode();
        Debug.Log("Underworld anim done");

        ResurrectPlayer();
        ChangeState(GameStates.PlayerLoop);
    }

    public void ChangeState(GameStates newState)
    {
        switch (newState)
        {
            case GameStates.PlayerLoop:
                _gameState = PlayerLoop;
                break;
            case GameStates.VampireLordLoop:
                _gameState = VampireLordLoop;
                break;
        }
    }

    public void OnPlayerDie()
    {
        Debug.Log($"player died");
        TransitionToUnderworld();
        Destroy(PlayerController.gameObject);
    }
    public void InvokeResurrectPlayer()
    {
        OnResurrectPlayer?.Invoke();
    }
    public void OnEntityDie(Entity entity)
    {
        if (entity)
        {
            if (entity is Villager)
            {
                if (!CameraManager.Instance.IsPlayingSounds)
                {
                    CameraManager.Instance.ChangeAudioSource(CameraManager.Instance._villigerDeath);
                    CameraManager.Instance._cameraAudioSource.Play();
                }
                CreateGrave(entity as Villager);
                Debug.Log($"{entity.Data.Name} grave created");
            }

            switch (entity.Type)
            {
                case EntityKind.Mouse:
                    if (!CameraManager.Instance.IsPlayingSounds)
                    {
                        CameraManager.Instance.ChangeAudioSource(CameraManager.Instance._mouseDeath);
                        CameraManager.Instance._cameraAudioSource.Play();
                    }
                    break;
                case EntityKind.Rabbit:
                    if (!CameraManager.Instance.IsPlayingSounds)
                    {
                        CameraManager.Instance.ChangeAudioSource(CameraManager.Instance._rabbitDeath);
                        CameraManager.Instance._cameraAudioSource.Play();
                    }
                    break;
                case EntityKind.Boar:
                    if (!CameraManager.Instance.IsPlayingSounds)
                    {
                        CameraManager.Instance.ChangeAudioSource(CameraManager.Instance._boarDeath);
                        CameraManager.Instance._cameraAudioSource.Play();
                    }
                    break;
                default:
                    break;
            }
            

            Debug.Log($"{entity.Data.Name} died");
            Destroy(entity.gameObject);
        }
    }
    private void ResurrectPlayer()
    {
        GameObject newPlayer = Instantiate(_playerPrefab, _playerSpawn);
        PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();
        newPlayerController.AbsorbedEntity = _chosenEngraved;
        ChangeState(GameStates.PlayerLoop);
    }
    public void CreateGrave(Villager villager)
    {
        NewGraveDirt = Instantiate(_graveDirt, villager.transform.position, Quaternion.identity);
        GameObject newGraveTombGO = Instantiate(_graveTomb, new Vector3(_graveDirt.transform.position.x, _graveDirt.transform.position.y + _tombInstansiationOffsetY), Quaternion.identity);

        GraveTomb newGraveTomb = newGraveTombGO.GetComponent<GraveTomb>();
        newGraveTomb.EngravedVillagerData = villager.Data;

        _allGraves.Add(NewGraveDirt, newGraveTombGO);
    }
}
