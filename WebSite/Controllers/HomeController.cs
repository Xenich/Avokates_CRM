﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avokates_CRM.Models.Outputs;
using Microsoft.AspNetCore.Mvc;

using Avokates_CRM.Models;
using Avokates_CRM.Models.ApiModels;
using WebSite.Models.Outputs;
using WebSite.DataLayer;
using WebSite.Helpers;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataLayer dl;      // dataLayer
        public HomeController(IDataLayer dataLayer)      // в Startup : AddScoped<IDataLayer, DataLayer>();
        {
            dl = dataLayer;
        }

        public IActionResult Index()
        {
            string token = GetToken();
            if (HelperSecurity.IsTokenValid(token))
            {
                GetMainPage_Out result = dl.GetMainPage(token);
                return View(result);
            }
            else
                return View("ErrorView");
        }

        public IActionResult Cabinet()
        {
            string token = GetToken();
            if (HelperSecurity.IsTokenValid(token))
            {             
                return View();
            }
            else
                return View("ErrorView");
        }

        // получение определенного дела
        public IActionResult Case(int id)
        {
            string token = GetToken();
            if (HelperSecurity.IsTokenValid(token))
            {
                ViewData["id"] = id.ToString();
                return View();
            }
            else
                return View("ErrorView");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetToken()
        {
            string b = Request.Headers["authorization"].ToString();
            return b.Substring(7, b.Length - 7);    // первые 7 символов - это слово "Bearer "
        }
    }
}