using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAF.Common.Specifications;

namespace BAF.Common.AutoMapper
{
    public class TimeZoneInfoValueResolver : IValueResolver
    {
        private readonly IUserCache _userCache;

        public TimeZoneInfoValueResolver(IUserCache userCache)
        {
            _userCache = userCache;
        }

        public ResolutionResult Resolve(ResolutionResult source)
        {
            return source.New(_userCache.GetCurrentUser().TimeZone, typeof(TimeZoneInfo));
        }
    }
}
