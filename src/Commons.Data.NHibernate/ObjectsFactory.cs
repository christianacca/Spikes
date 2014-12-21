using System;
using Castle.Windsor;
using NHibernate.Bytecode;

/// Original source: http://code.google.com/p/unhaddins/
/// See the follwing blog explaining its usage: http://fabiomaulo.blogspot.com/2009/05/nhibernate-ioc-integration.html
/// Modifications: 
/// - Changed fully qualified from uNhAddIns.CastleAdapters.EnhancedBytecodeProvider.ObjectsFactory
/// - Reformatted code and renamed member vairables to house standards
/// 
namespace Eca.Commons.Data.NHibernate
{
    public class ObjectsFactory : IObjectsFactory
    {
        #region Member Variables

        private readonly IWindsorContainer _container;

        #endregion


        #region Constructors

        public ObjectsFactory(IWindsorContainer container)
        {
            _container = container;
        }

        #endregion


        #region IObjectsFactory Members

        public object CreateInstance(Type type)
        {
            return _container.Kernel.HasComponent(type) ? _container.Resolve(type) : Activator.CreateInstance(type);
        }


        public object CreateInstance(Type type, bool nonPublic)
        {
            return _container.Kernel.HasComponent(type)
                       ? _container.Resolve(type)
                       : Activator.CreateInstance(type, nonPublic);
        }


        public object CreateInstance(Type type, params object[] ctorArgs)
        {
            return Activator.CreateInstance(type, ctorArgs);
        }

        #endregion
    }
}