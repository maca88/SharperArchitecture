using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace NHibernate
{
    public static class LockModeExtensions
    {
        public static LockMode GetNhLockMode(this PowerArhitecture.DataAccess.Enums.LockMode lockMode)
        {
            switch (lockMode)
            {
                case PowerArhitecture.DataAccess.Enums.LockMode.None:
                    return LockMode.None;
                case PowerArhitecture.DataAccess.Enums.LockMode.Read:
                    return LockMode.Read;
                case PowerArhitecture.DataAccess.Enums.LockMode.Upgrade:
                    return LockMode.Upgrade;
                case PowerArhitecture.DataAccess.Enums.LockMode.UpgradeNoWait:
                    return LockMode.UpgradeNoWait;
                case PowerArhitecture.DataAccess.Enums.LockMode.Write:
                    return LockMode.Write;
                case PowerArhitecture.DataAccess.Enums.LockMode.Force:
                    return LockMode.Force;
                default:
                    throw new NotSupportedException(lockMode.ToString());
            }
        }
    }
}
