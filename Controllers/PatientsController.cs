using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toothcare_Appointment_System.Data;
using Toothcare_Appointment_System.Models;

namespace Toothcare_Appointment_System.Controllers
{
    [Route("api/Patients")]
    [ApiController]
    public class APIPatientsController : ControllerBase
    {
        private readonly ToothcareAppointmentContext _context;
        public APIPatientsController(ToothcareAppointmentContext context)
        {
            _context = context;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patients>>> GetPatients()
        {
            return await _context.Patients.ToListAsync();
        }

        // GET: api/Patients/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Patients>> GetPatients(string id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return patient;
        }

        // POST: api/Patients
        [HttpPost]
        public async Task<ActionResult<Patients>> PostPatients(Patients patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPatients", new { id = patient.ICNumber }, patient);
        }

        // PUT: api/Patients/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatients(string id, Patients patient)
        {
            if (id != patient.ICNumber)
            {
                return BadRequest();
            }
            _context.Entry(patient).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Patients/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatients(string id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    [Route("Patients")]
    public class PatientsController : Controller
    {
        private readonly ToothcareAppointmentContext _context;
        public PatientsController(ToothcareAppointmentContext context)
        {
            _context = context;
        }

        // GET: Patients
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Patients.ToListAsync());
        }

        //GET: Patients/View/1
        [HttpGet("View/{id}")]
        public async Task<IActionResult> View(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var patients = await _context.Patients.FirstOrDefaultAsync(m => m.ICNumber == id);
            if (patients == null)
            {
                return NotFound();
            }
            return View(patients);
        }

        // GET: Patients/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patients patients)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patients);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patients);
        }

        // GET: Patients/Edit/1
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var patients = await _context.Patients.FindAsync(id);
            if (patients == null)
            {
                return NotFound();
            }
            return View(patients);
        }

        // POST: Patients/Edit/1
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Patients patients)
        {
            if (id != patients.ICNumber)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patients);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientsExists(patients.ICNumber))
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
            return View(patients);
        }

        // GET: Patients/Delete/1
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var patients = await _context.Patients.FirstOrDefaultAsync(m => m.ICNumber == id);
            if (patients == null)
            {
                return NotFound();
            }
            return View(patients);
        }

        // POST: Patients/Delete/1
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var patients = await _context.Patients.FindAsync(id);
            _context.Patients.Remove(patients);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientsExists(string id)
        {
            return _context.Patients.Any(e => e.ICNumber == id);
        }
    }
}
