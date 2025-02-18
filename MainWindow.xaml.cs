// <copyright file="MainWindow.xaml.cs" company="VIBES.technology">
// Copyright (c) VIBES.technology. All rights reserved.
// </copyright>

namespace LevelBarApp
{
    using System;
    using System.Windows;
    using LevelBarApp.ViewModels;
    using LevelBarGeneration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IServiceProvider _serviceProvider;
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            ConfigureServices();

            DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();

            InitializeComponent();
        }


        private void ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<DataThroughputJob>();

            services.AddSingleton<ILevelBarGenerator, LevelBarGenerator>();

            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
