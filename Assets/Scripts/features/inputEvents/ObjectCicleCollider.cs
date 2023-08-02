namespace td.features.inputEvents
{
    public struct ObjectCicleCollider
    {
        public float radius;
        public float sqrRadius;
        public float yScale;

        public void SetRadius(float r, float yScale = 1f)
        {
            radius = r;
            sqrRadius = radius * radius;
            this.yScale = yScale;
        }
    }
}