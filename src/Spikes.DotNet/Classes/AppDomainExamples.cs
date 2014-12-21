using System;
using System.IO;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Policy;
using Eca.Spikes.WinFormsApplication;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class AppDomainExamples
    {
        [Test, Ignore("Not unit tests - require manually closing exe that is launched")]
        public void CanLaunchExeFromWithinSameProcessAsThis()
        {
            AppDomain domain = AppDomain.CreateDomain("Testing");
            domain.ExecuteAssembly("Eca.Spikes.WinFormsApplication.exe");
            AppDomain.Unload(domain);
        }


        [Test, Ignore("Not unit tests - require manually closing exe that is launched")]
        public void CanLaunchExeWithLimitSecurityPermissions()
        {
            //when the exe launched try pressing on the button that creates a file

            object[] hostEvidence = {new Zone(SecurityZone.Internet)};
            Evidence internetEvidence = new Evidence(hostEvidence, null);
            AppDomain domain = AppDomain.CreateDomain("Testing");
            domain.ExecuteAssembly("Eca.Spikes.WinFormsApplication.exe", internetEvidence);

            AppDomain.Unload(domain);
        }


        [Test, Ignore("Not unit tests - require manually closing exe that is launched")]
        public void CanCreateAnObjectFromAnExeWithinNewAppDomain()
        {
            AppDomainSetup propertiesForNewAppDomains = new AppDomainSetup();
            propertiesForNewAppDomains.ApplicationBase = Directory.GetCurrentDirectory();
            AppDomain domain = AppDomain.CreateDomain("Testing", null, propertiesForNewAppDomains);

            string assemblyFullName = "Eca.Spikes.WinFormsApplication, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null";
            ObjectHandle instance = domain.CreateInstance(assemblyFullName, "Eca.Spikes.WinFormsApplication.ReflectionFriendlyLauncher");
            ReflectionFriendlyLauncher launcher = (ReflectionFriendlyLauncher)instance.Unwrap();

            launcher.LaunchApp();

            AppDomain.Unload(domain);
        }


        [Test, Ignore("Not unit tests - require manually closing exe that is launched")]
        public void CanCreateAnObjectFromAnExeWithinCurrentAppDomain()
        {
            //this is just to show that its possible to create an instance from an exe

            ReflectionFriendlyLauncher launcher = new ReflectionFriendlyLauncher();
            launcher.LaunchApp();
        }
    }
}