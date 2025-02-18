using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelBarGeneration
{
    /// <summary>
    /// Interface for the Level Bar Generator
    /// </summary>
    public interface ILevelBarGenerator
    {
        void Connect();
        void Disconnect();

        void ReceiveLevelData(int[] channelIds, float[] levels);

        event EventHandler<ChannelChangedEventArgs> ChannelAdded;
        event EventHandler<ChannelChangedEventArgs> ChannelRemoved;
        event EventHandler<ChannelDataEventArgs> ChannelLevelDataReceived;
        event EventHandler<GeneratorStateChangedEventArgs> GeneratorStateChanged;
    }
}
