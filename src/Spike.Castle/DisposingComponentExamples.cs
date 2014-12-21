using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Eca.Commons;
using NUnit.Framework;

namespace Spike.Castle
{
    [TestFixture]
    public class DisposingComponentExamples
    {
        private IWindsorContainer _container;


        #region Setup/Teardown

        [SetUp]
        public void TestInitialise()
        {
            _container = new WindsorContainer();
            _container.Register(Component.For<TransientService>().LifeStyle.Transient,
                                Component.For<SingletonService>().LifeStyle.Singleton,
                                Component.For<NonDisposableComponent>().LifeStyle.Transient,
                                Component.For<DisposableComponent>().LifeStyle.Transient,
                                Component.For<AnotherDisposableComponent>().LifeStyle.Transient);
        }


        [TearDown]
        public void TestCleanup()
        {
            _container.Dispose();
        }

        #endregion


        [Test]
        public void TrasientService_DependsOnDisposable_ThatDependsOnDisposable_ServiceIsReleased()
        {
            //given
            var service = _container.Resolve<TransientService>();

            //when
            _container.Release(service);

            //then
            Assert.That(service.DisposableComponent.AnotherDisposableComponent.IsDisposedCalled, Is.True);
        }


        [Test]
        public void TrasientService_DependendsOnNonDisposable_ThatDependsOnDisposable_ServiceIsReleased()
        {
            //given
            var service = _container.Resolve<TransientService>();

            //when
            _container.Release(service);

            //then
            Assert.That(service.NonDisposableComponent.DisposableComponent.IsDisposedCalled, Is.True);
        }


        [Test]
        public void SingletonService_DependsOnDisposable_ThatDependsOnDisposable_ServiceIsReleased()
        {
            //given
            var service = _container.Resolve<SingletonService>();

            //when
            _container.Release(service);

            //then
            Assert.That(service.DisposableComponent.AnotherDisposableComponent.IsDisposedCalled, Is.False);
        }

        
        [Test]
        public void SingletonService_DependsOnDisposable_ThatDependsOnDisposable_ContainerIsDisposed()
        {
            //given
            var service = _container.Resolve<SingletonService>();

            //when
            _container.Dispose();

            //then
            Assert.That(service.DisposableComponent.AnotherDisposableComponent.IsDisposedCalled, Is.True);
        }
    }



    [SkipFormatting]
    public class NonDisposableComponent
    {
        private readonly DisposableComponent _disposableComponent;


        public NonDisposableComponent(DisposableComponent disposableComponent)
        {
            _disposableComponent = disposableComponent;
        }


        public DisposableComponent DisposableComponent
        {
            get { return _disposableComponent; }
        }
    }



    [SkipFormatting]
    public class AnotherDisposableComponent : IDisposable
    {
        public bool IsDisposedCalled { get; set; }


        public void Dispose()
        {
            IsDisposedCalled = true;
        }
    }



    [SkipFormatting]
    public class DisposableComponent : IDisposable
    {
        public AnotherDisposableComponent AnotherDisposableComponent { get; private set; }


        public DisposableComponent(AnotherDisposableComponent anotherDisposableComponent)
        {
            AnotherDisposableComponent = anotherDisposableComponent;
        }


        public bool IsDisposedCalled { get; set; }


        public void Dispose()
        {
            IsDisposedCalled = true;
        }
    }



    [SkipFormatting]
    public class SingletonService
    {
        private readonly DisposableComponent _disposableComponent;
        private readonly NonDisposableComponent _nonDisposableComponent;

        public NonDisposableComponent NonDisposableComponent
        {
            get { return _nonDisposableComponent; }
        }


        public SingletonService(DisposableComponent disposableComponent, NonDisposableComponent nonDisposableComponent)
        {
            _disposableComponent = disposableComponent;
            _nonDisposableComponent = nonDisposableComponent;
        }


        public DisposableComponent DisposableComponent
        {
            get { return _disposableComponent; }
        }
    }



    [SkipFormatting]
    public class TransientService
    {
        private readonly DisposableComponent _disposableComponent;
        private readonly NonDisposableComponent _nonDisposableComponent;

        public NonDisposableComponent NonDisposableComponent
        {
            get { return _nonDisposableComponent; }
        }


        public TransientService(DisposableComponent disposableComponent, NonDisposableComponent nonDisposableComponent)
        {
            _disposableComponent = disposableComponent;
            _nonDisposableComponent = nonDisposableComponent;
        }


        public DisposableComponent DisposableComponent
        {
            get { return _disposableComponent; }
        }
    }
}