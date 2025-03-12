using System;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.MyProtocol;
using ServerCore;

public class ServerPacketManager
{
    public Action<ushort, IMessage> PacketCallback { get; set; }
    static ServerPacketManager _instance;
    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecvPacket = new();
    Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new();

    ServerPacketManager()
    {
        Register();
    }

    public void Register()
    {
       _onRecvPacket.Add((ushort)MessageId.SEnterGame, ParsePacket<S_EnterGame>);
       _handler.Add((ushort)MessageId.SEnterGame, PacketHandler.S_EnterGameHandler);
       _onRecvPacket.Add((ushort)MessageId.SLeaveGame, ParsePacket<S_LeaveGame>);
       _handler.Add((ushort)MessageId.SLeaveGame, PacketHandler.S_LeaveGameHandler);
       _onRecvPacket.Add((ushort)MessageId.SSpawn, ParsePacket<S_Spawn>);
       _handler.Add((ushort)MessageId.SSpawn, PacketHandler.S_SpawnHandler);
       _onRecvPacket.Add((ushort)MessageId.SDespawn, ParsePacket<S_Despawn>);
       _handler.Add((ushort)MessageId.SDespawn, PacketHandler.S_DespawnHandler);
       _onRecvPacket.Add((ushort)MessageId.SMove, ParsePacket<S_Move>);
       _handler.Add((ushort)MessageId.SMove, PacketHandler.S_MoveHandler);
       _onRecvPacket.Add((ushort)MessageId.SSkill, ParsePacket<S_Skill>);
       _handler.Add((ushort)MessageId.SSkill, PacketHandler.S_SkillHandler);
       _onRecvPacket.Add((ushort)MessageId.SChangeHp, ParsePacket<S_ChangeHp>);
       _handler.Add((ushort)MessageId.SChangeHp, PacketHandler.S_ChangeHpHandler);
       _onRecvPacket.Add((ushort)MessageId.SDie, ParsePacket<S_Die>);
       _handler.Add((ushort)MessageId.SDie, PacketHandler.S_DieHandler);
       _onRecvPacket.Add((ushort)MessageId.SConnected, ParsePacket<S_Connected>);
       _handler.Add((ushort)MessageId.SConnected, PacketHandler.S_ConnectedHandler);
       _onRecvPacket.Add((ushort)MessageId.SLogin, ParsePacket<S_Login>);
       _handler.Add((ushort)MessageId.SLogin, PacketHandler.S_LoginHandler);
       _onRecvPacket.Add((ushort)MessageId.SCreatePlayer, ParsePacket<S_CreatePlayer>);
       _handler.Add((ushort)MessageId.SCreatePlayer, PacketHandler.S_CreatePlayerHandler);
       _onRecvPacket.Add((ushort)MessageId.SItemList, ParsePacket<S_ItemList>);
       _handler.Add((ushort)MessageId.SItemList, PacketHandler.S_ItemListHandler);
       _onRecvPacket.Add((ushort)MessageId.SAddItem, ParsePacket<S_AddItem>);
       _handler.Add((ushort)MessageId.SAddItem, PacketHandler.S_AddItemHandler);
       _onRecvPacket.Add((ushort)MessageId.SEquipItem, ParsePacket<S_EquipItem>);
       _handler.Add((ushort)MessageId.SEquipItem, PacketHandler.S_EquipItemHandler);
       _onRecvPacket.Add((ushort)MessageId.SChangeStat, ParsePacket<S_ChangeStat>);
       _handler.Add((ushort)MessageId.SChangeStat, PacketHandler.S_ChangeStatHandler);
       _onRecvPacket.Add((ushort)MessageId.SPing, ParsePacket<S_Ping>);
       _handler.Add((ushort)MessageId.SPing, PacketHandler.S_PingHandler);

    }

    public static ServerPacketManager Instance
    {
        get
        {
            if (null == _instance)
                _instance = new ServerPacketManager();
            return _instance;
        }
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        int dataSize = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        dataSize += 2;
        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + dataSize);
        dataSize += 2;

       	Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecvPacket.TryGetValue(packetId, out action))
			action.Invoke(session, buffer, packetId);
    }

    void ParsePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
        if (PacketCallback != null)
        {
            PacketCallback(id, pkt);
        }
        else
        {
            Action<PacketSession, IMessage> action = null;
		    if (_handler.TryGetValue(id, out action))
			    action.Invoke(session, pkt);
        }
    }

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}