public class Power
{
    public int CastleNum { get; set; }
    public int Level1Bug { get; set; }
    public int Level2Bug { get; set; }
    public int Level3Bug { get; set; }
    public float TotalResource { get; set; }

    public void Add(Power power)
    {
        this.CastleNum += power.CastleNum;
        this.Level1Bug += power.Level1Bug;
        this.Level2Bug += power.Level2Bug;
        this.Level3Bug += power.Level3Bug;
        this.TotalResource += power.TotalResource;
    }
}
