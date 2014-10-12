﻿// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments

using System.Globalization;
using Ninject.Modules;
using BAF.Common.Specifications;

namespace BAF.Authentication.Generated
{
	public class Validation : AuthenticationSubResource, ISubResource
	{
		public Validation(IAuthenticationResource resource) : base(resource)
		{
		}
		public string InvalidUsernameOrPassword 
		{ 
			get { return Resource.GetResource("Authentication.Validation.InvalidUsernameOrPassword"); }
		}
		public string SystemUserDeleteNotAllowed 
		{ 
			get { return Resource.GetResource("Authentication.Validation.SystemUserDeleteNotAllowed"); }
		}
		public string InvalidRegexPattern 
		{ 
			get { return Resource.GetResource("Authentication.Validation.InvalidRegexPattern"); }
		}
		public string InvalidUserName 
		{ 
			get { return Resource.GetResource("Authentication.Validation.InvalidUserName"); }
		}
		public string DuplicatedUserName 
		{ 
			get { return Resource.GetResource("Authentication.Validation.DuplicatedUserName"); }
		}
		public string PasswordTooShort 
		{ 
			get { return Resource.GetResource("Authentication.Validation.PasswordTooShort"); }
		}
		public override string ToString()
		{
			return Resource.GetResource("Authentication.Validation"); 
		}
	}
}

namespace BAF.Authentication
{
	public interface IAuthenticationResource : IResource
	{
		Generated.Validation Validation { get; }
	
	}
	public abstract class AuthenticationSubResource
    {
        protected readonly IAuthenticationResource Resource;

        protected AuthenticationSubResource(IAuthenticationResource localizationResource)
        {
            Resource = localizationResource;
        }
    }
	public partial class AuthenticationResource : IAuthenticationResource
	{
		private readonly IResourceCache _localizationCache;

		public AuthenticationResource(IResourceCache locCache)  
		{
			_localizationCache = locCache;
			Validation = new Generated.Validation(this);
		}
		public string GetResource(string path)
        {
            return _localizationCache.GetResource(CultureInfo.CurrentUICulture.Name, path);
        }
		public string GetResource(ISubResource subResource)
        {
            return subResource.ToString();
        }
		public Generated.Validation Validation { get; private set; }
	}

	public class AuthenticationResourceRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind<IAuthenticationResource>().To<AuthenticationResource>().InSingletonScope();
        }
    }
}
