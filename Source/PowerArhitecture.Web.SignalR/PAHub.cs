using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Principal;
using PowerArhitecture.Common.Specifications;
using Microsoft.AspNet.SignalR;

namespace PowerArhitecture.Web.SignalR
{
    public class PAHub : Hub
    {
        private readonly IUserCache _userCache;

        public PAHub(IUserCache userCache)
        {
            _userCache = userCache;
        }

        protected IUser User
        {
            get
            {
                if(Context.User == null) 
                    throw new NullReferenceException("Context.User");
                return Context.User.Identity.IsAuthenticated 
                    ? _userCache.GetUser(Context.User.Identity.Name) 
                    : new GenericCustomPrincipal(Context.User);
            }
        }
    }
}
