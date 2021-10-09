using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Shapes;
using System.Timers;
using AssettoCorsaSharedMemory;
using Newtonsoft.Json;

namespace ACCTelemetrySharing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SocketIO client = new SocketIO("http://localhost:3000/data");
        private Timer updateTimer = new Timer();
        private Graphics lastUpdate = new Graphics();
        private AssettoCorsa sharedMemoryReader = new AssettoCorsa();

        private Graphics oldGraphicsUpdate = new Graphics();
        private Graphics currentGraphicsUpdate = new Graphics();

        public MainWindow()
        {
            InitializeComponent();

            updateTimer.Elapsed += new ElapsedEventHandler(sendUpdate);
            updateTimer.Interval = 1000;
            updateTimer.Stop();

            sharedMemoryReader.ConnectionStatusChanged += connectionStatusChanged;

            //sharedMemoryReader.GameStatusChanged += gameStatusChanged;
            //sharedMemoryReader.GraphicsUpdated += graphicsUpdated;

            sharedMemoryReader.Start();
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

        void graphicsUpdated(object sender, GraphicsEventArgs e)
        {
            //Trace.WriteLine("GAME STATUS EVENT: " + e.GameStatus.ToString());
            oldGraphicsUpdate = currentGraphicsUpdate;
            currentGraphicsUpdate = e.Graphics;
        }

        async private void connect()
        {
            client.On("hi", response =>
            {
                // You can print the returned data first to decide what to do next.
                // output: ["hi client"]
                Console.WriteLine(response);

                string text = response.GetValue<string>();

                Trace.WriteLine(text);

                // The socket.io server code looks like this:
                // socket.emit('hi', 'hi client');
            });

            client.OnConnected += (sender, e) =>
            {
                Trace.WriteLine("Connected!");

                updateTimer.Start();
            };

            await client.ConnectAsync();
        }

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            connect();
            Trace.WriteLine("Connecting...!");
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

                    var update = UpdateFactory.createRealTimeUpdate(graphics, physics, staticInfo);

                    var json = JsonConvert.SerializeObject(update);
                    await client.EmitAsync("acc update", json);
                }
            });
        }
    }
}
