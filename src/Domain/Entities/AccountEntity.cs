using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class AccountEntity : BaseEntity
{
    public required string Name { get; set; }
    public required string PasswordHash { get; set; }
    public required string Email { get; set; }
    public RoleEnum Role { get; set; }
}