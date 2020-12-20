using Identity.Context;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View(new UserSignInViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GirisYap(UserSignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var identityResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);

                if (identityResult.IsLockedOut)
                {
                    var gelen = _userManager.GetLockoutEndDateAsync(await _userManager.FindByNameAsync(model.UserName));
                    var kisitlananSure = gelen.Result.Value.Minute;

                    var kalanDakika =  kisitlananSure - DateTime.Now.Minute;

                    ModelState.AddModelError("", $"3 kere yanlış girdiğiniz için hesabınız {kalanDakika} dakika kilitlenmiştir. ");
                    return View("Index", model);
                }
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index", "Panel");
                }
                if (identityResult.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Email adresinizi lütfen doğrulayınız.");
                    return View("Index", model);
                }
                var yanlisSayisi = await _userManager.GetAccessFailedCountAsync(await _userManager.FindByNameAsync(model.UserName));
                ModelState.AddModelError("", $"Kullanıcı adı ya da şifre hatalı {3-yanlisSayisi} kadar yanlış girerseniz hesabınız bloklanacak");
               

                
            }
            return View("Index", model);
        }

        public IActionResult SignUp()
        {
            return View(new UserSignUpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserSignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    Email = model.Email,
                    Name = model.Name,
                    SurName = model.SurName,
                    UserName = model.UserName

                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }


}
