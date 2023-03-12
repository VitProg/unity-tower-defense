﻿using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using td.common;

namespace esc
{
    sealed class GridInitSystem : IEcsInitSystem {
        readonly EcsCustomInject<GridService> _gs = default;
        readonly EcsCustomInject<SceneData> _sceneData = default;
      
        readonly EcsPoolInject<Cell> _cellPool = default;

        public void Init (IEcsSystems systems) {
            var world = _cellPool.Value.GetWorld ();
            for (var i = 0; i < _sceneData.Value.Cells.Length; i++) {
                var cellView = _sceneData.Value.Cells[i];
                var entity = world.NewEntity ();
                ref var cell = ref _cellPool.Value.Add (entity);
                var position = cellView.transform.position;
                var x = (int) (position.x / cellView.step);
                var y = (int) (position.z / cellView.step);
                cell.View = cellView;
                _gs.Value.AddCell (new Int2 (x, y), entity);
            }
        }
    }
}