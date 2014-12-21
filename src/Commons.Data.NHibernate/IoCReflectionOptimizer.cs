using System;
using Castle.Windsor;
using NHibernate.Bytecode.Lightweight;
using NHibernate.Properties;

/// Original source: http://code.google.com/p/unhaddins/
/// See the follwing blog explaining its usage: http://fabiomaulo.blogspot.com/2008/11/entities-behavior-injection.html
/// Modifications: 
/// - Changed fully qualified from uNhAddIns.CastleAdapters.EnhancedBytecodeProvider.ReflectionOptimizer
/// - Reformatted code and renamed member vairables to house standards
/// 
namespace Eca.Commons.Data.NHibernate
{
    public class IoCReflectionOptimizer : ReflectionOptimizer
    {
        #region Member Variables

        private readonly IWindsorContainer _container;

        #endregion


        #region Constructors

        public IoCReflectionOptimizer(IWindsorContainer container, Type mappedType, IGetter[] getters, ISetter[] setters)
            : base(mappedType, getters, setters)
        {
            _container = container;
        }

        #endregion


        public override object CreateInstance()
        {
            return _container.Kernel.HasComponent(mappedType) ? _container.Resolve(mappedType) : base.CreateInstance();
        }


        protected override void ThrowExceptionForNoDefaultCtor(Type type) {}
    }
}