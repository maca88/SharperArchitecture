using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Enums;
using PowerArhitecture.Common.Specifications;
using Breeze.ContextProvider.NH.Configuration;

namespace PowerArhitecture.Breeze.Extensions
{
    public static class MemberConfigurationExtensions
    {
        #region SerializeWhenUserHasPermission

        public static IMemberConfiguration<TModel, TType> SerializeWhenUserHasPermission<TModel, TType>(
            this IMemberConfiguration<TModel, TType> configuration, string pattern, PatternOptions opt = PatternOptions.None)
        {
            var memberConfig = (IMemberConfiguration)configuration;
            var userCache = memberConfig.GetUserCache();
            memberConfig.ShouldSerializePredicate = o => userCache.GetCurrentUser().HasPermission(pattern, opt);
            return configuration;
        }

        //public static ICustomMemberConfiguration<TModel, TType> SerializeWhenUserHasPermission<TModel, TType>(
        //    this ICustomMemberConfiguration<TModel, TType> configuration, string pattern, PatternOptions opt = PatternOptions.None)
        //{
        //    var memberConfig = (IMemberConfiguration)configuration;
        //    memberConfig.SerializeWhenUserHasPermission(pattern, opt);
        //    return configuration;
        //}

        public static IMemberConfiguration SerializeWhenUserHasPermission(
            this IMemberConfiguration configuration, string pattern, PatternOptions opt = PatternOptions.None)
        {
            var userCache = configuration.GetUserCache();
            configuration.ShouldSerializePredicate = o => userCache.GetCurrentUser().HasPermission(pattern, opt);
            return configuration;
        }

        #endregion

        #region SerializeWhenIsSystemUser

        public static IMemberConfiguration<TModel, TType> SerializeWhenIsSystemUser<TModel, TType>(
            this IMemberConfiguration<TModel, TType> configuration)
        {
            var memberConfig = (IMemberConfiguration)configuration;
            memberConfig.SerializeWhenIsSystemUser();
            return configuration;
        }

        public static ICustomMemberConfiguration<TModel, TType> SerializeWhenIsSystemUser<TModel, TType>(
            this ICustomMemberConfiguration<TModel, TType> configuration)
        {
            var memberConfig = (IMemberConfiguration)configuration;
            memberConfig.SerializeWhenIsSystemUser();
            return configuration;
        }

        public static IMemberConfiguration SerializeWhenIsSystemUser(
            this IMemberConfiguration configuration)
        {
            var userCache = configuration.GetUserCache();
            configuration.ShouldSerializePredicate = o => userCache.GetCurrentUser().IsSystemUser;
            return configuration;
        }

        #endregion

        private static IUserCache GetUserCache(this IMemberConfiguration configuration)
        {
            return (IUserCache)configuration.Data["IUserCache"];
        }
    }
}
