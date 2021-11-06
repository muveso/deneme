using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Utils;
using Google.Protobuf.WellKnownTypes;
using System.Net;

public class ClientCommunicator : Utils.BaseThread {
    
    public SynchronizedCollection<ClientDetails> Clients { get; private set; }
    private Utils.Network.TcpClient _client;

    const int POLL_TIMEOUT_MS = 1000;

    public ClientCommunicator(string ipAddress, int port) {
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        Clients = new SynchronizedCollection<ClientDetails>();
        _client = new Utils.Network.TcpClient(serverEndPoint);
    }

    public void Send(Google.Protobuf.IMessage message) {
        _client.Send(MessagesHelpers.ConvertMessageToBytes(message));
    }

    protected override void RunThread() {
        while (ThreadShouldRun) {
            if (_client.Sock.Poll(POLL_TIMEOUT_MS, SelectMode.SelectRead)) {
                HandleMessage();
            }
        }
    }

    private void HandleMessage() {
        byte[] messageBytes = _client.Recieve();
        Any message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
        if (message.Is(ClientDetailsMessage.Descriptor)) {
            ClientDetailsMessage clientDetailsMessage = message.Unpack<ClientDetailsMessage>();
            UpdateClient(clientDetailsMessage);
        } else {
            Debug.LogError("Unknown message type");
        }
    }

    private void UpdateClient(ClientDetailsMessage clientDetailsMessage) {
        ClientDetails client = GetClient(clientDetailsMessage);
        if (client != null) {
            Debug.Log($"Got client update for: {clientDetailsMessage.Nickname}");
            client.IsReady = clientDetailsMessage.IsReady;
        } else {
            Debug.Log($"New client with nickname: {clientDetailsMessage.Nickname}");
            Clients.Add(new ClientDetails(clientDetailsMessage.Nickname, clientDetailsMessage.IsReady));
        }        
    }

    private ClientDetails GetClient(ClientDetailsMessage clientDetailsMessage) {
        foreach (ClientDetails client in Clients) {
            if (client.Nickname == clientDetailsMessage.Nickname) {
                return client;
            }
        }
        return null;
    }
}
