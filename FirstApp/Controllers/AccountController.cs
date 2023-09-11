﻿using FirstApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System.ComponentModel.DataAnnotations;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Identity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<Employee> userManager;
        private readonly SignInManager<Employee> signInManager;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        public AccountController(UserManager<Employee> userMgr, SignInManager<Employee> signinMgr)
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
                Employee appUser = await userManager.FindByEmailAsync(login.Email);
                if (appUser != null)
                {
                    await signInManager.SignOutAsync();
                    SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, login.Remember, false);

                    login.IsAuthenticated = result.Succeeded;

                    if ( result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(login.ReturnUrl) && Url.IsLocalUrl(login.ReturnUrl))
                        {
                            return View("~/Views/Home/Index.cshtml", login);
                        }
                        else
                        {
                            return Redirect(login.ReturnUrl);
                        }
                        //return View("~/Views/Home/Index.cshtml", login);
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