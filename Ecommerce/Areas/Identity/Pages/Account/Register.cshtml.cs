// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.UI.Services; // <-- ELIMINADO
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Ecommerce.Models; // <-- 1. AÑADIDO (¡Muy importante!)

namespace Ecommerce.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        // 2. CAMBIADO A ApplicationUser
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        // private readonly IEmailSender _emailSender; // <-- 3. ELIMINADO

        public RegisterModel(
            // 4. CAMBIADO A ApplicationUser
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger)
        // IEmailSender emailSender) // <-- 5. ELIMINADO
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            // _emailSender = emailSender; // <-- 6. ELIMINADO
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // 7. TUS CAMPOS PERSONALIZADOS (¡Están correctos!)
        public class InputModel
        {
            [Required(ErrorMessage = "El nombre es obligatorio.")]
            [StringLength(100)]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "El apellido es obligatorio.")]
            [StringLength(100)]
            [Display(Name = "Apellido")]
            public string Apellido { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} y un máximo de {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare("Password", ErrorMessage = "El password y la confirmación no coinciden.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // 8. CAMBIADO A ApplicationUser Y AÑADIDOS TUS CAMPOS
                var user = CreateUser(); // <-- Esto ahora crea un ApplicationUser

                // ¡Y AHORA ESTAS LÍNEAS FUNCIONAN!
                user.Nombre = Input.Nombre;
                user.Apellido = Input.Apellido;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // 9. LÓGICA DE EMAIL ELIMINADA.
                    // En lugar de enviar un email, simplemente iniciamos sesión.
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                // 10. CAMBIADO A ApplicationUser
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            // 11. CAMBIADO A ApplicationUser
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}