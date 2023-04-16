using System;
using td.common.cells;
using td.common.cells.interfaces;

namespace td.common.cells.hex
{
    public abstract class HexCell : ICell
    {
        public Int2 Coordinates { get; set; }

        private int x => Coordinates.x;
        private int y => Coordinates.y;

        private bool IsOddRow => Math.Abs(x) % 2 == 1;

        /*
         * North
         * West
         * South
         * East
         */

        public Int2 GetNortWestNeighbor() =>  IsOddRow ? new(x - 1, y + 1) : new(x - 1, y + 0);
        public Int2 GetNorthNeighbor() =>                new(x + 0, y + 1);
        public Int2 GetNortEastNeighbor() =>  IsOddRow ? new(x + 1, y + 1) : new(x + 1, y + 0);

        public Int2 GetSouthEastNeighbor() => IsOddRow ? new(x + 1, y - 0) : new(x + 1, y - 1);
        public Int2 GetSouthNeighbor() =>                new(x + 0, y - 1);
        public Int2 GetSouthWestNeighbor() => IsOddRow ? new(x - 1, y - 0) : new(x - 1, y - 1);
    }
}