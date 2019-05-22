using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProgrammingJokes.Web.Models;
using ProgrammingJokes.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ProgrammingJokes.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }

        public IActionResult Index(int? jokeId)
        {
            JokeRepository repo = new JokeRepository(_connectionString);
            IndexViewModel vm = new IndexViewModel();
            vm.Joke = jokeId.HasValue ?  repo.GetJokeByid(jokeId.Value) : repo.GetJoke();
            vm.LikeCount = repo.GetLikeCount(vm.Joke.Id);
            vm.CurrentUser = repo.GetUserByEmail(User.Identity.Name);
            return View(vm);
        }

        public IActionResult Jokes()
        {
            JokeRepository repo = new JokeRepository(_connectionString);
            JokesViewModel vm = new JokesViewModel();
            vm.Jokes = repo.GetAllJokes();
            vm.CurrentUser = repo.GetUserByEmail(User.Identity.Name);
            return View(vm);
        }

        public IActionResult AddLike(int jokeId, bool likes)
        {
            JokeRepository repo = new JokeRepository(_connectionString);

            return Json(repo.AddLike(jokeId, repo.GetUserByEmail(User.Identity.Name).Id, likes));
        }

        public IActionResult LogIn()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddLogIn(string email, string password)
        {
            JokeRepository repo = new JokeRepository(_connectionString);
            User user = repo.Verify(email, password);
            if (user == null)
            {
                return Redirect("/home/index");
            }
            var claims = new List<Claim> { new Claim("user", email) };

            HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/index");
        }

        [HttpPost]
        public IActionResult AddSignup(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            JokeRepository repo = new JokeRepository(_connectionString);
            repo.AddUser(user);
            return Redirect("/home/index");
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync().Wait();

            return Redirect("/home/index");
        }
    }
}
