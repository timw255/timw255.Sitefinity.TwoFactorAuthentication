using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace timw255.Sitefinity.TwoFactorAuthentication.MVC.Models
{
    public class LoginModel
    {
        public List<SelectListItem> ProvidersList { get; set; }
    }
}
