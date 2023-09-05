using System;
using Leopotam.EcsProto;
using td.features.eventBus;
using td.features.level.bus;
using td.features.level.systems;
using td.features.state.interfaces;
using td.utils.ecs;

namespace td.features.level {
    public class Level_Module : IProtoModuleWithEvents, IProtoModuleWithStateEx {
        public void Init(IProtoSystems systems) {
            systems
                .AddService(new LevelLoader_Service(), true)
                .AddService(new Level_Map_Service(), true)
                //
                .AddSystem(new Level_FinishedSystem())
                .AddSystem(new Level_LoadingSystem())
                ;
        }

        public IProtoAspect[] Aspects() {
            return null;
        }

        public IProtoModule[] Modules() {
            return new IProtoModule[] { new Path_Module() };
        }

        public IStateExtension StateEx() => new Level_State();

        public Type[] Events() =>
            Ev.E<
                Command_LoadLevel,
                Event_LevelFinished,
                Event_LevelPreLoaded,
                Event_LevelLoaded,
                Event_CellCanBuild_Clicked,
                Event_CellCanWalk_Clicked,
                Event_CellBarrier_Clicked
            >();
    }
}
