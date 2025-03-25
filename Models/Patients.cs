using System.ComponentModel.DataAnnotations;

namespace Toothcare_Appointment_System.Models
{
    public class Patients
    {
        [Key]
        [Required]
        [MaxLength(14)]
        public string ICNumber { get; set; }

        [Required]
        [MaxLength(250)]
        public string PatientName { get; set; }

        [Required]
        [MaxLength(20)]
        public string PatientPhoneNo { get; set; }

        [Required]
        [MaxLength(150)]
        public string PatientEmail { get; set; }

        [Required]
        public DateTime PatientDOB { get; set; }

        [Required]
        public string PatientAddress { get; set; }

        [Required]
        public string PatientGender { get; set; }
    }
}
