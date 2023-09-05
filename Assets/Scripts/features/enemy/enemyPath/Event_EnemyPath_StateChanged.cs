using td.features.state.interfaces;

namespace td.features.enemy.enemyPath {
    public struct Event_EnemyPath_StateChanged : IStateChangedEvent {
        public bool routes;
        public int routeIdx;

        public bool IsEmpty() => !routes && routeIdx < 0;

        public void Clear() {
            routes = false;
            routeIdx = -1;
        }

        public void All() {
            routes = true;
            routeIdx = -1;
        }
    }
}
