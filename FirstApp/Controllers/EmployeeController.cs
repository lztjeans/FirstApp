using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NLog;

namespace FirstApp.Controllers
{
    public class EmployeeController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private IPasswordHasher<ApplicationUser> passwordHasher;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public EmployeeController(UserManager<ApplicationUser> usrMgr, IPasswordHasher<ApplicationUser> passwordHash)
        {
            userManager = usrMgr;
            passwordHasher = passwordHash;
        }
        public IActionResult Index()
        {
            //return View();
            //return View(Repository.AllEmpoyees);
            return View(userManager.Users);
        }

        // HTTP GET VERSION
        public IActionResult Create()
        {
            return View();
        }

        // HTTP POST VERSION  
        //[HttpPost]
        //public IActionResult Create(Employee employee)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        Repository.Create(employee);
        //        return View("Thanks", employee);
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Department = user.Department,
                    Password = user.Password,
                    Age = user.Age,
                    Sex = user.Sex,
                    Salary = user.Salary ,
                    //TwoFactorEnabled = true
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    StringBuilder errorDescription = new();
                    //foreach (IdentityError error in result.Errors)
                    //    errorDescription = error.Description;
                    //    ModelState.AddModelError("CreateErrors", errorDescription);
                    //    Console.WriteLine(errorDescription);

                    result.Errors.ToList().ForEach(err => errorDescription.Append("<p>").Append(err.Description).Append("</p>"));
                    ModelState.AddModelError("CreateErrors", errorDescription.ToString());
                    logger.Error(errorDescription.ToString());
                }
            }

            return View(user);
        }


        public async Task<IActionResult> Update(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
                //return View("C:\\Users\\user\\Source\\Repos\\FirstApp\\FirstApp\\Views\\Employee\\Update.cshtml", user);
                return View(user);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, string UserName, string email, string Age, string Salary, string Department, char Sex)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(UserName))
                    user.UserName = UserName;
                else
                    ModelState.AddModelError("", "UserName cannot be empty");

                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(Age))
                    //user.PasswordHash = passwordHasher.HashPassword(user, password);
                    user.Age = Convert.ToInt32(Age);
                else
                    ModelState.AddModelError("", "Age cannot be empty");

                if (!string.IsNullOrEmpty(Salary))
                    user.Salary = Convert.ToDecimal(Salary);
                else
                    ModelState.AddModelError("", "Salary cannot be empty");

                if (!string.IsNullOrEmpty(Department))
                    user.Department = Department;
                else
                    ModelState.AddModelError("", "Department cannot be empty");

                user.Sex = Sex;
                if (ModelState.ErrorCount == 0)
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", userManager.Users);
        }

        //public IActionResult Update(string empname)
        //{
        //    Employee employee = Repository.AllEmpoyees.Where(e => e.UserName == empname).FirstOrDefault();
        //    return View(employee);
        //}

        //[HttpPost]
        //public IActionResult Update(Employee employee, string empname)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Repository.AllEmpoyees.Where(e => e.UserName == empname).FirstOrDefault().Age = employee.Age;
        //        Repository.AllEmpoyees.Where(e => e.UserName == empname).FirstOrDefault().Salary = employee.Salary;
        //        Repository.AllEmpoyees.Where(e => e.UserName == empname).FirstOrDefault().Department = employee.Department;
        //        Repository.AllEmpoyees.Where(e => e.UserName == empname).FirstOrDefault().Sex = employee.Sex;
        //        Repository.AllEmpoyees.Where(e => e.UserName == empname).FirstOrDefault().UserName = employee.UserName;

        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}
        //[HttpPost]
        //public IActionResult Delete(string empname)
        //{
        //    Employee employee = Repository.AllEmpoyees.Where(e => e.UserName == empname).FirstOrDefault();
        //    Repository.Delete(employee);
        //    return RedirectToAction("Index");
        //}


        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

    }
}