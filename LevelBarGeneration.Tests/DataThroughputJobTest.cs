using Moq;

namespace LevelBarGeneration.Tests
{
    public class DataThroughputJobTest
    {
        private DataThroughputJob dataThroughputJob;
        private Mock<ILevelBarGenerator> mockLevelBarGenerator;
        
        public DataThroughputJobTest() 
        {
            mockLevelBarGenerator = new Mock<ILevelBarGenerator>();
            dataThroughputJob = new DataThroughputJob(mockLevelBarGenerator.Object);
        }

        [Fact]
        public async void SendScheduledData_RunReceivedData5times_Test()
        {
            //Arrange
            int[] channelIds = [1, 2];
            float[][] levels = [[0, 0.12f], [0.1f, 0.2f]];

            bool isReceived = false;

            int count = 0;
            int expectedCount = 5;
            CancellationTokenSource cts = new CancellationTokenSource();

            mockLevelBarGenerator.Setup(m => m.ReceiveLevelData(It.IsAny<int[]>(), It.IsAny<float[]>()))
               .Callback(() =>
               {
                   isReceived = true;
                   count++;
                   if (count == expectedCount)
                       cts.Cancel();
               });

            
            //Act
            await dataThroughputJob.SendScheduledData(20, channelIds, levels, cts.Token);


            //Assert
            mockLevelBarGenerator.Verify(m => m.ReceiveLevelData(It.IsAny<int[]>(), It.IsAny<float[]>()), 
                Times.Exactly(expectedCount));
            Assert.True(isReceived);
        }

        [Fact]
        public async void SendScheduledData_LevelsEmpty_TaskShouldExit()
        {
            //Arrange
            int[] channelIds = [1, 2];
            float[][] levels = [];

            bool isReceived = false;

            int count = 0;
            int expectedCount = 0;
            CancellationTokenSource cts = new CancellationTokenSource();

            
            mockLevelBarGenerator.Setup(m => m.ReceiveLevelData(It.IsAny<int[]>(), It.IsAny<float[]>()))
               .Callback(() =>
               {
                   isReceived = true;
                   count++;
                   if (count == expectedCount)
                       cts.Cancel();
               });

            //Act
            await dataThroughputJob.SendScheduledData(20, channelIds, levels, cts.Token);


            //Assert
            mockLevelBarGenerator.Verify(m => m.ReceiveLevelData(It.IsAny<int[]>(), It.IsAny<float[]>()),
                Times.Exactly(expectedCount));
            Assert.True(!isReceived);
        }
    }



}