namespace TaskManagement.Api.Models.Common;

public class StatusCount
{
    public Status Status { get; set; }
    public int Count { get; set; }

    public StatusCount() { }
    public StatusCount(Status status, int count)
    {
        Status = status;
        Count = count;
    }
}

public class StatusSummary
{
    public List<StatusCount> Counts { get; set; } = new List<StatusCount>();
    public int Total { get; set; }

    public StatusSummary() { }
    public StatusSummary(List<StatusCount> counts, int total)
    {
        Counts = counts;
        Total = total;
    }
}
