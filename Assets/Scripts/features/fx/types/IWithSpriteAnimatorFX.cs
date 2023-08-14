namespace td.features.fx.types
{
    public interface IWithSpriteAnimatorFX
    {
        public string PrefabName { get; set; }
        public bool? IsReverse { get; set; }
        public float? Speed { get; set; }
        public bool? IsLoop { get; set; }
    }
}