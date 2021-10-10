using SocketIOClient;
using System;
using System.Windows;
using System.Diagnostics;
using System.Timers;
using AssettoCorsaSharedMemory;
using Newtonsoft.Json;

namespace ACCTelemetrySharing
{
    enum ConnectionState
    {
        DISCONNECTED,
        CONNECTING,
        CONNECTED
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string API = "http://localhost:3000/data";
        private SocketIO client;
        private Timer updateTimer = new Timer();
        private AssettoCorsa sharedMemoryReader = new AssettoCorsa();
        private string shortName { get; set; }

        private RealTimeUpdate lastUpdate;
        private LapUpdate currentLap;
        private StintUpdate currentStint;

        public MainWindow()
        {
            InitializeComponent();

            updateTimer.Elapsed += new ElapsedEventHandler(sendUpdate);
            updateTimer.Interval = 1000;
            updateTimer.Stop();

            sharedMemoryReader.ConnectionStatusChanged += connectionStatusChanged;

            sharedMemoryReader.Start();

            updateConnectButton(ConnectionState.DISCONNECTED);
            connectButton.IsEnabled = false;
        }

        void connectionStatusChanged(object sender, ConnectionStatusEventArgs e)
        {
            var newContent = "";
            if (e.connectionStatus == ACC_CONNECTION_STATUS.CONNECTED)
            {
                newContent = "Connected!";
            } 
            else
            {
                newContent = "Disconnected!";
            }

            this.Dispatcher.Invoke(() =>
            {
                connectionStatus.Content = newContent;
            });
        }

        private void setupConnection()
        {
            client.OnConnected += (sender, e) =>
            {
                Trace.WriteLine("Connected!");
                updateConnectButton(ConnectionState.CONNECTED);

                updateTimer.Start();
            };

            client.OnDisconnected += (sender, e) =>
            {
                updateConnectButton(ConnectionState.DISCONNECTED);
            };
        }

        private void updateConnectButton(ConnectionState state)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (state)
                {
                    case ConnectionState.DISCONNECTED:
                        connectButton.IsEnabled = true;
                        connectButton.Content = "Connect";
                        break;
                    case ConnectionState.CONNECTING:
                        connectButton.IsEnabled = false;
                        connectButton.Content = "Connecting...";
                        break;

                    case ConnectionState.CONNECTED:
                        connectButton.IsEnabled = true;
                        connectButton.Content = "Disconnect";
                        break;
                }
            });
        }

        async private void sendUpdate(object sender, EventArgs e)
        {
            await this.Dispatcher.Invoke(async () =>
            {
                if (sharedMemoryReader.IsRunning)
                {
                    var graphics = sharedMemoryReader.ReadGraphics();
                    var physics = sharedMemoryReader.ReadPhysics();
                    var staticInfo = sharedMemoryReader.ReadStaticInfo();

                    if (lastUpdate.completedLaps < graphics.completedLaps)
                    {
                        // update stint
                        currentStint.update(graphics, physics, staticInfo);
                        var newLapUpdate = UpdateFactory.createNewLapUpdate(shortName, currentLap, currentStint);
                        var newLapUpdateJson = JsonConvert.SerializeObject(newLapUpdate);

                        await client.EmitAsync("new-lap-update", newLapUpdateJson);

                        // new lap
                        currentLap = new LapUpdate(graphics.completedLaps);
                    }

                    if (lastUpdate.isInPitLane == 1 && graphics.isInPitLane == 0)
                    {
                        // new stint
                        currentStint = new StintUpdate();
                    }

                    currentLap.update(graphics, physics, staticInfo);
                    
                    var realTimeUpdate = UpdateFactory.createRealTimeUpdate(
                        shortName, 
                        graphics, 
                        physics, 
                        staticInfo
                    );

                    var realTimeUpdateJson = JsonConvert.SerializeObject(realTimeUpdate);
                    await client.EmitAsync("real-time-update", realTimeUpdateJson);
                    lastUpdate = realTimeUpdate;
                }
            });
        }

        /// ui callbacks
        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            if (client != null && client.Connected)
            {
                updateTimer.Stop();

                await client.DisconnectAsync();
                client.Dispose();

                Trace.WriteLine("Disconnected!");
                updateConnectButton(ConnectionState.DISCONNECTED);
            }
            else if (client == null || client.Disconnected)
            {
                updateConnectButton(ConnectionState.CONNECTING);
                client = new SocketIO(API);
                setupConnection();
                _ = client.ConnectAsync();
                Trace.WriteLine("Connecting...!");
            }
        }

        private void shortNameTextBox_onChange(object sender, EventArgs e)
        {
            shortName = shortNameTextBox.Text;
            if (shortName == null)
            {
                connectButton.IsEnabled = false;
                return;
            }

            connectButton.IsEnabled = shortName.Length == 3 ? true : false;
        }
    }
}
