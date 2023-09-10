using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FirstApp.Controllers
{
    public class EmployeeController : Controller
    {
        private UserManager<Employee> userManager;
        private IPasswordHasher<Employee> passwordHasher;

        public EmployeeController(UserManager<Employee> usrMgr, IPasswordHasher<Employee> passwordHash)
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
        public async Task<IActionResult> Create(Employee user)
        {
            if (ModelState.IsValid)
            {
                Employee appUser = new Employee
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    //TwoFactorEnabled = true
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }


        public async Task<IActionResult> Update(string id)
        {
            Employee user = await userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, string email, string password)
        {
            Employee user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(password))
                    user.PasswordHash = passwordHasher.HashPassword(user, password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
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
            Employee user = await userManager.FindByIdAsync(id);
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