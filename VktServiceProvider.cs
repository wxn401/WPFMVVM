using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfCore
{
    public class VktServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable
    {

        private IContainer _container;

        private bool _disposed = false;

        public string name;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="container">IContainer</param>
        public VktServiceProvider(IContainer container)
        {
            this._container = container;
        }

        /// <summary>
        /// 获取服务的实现
        /// </summary>
        /// <param name="serviceType">serviceType</param>
        /// <returns>service object</returns>
        public object GetRequiredService(Type serviceType)
        {
            return this._container.Resolve(serviceType);
        }
        /// <summary>
        /// 获取服务的实现
        /// </summary>
        /// <param name="serviceType">serviceType</param>
        /// <returns>service object</returns>
        public object GetService(Type serviceType)
        {
            return this._container.ResolveOptional(serviceType);
        }
        /// <summary>
        /// 是否已经注册
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>true or false</returns>
        public bool IsRegistered(Type type)
        {
            return this._container.IsRegistered(type);
        }

        //public ILifetimeScope LifetimeScope => _lifetimeScope;

        /// <summary>
        /// 释放标识
        /// </summary>
        /// <param name="disposing">是否强制</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._container.Dispose();
                }

                this._disposed = true;
            }
        }
        /// <summary>
        /// 释放资源 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
