using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Telerik.OpenAccess;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security.Model;

namespace timw255.Sitefinity.TwoFactorAuthentication.Security
{
    [Persistent]
    public class AuthyProfile : UserProfile
    {
        [UserFriendlyDataType(UserFriendlyDataType.ShortText)]
        public string AuthyId { get; set; }

        public AuthyProfile()
        {
        }
    }
}
