﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ImkStala.DataAccess;
using ImkStala.DataAccess.Entities;
using ImkStala.Services;
using ImkStala.ServicesContracts;
using ImkStala.Web.Services;
using ImkStala.Web.ViewModels.Restaurant;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ImkStala.Web.Controllers
{
    [Authorize]
    public class RestaurantController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IApplicationService _applicationService;
        //private readonly ApplicationDbContext _context;

        public RestaurantController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory, IApplicationService applicationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _applicationService = applicationService;
        }

        // GET: /<controller>/
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            IndexViewModel model = null;
            if (user.AccountType == "Restaurant")
            {
                model = new IndexViewModel()
                {

                };
            }
            return View(model);
        }

        public async Task<IActionResult> ViewTables()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            //Restaurant restaurantData = await _context.Restaurants.FirstOrDefaultAsync(w => w.ApplicationUser.Id == user.Id);
            //List<RestaurantTable> tables = await _context.RestaurantTables.Where(w => w.Restaurant.Id == restaurantData.Id).ToListAsync();
            IList<RestaurantTable> tables =
                _applicationService.GetRestaurantTablesByUserId(user.Id);
            //IEnumerable<Table> tables = GetTablesEnumeration();
            ViewTablesViewModel viewTablesViewModel = null;
            if (user.AccountType == "Restaurant")
            {
                viewTablesViewModel = new ViewTablesViewModel()
                {
                    Tables = tables
                };
            }
            return View(viewTablesViewModel);
        }

        [HttpGet]
        public IActionResult AddTable()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTable(AddTableViewModel tableViewModel)
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            //ApplicationUser applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(w => w.Id == user.Id);
            //Restaurant restaurantData = await _context.Restaurants.FirstOrDefaultAsync(w => w.ApplicationUser.Id == user.Id);
            if (user.AccountType == "Restaurant")
            {
                Restaurant restaurant = _applicationService.GetRestaurantByUserId(user.Id);
                if (ModelState.IsValid)
                {
                    RestaurantTable table = new RestaurantTable()
                    {
                        RestaurantTableSeats = tableViewModel.TableSeats
                    };
                    //_context.RestaurantTables.Add(table);
                    _applicationService.AddTableByRestaurantId(table, restaurant.Id);
                    if (restaurant == null)
                        user.RestaurantData = new Restaurant();
                    if(restaurant.RestaurantTables == null)
                        user.RestaurantData.RestaurantTables = new List<RestaurantTable>();
                    restaurant.RestaurantTables.Add(table);
                    //_context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            
            return View(tableViewModel);
        }

        /*[Authorize]
        public IActionResult ViewTables()
        {
            IEnumerable<Table> tables = GetTablesEnumeration();
            ViewTablesViewModel viewTablesViewModel = new ViewTablesViewModel()
            {
                Tables = tables
            };
            return View(viewTablesViewModel);
        }*/
    }


}