//using FirstApp.Models;
using FirstApp.Services;
using FirstApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
using NLog;
//using System.Text;
using FirstApp.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Microsoft.AspNetCore.Identity;
using System.Text;
using FirstApp.Models;

namespace FirstApp.Controllers
{
    public class CatalogController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ICatalogViewModelService _catalogViewModelService;

        public CatalogController(ICatalogViewModelService catalogViewModelService)
        {
            _catalogViewModelService = catalogViewModelService;
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> IndexAsync()
        //{
        //    ////return View();
        //    ////return View(Repository.AllEmpoyees);
        //    var vm = await _catalogViewModelService.GetCatalogItems(0, Constants.ITEMS_PER_PAGE, null, null);
        //    return View(vm);
        //}



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(CatalogIndexViewModel vm, int? pageId)
        {
            vm = await _catalogViewModelService.GetCatalogItems(pageId ?? 0, 5, vm.BrandFilterApplied, vm.TypesFilterApplied);
            return View(vm);
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> List()
        {
            var _list = await _catalogViewModelService.GetAllItems();

            return View(_list);
            //return View("Catalog/List.cshtml", _list);
            //return View(Repository.AllEmpoyees);
            //return View(userManager.Users);
        }

        public async Task<IActionResult> Create()
        {
            List<SelectListItem> _brands = (await _catalogViewModelService.GetBrands()).ToList();
            List<SelectListItem> _types = (await _catalogViewModelService.GetTypes()).ToList();
            ViewBag.Brands = _brands;
            ViewBag.Types = _types;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CatalogItemViewModel item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                     _catalogViewModelService.CreateItem(new CatalogItem(item.TypeId,item.BrandId ,item.Description ,item.Name ,item.Price,item.PictureUri ));
                    return RedirectToAction ("Create");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }

            }
            return View(item);
        }


        public async Task<IActionResult> Update(string id)
        {
            CatalogItemViewModel user = await _catalogViewModelService.GetItemById(Convert.ToInt32(id));
            if (user != null)
            {
                List<SelectListItem> _brands = (await _catalogViewModelService.GetBrands()).ToList();
                List<SelectListItem> _types = (await _catalogViewModelService.GetTypes()).ToList();
                ViewBag.Brands = _brands;
                ViewBag.Types = _types;
                //_brands.Find(x =>user.BrandId.ToString().Equals( x.Value)).Selected = true;

                return View(user);
            }
            else
                return RedirectToAction("List");
        }

        //[HttpPost]
        //public async Task<IActionResult> Update(string id, string UserName, string email, string Age, string Salary, string Department, char Sex)
        //{
        //    ApplicationUser user = await userManager.FindByIdAsync(id);
        //    if (user != null)
        //    {
        //        if (!string.IsNullOrEmpty(UserName))
        //            user.UserName = UserName;
        //        else
        //            ModelState.AddModelError("", "UserName cannot be empty");

        //        if (!string.IsNullOrEmpty(email))
        //            user.Email = email;
        //        else
        //            ModelState.AddModelError("", "Email cannot be empty");

        //        if (!string.IsNullOrEmpty(Age))
        //            //user.PasswordHash = passwordHasher.HashPassword(user, password);
        //            user.Age = Convert.ToInt32(Age);
        //        else
        //            ModelState.AddModelError("", "Age cannot be empty");

        //        if (!string.IsNullOrEmpty(Salary))
        //            user.Salary = Convert.ToDecimal(Salary);
        //        else
        //            ModelState.AddModelError("", "Salary cannot be empty");

        //        if (!string.IsNullOrEmpty(Department))
        //            user.Department = Department;
        //        else
        //            ModelState.AddModelError("", "Department cannot be empty");

        //        user.Sex = Sex;
        //        if (ModelState.ErrorCount == 0)
        //        {
        //            IdentityResult result = await userManager.UpdateAsync(user);
        //            if (result.Succeeded)
        //                return RedirectToAction("Index");
        //            else
        //                Errors(result);
        //        }
        //    }
        //    else
        //        ModelState.AddModelError("", "User Not Found");
        //    return View(user);
        //}

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            CatalogItemViewModel vm = await _catalogViewModelService.GetItemById(Convert.ToInt32(id));
            if (vm != null)
                 _catalogViewModelService.DeleteItem(vm.ToEntity());
            else
                ModelState.AddModelError("", "User Not Found");
            //return View("Index", userManager.Users);
            return RedirectToAction("List");
        }
    }

}

