using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.UnityEditor;
using UnityEngine;

namespace esc
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] SceneData _sceneData;
        [SerializeField] Configuration _configuration;
        EcsSystems _systems;

        // Start is called before the first frame update
        void Start()
        {
            var world = new EcsWorld();
            _systems = new EcsSystems(world);
            var ts = new TimeService();
            var gs = new GridService(_configuration.GridWidth, _configuration.Gridheight);

            _systems
                .Add(new GridInitSystem())
                .Add(new TimeSystem())
#if UNITY_EDITOR
                .Add(new EcsWorldDebugSystem())
#endif
                .Inject(ts, gs, _sceneData)
                .Init();
        }

        // Update is called once per frame
        void Update()
        {
            _systems?.Run();
        }

        void OnDestroy()
        {
            _systems?.Destroy();
            _systems?.GetWorld()?.Destroy();
            _systems = null;
        }
    }
}