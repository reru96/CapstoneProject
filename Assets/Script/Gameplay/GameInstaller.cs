using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private AudioManager audioManager;    
    [SerializeField] private ClassSelectionManager classManager;
    [SerializeField] private PlayerSpawnManager spawnManager;
    [SerializeField] private RespawnManager respawnManager;
    [SerializeField] private CheckpointManager checkPointManager;

    private DIContainer _container;
    private ObjectResolver _resolver;

    private void Awake()
    {
        _container = new DIContainer();
        _resolver = new ObjectResolver(_container);

        _container.Register(inputManager);
        _container.Register(gameManager);
        _container.Register(audioManager);
        _container.Register(classManager);
        _container.Register(spawnManager);
        _container.Register(respawnManager);
        _container.Register(checkPointManager);

        _resolver.Resolve(inputManager);
        _resolver.Resolve(gameManager);
        _resolver.Resolve(audioManager); 
        _resolver.Resolve(classManager);
        _resolver.Resolve(spawnManager);
        _resolver.Resolve(respawnManager);
        _resolver.Resolve(checkPointManager);
    }
}
