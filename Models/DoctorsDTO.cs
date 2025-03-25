using System.ComponentModel.DataAnnotations;

namespace Toothcare_Appointment_System.Models
{
    public class DoctorsDTO
    {
        [Key]
        public int DoctorID { get; set; }

        [Required]
        [MaxLength(250)]
        public string DoctorName { get; set; }

        [Required]
        [MaxLength(20)]
        public string DoctorPhoneNo { get; set; }

        public string? DoctorPass { get; set; }

        [Required]
        [MaxLength(150)]
        public string DoctorEmail { get; set; }

        [Required]
        public string DoctorAddress { get; set; }

        [Required]
        [MaxLength(50)]
        public string DoctorLicenseNumber { get; set; }

        [Required]
        public string DoctorAvailability { get; set; }
    }
}
