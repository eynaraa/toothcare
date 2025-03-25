using Microsoft.EntityFrameworkCore;
using Toothcare_Appointment_System.Models;

namespace Toothcare_Appointment_System.Data
{
    public class ToothcareAppointmentContext : DbContext
    {
        public ToothcareAppointmentContext(DbContextOptions<ToothcareAppointmentContext> options) : base(options) { }

        public DbSet<Staff> Staff { get; set; }
        public DbSet<Doctors> Doctors { get; set; }
        public DbSet<Patients> Patients { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
    }
}
