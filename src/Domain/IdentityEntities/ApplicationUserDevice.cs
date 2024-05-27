using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.IdentityEntities;

public class ApplicationUserDevice
{
    [Key] [Required] public Guid Id { get; set; }

    [Required] public Guid UserId { get; set; }

    [Required]
    public string PushNotificationProvider { get; set; }

    [Required]
    public string DeviceId { get; set; }

    [Required]
    public string PushNotificationToken { get; set; }

    public bool Active { get; set; }

    [ForeignKey("UserId")] public virtual ApplicationUser User { get; set; }
}