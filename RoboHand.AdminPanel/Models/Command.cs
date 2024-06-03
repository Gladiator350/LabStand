namespace WpfApp23.Models;

public class Command
{
    public long Id { get; set; }
    public long Uid { get; set; }
    public long Timestamp { get; set; }
    public List<Angles> Angles { get; set; } = [];
    public override string ToString()
    {
        return $"ID {Id} UserID {Uid} Time {DateTimeOffset.FromUnixTimeSeconds(Timestamp).LocalDateTime:g}";
    }
}