using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Toothcare_Appointment_System.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentID { get; set; }

        [Required]
        public int DoctorID { get; set; } // Stores Doctor's Primary Key

        [ForeignKey("DoctorID")]
        public Doctors Doctor { get; set; } // Navigation Property

        [Required]
        [MaxLength(14)]
        public string ICNumber { get; set; } // Stores Patient's Primary Key

        [ForeignKey("ICNumber")]
        public Patients Patient { get; set; } // Navigation Property

        public DateTime AppointmentDateTime { get; set; }
        public string AppointmentReason { get; set; } // Description of the appointment
        public string AppointmentStatus { get; set; } // Scheduled, Confirmed, Cancelled, Completed

        [AllowNull]
        public string? AppointmentNotes { get; set; } // Notes from the doctor

        [AllowNull]
        public int? RoomNumber { get; set; }

        public string AppointmentType { get; set; } // consultation, check up, procedure
    }
}
