public class HordePoolObject
{
    public Horde Horde { get; set; }
    public HordePoolObjectState State { get; set; }

    public enum HordePoolObjectState
    {
        ReadyToUse,
        Used
    }
}
