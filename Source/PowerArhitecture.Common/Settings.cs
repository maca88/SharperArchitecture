﻿
// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments

using System;
using Ninject.Modules;
using PowerArhitecture.Common.Configuration;


namespace PowerArhitecture.Common.Settings
{
	public static class CommonSettingKeys
	{
		public const string NinjectXmlConfig = "PowerArhitecture.Common.Ninject.XmlConfig:string";
		public const string DefaultCulture = "PowerArhitecture.Common.DefaultCulture:string";
		public const string TranslationsByCulturePattern = "PowerArhitecture.Common.TranslationsByCulturePattern:string";
		public const string DefaultTranslationsPath = "PowerArhitecture.Common.DefaultTranslationsPath:string";
	}
}


namespace PowerArhitecture.Common.Settings
{
	public class NinjectSettings
	{
		public NinjectSettings()
		{
		}
		public virtual string XmlConfig 
		{ 
			get { return AppConfiguration.GetSetting<string>("PowerArhitecture.Common.Ninject.XmlConfig:string"); }
		}
	}
}

namespace PowerArhitecture.Common
{
	public partial class CommonSettings : Specifications.ICommonSettings
	{
		public CommonSettings()
		{
			Ninject = new Settings.NinjectSettings(); 
		}
		public virtual string DefaultCulture 
		{ 
			get { return AppConfiguration.GetSetting<string>("PowerArhitecture.Common.DefaultCulture:string"); } 
		}
		public virtual string TranslationsByCulturePattern 
		{ 
			get { return AppConfiguration.GetSetting<string>("PowerArhitecture.Common.TranslationsByCulturePattern:string"); } 
		}
		public virtual string DefaultTranslationsPath 
		{ 
			get { return AppConfiguration.GetSetting<string>("PowerArhitecture.Common.DefaultTranslationsPath:string"); } 
		}
		public virtual Settings.NinjectSettings Ninject { get; private set; } 
	}

	public class CommonSettingsRegistration : NinjectModule
	{
		public override void Load()
		{
			Bind<Specifications.ICommonSettings>().To<CommonSettings>().InSingletonScope();
		}
	}
}

namespace PowerArhitecture.Common.Specifications
{
	public interface ICommonSettings
	{
		string DefaultCulture { get; }
		string TranslationsByCulturePattern { get; }
		string DefaultTranslationsPath { get; }
		Settings.NinjectSettings Ninject { get; }
	}
}

