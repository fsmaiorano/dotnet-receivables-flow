using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class RoleEntity : BaseEntity
{
    public RoleEnum Roles { get; set; }
}