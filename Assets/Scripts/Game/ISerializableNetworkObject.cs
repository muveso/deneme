using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Assets.Scripts.Game {
    public interface ISerializableNetworkObject {
        /// <summary>
        ///     Gets state from server and deserialize it to the object
        /// </summary>
        /// <param name="message">The input message from client</param>
        public void DeserializeState(Any message);

        /// <summary>
        ///     Serialize the object into message
        /// </summary>
        /// <returns>State message to send to client</returns>
        public IMessage SerializeState();
    }
}