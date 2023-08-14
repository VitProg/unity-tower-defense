using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsProto;
using td.features._common;
using UnityEngine;

namespace td.features.shard.components
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
    public struct Shard : IProtoAutoReset<Shard>
    {
        public const string Type = "shard";
        
        public void AutoReset(ref Shard c)
        {
            c = default;
            c._id_ = CommonUtils.ID(Type);
        }
        
        // ReSharper disable once InconsistentNaming
        public uint _id_;
        
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
        
        /** увеличивает скорострельность и уменьшает разброс/кучность*/
        public byte pink;
        
        /** шок, кантузия… останавливает цель на короткое время. срабатывает с % вероятности */
        public byte violet;
        
        /** молния. цепная реакция от моба к мобу */
        public byte aquamarine;

        public uint cost;
        public uint costInsert;
        public uint costCombine;
        public uint costDrop;
        public uint costRemove;
        
        public Color currentColor;

#if UNITY_EDITOR && DEBUG
        public override string ToString() => $"#{_id_}:{red}-{green}-{blue}-{aquamarine}-{yellow}-{orange}-{pink}-{violet}";
#else
        public override string ToString() => $"{red}-{green}-{blue}-{aquamarine}-{yellow}-{orange}-{pink}-{violet}";
#endif

        public ushort Quantity => (ushort)(red + green + blue + yellow + orange + pink + violet + aquamarine);
        
        public byte this[int index] => (index % 8) switch
        {
            0 => red,
            1 => green,
            2 => blue,
            3 => aquamarine,
            4 => yellow,
            5 => orange,
            6 => pink,
            7 => violet,
            _ => red
        };

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool IsEquals(Shard a, Shard b) => IsEquals(ref a, ref b); 
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool IsEquals(ref Shard a, ref Shard b)
        {
            if (CommonUtils.IdsIsEquals(a._id_, b._id_)) return true;
            return a.red == b.red &&
                   a.green == b.green &&
                   a.blue == b.blue &&
                   a.aquamarine == b.aquamarine &&
                   a.yellow == b.yellow &&
                   a.orange == b.orange &&
                   a.pink == b.pink &&
                   a.violet == b.violet;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsEquals(ref Shard b) => IsEquals(ref this, ref b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Shard CombineTwoShardsToNew(ref Shard a, ref Shard b) => new()
        {
            red = (byte)Math.Min(100, a.red + b.red),
            green = (byte)Math.Min(100, a.green + b.green),
            blue = (byte)Math.Min(100, a.blue + b.blue),
            aquamarine = (byte)Math.Min(100, a.aquamarine + b.aquamarine),
            yellow = (byte)Math.Min(100, a.yellow + b.yellow),
            orange = (byte)Math.Min(100, a.orange + b.orange),
            pink = (byte)Math.Min(100, a.pink + b.pink),
            violet = (byte)Math.Min(100, a.violet + b.violet),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CombineWith(ref Shard b)
        {
            red = (byte)Math.Min(100, red + b.red);
            green = (byte)Math.Min(100, green + b.green);
            blue = (byte)Math.Min(100, blue + b.blue);
            aquamarine = (byte)Math.Min(100, aquamarine + b.aquamarine);
            yellow = (byte)Math.Min(100, yellow + b.yellow);
            orange = (byte)Math.Min(100, orange + b.orange);
            pink = (byte)Math.Min(100, pink + b.pink);
            violet = (byte)Math.Min(100, violet + b.violet);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Shard MakeCopy() => new()
        {
            red = red,
            green = green,
            blue = blue,
            aquamarine = aquamarine,
            yellow = yellow,
            orange = orange,
            pink = pink,
            violet = violet,
            
            cost = cost,
            costRemove = costRemove,
            costDrop = costDrop,
            costInsert = costInsert,
            costCombine = costCombine,
        };
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFrom(ref Shard shard)
        {
            red = shard.red;
            green = shard.green;
            blue = shard.blue;
            aquamarine = shard.aquamarine;
            yellow = shard.yellow;
            orange = shard.orange;
            pink = shard.pink;
            violet = shard.violet;
            
            cost = shard.cost;
            costRemove = shard.costRemove;
            costDrop = shard.costDrop;
            costInsert = shard.costInsert;
            costCombine = shard.costCombine;
        }
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
}