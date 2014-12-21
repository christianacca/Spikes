using System;
using Castle.Windsor;
using NHibernate.Bytecode;
using NHibernate.ByteCode.Castle;
using NHibernate.Properties;
using NHibernate.Type;

/// Original source: http://code.google.com/p/unhaddins/
/// See the follwing blog explaining its usage: http://fabiomaulo.blogspot.com/2008/11/entities-behavior-injection.html
/// Modifications: 
/// - Changed fully qualified from uNhAddIns.CastleAdapters.EnhancedBytecodeProvider.EnhancedBytecode
/// - Reformatted code and renamed member vairables to house standards
/// 
namespace Eca.Commons.Data.NHibernate
{
    public class BytecodeProvider : IBytecodeProvider,
                                    IInjectableCollectionTypeFactoryClass,
                                    IInjectableProxyFactoryFactory
    {
        #region Member Variables

        private ICollectionTypeFactory _collectionTypeFactory;
        private Type _colletionTypeFactoryType = typeof (DefaultCollectionTypeFactory);
        private readonly IWindsorContainer _container;
        private readonly IObjectsFactory _objectsFactory;

        private IProxyFactoryFactory _proxyFactoryFactory;
        private Type _proxyFactoryFactoryType = typeof (ProxyFactoryFactory);

        #endregion


        #region Constructors

        public BytecodeProvider(IWindsorContainer container)
        {
            _container = container;
            _objectsFactory = new ObjectsFactory(container);
        }

        #endregion


        #region IBytecodeProvider Members

        public ICollectionTypeFactory CollectionTypeFactory
        {
            get
            {
                if (_collectionTypeFactory == null)
                {
                    if (_container.Kernel.HasComponent(_colletionTypeFactoryType))
                        _collectionTypeFactory = (ICollectionTypeFactory) _container.Resolve(_colletionTypeFactoryType);
                    else
                        _collectionTypeFactory =
                            (ICollectionTypeFactory) Activator.CreateInstance(_colletionTypeFactoryType);
                }
                return _collectionTypeFactory;
            }
        }


        public IReflectionOptimizer GetReflectionOptimizer(Type clazz, IGetter[] getters, ISetter[] setters)
        {
            return new IoCReflectionOptimizer(_container, clazz, getters, setters);
        }


        public IObjectsFactory ObjectsFactory
        {
            get { return _objectsFactory; }
        }

        public IProxyFactoryFactory ProxyFactoryFactory
        {
            get
            {
                if (_proxyFactoryFactory == null)
                {
                    if (_container.Kernel.HasComponent(_proxyFactoryFactoryType))
                        _proxyFactoryFactory = (IProxyFactoryFactory) _container.Resolve(_proxyFactoryFactoryType);
                    else
                        _proxyFactoryFactory = (IProxyFactoryFactory) Activator.CreateInstance(_proxyFactoryFactoryType);
                }
                return _proxyFactoryFactory;
            }
        }

        #endregion


        #region IInjectableCollectionTypeFactoryClass Members

        public void SetCollectionTypeFactoryClass(string typeAssemblyQualifiedName)
        {
            SetCollectionTypeFactoryClass(Type.GetType(typeAssemblyQualifiedName, true));
        }


        public void SetCollectionTypeFactoryClass(Type type)
        {
            _colletionTypeFactoryType = type;
        }

        #endregion


        #region IInjectableProxyFactoryFactory Members

        public void SetProxyFactoryFactory(string typeName)
        {
            _proxyFactoryFactoryType = Type.GetType(typeName, true);
        }

        #endregion
    }
}