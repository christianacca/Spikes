using System;
using System.ComponentModel;
using System.Configuration.Install;

namespace Eca.Spikes.SimpleService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}