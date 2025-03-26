using System.ComponentModel.DataAnnotations;

namespace Toothcare_Appointment_System.Models
{
    public class Staff
    {
        [Key]
        public int StaffID { get; set; }

        [Required]
        [MaxLength(250)]
        public string StaffName { get; set; }

        [Required]
        [MaxLength(20)]
        public string StaffPhoneNo { get; set; }

        [Required]
        public string StaffPass { get; set; }

        [Required]
        [MaxLength(20)]
        public string StaffRole { get; set; }

        [Required]
        [MaxLength(150)]
        public string StaffEmail { get; set; }
    }
}
