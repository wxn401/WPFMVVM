using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfCore.ViewModel;

namespace WpfCore
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceTypesInAssembly = (new List<Type>( typeof(MainViewModel).Assembly.GetExportedTypes())).Where(type => !type.IsInterface && type.FullName.EndsWith("Service")).ToArray();
            builder.RegisterTypes(serviceTypesInAssembly).AsImplementedInterfaces();
        }
    }
}
