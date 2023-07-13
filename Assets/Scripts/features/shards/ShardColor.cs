using System.Collections.Generic;
using UnityEngine;

namespace td.features.shards
{
    public struct ShardColor
    {
        public List<Item> colors;
        public int prevColor;
        public int currentColor;
        public int nextColor;
        public float colorTime;
        public bool animate;
        public Color resultColor;

        public struct Item
        {
            public byte color;
            public float weight;

            public override string ToString()
            {
                return $"{color}:{weight}";
            }
        }
    }
    
    
}