using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SocketIOClient;

namespace ACCTelemetrySharing
{
    enum SERVERSTATE
    {
        DISCONNECTED,
        CONNECTED,
    }

    class ServerCommunicator
    {
        private string API = "http://localhost:3000/data";
        private SocketIO client;

        public SERVERSTATE state { get; private set; }

        public bool isConnected {
            get
            {
                return state == SERVERSTATE.CONNECTED;
            }
        }

        public ServerCommunicator() {
            state = SERVERSTATE.DISCONNECTED;
        }

        public async Task connect(RoomConnectUpdate roomConnectUpdate, Action onConnect) {
            await private_connect(() => {
                client.EmitAsync(roomConnectUpdate.eventName, JsonConvert.SerializeObject(roomConnectUpdate));
                onConnect();
            });
        }

        public async Task connect(RoomCreateUpdate roomCreateUpdate, Action onConnect) {
            await private_connect(() => {
                client.EmitAsync(roomCreateUpdate.eventName, JsonConvert.SerializeObject(roomCreateUpdate));
                onConnect();
            });
        }

        private async Task private_connect(Action onConnect) {
            client = new SocketIO(API);

            client.OnConnected += (sender, e) => {
                state = SERVERSTATE.CONNECTED;
                onConnect();
            };

            client.OnDisconnected += (sender, e) => {
                state = SERVERSTATE.DISCONNECTED;
            };

            await client.ConnectAsync();
        }

        public async Task disconnect() {
            state = SERVERSTATE.DISCONNECTED;
            await client.DisconnectAsync();
            client.Dispose();
        }

        public async Task sendUpdate(ACCEvent accEvent) {
            var updateJson = JsonConvert.SerializeObject(accEvent);
            await client.EmitAsync(accEvent.eventName, updateJson);
        }
    }
}
