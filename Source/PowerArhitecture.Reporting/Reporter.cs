using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PowerArhitecture.Common.Reporting;
using PowerArhitecture.Reporting.Specifications;
using PowerArhitecture.Common.Specifications;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace PowerArhitecture.Reporting
{
    public class Reporter : IReporter
    {
        //Dictionary<reportName, Dictionary<reportType, Dictionary<reportLanguage, Dictionary<ReportInfo, IReport>>>>
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, KeyValuePair<ReportInfo, IReport>>>> _reports;
        private readonly ILogger _logger;
        private readonly Container _resolutionRoot;
        private readonly IReportSettingsProvider _settingsProvider;

        public Reporter(ILogger logger, Container resolutionRoot, IReportSettingsProvider settingsProvider)
        {
            _logger = logger;
            _resolutionRoot = resolutionRoot;
            _settingsProvider = settingsProvider;
            _reports = new Dictionary<string, Dictionary<string, Dictionary<string, KeyValuePair<ReportInfo, IReport>>>>();
            Initialize();
        }

        #region Hmm

        private void Initialize()
        {/*
            var manager = new VCNEntities();

            var reportsDb = manager.Report
                .Include(Report.PathFor(o => o.ReportReportType) + "." +
                         ReportReportType.PathFor(o => o.ReportType))
                .Include(Report.PathFor(o => o.ReportReportType) + "." +
                         ReportReportType.PathFor(o => o.ReportReportTypeLanguage))
                .ToList();*/
            try
            {
                    /*
                    AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(o => o.FullName.StartsWith("PCS"))
                    .SelectMany(o => o.GetTypes().Where(t => typeof (IReport).IsAssignableFrom(t)))*/
                /*
                foreach (var type in reportsApp)
                {
                    var report = ServiceLocator.Current.GetInstance(type) as IReport;
                    if (report == null)
                        throw new NullReferenceException("report");

                    //Db validation
                    var reportDb = reportsDb.FirstOrDefault(o => o.Name == report.Name);
                    if (reportDb == null)
                    {
                        _logger.Warn("Report '{0}' do not exist in db!", report.Name);
                        continue;
                    }
                    var reportTypeDb = reportDb.ReportReportType.FirstOrDefault(o => o.ReportType.Type == report.Type);
                    if (reportTypeDb == null)
                    {
                        _logger.Warn("Report '{0}' do not have any ReportReportType with ReportType '{1}' in db!", report.Name, report.Type);
                        continue;
                    }
                    if (!reportTypeDb.ReportReportTypeLanguage.Any())
                    {
                        _logger.Warn("Report '{0}' do not have any ReportReportTypeLanguage in db!", report.Name);
                        continue;
                    }

                    //Fill dictionary
                    if (!_reports.ContainsKey(report.Name))
                        _reports.Add(report.Name, new Dictionary<string, Dictionary<string, KeyValuePair<ReportInfo, IReport>>>());
                    if (!_reports[report.Name].ContainsKey(report.Type))
                        _reports[report.Name].Add(report.Type, new Dictionary<string, KeyValuePair<ReportInfo, IReport>>());
                    foreach (var reportLangDb in reportTypeDb.ReportReportTypeLanguage)
                    {
                        if (!_reports[report.Name][report.Type].ContainsKey(reportLangDb.LanguageCode))
                            _reports[report.Name][report.Type].Add(reportLangDb.LanguageCode, new KeyValuePair<ReportInfo, IReport>());
                        var parameters = new ReportInfo
                        {
                            LanguageCode = reportLangDb.LanguageCode,
                            TemplatePath = reportLangDb.FilePath
                        };
                        _reports[report.Name][report.Type][reportLangDb.LanguageCode] = new KeyValuePair<ReportInfo, IReport>(parameters, report);
                    }
                }*/
            }
            catch (Exception e)
            {
                _logger.Fatal(e.ToString());
                throw;
            }
        }

        public string CreateReportAsUrl<T>(string reportType, string language, T parameter, string reportName = null)
        {
            return null;
            /*
            try
            {
                if (!_reports.ContainsKey(reportName))
                    throw new KeyNotFoundException(string.Format("Report '{0}' does not exist in db or in code or both", reportName));
                if (!_reports[reportName].ContainsKey(reportType))
                    throw new KeyNotFoundException(string.Format("Report '{0}' do not have any report with type '{1}'", reportName, reportType));
                if (!_reports[reportName][reportType].ContainsKey(language))
                    throw new KeyNotFoundException(string.Format("Report '{0}' do not have any report with type '{1}' and language '{2}'",
                        reportName, reportType, language));
                var report = _reports[reportName][reportType][language].Value;
                if (print)
                    report = report.PrintReport ?? report;
                var reportInfo = _reports[reportName][reportType][language].Key;
                var appPath = HttpContext.Current.Request.PhysicalApplicationPath;
                var reportFolder = "Reports"; //TODO: from config
                var guid = Guid.NewGuid().ToString();
                var finalReportFolderPath = String.Format(@"{0}{1}", appPath, reportFolder);
                if (!Directory.Exists(finalReportFolderPath))
                    Directory.CreateDirectory(finalReportFolderPath);
                var finalReportPath = String.Format(@"{0}\{1}.{2}", finalReportFolderPath, guid, report.Extension);
                report.Fill(user, finalReportPath, reportInfo, parameter);
                return string.Format("{0}/{1}.{2}", reportFolder, guid, report.Extension);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }*/
        }

        #endregion

        public ReportResult CreateReport<TItem>(ReportParameters<TItem> parameters) where TItem : class
        {
            var report = _resolutionRoot.GetInstance<IReport>(parameters.ReportType);
            parameters.Settings = parameters.Settings ?? _settingsProvider.GetSettings(parameters.ReportType);
            var reportData = report.Create(parameters);
            return new ReportResult
            {
                Data = reportData.ToByteArray(),
                Extension = report.Extension,
                MimeType = report.MimeType
            };
        }

        public ReportResult Print<TItem>(PrintParameters<TItem> parameters) where TItem : class
        {
            var printReport = _resolutionRoot.GetInstance<IPrintReport>();
            parameters.Settings = parameters.Settings ?? _settingsProvider.GetSettings(ReportType.Pdf);
            var printData = printReport.Print(parameters);
            return new ReportResult
            {
                Data = printData.ToByteArray(),
                Extension = printReport.Extension,
                MimeType = printReport.MimeType
            };
        }
    }
}
