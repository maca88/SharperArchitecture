namespace SharperArchitecture.Common.Reporting
{
    public interface IReportSettingsProvider
    {
        IReportSettings GetSettings(string reportType);
    }
}