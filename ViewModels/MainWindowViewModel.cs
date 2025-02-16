// <copyright file="MainWindowViewModel.cs" company="VIBES.technology">
// Copyright (c) VIBES.technology. All rights reserved.
// </copyright>

namespace LevelBarApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Threading;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using LevelBarGeneration;
    using System.Configuration;

    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    /// <seealso cref="ViewModelBase" />
    public class MainWindowViewModel : ViewModelBase
    {
        // Fields
        private readonly LevelBarGenerator levelBarGenerator;
        private RelayCommand connectToGeneratorCommand;
        private RelayCommand disconnectToGeneratorCommand;
        private DispatcherTimer _dispatcherTimer;
        private readonly int _levelBarViewUpdateRate;
        private Dictionary<int, float> _levelsBuffer = new Dictionary<int, float>();

        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            levelBarGenerator = LevelBarGenerator.Instance;

            levelBarGenerator.GeneratorStateChanged += LevelBarGenerator_GeneratorStateChanged;
            levelBarGenerator.ChannelAdded += LevelBarGenerator_ChannelAdded;
            levelBarGenerator.ChannelLevelDataReceived += LevelBarGenerator_ChannelDataReceived;
            levelBarGenerator.ChannelRemoved += LevelBarGenerator_ChannelRemoved;


            //Get from config file
            _levelBarViewUpdateRate = int.Parse(ConfigurationManager.AppSettings["LevelBarViewUpdateRate"]);
            //Configure Dispatcher Timer for Scheduled updates of the bars
            _dispatcherTimer = new DispatcherTimer 
            {
                Interval = TimeSpan.FromMilliseconds(_levelBarViewUpdateRate)
            };

            _dispatcherTimer.Tick += UpdateViewLevelBars;
            _dispatcherTimer.Start();
        }

        // Properties

        /// <summary>
        /// Gets or sets the level bars, one for each channel.
        /// </summary>
        /// <value>
        /// The level bars.
        /// </value>
        public ObservableCollection<LevelBarViewModel> LevelBars { get; set; } = new ObservableCollection<LevelBarViewModel>();


        /// <summary>
        /// Gets the command to connect the generator
        /// </summary>
        /// <value>
        /// The connect generator.
        /// </value>
        public RelayCommand ConnectGeneratorCommand => connectToGeneratorCommand ??
            (connectToGeneratorCommand = new RelayCommand(async () =>
            {
                IsConnecting = true;

                await levelBarGenerator.Connect();

                IsConnecting = false;
                IsConnected = true;
            }));

        /// <summary>
        /// Gets the command to disconnect the generator
        /// </summary>
        /// <value>
        /// The disconnect generator.
        /// </value>
        public RelayCommand DisconnectGeneratorCommand => disconnectToGeneratorCommand ??
            (disconnectToGeneratorCommand = new RelayCommand(async () =>
            {

                await levelBarGenerator.Disconnect();
                IsConnected = false;

            }));

        
        private GeneratorState _state;
        /// <summary>
        /// Gets the State of the generator from the event
        /// </summary>
        /// <value>
        /// LevelsGenerator State value (Running or Stopped)
        /// </value>
        public GeneratorState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                RaisePropertyChanged(nameof(State));
                UpdateConnectButtonCondition();
            }
        }

        private bool _isConnecting = false;
        /// <summary>
        /// Gets if the App is connected or not to the generator
        /// useful to enable or disable the UI buttons
        /// </summary>
        /// <value>
        /// bool 
        /// </value>
        public bool IsConnecting
        {
            get
            {
                return _isConnecting;
            }
            set
            {
                if (value != _isConnecting)
                {
                    _isConnecting = value;
                    RaisePropertyChanged(nameof(IsConnecting));
                }
            }
        }


        
        private bool _isConnected = false;
        /// <summary>
        /// Checks if the app is connected to the generator. Disables or enables the connect button
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                _isConnected = value;
                IsDisconnected = !value;
                RaisePropertyChanged(nameof(IsConnected));
            }
        }

        private bool _isDisconnected = true;
        /// <summary>
        /// Checks if the app is disconnected to the generator. Disables or enables the disconnect button
        /// </summary>
        public bool IsDisconnected
        {
            get
            {
                return _isDisconnected;
            }
            set
            {
                _isDisconnected = value;
                RaisePropertyChanged(nameof(IsDisconnected));
            }
        }

        // Methods
        private void LevelBarGenerator_ChannelAdded(object sender, ChannelChangedEventArgs e)
        {
            //Create a level bar
            LevelBarViewModel newLevelBarViewModel = new LevelBarViewModel { Id = e.ChannelId, Name = e.ChannelId.ToString(), Level = 0, MaxLevel=0};
            LevelBars.Add(newLevelBarViewModel);

            
            if(!_levelsBuffer.ContainsKey(e.ChannelId))
                _levelsBuffer.Add(e.ChannelId, 0);
        }

        private void LevelBarGenerator_ChannelRemoved(object sender, ChannelChangedEventArgs e)
        {
            // Remove the corresponding LevelBarViewModel
            int index = LevelBars.ToList().IndexOf(LevelBars.FirstOrDefault(x => x.Id == e.ChannelId));
            if (index >= 0)
            {
                LevelBars.RemoveAt(index);
            }
        }

        private void LevelBarGenerator_GeneratorStateChanged(object sender, GeneratorStateChangedEventArgs e)
        {
            State = e.State;
        }

        private void LevelBarGenerator_ChannelDataReceived(object sender, ChannelDataEventArgs e)
        {
            for (int i = 0; i < e.ChannelIds.Count(); i++)
            {
                //Update Buffer with the latest value
                _levelsBuffer[e.ChannelIds[i]] = e.Levels[i];
            }
        }


        /// <summary>
        /// Method to Update the IsConnected Value to enable or disable the buttons
        /// </summary>
        private void UpdateConnectButtonCondition()
        {
            if (State == GeneratorState.Running)
                IsConnected = true;
            else
                IsConnected = false;
        }

        /// <summary>
        /// Method to Update Level Bars binded in the View with timer of DispatcherTimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateViewLevelBars(object sender, EventArgs e)
        {
            foreach (var levelBar in LevelBars) 
            {
                //Check if levels buffer has Id
                if (_levelsBuffer.TryGetValue(levelBar.Id, out float newValue))
                {
                    levelBar.Level = newValue;
                }
            }
        }
    }
}
