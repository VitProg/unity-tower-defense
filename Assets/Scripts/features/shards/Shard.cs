using System;
using Leopotam.EcsLite;

namespace td.features.shards
{
    /**
     * ОСКОЛКИ можно объединять друг с другом, как одинакового цвета, так и разного
     * - [формула рассчета коофицента:](https://docs.google.com/spreadsheets/d/1Vze5h6492TZL5gN8KHKPSS2ckDovCIrYgW4ylsVhmCk/edit?usp=sharing)
     *
     * - ОСКОЛКИ можно скидывать Drag-n-Drop на поле, тем самым нанося урон по области.
     *    - все расчтеты также как и у “башен”, но умноженые на 5
     *
     * Форма меняется от общего кол-ва осколков внутри
     */
    [Serializable]
    public struct Shard
    {
        /** разрывной. удар по области */
        public byte red;
        
        /** отравляет мобов на время */
        public byte green;
        
        /** замедляет мобов на время */
        public byte blue;
        
        /** увеличивает радиус стрельбы */
        public byte yellow;
        
        /** увеличивает приток энергии от убитых им мобов */
        public byte orange;
        
        /** увеличивает скорострельность */
        public byte pink;
        
        /** шок, кантузия… останавливает цель на короткое время. срабатывает с % вероятности */
        public byte violet;
        
        /** молния. цепная реакция от моба к мобу */
        public byte aquamarine;
    }

    public enum ShardTypes
    {
        Red = 1,
        Green = 2,
        Blue = 3,
        Yellow = 4,
        Orange = 5,
        Pink = 6,
        Violet = 7,
        Aquamarine = 8,
    }

    public struct ShardStoreItem
    {
        public EcsPackedEntity shard;
        public ushort cost;
    }
}