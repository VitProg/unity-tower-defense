namespace td.features.inputEvents
{
    public interface IInputEventsHandler
    {
        void OnPointerEnter(float x, float y);
        void OnPointerLeave(float x, float y);
        void OnPointerDown(float x, float y);
        void OnPointerUp(float x, float y, bool inRadius);
        void OnPointerClick(float x, float y);
        
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
    }
}