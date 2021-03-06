﻿using Microsoft.AspNetCore.Mvc;

namespace PlayCat.Web.Controllers
{
    public class BaseController : Controller
    {
        private const string AccessTokenKey = "AccessToken";

        public string AccessToken => HttpContext.Request.Headers[AccessTokenKey];
    }
}
