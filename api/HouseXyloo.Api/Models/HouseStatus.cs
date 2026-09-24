namespace HouseXyloo.Api.Models;

public class HouseStatus
{
    public int Id { get; set; }

    public bool Online { get; set; }

    public string Mode { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public int GuestCount { get; set; }

    public DateTime UpdatedAt { get; set; }
}