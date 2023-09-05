using td.features.state.interfaces;

namespace td.features.level
{
    public struct Event_Level_StateChanged : IStateChangedEvent
    {
        public bool levelNumber;
        public bool levelConfig;
        public bool kernels;
        public bool spawns;
        public bool cells;
        public bool mapSize;
        public bool isPrebuild;

        //todo
        public bool IsEmpty() => !levelNumber && !levelConfig && !kernels && !spawns && !cells && !mapSize && !isPrebuild;

        public void Clear()
        {
            levelNumber = false;
            levelConfig = false;
            kernels = false;
            spawns = false;
            cells = false;
            mapSize = false;
            isPrebuild = false;
        }

        public void All()
        {
            levelNumber = true;
            levelConfig = true;
            kernels = true;
            spawns = true;
            cells = true;
            mapSize = true;
            isPrebuild = true;
        }
    }
}