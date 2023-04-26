using System;

namespace td.features.shards
{
    /**
     * ОСКОЛКИ можно объединять друг с другом, как одинакового цвета, так и разного
     * - [формула рассчета коофицента:](https://docs.google.com/spreadsheets/d/1Vze5h6492TZL5gN8KHKPSS2ckDovCIrYgW4ylsVhmCk/edit?usp=sharing)
     *   `countOne / countAll * (ln(countAll ^ count One) + 1)`
     *
     * - ОСКОЛКИ можно скидывать Drag-n-Drop на поле, тем самым нанося урон по области.
     *    - все расчтеты также как и у “башен”, но умноженые на 5
     *
     * Форма меняется от общего кол-ва осколков внутри
     */
    [Serializable]
    public struct Shard
    {
        public int aquamarine;
        public int blue;
        public int green;
        public int orange;
        public int pink;
        public int red;
        public int violet;
        public int yellow;
    }
}