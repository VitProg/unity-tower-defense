namespace td.common.ecs
{
    public interface IEcsAutoMerge<T> where T : struct
    {
        void AutoMerge(ref T result, T def);        
    }
}