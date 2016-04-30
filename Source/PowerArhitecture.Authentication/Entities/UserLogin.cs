using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Authentication.Entities
{
    [Serializable]
    public partial class UserLogin : Entity
    {
        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }

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
