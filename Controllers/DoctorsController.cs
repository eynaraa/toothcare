using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toothcare_Appointment_System.Data;
using Toothcare_Appointment_System.Models;


namespace Toothcare_Appointment_System.Controllers
{
    [Route("api/Doctors")]
    [ApiController]
    public class APIDoctorsController : ControllerBase
    {
        private readonly ToothcareAppointmentContext _context;

        public APIDoctorsController(ToothcareAppointmentContext context)
        {
            _context = context;
        }

        // GET: api/Doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctors>>> GetDoctors()
        {
            return await _context.Doctors.ToListAsync();
        }

        // GET: api/Doctors/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctors>> GetDoctors(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return doctor;
        }


        // POST: api/Doctors
        [HttpPost]
        public async Task<ActionResult<Doctors>> PostDoctors(Doctors doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDoctors", new { id = doctor.DoctorID }, doctor);
        }

        // PUT: api/Doctors/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctors(int id, Doctors doctor)
        {
            if (id != doctor.DoctorID)
            {
                return BadRequest();
            }

            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Doctors/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctors(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }


    [Authorize]
    [Route("Doctors")]
    public class DoctorsController : Controller
    {
        private readonly ToothcareAppointmentContext _context;
        public DoctorsController(ToothcareAppointmentContext context)
        {
            _context = context;
        }

        // GET: Doctors
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(doctorIdClaim, out int doctorId))
            {
                return Unauthorized(); // ✅ Block unauthorized access
            }

            object model;

            if (User.IsInRole("Admin"))
            {
                model = await _context.Doctors.ToListAsync();
            }
            else
            {
                model = await _context.Appointment
                    .Include(a => a.Doctor)
                    .Include(a => a.Patient)
                    .Where(a => a.Doctor.DoctorID == doctorId)
                    .Select(item => new AppointmentDTO
                    {
                        AppointmentID = item.AppointmentID,
                        AppointmentDateTime = item.AppointmentDateTime,
                        AppointmentReason = item.AppointmentReason,
                        AppointmentStatus = item.AppointmentStatus,
                        AppointmentNotes = item.AppointmentNotes,
                        RoomNumber = item.RoomNumber,
                        AppointmentType = item.AppointmentType
                    }).ToListAsync();
            }

            return View(model);
        }

        // GET: Doctors/View/1
        [HttpGet("View/{id}")]
        public async Task<IActionResult> View(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var doctors = await _context.Doctors.FindAsync(id);
            if (doctors == null)
            {
                return NotFound();
            }
            return View(doctors);
        }

        // GET: Doctors/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Doctors/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctors doctors)
        {
            if (ModelState.IsValid)
            {
                doctors.DoctorPass = HashPassword(doctors.DoctorPass);
                _context.Add(doctors);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(doctors);
        }

        // GET: Doctors/Edit/1
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var doctors = await _context.Doctors.FindAsync(id);
            if (doctors == null)
            {
                return NotFound();
            }
            return View(doctors);
        }

        // POST: Doctors/Edit/1
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorsDTO doctor)
        {
            if (id != doctor.DoctorID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(doctor);
            }

            var existingDoctor = await _context.Doctors.FindAsync(id);
            if (existingDoctor == null)
            {
                return NotFound();
            }

            try
            {
                // ✅ Keep existing password if no new password is provided
                if (!string.IsNullOrEmpty(doctor.DoctorPass))
                {
                    existingDoctor.DoctorPass = HashPassword(doctor.DoctorPass); // ✅ Hash only if changed
                }

                // ✅ Update only necessary fields
                existingDoctor.DoctorName = doctor.DoctorName;
                existingDoctor.DoctorPhoneNo = doctor.DoctorPhoneNo;
                existingDoctor.DoctorEmail = doctor.DoctorEmail;
                existingDoctor.DoctorAddress = doctor.DoctorAddress;
                existingDoctor.DoctorLicenseNumber = doctor.DoctorLicenseNumber;
                existingDoctor.DoctorAvailability = doctor.DoctorAvailability;

                _context.Update(existingDoctor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Doctors.Any(d => d.DoctorID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }


        // GET: Doctors/Delete/1
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var doctors = await _context.Doctors
                .FirstOrDefaultAsync(m => m.DoctorID == id);
            if (doctors == null)
            {
                return NotFound();
            }
            return View(doctors);
        }

        // POST: Doctors/Delete/1
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctors = await _context.Doctors.FindAsync(id);
            _context.Doctors.Remove(doctors);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorsExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorID == id);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
