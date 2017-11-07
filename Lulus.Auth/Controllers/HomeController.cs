﻿
using System.Threading.Tasks;
using IdentityServer4.Services;
using Lulus.Auth.DataSource;
using Lulus.Auth.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
namespace Lulus.Auth.Controllers
{
    [SecurityHeaders]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        public HomeController(IIdentityServerInteractionService interaction,AuthDbContext authDbContext)
        {
            authDbContext.Database.EnsureCreated();
            _interaction = interaction;
        }
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();
            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }
            return View("Error", vm);
        }
    }
}