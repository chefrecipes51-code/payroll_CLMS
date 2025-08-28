using System.ComponentModel.DataAnnotations;

namespace Payroll.WebApp.Models
{
    public class Notificationcs
    {
        [Key]
        public int Notification_Id { get; set; }

        [Required]
        public string? NotificationTitle { get; set; }

        [Required]
        public string? NotificationDesc { get; set; }
        [Required]
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsCommon { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
