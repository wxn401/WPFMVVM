using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfCore
{
    public class VktServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly Action<ContainerBuilder> _configurationAction;
        private readonly ContainerBuildOptions _containerBuildOptions = ContainerBuildOptions.None;

        private VktServiceProvider _serviceProider;
        /// <summary>
        /// 提供服务类
        /// </summary>
        public VktServiceProvider ServiceProvider
        {
            get
            {
                return _serviceProider;
            }
        }

        /// <summary>
        /// 初始化一个 AutofacServiceProviderFactory 类型的工厂
        /// </summary>
        /// <param name="containerBuildOptions">容器的构建参数</param>
        /// <param name="configurationAction">回调</param>
        public VktServiceProviderFactory(
            ContainerBuildOptions containerBuildOptions,
            Action<ContainerBuilder> configurationAction = null)
            : this(configurationAction) =>
            _containerBuildOptions = containerBuildOptions;

        /// <summary>
        /// 初始化一个 AutofacServiceProviderFactory 类型的工厂
        /// </summary>
        /// <param name="configurationAction">回调</param>
        public VktServiceProviderFactory(Action<ContainerBuilder> configurationAction = null) =>
            _configurationAction = configurationAction ?? (builder => { });



        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<AutofacModule>();
            builder.Populate(services);

            _configurationAction(builder);

            return builder;
        }
        /// <summary>
        /// 使用容器创建
        /// </summary>
        /// <param name="containerBuilder">容器构建</param>
        /// <returns>IServiceProvider</returns>
        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));

            var container = containerBuilder.Build(_containerBuildOptions);
            _serviceProider = new VktServiceProvider(container);
            return _serviceProider;
        }
    }
}
