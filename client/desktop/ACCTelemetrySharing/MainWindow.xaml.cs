﻿
using System;
using System.Windows;
using System.Diagnostics;
using System.Timers;
using AssettoCorsaSharedMemory;
using Newtonsoft.Json;
using System.Text;

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
        
        private Timer updateTimer = new Timer();
        private AssettoCorsa sharedMemoryReader = new AssettoCorsa();
        private string shortName { get; set; }

        private RealTimeUpdate lastUpdate;
        private LapUpdate currentLap = new LapUpdate(-1);
        private StintUpdate currentStint = new StintUpdate();
        private readonly Random _random = new Random();
        private int lastGraphicsPacketId = -1;

        private ServerCommunicator serverComms = new ServerCommunicator();

        public MainWindow() {
            InitializeComponent();

            updateTimer.Elapsed += new ElapsedEventHandler(sendUpdate);
            updateTimer.Interval = 1000;
            updateTimer.Stop();

            sharedMemoryReader.ConnectionStatusChanged += connectionStatusChanged;

            sharedMemoryReader.Start();

            updateConnectButton(ConnectionState.DISCONNECTED);
            connectButton.IsEnabled = false;
        }

        void connectionStatusChanged(object sender, ConnectionStatusEventArgs e) {
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

        private void updateConnectButton(ConnectionState state) {
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

        private void sendUpdate(object sender, EventArgs e) {
            
            if (sharedMemoryReader.IsRunning)
            {
                var graphics = sharedMemoryReader.ReadGraphics();
                var physics = sharedMemoryReader.ReadPhysics();
                var staticInfo = sharedMemoryReader.ReadStaticInfo();

                Trace.WriteLine("ACtiveCars: " + graphics.ActiveCars);

                // naive check to see if we are actually driving
                if (lastGraphicsPacketId == graphics.PacketId) {
                    lastUpdate = null;
                    currentLap = new LapUpdate(0);
                    return;
                }

                // naive check to see if we have disconnected
                if (graphics.ActiveCars == 0) {
                    lastUpdate = null;
                    currentLap = new LapUpdate(0);
                    return;
                }

                if (lastUpdate != null && lastUpdate.completedLaps < graphics.completedLaps) {
                    // update stint
                    currentStint.update(graphics, physics, staticInfo);
                    var newLapUpdate = UpdateFactory.createNewLapUpdate(shortName, currentLap, currentStint);

                    _ = serverComms.sendUpdate(newLapUpdate);

                    // new lap
                    currentLap = new LapUpdate(graphics.completedLaps);
                }
                // pit in
                if (lastUpdate != null && lastUpdate.isInPitLane == 0 && graphics.isInPitLane == 1) {
                    Trace.WriteLine("Sending pit in update!");
                    _ = serverComms.sendUpdate(UpdateFactory.createPitInUpdate(shortName, graphics));
                }

                // pit out
                if (lastUpdate != null && lastUpdate.isInPitLane == 1 && graphics.isInPitLane == 0) {
                    Trace.WriteLine("Sending pit out update!");
                    _ = serverComms.sendUpdate(UpdateFactory.createPitOutUpdate(shortName, graphics));
                }

                currentLap.update(graphics, physics, staticInfo);

                var realTimeUpdate = UpdateFactory.createRealTimeUpdate(
                    shortName,
                    graphics,
                    physics,
                    staticInfo
                );

                // don't send the first update, to avoid overlaps with the previous driver
                if (lastUpdate == null) {
                    lastUpdate = realTimeUpdate;
                    return;
                }

                _ = serverComms.sendUpdate(realTimeUpdate);
                lastUpdate = realTimeUpdate;
            }
            
        }

        /// ui callbacks
        private async void connectButton_Click(object sender, RoutedEventArgs e) {
            if (serverComms.isConnected)
            {
                updateTimer.Stop();

                await serverComms.disconnect();

                Trace.WriteLine("Disconnected!");
                updateConnectButton(ConnectionState.DISCONNECTED);
            }
            else
            {
                updateConnectButton(ConnectionState.CONNECTING);

                if (joinRoom.IsChecked != null && (bool)joinRoom.IsChecked) {
                    await serverComms.connect(UpdateFactory.createRoomConnectUpdate(shortName, roomName.Text), () => {
                        updateConnectButton(ConnectionState.CONNECTED);
                        updateTimer.Start();
                        Trace.WriteLine("Connected");
                    });
                } else {
                    var generatedRoomName = randomString(5, true);
                    this.Dispatcher.Invoke(() => {
                        roomName.Text = generatedRoomName;
                    });
                    await serverComms.connect(UpdateFactory.createRoomCreateUpdate(shortName, generatedRoomName), () => {
                        updateConnectButton(ConnectionState.CONNECTED);
                        updateTimer.Start();
                        Trace.WriteLine("Connected");
                    });
                }

                Trace.WriteLine("Connecting...!");
            }
        }

        private string randomString(int size, bool lowerCase = false) {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++) {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
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
