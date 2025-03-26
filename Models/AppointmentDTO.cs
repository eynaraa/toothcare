using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Toothcare_Appointment_System.Models
{
    public class AppointmentDTO
    {
        [Key]
        public int AppointmentID { get; set; }

        public DateTime AppointmentDateTime { get; set; }
        public string AppointmentReason { get; set; } // Description of the appointment
        public string AppointmentStatus { get; set; } // Scheduled, Confirmed, Cancelled, Completed

        [AllowNull]
        public string? AppointmentNotes { get; set; } // Notes from the doctor

        [AllowNull]
        public int? RoomNumber { get; set; }

        public string AppointmentType { get; set; } // consultation, check up, procedure

        public int DoctorID { get; set; }
        public string ICNumber { get; set; }
    }
}
