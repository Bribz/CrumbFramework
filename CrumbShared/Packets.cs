using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace CrumbShared
{
    public enum PacketType : byte
    {
        LoginRequest,
        LoginResponse,
        CharacterSelection,
        WorldState, 
        InputUpdate,
        SpawnEntity,

        NULL = 0xFF
    }
    
    [MessagePackObject]
    public class LoginRequest
    {
        [Key(0)]
        public string AccountHash;
        public string PasswordHash;
    }

    [MessagePackObject]
    public class LoginResponse
    {
        [Key(0)]
        public ulong AccountID;
        [Key(1)]
        public bool Response;
    }

    [MessagePackObject]
    public class CharacterSelection
    {
        [Key(0)]
        public ulong CharacterID;
    }

    [MessagePackObject]
    public class WorldState
    {
        [Key(0)]
        public ushort Tick;
        [Key(1)]
        public List<PlayerState> States;
    }

    [MessagePackObject]
    public class PlayerState
    {
        [Key(0)]
        public ulong CharacterID;
        [Key(1)]
        public Vector3f Position;
        [Key(2)]
        public float Rotation;
    }

    [Flags]
    public enum InputKeys : ushort
    {
        Left = 1<<1,
        Right = 1<<2,
        Up = 1<<3,
        Down = 1<<4,
        Interact = 1<<5,
        Primary = 1<<6,
        Secondary = 1<<7,
        Tertiary = 1<<8,
        Class = 1<<9,
        Guard = 1<<10,
        Jump = 1<<11
    }

    [MessagePackObject]
    public class InputUpdate
    {
        [Key(0)]
        public ulong ID;
        [Key(1)]
        public InputKeys Keys;
        [Key(2)]
        public ushort ACK_Tick;
    }

    [MessagePackObject]
    public class SpawnEntity
    {
        [Key(0)]
        public ushort ID;
        [Key(1)]
        public byte[] SpawnData;
    }
}