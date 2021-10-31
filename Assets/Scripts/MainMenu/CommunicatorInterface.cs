using System.Collections.Generic;

public interface ICommunicator {
    SynchronizedCollection<Network.Utils.TcpClient> GetClientList();
}