using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginAndReg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LoginAndReg.Controllers
{
    public class HomeController : Controller
    {
        private string UserInSession
        {
            get{ return HttpContext.Session.GetString("UserSession");}
            set{ HttpContext.Session.SetString("UserSession", value);}
        }

        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }


        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("add")]
        public IActionResult AddUser(User fromForm)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == fromForm.Email))
                {
                    ModelState.AddModelError("Email", "This Email is already in use!");
                    return RedirectToAction("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                fromForm.Password = Hasher.HashPassword(fromForm, fromForm.Password);
                dbContext.Add(fromForm);
                dbContext.SaveChanges();
                return RedirectToAction("LoginPage");
            }
            else
            {
                return View("Index");
            }

        }

        [HttpGet("login")]

        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost("loginUser")]

        public IActionResult LoginUser(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);

                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("LoginPage");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                if (result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return View("LoginPage");                    
                }
                HttpContext.Session.SetString("UserSession", "true");
                return RedirectToAction ("Success");
            }
            else
            {
                
                return View("LoginPage");
            }
        }

        [HttpGet("Success")]
        public IActionResult Success()
        {
            if(UserInSession == "true")
            {
            return View("Success");
            }
            else
            {
                return RedirectToAction("LoginPage");
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

  
    }
}
