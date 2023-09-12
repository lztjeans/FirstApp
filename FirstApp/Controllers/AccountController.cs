using FirstApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using FirstApp.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using FirstApp.Service;
using Microsoft.AspNetCore.Http;

namespace Identity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public const string SessionKeyName = "_Name";
        public const string SessionKeyAge = "_Age";

        private readonly ITokenClaimsService _tokenClaimsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        public AccountController(UserManager<ApplicationUser> userMgr, SignInManager<ApplicationUser> signinMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/";
            }

            Login login = new()
            {
                ReturnUrl = returnUrl
            };
            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = await userManager.FindByEmailAsync(login.Email);
                if (appUser != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result  = await signInManager.PasswordSignInAsync(appUser, login.Password, login.Remember, false);

                    if ( result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(login.ReturnUrl) && Url.IsLocalUrl(login.ReturnUrl))
                        {
                            var claims = new List<Claim>
                            {
                                new Claim("Account", appUser.UserName),
                                new Claim("Email", appUser.Email ),
                               // new Claim(ClaimTypes.Role, "Administrator")
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                            //if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
                            //{
                            //    //Session["LoginAccount"] = item.Account;
                            //    //HttpContext.Session.SetString("LoginAccount", "The Doctor");
                            //    //HttpContext.Session.SetString("IsAuthenticated", result.Succeeded);
                            //    //HttpContext.Session.SetInt32(SessionKeyAge, 73);
                            //}
                            //var name = HttpContext.Session.GetString(SessionKeyName);
                            //var age = HttpContext.Session.GetInt32(SessionKeyAge).ToString();

                            //logger.Info("Session Name: {Name}", name);
                            //logger.Info("Session Age: {Age}", age);

                            return Redirect(login.ReturnUrl);
                            //return View("~/Views/Home/Index.cshtml", login);
                        }

                    }
                }
                logger.Error("Login Failed: Invalid Email or password.({0}:{1}, {2}:{3})", nameof(login.Email), login.Email, nameof(login.Email), nameof(login.Password), login.Password);
                ModelState.AddModelError(nameof(login.Email), "Login Failed: Invalid Email or password");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }



        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            if (!ModelState.IsValid)
                return View(email);

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return RedirectToAction(nameof(ForgotPasswordConfirmation));

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

            //EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = EmailHelper.SendEmailPasswordReset(user.Email, link);

            if (emailResponse)
                return RedirectToAction("ForgotPasswordConfirmation");
            else
            {
                // log email failed 
                logger.Fatal("email failed ");
            }
            return View("ForgotPassword");
        }

        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            //var model = new ResetPassword { Token = token, Email = email };
            return View(new ResetPassword { Token = token, Email = email });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if (!ModelState.IsValid)
                return View(resetPassword);

            var user = await userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
                RedirectToAction("ResetPasswordConfirmation");

            var resetPassResult = await userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return View();
            }

            return RedirectToAction("ResetPasswordConfirmation");
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurrentUser() =>    Ok(await CreateUserInfo(User));


        private async Task<UserInfo> CreateUserInfo(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity == null || claimsPrincipal.Identity.Name == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                return UserInfo.Anonymous;
            }

            var userInfo = new UserInfo
            {
                IsAuthenticated = true
            };

            if (claimsPrincipal.Identity is ClaimsIdentity claimsIdentity)
            {
                userInfo.NameClaimType = claimsIdentity.NameClaimType;
                userInfo.RoleClaimType = claimsIdentity.RoleClaimType;
            }
            else
            {
                userInfo.NameClaimType = "name";
                userInfo.RoleClaimType = "role";
            }

            if (claimsPrincipal.Claims.Any())
            {
                var claims = new List<ClaimValue>();
                var nameClaims = claimsPrincipal.FindAll(userInfo.NameClaimType);
                foreach (var claim in nameClaims)
                {
                    claims.Add(new ClaimValue(userInfo.NameClaimType, claim.Value));
                }

                foreach (var claim in claimsPrincipal.Claims.Except(nameClaims))
                {
                    claims.Add(new ClaimValue(claim.Type, claim.Value));
                }

                userInfo.Claims = claims;
            }

            var token = await _tokenClaimsService.GetTokenAsync(claimsPrincipal.Identity.Name);
            userInfo.Token = token;

            return userInfo;
        }
    }

}
//[AllowAnonymous]
//public IActionResult GoogleLogin()
//{
//    string redirectUrl = Url.Action("GoogleResponse", "Account");
//    var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
//    return new ChallengeResult("Google", properties);
//}

//[AllowAnonymous]
//public async Task<IActionResult> GoogleResponse()
//{
//    ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
//    if (info == null)
//        return RedirectToAction(nameof(Login));

//    var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
//    string[] userInfo = { info.Principal.FindFirst(ClaimTypes.Name).Value, info.Principal.FindFirst(ClaimTypes.Email).Value };
//    if (result.Succeeded)
//        return View(userInfo);
//    else
//    {
//        Employee user = new Employee
//        {
//            Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
//            UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
//        };

//        IdentityResult identResult = await userManager.CreateAsync(user);
//        if (identResult.Succeeded)
//        {
//            identResult = await userManager.AddLoginAsync(user, info);
//            if (identResult.Succeeded)
//            {
//                await signInManager.SignInAsync(user, false);
//                return View(userInfo);
//            }
//        }
//        return AccessDenied();
//    }
//}

//[AllowAnonymous]
//public async Task<IActionResult> LoginTwoStep(string email, string returnUrl)
//{
//    var user = await userManager.FindByEmailAsync(email);

//    var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

//    EmailHelper emailHelper = new EmailHelper();
//    bool emailResponse = emailHelper.SendEmailTwoFactorCode(user.Email, token);

//    return View();
//}

//[HttpPost]
//[AllowAnonymous]
//public async Task<IActionResult> LoginTwoStep(TwoFactor twoFactor, string returnUrl)
//{
//    if (!ModelState.IsValid)
//    {
//        return View(twoFactor.TwoFactorCode);
//    }

//    var result = await signInManager.TwoFactorSignInAsync("Email", twoFactor.TwoFactorCode, false, false);
//    if (result.Succeeded)
//    {
//        return Redirect(returnUrl ?? "/");
//    }
//    else
//    {
//        ModelState.AddModelError("", "Invalid Login Attempt");
//        return View();
//    }
//}