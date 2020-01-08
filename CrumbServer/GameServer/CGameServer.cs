using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrumbServer.Database;
using CrumbServer.GameServer;
using CrumbShared;
using CrumbShared.Objects;
using LiteNetLib;
using LiteNetLib.Utils;
using MessagePack;

namespace CrumbServer
{
    public class CGameServer : CServer
    {
        private DatabaseConnection m_DBConnection;

        private GameConnectionManager m_ConnectionManager;
        private List<WaitingConnectionRequest> m_WaitQueue;

        private List<PlayerAvatar> players;
        //private MapManager m_MapManager;

        private WorldState m_CachedWorldState;

        public CGameServer() : base()
        {
            m_DBConnection = new LocalDatabaseConnection();

            //m_MapManager = new MapManager();
            players = new List<PlayerAvatar>();

            m_ConnectionManager = new GameConnectionManager();
            //Create queue for waiting on entering players
            m_WaitQueue = new List<WaitingConnectionRequest>();
        }

        public override void Start()
        {
            if (RUN_STATE == RunState.RUNNING)
                return;

            Log.LogSystem("Starting Server...");

            RUN_STATE = RunState.RUNNING;
            m_NetManager.Start(CrumbShared.Version.Port);

            while (RUN_STATE == RunState.RUNNING)
            {
                m_NetManager.PollEvents();

                tick(m_NetManager.UpdateTime);

                Thread.Sleep(m_NetManager.UpdateTime);
            }

            Log.LogSystem("Stopping Server...");
            Log.LogSystem("Disconnecting all peers.");
            m_NetManager.DisconnectAll();
            Log.LogSystem("Server Stopped.");
            m_NetManager.Stop();
        }

        private void tick(float delta)
        {
            //m_MapManager.tick(delta);
            foreach(var player in players)
            {
                player.tick(delta);
            }

            generate_world_state();
            broadcast_world_state();
            m_CurrentTick++;
        }

        private void generate_world_state()
        {
            m_CachedWorldState = new WorldState();
            m_CachedWorldState.Tick = m_CurrentTick;
            m_CachedWorldState.States = new List<PlayerState>();
            foreach(var player in players)
            {
                var state = new PlayerState();
                state.CharacterID = player.Data.CharacterID;
                state.Position = player.Position;
                state.Rotation = player.Rotation;
                m_CachedWorldState.States.Add(state);

                //Log.LogVerbose($"PlayerPosUpdate<{state.CharacterID}>: {state.Position.ToString()}");
            }
        }

        private void broadcast_world_state()
        {
            m_NetManager.SendToAll(write_packet<WorldState>(PacketType.WorldState, m_CachedWorldState), DeliveryMethod.Unreliable);
        }

        //public override void Stop() { }

        //protected override void network_error(IPEndPoint endPoint, SocketError socketError) { }

        //protected override void log_msg(LogLevel level, string msg) { }

        protected override void latency_update(NetPeer peer, int latency)
        {
            Log.LogVerbose("Latency Update : <{0}> {1}", peer.Id, latency);
            m_ConnectionManager.latency_update(peer, latency);
        }

        protected override void peer_disconnected_event(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.LogMsg("Peer disconnected : <{0}> {1}", peer.Id, System.Enum.GetName(typeof(DisconnectReason), disconnectInfo.Reason));
            //m_MapManager.player_left(m_ConnectionManager.GetConnection(peer));
            m_ConnectionManager.Remove(peer);
            players.Remove((PlayerAvatar)peer.Tag);
        }

        protected override void peer_connected_event(NetPeer peer)
        {
            Log.LogMsg("Peer connected : <{0}> {1}", peer.Id, peer.EndPoint.ToString());
        }

        protected override void connection_request_event(ConnectionRequest request)
        {
            var accountID = request.Data.GetULong();
            var characterID = request.Data.GetULong();

            m_WaitQueue.Add(new WaitingConnectionRequest(accountID, characterID, request));
            m_DBConnection.PollCharacterData(characterID, connection_request_callback);
        }

        /// <summary>
        /// Event callback coming from Database poll for Character data. 
        /// </summary>
        /// <param name="data"></param>
        protected void connection_request_callback(CharacterData data)
        {
            var response = m_WaitQueue.FirstOrDefault(p => p.CharacterID == data.CharacterID);
            if (response != null)
            {
                m_WaitQueue.Remove(response);
                var peer = response.ConnectionRequest.Accept();
                var newConnection = new GameConnection(peer, response.AccountID, response.CharacterID);
                m_ConnectionManager.Add(newConnection);
                //m_MapManager.player_join(newConnection, data.position, data.Rotation);

                var newPlayer = new PlayerAvatar(data);
                players.Add(newPlayer);
                peer.Tag = newPlayer;
            }
        }

        //protected override void receive_unconnected(System.Net.IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }

        protected override void receive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketType packetType = (PacketType) reader.GetByte();
            switch(packetType)
            {
                case PacketType.InputUpdate:
                    var packet = read_packet<InputUpdate>(reader); //MessagePackSerializer.Deserialize<InputUpdate>(reader.GetRemainingBytes());
                    input_update_event(peer, packet);
                    break;
                case PacketType.CharacterSelection:
                    //Just in case
                    break;
                default:
                    Log.LogWarning($"Unknown Packet Type received by Game Server <{packetType.ToString()}>");
                    break;
            }
        }

        /// <summary>
        /// Event callback when an input packet has been sent to the server from a client.
        /// </summary>
        protected virtual void input_update_event(NetPeer peer, InputUpdate packet)
        {
            var player = players.Find(p => p.Data.CharacterID == packet.ID);
            player.update_input(packet.Keys);
            m_ConnectionManager.set_recent_ack(peer, packet.ACK_Tick);
        }
    }
}
