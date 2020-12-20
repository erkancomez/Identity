using Identity.Context;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Controllers
{
    [Authorize]
    public class RolController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RolController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {

            return View(_roleManager.Roles.ToList());
        }
        public IActionResult AddRole()
        {
            return View(new RoleViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppRole role = new AppRole
                {
                    Name = model.Name
                };
                var identityResult = await _roleManager.CreateAsync(role);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public IActionResult UpdateRole(int id)
        {
            var role = _roleManager.Roles.FirstOrDefault(I => I.Id == id);

            RoleUpdateViewModel model = new RoleUpdateViewModel
            {
                Id = role.Id,
                Name = role.Name

            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(RoleUpdateViewModel model)
        {
            var tobeUpdateRole = _roleManager.Roles.Where(I => I.Id == model.Id).FirstOrDefault();
            tobeUpdateRole.Name = model.Name;
            var identityResult = await _roleManager.UpdateAsync(tobeUpdateRole);

            if (identityResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteRole(int id)
        {
            var tobeUpdateRole = _roleManager.Roles.Where(I => I.Id == id).FirstOrDefault();
            
            var identityResult = await _roleManager.DeleteAsync(tobeUpdateRole);

            if (identityResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            TempData["Errors"] = identityResult.Errors;
            return RedirectToAction("Index");
        }

        public IActionResult UserList()
        {
            return View(_userManager.Users.ToList());
        }
        public async Task<IActionResult> AssignRole(int id)
        {
            var user = _userManager.Users.FirstOrDefault(I => I.Id == id);

            TempData["UserId"] = user.Id;

            var roles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            List<RoleAssignViewModel> models = new List<RoleAssignViewModel>();

            foreach (var item in roles)
            {
                RoleAssignViewModel model = new RoleAssignViewModel();
                model.RoleId = item.Id;
                model.Name = item.Name;
                model.Exist = userRoles.Contains(item.Name);
                models.Add(model);
            }
            return View(models);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(List<RoleAssignViewModel> models)
        {
            var userId = (int)TempData["UserId"];
            var user = _userManager.Users.FirstOrDefault(IActionResult => IActionResult.Id == userId);

            foreach (var item in models)
            {
                if (item.Exist)
                {
                    await _userManager.AddToRoleAsync(user, item.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, item.Name);
                }
            }
            return RedirectToAction("UserList");
        }

    }
}
