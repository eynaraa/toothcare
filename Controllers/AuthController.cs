using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Toothcare_Appointment_System.Data;
using Toothcare_Appointment_System.Models;

public class AuthController : Controller
{
    private readonly ToothcareAppointmentContext _context;

    public AuthController(ToothcareAppointmentContext context)
    {
        _context = context;
    }

    // GET /login
    [HttpGet("login")]
    public IActionResult Login()
    {
        //  Check if user is already logged in
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")))
        {
            // Redirect based on role
            return RedirectToAction("Index", HttpContext.Session.GetString("UserRole") == "Doctor" ? "Doctors" : "Staff");
        }

        return View();
    }

    [HttpGet("Register/{id}")]
    public IActionResult Register(string id)
    {
        //  Check if user is already logged in
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")))
        {
            // Redirect based on role
            return RedirectToAction("Index", HttpContext.Session.GetString("UserRole") == "Doctor" ? "Doctors" : "Staff");
        }

        if (id == "Doctor")
        {
            return View("~/Views/Auth/RegisterDoctor.cshtml");
        }
        else if (id == "Staff")
        {
            return View("~/Views/Auth/RegisterStaff.cshtml");
        }
        else
        {
            return NotFound();
        }
    }

    // POST: Register/Staff
    [HttpPost("Register/Staff")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterStaff(Staff staff)
    {
        if (ModelState.IsValid)
        {
            staff.StaffPass = HashPassword(staff.StaffPass);
            _context.Add(staff);
            await _context.SaveChangesAsync();

            TempData["RegisterMessage"] = "Account created successfully!";
            TempData["RegisterUserId"] = "STF" + staff.StaffID;

            return RedirectToAction("Login");
        }
        return View(staff); // Returns the create view of a staff
    }

    // POST: Register/Doctor
    [HttpPost("Register/Doctor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterDoctor(Doctors doctors)
    {
        if (ModelState.IsValid)
        {
            doctors.DoctorPass = HashPassword(doctors.DoctorPass);
            _context.Add(doctors);
            await _context.SaveChangesAsync();

            TempData["RegisterMessage"] = "Account created successfully!";
            TempData["RegisterUserId"] = "DOC" + doctors.DoctorID;

            return RedirectToAction("Login");
        }
        return View(doctors); // Returns the create view of a staff
    }

    // POST /login
    [HttpPost("login")]
    public async Task<IActionResult> Login(string ID, string password)
    {
        string hashedPassword = HashPassword(password);
        if (ID.ToUpper().StartsWith("STF"))
        {
            int id = int.Parse(ID.Substring(3));
            var user = _context.Staff.FirstOrDefault(u => u.StaffID == id && u.StaffPass == hashedPassword);
            if (user == null)
            {
                ViewBag.Error = "Invalid id or password.";
                return View();
            }

            // Ensure authentication cookie is set
            HttpContext.Session.SetString("UserID", user.StaffID.ToString());
            HttpContext.Session.SetString("UserRole", user.StaffRole.ToString());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.StaffID.ToString()), //  Use an ID instead
                new Claim(ClaimTypes.Role, user.StaffRole)
            };

            var identity = new ClaimsIdentity(claims, "MyAuthScheme");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);

            return RedirectToAction("Index", "Staff");
        }
        else if (ID.ToUpper().StartsWith("DOC"))
        {
            int id = int.Parse(ID.Substring(3));
            var user = _context.Doctors.FirstOrDefault(u => u.DoctorID == id && u.DoctorPass == hashedPassword);
            if (user == null)
            {
                ViewBag.Error = "Invalid id or password.";
                return View();
            }

            // Ensure authentication cookie is set
            HttpContext.Session.SetString("UserID", user.DoctorID.ToString());
            HttpContext.Session.SetString("UserRole", "Doctor");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.DoctorID.ToString()), //  Use an ID instead
                new Claim(ClaimTypes.Role, "Doctor")
            };

            var identity = new ClaimsIdentity(claims, "MyAuthScheme");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);

            return RedirectToAction("Index", "Doctors");
        }
        else
        {
            ViewBag.Error = "Id not follow system specification.";
            return View();
        }
    }

    // Logout and clear session
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Remove("UserID");
        HttpContext.Session.Remove("UserRole");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
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
