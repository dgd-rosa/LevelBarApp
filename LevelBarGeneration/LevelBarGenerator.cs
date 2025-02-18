// <copyright file="LevelBarGenerator.cs" company="VIBES.technology">
// Copyright (c) VIBES.technology. All rights reserved.
// </copyright>

namespace LevelBarGeneration
{
    using System;
    using System.Threading.Tasks;
    using Quartz;
    using Quartz.Impl;

    /// <summary>
    /// LevelBarGenerator
    /// </summary>
    public class LevelBarGenerator : ILevelBarGenerator
    {
        // Fields
        private GeneratorState state = GeneratorState.Stopped;
        private DataThroughputJob dataThroughputJob;

        // Constructor

        /// <summary>
        /// Prevents a default instance of the <see cref="LevelBarGenerator"/> class from being created.
        /// </summary>
        public LevelBarGenerator()
        {
            dataThroughputJob = new DataThroughputJob(this);
        }

        // Events

        /// <summary>
        /// Occurs when [channel added].
        /// </summary>
        public event EventHandler<ChannelChangedEventArgs> ChannelAdded;

        /// <summary>
        /// Occurs when [channel removed].
        /// </summary>
        public event EventHandler<ChannelChangedEventArgs> ChannelRemoved;

        /// <summary>
        /// Occurs when [channel data received].
        /// </summary>
        public event EventHandler<ChannelDataEventArgs> ChannelLevelDataReceived;

        /// <summary>
        /// Occurs when [state changed].
        /// </summary>
        public event EventHandler<GeneratorStateChangedEventArgs> GeneratorStateChanged;

        // Properties


        // Methods

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns>Connect Task</returns>
        public void Connect()
        {
            if (state == GeneratorState.Running)
            {
                Console.WriteLine("Generator is already connected");
                return;
            }

            // What properties are used
            int channelBlockSize = 512;
            int samplingRate = 16384;
            double samplingTime = 1.0d;

            // Setup the channels
            int numberOfChannels = RegisterChannels();

            // Setup and fire the data generator
            SetupDataGenerator(channelBlockSize, samplingRate, samplingTime, numberOfChannels);

            state = GeneratorState.Running;
            GeneratorStateChanged?.Invoke(this, new GeneratorStateChangedEventArgs { State = GeneratorState.Running });
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        /// <returns>Disconnect Task</returns>
        public void Disconnect()
        {
            dataThroughputJob.CancelDataGeneration();

            DeregisterChannels();

            state = GeneratorState.Stopped;
            GeneratorStateChanged?.Invoke(this, new GeneratorStateChangedEventArgs { State = GeneratorState.Stopped });
        }

        /// <summary>
        /// Receives the level data.
        /// </summary>
        /// <param name="channelIds">The channel ids.</param>
        /// <param name="levels">The levels.</param>
        public void ReceiveLevelData(int[] channelIds, float[] levels)
        {
            ChannelLevelDataReceived?.Invoke(this, new ChannelDataEventArgs { ChannelIds = channelIds, Levels = levels });
        }

        private void SetupDataGenerator(int channelBlockSize, int samplingRate, double samplingTime, int numberOfChannels)
        {
            dataThroughputJob.SetupJob(samplingRate, channelBlockSize, samplingTime, numberOfChannels);

            double intervalSpan = (double)(channelBlockSize / 8d) / samplingRate * 1000d;
            
            var task = dataThroughputJob.GenerateLevelDataAsync(intervalSpan);
        }

        private int RegisterChannels()
        {
            int numberOfChannels = 75;
            for (int i = 0; i < numberOfChannels; ++i)
            {
                ChannelAdded?.Invoke(this, new ChannelChangedEventArgs { ChannelId = i });
            }

            return numberOfChannels;
        }

        private void DeregisterChannels()
        {
            int numberOfChannels = 75;
            for (int i = 0; i < numberOfChannels; ++i)
            {
                ChannelRemoved?.Invoke(this, new ChannelChangedEventArgs { ChannelId = i });
            }
        }
    }
}
