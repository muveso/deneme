using System.Net;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.WellKnownTypes;

namespace Evade.Game {
    public class ClientManager : MonoBehaviour {
        private UdpCommunicator _clientCommunicator;
        private UdpServerCommunicator _serverCommunicator;
        void Start() {
            SynchronizedCollection<IPEndPoint> clients = new SynchronizedCollection<IPEndPoint>();
            clients.Add(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555));
            _serverCommunicator = new UdpServerCommunicator(5555, clients);
            _serverCommunicator.Start();
            _clientCommunicator = new UdpCommunicator("127.0.0.1", 5555);

            ClientReadyMessage clientReadyMessage = new ClientReadyMessage();
            _clientCommunicator.Send(clientReadyMessage);
            
            // Any messageFromClient;
            // _serverCommunicator.MessagesQueue.TryDequeue(out messageFromClient);
            // ClientReadyMessage messageParsed = messageFromClient.Unpack<ClientReadyMessage>();
        }

        void Update() {
        }
    }
}
