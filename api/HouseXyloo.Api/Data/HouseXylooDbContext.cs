using HouseXyloo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HouseXyloo.Api.Data;

public class HouseXylooDbContext : DbContext
{
    public HouseXylooDbContext(
        DbContextOptions<HouseXylooDbContext> options)
        : base(options)
    {
    }

    public DbSet<HouseStatus> HouseStatuses { get; set; }
}