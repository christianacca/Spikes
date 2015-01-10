using System;
using System.Reflection;
using CLAP;

namespace MultiMigrate
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveEmbeddedAssemblies;
            ProgramImpl.Run(args);
        }

        /// <summary>
        /// Assembly resolver that will load the assemblies this exe depend on which have been embedded as resources
        /// </summary>
        private static Assembly ResolveEmbeddedAssemblies(object sender, ResolveEventArgs args)
        {
            string thisAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            string dependencyAssemblyName = new AssemblyName(args.Name).Name;
            String resourceName = string.Format("{0}.Dependencies.{1}.dll", thisAssemblyName, dependencyAssemblyName);

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null;
                }

                var assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }


        /// <summary>
        /// this private class ensures that the CLR does not try and load the CLAP assembly from the usual bin location or GAC, before
        /// we have had a chance to override how assemblies are loaded
        /// </summary>
        private static class ProgramImpl
        {
            internal static void Run(string[] args)
            {
                Parser.RunConsole<ConsoleApp>(args);
            }
        }
    }
}