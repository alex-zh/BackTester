﻿namespace Launcher
{
	using System.ComponentModel;
	using System.Configuration.Install;

	[RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}