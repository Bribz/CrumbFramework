using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CrumbShared;
using LiteNetLib;
using LiteNetLib.Utils;
using MessagePack;

namespace CrumbServer
{
    public class CServer
    {
        protected NetManager m_NetManager;
        protected readonly EventBasedNetListener m_EventListener;
        protected readonly NetDataWriter m_CachedWriter;
        protected ConnectionManager m_ConnectionManager;

        protected enum RunState { RUNNING, STOPPING };
        protected RunState RUN_STATE = RunState.STOPPING;

        public ushort Tick => m_CurrentTick;
        protected ushort m_CurrentTick;

        public const int MAX_CONNECTIONS = 64;
        public int CurrentConnectionCount = 0;

        public CServer()
        {
            m_EventListener = new EventBasedNetListener();
            m_NetManager = new NetManager(m_EventListener);
            m_CachedWriter =  new NetDataWriter();
            m_ConnectionManager = new ConnectionManager();

            //Hook event callbacks to Event Listener
            SubscribeLiteNetEvents();

            Log.LogReceived += log_msg;
        }

        public virtual void Start()
        {
            if (RUN_STATE == RunState.RUNNING)
                return;

            Log.LogSystem("Starting Server...");

            RUN_STATE = RunState.RUNNING;

            m_NetManager.Start(CrumbShared.Version.Port);

            while(RUN_STATE == RunState.RUNNING)
            {
                m_NetManager.PollEvents();
                Thread.Sleep(m_NetManager.UpdateTime);
            }

            Log.LogSystem("Stopping Server...");
            Log.LogSystem("Disconnecting all peers.");
            m_NetManager.DisconnectAll();
            Log.LogSystem("Server Stopped.");
            m_NetManager.Stop();
        }

        public virtual void Stop()
        {
            RUN_STATE = RunState.STOPPING;
        }

        /// <summary>
        /// Subscribe the server event callbacks to the EventListener
        /// </summary>
        /// <param name="subscribe">Should subscribe or unsubscribe?</param>
        protected void SubscribeLiteNetEvents(bool subscribe = true)
        {
            if(subscribe)
            {
                m_EventListener.ConnectionRequestEvent += connection_request_event;
                m_EventListener.PeerConnectedEvent += peer_connected_event;
                m_EventListener.PeerDisconnectedEvent += peer_disconnected_event;
                m_EventListener.NetworkErrorEvent += network_error;
                m_EventListener.NetworkLatencyUpdateEvent += latency_update;
                m_EventListener.NetworkReceiveUnconnectedEvent += receive_unconnected;
                m_EventListener.NetworkReceiveEvent += receive;
            }
            else
            {
                m_EventListener.ConnectionRequestEvent -= connection_request_event;
                m_EventListener.PeerConnectedEvent -= peer_connected_event;
                m_EventListener.PeerDisconnectedEvent -= peer_disconnected_event;
                m_EventListener.NetworkErrorEvent -= network_error;
                m_EventListener.NetworkLatencyUpdateEvent -= latency_update;
                m_EventListener.NetworkReceiveUnconnectedEvent -= receive_unconnected;
                m_EventListener.NetworkReceiveEvent -= receive;
            }
            
        }

        #region Technical Upkeep
        /// <summary>
        /// Event callback when LiteNetLib experiences a socket error.
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="socketError"></param>
        protected virtual void network_error(IPEndPoint endPoint, SocketError socketError)
        {
            Log.LogError($"{socketError.ToString()} - {endPoint.Address.ToString()}");
        }

        /// <summary>
        /// Write a message to console. Called from Log class.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        protected virtual void log_msg(LogLevel level, string msg)
        {
            Console.WriteLine($"[{level.ToString()}] : {msg}");
        }

        /// <summary>
        /// Ping update from peer received
        /// </summary>
        /// <param name="peer">NetPeer sender</param>
        /// <param name="latency">current ping</param>
        protected virtual void latency_update(NetPeer peer, int latency)
        {

        }
        #endregion

        #region LiteNetLib Connections
        /// <summary>
        /// Event callback when a NetPeer disconnects from the server.
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="disconnectInfo"></param>
        protected virtual void peer_disconnected_event(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.LogMsg("Peer disconnected : <{0}> {1}", peer.Id, System.Enum.GetName(typeof(DisconnectReason),disconnectInfo.Reason));
            m_ConnectionManager.Remove(peer);
        }

        /// <summary>
        /// Event callback when a NetPeer connects to the server.
        /// </summary>
        /// <param name="peer"></param>
        protected virtual void peer_connected_event(NetPeer peer)
        {
            Log.LogMsg("Peer connected : <{0}> {1}", peer.Id, peer.EndPoint.ToString());
            m_ConnectionManager.Add(new ClientConnection(peer));
        }

        /// <summary>
        /// Event Callback when a NetPeer requests to connect to the server.
        /// </summary>
        /// <param name="request"></param>
        protected virtual void connection_request_event(ConnectionRequest request)
        {
            if(CurrentConnectionCount < MAX_CONNECTIONS)
            {
                Log.LogVerbose("Accepted connection request.");

                request.AcceptIfKey(CrumbShared.Version.Stub);
            }
            else
            {
                Log.LogVerbose("Rejected connection request.");

                request.Reject();
            }
        }
        #endregion

        #region Network Receive Events
        /// <summary>
        /// Event callback when the NetManager receives a message from an unconnected client.
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="reader"></param>
        /// <param name="messageType"></param>
        protected virtual void receive_unconnected(System.Net.IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {

        }

        /// <summary>
        /// Event callback when a message is received in the event poll
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="reader"></param>
        /// <param name="deliveryMethod"></param>
        protected virtual void receive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {

        }
        #endregion

        #region Packet Events

        /// <summary>
        /// Write a packet specifically for MessagePack.
        /// </summary>
        /// <typeparam name="T">Packet Type</typeparam>
        /// <param name="type">Packet Type to pack into writer</param>
        /// <param name="packet">Packet object</param>
        /// <returns></returns>
        public NetDataWriter write_packet<T>(PacketType type, T packet) where T : class, new()
        {
            m_CachedWriter.Reset();
            m_CachedWriter.Put((byte)type);
            m_CachedWriter.PutBytesWithLength(MessagePackSerializer.Serialize(packet));
            return m_CachedWriter;
        }

        /// <summary>
        /// Read a packet after the type has been resolved
        /// </summary>
        /// <typeparam name="T">Type of packet</typeparam>
        /// <param name="reader">NetDataReader to read from</param>
        /// <returns>Packet Object</returns>
        public T read_packet<T>(NetDataReader reader)
        {
            var data = reader.GetBytesWithLength();
            return MessagePackSerializer.Deserialize<T>(data);
        }


        #endregion
    }
}
