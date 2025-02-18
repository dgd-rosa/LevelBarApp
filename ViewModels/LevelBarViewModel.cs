// <copyright file="LevelBarViewModel.cs" company="VIBES.technology">
// Copyright (c) VIBES.technology. All rights reserved.
// </copyright>

namespace LevelBarApp.ViewModels
{
    using System;
    using System.Windows.Threading;
    using GalaSoft.MvvmLight;


    /// <summary>
    /// Represents a level bar for a channel
    /// </summary>
    /// <seealso cref="ViewModelBase" />
    public class LevelBarViewModel : ViewModelBase
    {
        // Fields
        private string name = string.Empty;
        private float level = 0.0f;
        private float maxLevel = 0.0f;
        private int id;
        private DispatcherTimer _timerPeakHold;
        private bool isPeakHoldVisible = false;
        private int _peakHoldMaintenanceTime;

        /// <summary>
        /// LevelBarViewModel
        /// </summary>
        public LevelBarViewModel()
        {
            _timerPeakHold = new DispatcherTimer();

            //Define the time span for the peak hold to be seen
            _peakHoldMaintenanceTime = AppConfigurationSettings.PeakHoldMaintenanceTime;
            _timerPeakHold.Interval = TimeSpan.FromSeconds(_peakHoldMaintenanceTime);
            _timerPeakHold.Tick += UpdatePeakHoldVisibility;
        }

       
        // Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id
        {
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        /// <summary>
        /// Gets or sets the name of the channel.
        /// </summary>
        /// <value>
        /// The name of the channel.
        /// </value>
        public string Name
        {
            get => name;
            set
            {
                name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public float Level
        {
            get => level;
            set
            {
                level = value;
                UpdateMaxLevel(); //Update Max Level
                RaisePropertyChanged(nameof(Level));
                RaisePropertyChanged(nameof(LevelDecibel));
            }
        }

        /// <summary>
        /// Gets or sets the maximum level used of the peakhold.
        /// </summary>
        /// <value>
        /// The maximum level.
        /// </value>
        public float MaxLevel
        {
            get => maxLevel;
            set
            {
                maxLevel = value;
                RaisePropertyChanged(nameof(MaxLevel));
                RaisePropertyChanged(nameof(MaxLevelDecibel));
                if(maxLevel > 0)
                    IsPeakHoldVisible = true;
            }
        }

        /// <summary>
        /// Gets or sets the boolean used to put the peakhold visible
        /// <value>
        /// if peahold is visible
        /// </value>
        /// </summary>
        public bool IsPeakHoldVisible
        {
            get => isPeakHoldVisible;
            set
            {
                isPeakHoldVisible = value;
                RaisePropertyChanged(nameof(IsPeakHoldVisible));

                //if the peakhold is visible than the timer should start
                if (isPeakHoldVisible == true)
                {
                    _timerPeakHold.Interval = TimeSpan.FromSeconds(_peakHoldMaintenanceTime); //Reset Timer
                    _timerPeakHold.Start();
                }
            }
        }

        /// <summary>
        /// Gets the Level value in dB
        /// </summary>
        public float LevelDecibel => LinearToDecibel(Level);

        /// <summary>
        /// Gets the Max Level value in dB
        /// </summary>
        public float MaxLevelDecibel => LinearToDecibel(MaxLevel);
        

        //method

        private float LinearToDecibel(float value)
        {
            if (value <= 0.0f)
            {
                return -60;
            }

            return (float)(20*Math.Log10(value));
        }

        private void UpdateMaxLevel()
        {
            if (MaxLevel <= level)
            {
                MaxLevel = level;
            }
        }

        private void UpdatePeakHoldVisibility(object sender, EventArgs e)
        {
            _timerPeakHold.Stop();
            IsPeakHoldVisible = false;
            MaxLevel = 0; // Reset the Peak
        }
    }
}