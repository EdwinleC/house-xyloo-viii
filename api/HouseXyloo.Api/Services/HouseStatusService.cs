using HouseXyloo.Api.Data;
using HouseXyloo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HouseXyloo.Api.Services;

public class HouseStatusService
{
    private readonly HouseXylooDbContext _dbContext;

    public HouseStatusService(HouseXylooDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HouseStatus> GetStatusAsync()
    {
        var status = await _dbContext.HouseStatuses
            .FirstOrDefaultAsync();

        if (status is not null)
        {
            return status;
        }

        status = new HouseStatus
        {
            Online = true,
            Mode = "Chill",
            Message = "Welcome to House Xyloo VIII.",
            GuestCount = 0,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.HouseStatuses.Add(status);
        await _dbContext.SaveChangesAsync();

        return status;
    }

    public async Task<HouseStatus> UpdateStatusAsync(HouseStatus updatedStatus)
    {
        var status = await _dbContext.HouseStatuses
            .FirstOrDefaultAsync();

        if (status is null)
        {
            updatedStatus.Id = 0;
            updatedStatus.UpdatedAt = DateTime.UtcNow;

            _dbContext.HouseStatuses.Add(updatedStatus);
        }
        else
        {
            status.Online = updatedStatus.Online;
            status.Mode = updatedStatus.Mode;
            status.Message = updatedStatus.Message;
            status.GuestCount = updatedStatus.GuestCount;
            status.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        return status ?? updatedStatus;
    }
}