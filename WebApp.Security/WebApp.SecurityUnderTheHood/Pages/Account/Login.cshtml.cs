using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;

namespace WebApp.SecurityUnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new Credential();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Xác minh thông tin
            if (Credential.UserName == "admin" && Credential.Password == "123")
            {
                // Tạo danh sách các claim cho người dùng.
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("Department","HR"),
                    new Claim("Admin", "true"),
                    new Claim("Manager", "true"),
                    new Claim("EmploymentDate", "2024-01-05")
                };

                // Tạo một đối tượng ClaimsIdentity từ danh sách các claim và scheme xác thực.
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                // Tạo một đối tượng ClaimsPrincipal chứa ClaimsIdentity
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };

                // Đăng nhập người dùng bằng hệ thống xác thực dựa trên cookie.
                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authProperties);

                return RedirectToPage("/Index");
            }

            return Page();

            //if (!ModelState.IsValid) return Page();

            //// Verify the credential
            //if (Credential.UserName == "admin" && Credential.Password == "password")
            //{
            //    // Creating the security context
            //    var claims = new List<Claim> {
            //        new Claim(ClaimTypes.Name, "admin"),
            //        new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
            //        new Claim("Department", "HR"),
            //        new Claim("Admin", "true"),
            //        new Claim("Manager", "true"),
            //        new Claim("EmploymentDate", "2023-01-01")
            //    };
            //    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            //    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            //    var authProperties = new AuthenticationProperties
            //    {
            //        IsPersistent = Credential.RememberMe
            //    };

            //    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authProperties);

            //    return RedirectToPage("/Index");
            //}

            //return Page();
        }
    }
    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
