namespace SharperArchitecture.Breeze
{
    public interface IClientModel
    {
        long Id { get; set; }

        bool IsNew();
    }

    public abstract class ClientModel : IClientModel
    {
        private const int HashMultiplier = 31;
        private int? _cachedHashcode;

        public long Id { get; set; }

        public bool IsNew()
        {
            return Id <= 0;
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as IClientModel;
            if (ReferenceEquals(this, compareTo))
                return true;
            if (compareTo == null || !(GetType() == compareTo.GetType()))
                return false;
            return HasSameNonDefaultIdAs(compareTo);
        }

        public override int GetHashCode()
        {
            if (_cachedHashcode.HasValue)
                return _cachedHashcode.Value;
            if (IsNew())
                _cachedHashcode = base.GetHashCode();
            else
            {
                unchecked
                {
                    var hashCode = GetType().GetHashCode();
                    _cachedHashcode = (hashCode * HashMultiplier) ^ Id.GetHashCode();
                }
            }
            return _cachedHashcode.Value;
        }

        private bool HasSameNonDefaultIdAs(IClientModel compareTo)
        {
            return !IsNew() && !compareTo.IsNew() && Id.Equals(compareTo.Id);
        }
    }
}