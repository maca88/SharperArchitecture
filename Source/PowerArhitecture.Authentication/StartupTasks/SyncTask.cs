using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Schema;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataAccess.Specifications;

namespace PowerArhitecture.Authentication.StartupTasks
{
    //[Task(PositionInSequence = 90)] //Has to be executed before caching task
    //public class SyncTask : IStartupTask
    //{
    //    private readonly IResolutionRoot _resolutionRoot;

    //    public SyncTask(IResolutionRoot resolutionRoot)
    //    {
    //        _resolutionRoot = resolutionRoot;
    //    }

    //    public void Run()
    //    {
    //        return;
    //        /*
    //        if(!_resolutionRoot.Get<ILocalizationSettings>().SyncResource) return;
    //        var xsdStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PowerArhitecture.Authentication.Rights.xsd");
    //        if(xsdStream == null)
    //            throw new NullReferenceException("xsdStream");
    //        var xsd = XmlSchema.Read(xsdStream, null);
    //        var schemas = new XmlSchemaSet();
    //        schemas.Add(xsd);

    //        var modules = typeof (IResource)
    //            .Assembly.GetDependentAssemblies()
    //            .SelectMany(a => a.GetManifestResourceNames()
    //                              .Where(r => r.EndsWith(".xml"))
    //                              .Select(r =>
    //                                  {
    //                                      try
    //                                      {
    //                                          var xdoc = XDocument.Load(a.GetManifestResourceStream(r));
    //                                          xdoc.Validate(schemas, null);
    //                                          var module = Module.Deserialize(xdoc.ToString());
    //                                          module.Resources.ForEach(o => o.ModuleName = module.Name);
    //                                          return module;
    //                                      }
    //                                      catch
    //                                      {
    //                                          return null;
    //                                      }
    //                                  })
    //                              .Where(o => o != null))
    //            .ToList();

    //        using (var unitOfWork = _resolutionRoot.Get<IUnitOfWork>())
    //        {
    //            var resourceRepo = unitOfWork.GetRepository<Resource>();
    //            var dbResources = resourceRepo.GetEntitiesQuery().ToList().ToDictionary(o => o.FullName);

    //            var duplicates = modules.SelectMany(o => o.Resources.Select(r => r.FullName)).Duplicates(o => o).ToList();
    //            if(duplicates.Any())
    //                throw new DuplicateNameException(string.Join(", ", duplicates));

    //            var xmlResources = modules.SelectMany(o => o.Resources).Distinct().ToDictionary(o => o.FullName);

    //            //If resouce is contained only in db, delete it
    //            foreach (var fullName in dbResources.Keys.Except(xmlResources.Keys))
    //            {
    //                resourceRepo.Delete(dbResources[fullName]);
    //            }

    //            //If resouce is contained only in xml, add it to db
    //            foreach (var xmlResource in xmlResources.Keys.Except(dbResources.Keys).Select(fullName => xmlResources[fullName]))
    //            {
    //                resourceRepo.Save(new Resource
    //                    {
    //                        Module = xmlResource.ModuleName,
    //                        Name = xmlResource.Name,
    //                        Namespace = xmlResource.Ns
    //                    });
    //            }
    //        }*/
    //    }

    //    public void Reset()
    //    {
    //    }
    //}
}
