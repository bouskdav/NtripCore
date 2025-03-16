//using Microsoft.Extensions.DependencyInjection;
//using NtripCore.Manager.Shared.Interfaces.Contracts.Api;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NtripCore.Manager.Shared.Extensions
//{
//    //public static class ServiceCollectionExtensions
//    //{
//        //public static IServiceCollection AddManagers(this IServiceCollection services)
//        //{
//            //var managers = typeof(IApiService);

//            //var pages = typeof(IRegisteredPage);

//            //var pageTypes = AppDomain.CurrentDomain.GetAssemblies()
//            //    .SelectMany(s => s.GetTypes())
//            //    .Where(p => pages.IsAssignableFrom(p) && !p.IsInterface);

//            //foreach (var type in pageTypes)
//            //{
//            //    services.AddTransient(type);
//            }

//            //var types = managers
//            //    .Assembly
//            //    .GetExportedTypes()
//            //    .Where(t => t.IsClass && !t.IsAbstract)
//            //    .Select(t => new
//            //    {
//            //        Service = t.GetInterface($"I{t.Name}"),
//            //        Implementation = t
//            //    })
//            //    .Where(t => t.Service != null);

//            //foreach (var type in types)
//            //{
//            //    if (managers.IsAssignableFrom(type.Service))
//            //    {
//            //        services.AddTransient(type.Service, type.Implementation);
//            //    }
//            //}

//            //return services;
//        //}
//    //}
//}
