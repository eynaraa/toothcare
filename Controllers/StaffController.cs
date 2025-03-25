using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toothcare_Appointment_System.Data;
using Toothcare_Appointment_System.Models;


namespace Toothcare_Appointment_System.Controllers
{
    [Route("api/Staff")]
    [ApiController]
    public class APIStaffController : ControllerBase
    {
        private readonly ToothcareAppointmentContext _context;

        public APIStaffController(ToothcareAppointmentContext context)
        {
            _context = context;
        }

        // GET: api/Staff
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaff()
        {
            return await _context.Staff.ToListAsync();
        }

        // GET: api/Staff/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Staff>> GetStaff(int id)
        {
            var staff = await _context.Staff.FindAsync(id);

            if (staff == null)
            {
                return NotFound();
            }

            return staff;
        }


        // POST: api/staff
        [HttpPost]
        public async Task<ActionResult<Staff>> PostStaff(Staff staff)
        {
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStaff", new { id = staff.StaffID }, staff);
        }

        // PUT: api/Staff/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaff(int id, Staff staff)
        {
            if (id != staff.StaffID)
            {
                return BadRequest();
            }

            _context.Entry(staff).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Staff/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }

    [Authorize]
    [Route("staff")]
    public class StaffController : Controller
    {
        private readonly ToothcareAppointmentContext _context;

        public StaffController(ToothcareAppointmentContext context)
        {
            _context = context;
        }

        // GET: Staff
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            object model;

            if (User.IsInRole("Admin"))
            {
                model = await _context.Staff.ToListAsync();
            }
            else
            {
                model = await _context.Appointment.Select(item => new AppointmentDTO
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
            ViewData["ModelType"] = model.GetType(); // Store type info for Razor view

            return View(model); // Returns the list view of staff
        }

        // GET: Staff/Details/1
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff); // Returns the details view of a staff
        }

        // GET: Staff/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(); // Returns the create view of a staff
        }

        // POST: Staff/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {
            if (ModelState.IsValid)
            {
                staff.StaffPass = HashPassword(staff.StaffPass);
                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(staff); // Returns the create view of a staff
        }

        // GET: Staff/Edit/1
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff); // Returns the edit view of a staff
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StaffDTO staff)
        {
            if (id != staff.StaffID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(staff);
            }

            var existingStaff = await _context.Staff.FindAsync(id);
            if (existingStaff == null)
            {
                return NotFound();
            }

            try
            {
                // Keep existing password if no new password is provided
                if (!string.IsNullOrEmpty(staff.StaffPass))
                {
                    existingStaff.StaffPass = HashPassword(staff.StaffPass); // Hash only if changed
                }

                // Update only necessary fields
                existingStaff.StaffName = staff.StaffName;
                existingStaff.StaffPhoneNo = staff.StaffPhoneNo;
                existingStaff.StaffRole = staff.StaffRole;
                existingStaff.StaffEmail = staff.StaffEmail;

                _context.Update(existingStaff);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(staff.StaffID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: Staff/Delete/1
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var staff = await _context.Staff.FirstOrDefaultAsync(m => m.StaffID == id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff); // Returns the delete view of a student
        }

        // POST: Staff/Delete/1
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: staff/View/1
        [HttpGet("View/{id}")]
        public async Task<IActionResult> View(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff); // Returns the view view of a staff
        }

        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.StaffID == id);
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
