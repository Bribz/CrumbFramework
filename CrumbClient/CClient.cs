using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrumbShared;
using CrumbShared.Objects;
using LiteNetLib;
using LiteNetLib.Utils;
using MessagePack;

namespace CrumbClient
{
    public class CClient
    {
        protected NetManager m_NetManager;
        protected EventBasedNetListener m_EventListener;
        protected readonly NetDataWriter m_CachedWriter;

        protected NetPeer m_ServerPeer;
        protected ClientPlayerManager m_PlayerManager;

        private List<PlayerAvatar> players;

        private ulong LocalCharID = 0;
        private LocalPlayerAvatar LocalPlayerAvatar;
        internal InputUpdate CachedInputUpdate;

        public int Ping = 0;
        protected bool connected = false;

        public CClient()
        {
            m_EventListener = new EventBasedNetListener();
            m_NetManager = new NetManager(m_EventListener);
            m_CachedWriter = new NetDataWriter();

            m_PlayerManager = new ClientPlayerManager(this);
            players = new List<PlayerAvatar>();

            //Hook event callbacks to Event Listener
            SubscribeLiteNetEvents();

            Log.LogReceived += log_msg;

            m_NetManager.Start();
        }

        public void Connect()
        {
            Log.LogSystem("Connecting to Server...");

            //Put AccountID;
            m_CachedWriter.Put((ulong)0);
            //Put CharacterID;
            m_CachedWriter.Put(LocalCharID);

            m_NetManager.Connect(CrumbShared.Version.Address, CrumbShared.Version.Port, m_CachedWriter);

            CachedInputUpdate = new InputUpdate();
            CachedInputUpdate.ID = LocalCharID; 

            m_CachedWriter.Reset();
            
            Run();
        }

        public void Disconnect()
        {
            connected = false;
        }

        protected void Run()
        {
            connected = true;

            while (connected)
            {
                m_NetManager.PollEvents();

                if (m_ServerPeer != null)
                {
                    tick(m_NetManager.UpdateTime);
                }

                Thread.Sleep(m_NetManager.UpdateTime);
            }

            Log.LogSystem("Disconnecting from server...");
            m_NetManager.DisconnectPeer(m_ServerPeer);

            Log.LogSystem("NetManager Stopped.");
            m_NetManager.Stop();
        }

        protected void tick(float delta)
        {
            m_PlayerManager.tick(delta);

            m_ServerPeer.Send(write_packet<InputUpdate>(PacketType.InputUpdate, CachedInputUpdate), DeliveryMethod.ReliableOrdered);
            
        }

        /// <summary>
        /// Subscribe the server event callbacks to the EventListener
        /// </summary>
        /// <param name="subscribe">Should subscribe or unsubscribe?</param>
        protected void SubscribeLiteNetEvents(bool subscribe = true)
        {
            if (subscribe)
            {
                Log.LogVerbose("Subscribing to Network Events.");
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
                Log.LogVerbose("Unsubscribing from Network Events.");
                m_EventListener.ConnectionRequestEvent -= connection_request_event;
                m_EventListener.PeerConnectedEvent -= peer_connected_event;
                m_EventListener.PeerDisconnectedEvent -= peer_disconnected_event;
                m_EventListener.NetworkErrorEvent -= network_error;
                m_EventListener.NetworkLatencyUpdateEvent -= latency_update;
                m_EventListener.NetworkReceiveUnconnectedEvent -= receive_unconnected;
                m_EventListener.NetworkReceiveEvent -= receive;
            }

        }
        
        protected void peer_disconnected_event(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.LogSystem("Disconnected from Server NetPeer!");
            m_ServerPeer = null;
        }

        protected void peer_connected_event(NetPeer peer)
        {
            Log.LogMsg("Connected to Server NetPeer!");
            m_ServerPeer = peer;
        }

        protected void connection_request_event(ConnectionRequest request)
        {
            Log.LogVerbose("Received connection request.");
            request.Reject();
        }

        #region Technical Upkeep
        /// <summary>
        /// Write a message to console. Called from Log class.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        protected virtual void log_msg(LogLevel level, string msg)
        {
            Console.WriteLine($"[{level.ToString()}] : {msg}");
        }

        private void latency_update(NetPeer peer, int latency)
        {
            Log.LogVerbose("Latency Update : {0}", latency);
            Ping = latency;
        }

        private void network_error(IPEndPoint endPoint, SocketError socketError)
        {
            Log.LogError($"{socketError.ToString()} - {endPoint.Address.ToString()}");
        }
        #endregion

        protected void receive_unconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Log.LogVerbose($"Received unconnected message : {remoteEndPoint.Address.ToString()}");
        }

        protected void receive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketType type = (PacketType) reader.GetByte();
            switch (type)
            {
                case PacketType.LoginResponse:
                    break;
                case PacketType.CharacterSelection:
                    break;
                case PacketType.SpawnEntity:
                    break;
                case PacketType.WorldState:
                    var packet = read_packet<WorldState>(reader);
                    update_world_state(packet);
                    break;
            }
        }

        protected void update_world_state(WorldState state)
        {
            CachedInputUpdate.ACK_Tick = state.Tick;

            //Log.LogMsg("Updated world state!");
            foreach(var pstate in state.States)
            {
                var playerobj = m_PlayerManager.Players.Find(p => p.Data.CharacterID == pstate.CharacterID);
                if(playerobj == null)
                {
                    if (pstate.CharacterID == LocalCharID)
                    {
                        LocalPlayerAvatar = m_PlayerManager.add(null, pstate.Position, pstate.Rotation, true) as LocalPlayerAvatar;
                        Log.LogMsg("Found new local player!");
                    }
                    else
                    {
                        m_PlayerManager.add(new CharacterData() { CharacterID = pstate.CharacterID, position = pstate.Position, Rotation = pstate.Rotation }, pstate.Position, pstate.Rotation);
                        Log.LogMsg("Found new player!");
                    }
                }
                else
                {
                    playerobj.Position = pstate.Position;
                    playerobj.Rotation = pstate.Rotation;
                }
            }
        }

        #region Packet reading/writing
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
