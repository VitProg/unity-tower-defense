namespace td.features._common.interfaces
{
    public interface IInputEventsHandler
    {
        void OnPointerEnter(float x, float y);
        void OnPointerLeave(float x, float y);
        // ReSharper disable Unity.PerformanceAnalysis
        void OnPointerDown(float x, float y);
        void OnPointerUp(float x, float y, bool inside);
        void OnPointerClick(float x, float y, bool isLong = false);
        
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public float TimeFromDown { get; set; }
    }
}