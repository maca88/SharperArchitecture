namespace SharperArchitecture.Domain.Attributes.Mssql
{
    public class MssqlIndexAttribute : IndexAttribute
    {
        public MssqlIndexAttribute(string keyName) : base(keyName)
        {
        }

        public MssqlIndexAttribute()
        {
        }

        public string[] Include { get; set; }
    }
}
