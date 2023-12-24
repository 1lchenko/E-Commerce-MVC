using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Models;
using ECommerce.Models.IdentityModels;
using ECommerce.Models.IdentityModels.ResetPassword;
using ECommerce.Repositories;
using ECommerce.Services;
using NuGet.Protocol;

namespace ECommerce.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;
        private readonly ISendEmailService _sendEmailService;
        private readonly ILogger<AccountController> _logger;
       

        public AccountController(
            UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, 
            IHttpContextAccessor httpContextAccessor, 
            IOrderRepository repository,
            ISendEmailService sendEmailService,
            ILogger<AccountController> logger
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = repository;
            _sendEmailService = sendEmailService;
            _logger = logger;

        }
        
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
       
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (ModelState.IsValid)
            {   
                IdentityUser? user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager
                        .PasswordSignInAsync(user, loginDto.Password, loginDto.Remember, false);
                    
                    if (result.Succeeded)
                    {
                        return Redirect(loginDto.ReturnUrl ?? "/");
                    }
                    ModelState.AddModelError("Fail Sign In", "Fail Sign In");
                }
                else
                {
                    ModelState.AddModelError("Fail Authentication", "Incorrect input email or password");
                }
                
            }
            return View(loginDto);

        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View("ResetPassword/ForgotPassword");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
              
                if (user == null)
                {
                    return View("ResetPassword/ForgotPasswordConfirmation");
                }
                
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

                bool isSendEmail = _sendEmailService.EmailSend(model.Email, 
                    "Reset Your Password", 
                    "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>",
                    true);
                
                if (isSendEmail)
                {
                    return View("ResetPassword/ForgotPasswordConfirmation");
                }
                
                _logger.LogError("In Action {0} email not sending. Method EmailSend of ISendEmailServise returns variable isSendEmail with {1} result", 
                    nameof(ForgotPassword),isSendEmail );
            }
            
            return View("ResetPassword/ForgotPassword",model);
        }

        
        public async Task<IActionResult> UpdatePassword()
        {
            var user = await _userManager.FindByIdAsync(GetUserId());
            
            string code = await _userManager.GeneratePasswordResetTokenAsync(user!);
            ViewBag.IsConfirm = user!.EmailConfirmed;
            
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

            var isSendEmail = _sendEmailService.EmailSend(user.Email!, "Reset Your Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>", true);
            if (!isSendEmail)
            {
                _logger.LogError("In Action {0} email not sending. Method EmailSend of ISendEmailServise returns variable isSendEmail with {1} result", 
                    nameof(UpdatePassword),isSendEmail );
                 
            }
            return View("Settings");
        }

         

        [AllowAnonymous]
        public ActionResult ResetPassword(string? code, string userId)
        {

            return code == null ? NotFound() : View("ResetPassword/ResetPassword",new ResetPasswordViewModel {Code = code, UserId  = userId });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ResetPassword/ResetPassword",model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            
            if (user == null)
            {
                return NotFound("User Not Found");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
           
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Settings));
            }
 
            _logger.LogError("In Action {Name}, function {FuncName} return Not Succeed result. Errors: {Errors}", 
                nameof(ResetPassword), nameof(_userManager.ResetPasswordAsync) ,result.Errors.ToJson());
            
            ModelState.AddModelError("ResetPasswordError", "Happened error during reset password");
            
            return View("ResetPassword/ResetPassword",model);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        } 

        [AllowAnonymous]
        public IActionResult Registration()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(RegistrationDTO registrationDto)
        {
            
            if (ModelState.IsValid)
            {
                IdentityUser newUser = new IdentityUser
                {
                    Email = registrationDto.Email,
                    UserName = registrationDto.UserName,
                };
                IdentityResult result = await _userManager.CreateAsync(newUser, registrationDto.Password);
               
                if (result.Succeeded)
                {
                     
                    await _userManager.AddToRoleAsync(newUser, Roles.User.ToString());
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(newUser, registrationDto.Password, false, false);


                    if (signInResult.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");   
                    }

                    _logger.LogError("In Action {Name}, function {FuncName} return Not Succeed result. Errors: {Errors}", 
                            nameof(Registration), nameof(_signInManager.PasswordSignInAsync) ,result.Errors.ToJson());
                    ModelState.AddModelError("IdentityErrors", "Registration is failed");
                }
                else
                {
                    _logger.LogError("In Action {Name}, function {FuncName} return Not Succeed result. Errors: {Errors}", 
                        nameof(Registration), nameof(_userManager.CreateAsync) ,result.Errors.ToJson());

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("IdentityErrors", "Registration is failed"); 
                    }
                }
            }
            return View(registrationDto);
        }

        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.FindByIdAsync(GetUserId());

            ViewBag.IsConfirm = user!.EmailConfirmed;

            return View();
        }

        public async Task<IActionResult> ConfirmEmail()
        {
            var user = await _userManager.FindByIdAsync(GetUserId());
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user!);
            var confirmationLink = Url.Action("ApplyConfirmEmail", "Account", new { token, email = user!.Email }, Request.Scheme);
            bool isSendEmail = _sendEmailService.EmailSend(
                user.Email!, 
                "Confirm Your Email", 
                "Please confirm your email by clicking <a href=\"" + confirmationLink + "\">here</a>", 
                true);
            
            if (!isSendEmail)
            {
                _logger.LogError("In Action {0} email not sending. Method EmailSend of ISendEmailServise returns variable isSendEmail with {1} result", 
                    nameof(ConfirmEmail),isSendEmail ); ;
            }

            return RedirectToAction("Settings");
        }

        public async Task<IActionResult> ApplyConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found for Apply Confirm Email");
            }
            
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                _logger.LogError("In Action {Name}, function {FuncName} return Not Succeed result. Errors: {Errors}", 
                    nameof(ApplyConfirmEmail), nameof(_userManager.ConfirmEmailAsync) ,result.Errors.ToJson());
                return BadRequest("Apply Confirm Email Operation Is Failed");
            }
                
            return RedirectToAction(nameof(Settings));
            
        }
        
        public async Task<IActionResult> Manage(bool loadAll = false)
        {
            string userId = GetUserId();
            IdentityUser? user = await _userManager.FindByIdAsync(userId);
            List<Order> orders = await _orderRepository.GetAllOrderByUserAsync(userId,loadAll);

            UserDTO userDto = new UserDTO
            {
                UserName = user!.UserName!,
                Email = user.Email!,
                Orders = orders,

            };

            ViewBag.LoadAll = loadAll;
            return View(userDto);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Control(int? orderNumber)
        {
            if (orderNumber != null)
            {
                var order = await _orderRepository.GetOrderByIdAsync(orderNumber, true);
                
                if (order == null)
                {
                    ModelState.AddModelError("OrderNotFound", "Order not found");
                    return View();
                }
                
                return View(order);
            }

            return View();

        }

        
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
