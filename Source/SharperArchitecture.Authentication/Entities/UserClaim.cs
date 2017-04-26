using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.Common.Specifications;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Authentication.Entities
{
    [Serializable]
    public partial class UserClaim : Entity
    {
        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }

        #region User

        [ReadOnly(true)]
        public virtual long? UserId
        {
            get
            {
                if (_userIdSet) return _userId;
                return User == null ? default(long?) : User.Id;
            }
            set
            {
                _userIdSet = true;
                _userId = value;
            }
        }

        private long? _userId;

        private bool _userIdSet;

        public virtual IUser User { get; set; }

        #endregion
    }
}
