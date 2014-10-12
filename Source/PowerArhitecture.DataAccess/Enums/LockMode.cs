namespace PowerArhitecture.DataAccess.Enums
{
    public enum LockMode
    {
        None,
        Read,
        Upgrade,
        UpgradeNoWait,
        Write,
        Force
    }
}
