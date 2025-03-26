using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toothcare_Appointment_System.Data;
using Toothcare_Appointment_System.Models;

namespace Toothcare_Appointment_System.Controllers
{
    [Route("api/Appointments")]
    [ApiController]
    public class APIAppointmentsController : ControllerBase
    {
        private readonly ToothcareAppointmentContext _context;

        public APIAppointmentsController(ToothcareAppointmentContext context)
        {
            _context = context;
        }

        // GET: api/Appoinments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppoinment()
        {
            return await _context.Appointment
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .ToListAsync();
        }

        // GET: api/Appoinments/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppoinment(int id)
        {
            var appoinment = await _context.Appointment
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentID == id);
            if (appoinment == null)
            {
                return NotFound();
            }
            return appoinment;
        }

        // POST: api/Appoinments
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppoinment(AppointmentDTO appointmentDto)
        {
            if (appointmentDto == null)
            {
                return BadRequest("Invalid appointment data.");
            }

            var doctor = await _context.Doctors.FindAsync(appointmentDto.DoctorID);
            var patient = await _context.Patients.FindAsync(appointmentDto.ICNumber);

            if (doctor == null || patient == null)
            {
                return NotFound("Doctor or Patient not found.");
            }

            // Create the new Appointment object
            var appointment = new Appointment
            {
                AppointmentDateTime = appointmentDto.AppointmentDateTime,
                AppointmentReason = appointmentDto.AppointmentReason,
                AppointmentStatus = appointmentDto.AppointmentStatus,
                AppointmentNotes = appointmentDto.AppointmentNotes,
                RoomNumber = appointmentDto.RoomNumber,
                AppointmentType = appointmentDto.AppointmentType,
                Doctor = doctor,
                Patient = patient
            };

            _context.Appointment.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppointment", new { id = appointment.AppointmentID }, appointment);
        }

        // PUT: api/Appoinments/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppoinment(int id, Appointment appoinment)
        {
            if (id != appoinment.AppointmentID)
            {
                return BadRequest();
            }
            _context.Entry(appoinment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Appoinments/1
        [HttpDelete("{id}")]
        public async Task<ActionResult<Appointment>> DeleteAppoinment(int id)
        {
            var appoinment = await _context.Appointment.FindAsync(id);
            if (appoinment == null)
            {
                return NotFound();
            }
            _context.Appointment.Remove(appoinment);
            await _context.SaveChangesAsync();
            return appoinment;
        }
    }

    [Route("Appointments")]
    public class AppointmentsController : Controller
    {
        private readonly ToothcareAppointmentContext _context;
        public AppointmentsController(ToothcareAppointmentContext context)
        {
            _context = context;
        }

        // GET: Appointments
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var appointment = await _context.Appointment.ToListAsync();
            var appointmentDto = appointment.Select(item => new AppointmentDTO
            {
                AppointmentID = item.AppointmentID,
                AppointmentDateTime = item.AppointmentDateTime,
                AppointmentReason = item.AppointmentReason,
                AppointmentStatus = item.AppointmentStatus,
                AppointmentNotes = item.AppointmentNotes,
                RoomNumber = item.RoomNumber,
                AppointmentType = item.AppointmentType,
                DoctorID = item.DoctorID,
                ICNumber = item.ICNumber
            }).ToList();

            return View(appointmentDto);
        }

        //GET: Appointments/View/1
        [HttpGet("View/{id}")]
        public async Task<IActionResult> View(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var appointment = await _context.Appointment
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["Doctors"] = await _context.Doctors.ToListAsync();
            ViewData["Patients"] = await _context.Patients.ToListAsync();
            return View();
        }

        // POST: Appointments/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentDTO appointmentDto)
        {
            if (ModelState.IsValid)
            {

                if (appointmentDto == null)
                {
                    return BadRequest("Invalid appointment data.");
                }

                var doctor = await _context.Doctors.FindAsync(appointmentDto.DoctorID);
                var patient = await _context.Patients.FindAsync(appointmentDto.ICNumber);

                if (doctor == null || patient == null)
                {
                    return NotFound("Doctor or Patient not found.");
                }

                // Create the new Appointment object
                var appointment = new Appointment
                {
                    AppointmentDateTime = appointmentDto.AppointmentDateTime,
                    AppointmentReason = appointmentDto.AppointmentReason,
                    AppointmentStatus = appointmentDto.AppointmentStatus,
                    AppointmentType = appointmentDto.AppointmentType,
                    Doctor = doctor,
                    Patient = patient
                };

                _context.Appointment.Add(appointment);
                await _context.SaveChangesAsync();

                if (User.IsInRole("Doctor"))
                {
                    return RedirectToAction("Index", "Doctors");
                }
                else
                {
                    return RedirectToAction("Index", "Staff");
                }
            }

            ViewData["Doctors"] = await _context.Doctors.ToListAsync();
            ViewData["Patients"] = await _context.Patients.ToListAsync();
            return View(appointmentDto);
        }

        // GET: Appointments/Edit/1
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var appointment = await _context.Appointment
                .Include(a => a.Doctor)  // Ensure Doctor data is loaded
                .Include(a => a.Patient) // Ensure Patient data is loaded
                .FirstOrDefaultAsync(a => a.AppointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            var appoinmentdto = new AppointmentDTO
            {
                AppointmentID = appointment.AppointmentID,
                AppointmentDateTime = appointment.AppointmentDateTime,
                AppointmentReason = appointment.AppointmentReason,
                AppointmentStatus = appointment.AppointmentStatus,
                AppointmentType = appointment.AppointmentType,
                AppointmentNotes = appointment.AppointmentNotes,
                RoomNumber = appointment.RoomNumber,
                DoctorID = appointment.Doctor.DoctorID,
                ICNumber = appointment.Patient.ICNumber
            };

            ViewData["Doctors"] = await _context.Doctors.ToListAsync();
            ViewData["Patients"] = await _context.Patients.ToListAsync();
            return View(appoinmentdto);
        }

        // POST: Appointments/Edit/1
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDTO appointmentdto)
        {
            if (id != appointmentdto.AppointmentID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(appointmentdto);
            }

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            // ✅ Ensure Doctor and Patient exist
            var doctor = await _context.Doctors.FindAsync(appointmentdto.DoctorID);
            var patient = await _context.Patients.FindAsync(appointmentdto.ICNumber);
            if (doctor == null || patient == null)
            {
                return NotFound("Doctor or Patient not found.");
            }

            try
            {
                appointment.AppointmentDateTime = appointmentdto.AppointmentDateTime;
                appointment.AppointmentReason = appointmentdto.AppointmentReason;
                appointment.AppointmentStatus = appointmentdto.AppointmentStatus;
                appointment.AppointmentType = appointmentdto.AppointmentType;
                appointment.AppointmentNotes = appointmentdto.AppointmentNotes;
                appointment.RoomNumber = appointmentdto.RoomNumber;

                appointment.DoctorID = appointmentdto.DoctorID;
                appointment.ICNumber = appointmentdto.ICNumber;

                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Appointment.Any(a => a.AppointmentID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Redirect based on user role
            if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("Index", "Doctors");
            }
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Staff");
            }
        }


        // GET: Appointments/Delete/1
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var appointment = await _context.Appointment
                .FirstOrDefaultAsync(m => m.AppointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/1
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Doctor")
            {
                return RedirectToAction("Index", "Doctors");
            }
            else
            {
                return RedirectToAction("Index", "Staff");
            }
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointment.Any(e => e.AppointmentID == id);
        }
    }
}
