using System;
using System.Linq.Expressions;

namespace PowerArhitecture.Domain
{
    public interface ICompositeKey
    {
        bool IsTransient();
    }

    public class CompositeKey<TType, TCol1, TCol2, TCol3, TCol4> : CompositeKey<TType, TCol1, TCol2, TCol3>
    {
        public CompositeKey(TType entity, Expression<Func<TType, TCol1>> column1Expression, Expression<Func<TType, TCol2>> column2Expression,
            Expression<Func<TType, TCol3>> column3Expression, Expression<Func<TType, TCol4>> column4Expression)
            : base(entity, column1Expression, column2Expression, column3Expression)
        {
            Column4 = column4Expression.Compile();
        }

        public Func<TType, TCol4> Column4 { get; }

        public override bool IsTransient()
        {
            return base.IsTransient() || Equals(Column4(Entity), default(TCol4));
        }

        public static bool operator ==(CompositeKey<TType, TCol1, TCol2, TCol3, TCol4> c1, CompositeKey<TType, TCol1, TCol2, TCol3, TCol4> c2)
        {
            return !(c1 == null) && c1.Equals(c2);
        }

        public static bool operator !=(CompositeKey<TType, TCol1, TCol2, TCol3, TCol4> c1, CompositeKey<TType, TCol1, TCol2, TCol3, TCol4> c2)
        {
            if (c1 == null) return true;
            return !c1.Equals(c2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var item = (CompositeKey<TType, TCol1, TCol2, TCol3, TCol4>)obj;
            return
                Equals(item.Column1(item.Entity), Column1(Entity)) &&
                Equals(item.Column2(item.Entity), Column2(Entity)) &&
                Equals(item.Column3(item.Entity), Column3(Entity)) &&
                Equals(item.Column4(item.Entity), Column4(Entity));
        }

        public override int GetHashCode()
        {
            if (IsTransient()) return base.GetHashCode();
            unchecked
            {
                var hashCode = GetType().GetHashCode() * 31;
                return
                    (hashCode * 31) ^
                    Column1(Entity).GetHashCode() ^
                    Column2(Entity).GetHashCode() ^
                    Column3(Entity).GetHashCode() ^
                    Column4(Entity).GetHashCode();
            }
        }
    }

    public class CompositeKey<TType, TCol1, TCol2, TCol3> : CompositeKey<TType, TCol1, TCol2>
    {
        public CompositeKey(TType entity, Expression<Func<TType, TCol1>> column1Expression, Expression<Func<TType, TCol2>> column2Expression,
            Expression<Func<TType, TCol3>> column3Expression) 
            : base(entity, column1Expression, column2Expression)
        {
            Column3 = column3Expression.Compile();
        }

        public Func<TType, TCol3> Column3 { get; }

        public override bool IsTransient()
        {
            return base.IsTransient() || Equals(Column3(Entity), default(TCol3));
        }

        public static bool operator ==(CompositeKey<TType, TCol1, TCol2, TCol3> c1, CompositeKey<TType, TCol1, TCol2, TCol3> c2)
        {
            return !(c1 == null) && c1.Equals(c2);
        }

        public static bool operator !=(CompositeKey<TType, TCol1, TCol2, TCol3> c1, CompositeKey<TType, TCol1, TCol2, TCol3> c2)
        {
            if (c1 == null) return true;
            return !c1.Equals(c2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var item = (CompositeKey<TType, TCol1, TCol2, TCol3>)obj;
            return
                Equals(item.Column1(item.Entity), Column1(Entity)) &&
                Equals(item.Column2(item.Entity), Column2(Entity)) &&
                Equals(item.Column3(item.Entity), Column3(Entity));
        }

        public override int GetHashCode()
        {
            if (IsTransient()) return base.GetHashCode();
            unchecked
            {
                var hashCode = GetType().GetHashCode() * 31;
                return 
                    (hashCode * 31) ^ 
                    Column1(Entity).GetHashCode() ^ 
                    Column2(Entity).GetHashCode() ^ 
                    Column3(Entity).GetHashCode();
            }
        }
    }

    public class CompositeKey<TType, TCol1, TCol2> : ICompositeKey
    {
        public CompositeKey(TType entity, Expression<Func<TType, TCol1>> column1Expression, Expression<Func<TType, TCol2>> column2Expression)
        {
            Entity = entity;
            Column1 = column1Expression.Compile();
            Column2 = column2Expression.Compile();
        }

        public TType Entity { get; }

        public Func<TType, TCol1> Column1 { get; }

        public Func<TType, TCol2> Column2 { get; }

        public virtual bool IsTransient()
        {
            return Equals(Column1(Entity), default(TCol1)) || Equals(Column2(Entity), default(TCol2));
        }

        public static bool operator ==(CompositeKey<TType, TCol1, TCol2> c1, CompositeKey<TType, TCol1, TCol2> c2)
        {
            return !(c1 == null) && c1.Equals(c2);
        }

        public static bool operator !=(CompositeKey<TType, TCol1, TCol2> c1, CompositeKey<TType, TCol1, TCol2> c2)
        {
            if (c1 == null) return true;
            return !c1.Equals(c2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var item = (CompositeKey<TType, TCol1, TCol2>)obj;
            return 
                Equals(item.Column1(item.Entity), Column1(Entity)) && 
                Equals(item.Column2(item.Entity), Column2(Entity));
        }

        public override int GetHashCode()
        {
            if (IsTransient()) return base.GetHashCode();
            unchecked
            {
                var hashCode = GetType().GetHashCode() * 31;
                return 
                    (hashCode * 31) ^ 
                    Column1(Entity).GetHashCode() ^ 
                    Column2(Entity).GetHashCode();
            }
        }
    }
}
