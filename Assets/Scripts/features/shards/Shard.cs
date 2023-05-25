using System;

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
    
    // public struct ShardCost
    // {
    //     public ushort red;
    //     public ushort green;
    //     public ushort blue;
    //     public ushort yellow;
    //     public ushort orange;
    //     public ushort pink;
    //     public ushort violet;
    //     public ushort aquamarine;
    //
    //     public ushort this[string fieldName]
    //     {
    //         get => fieldName switch
    //             {
    //                 "red" => red,
    //                 "green" => green,
    //                 "blue" => blue,
    //                 "yellow" => yellow,
    //                 "orange" => orange,
    //                 "pink" => pink,
    //                 "violet" => violet,
    //                 "aquamarine" => aquamarine,
    //                 _ => 0
    //             };
    //         set
    //         {
    //             switch (fieldName)
    //             {
    //                 case "red": red = value; break;
    //                 case "green": green = value; break;
    //                 case "blue": blue = value; break;
    //                 case "yellow": yellow = value; break;
    //                 case "orange": orange = value; break;
    //                 case "pink": pink = value; break;
    //                 case "violet": violet = value; break;
    //                 case "aquamarine": aquamarine = value; break;
    //             }
    //         }
    //     }
    // }
}