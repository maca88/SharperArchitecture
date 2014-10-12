namespace PowerArhitecture.Common.Reporting
{
    public interface IReportSettingsProvider
    {
        IReportSettings GetSettings(string reportType);
    }
}