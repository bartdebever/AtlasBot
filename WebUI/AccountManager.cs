using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using DataLibary.Models;
using DataLibary.MSSQLContext;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using WebUI.Models;

namespace WebUI
{
    public class AccountManager
    {
        public SignInStatus Login(LoginViewModel model)
        {
            if (new UserRepo(new UserContext()).Login(model.Username, new Encryptor().Hash(model.Password)))
            {
                return SignInStatus.Success;
            }
            return SignInStatus.Failure;
        }
    }
}