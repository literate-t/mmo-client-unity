using System;
using System.Collections.Generic;
using Google.Protobuf.MyProtocol;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new();

    public static GameObjectType GetObjectTypeById(int id)
    {
        return (GameObjectType)((id >> 24) & 0x7F);
    }

    public void Add(ObjectInfo info, bool isMyPlayer = false)
    {
        if (MyPlayer != null && MyPlayer.Id == info.ObjectId)
            return;
        if (Contains(info.ObjectId))
            return;

        GameObjectType type = GetObjectTypeById(info.ObjectId);

        if (type == GameObjectType.Player)
        {
            if (isMyPlayer)
            {
                GameObject go = Managers.Resource.Instantiate("Entity/MyPlayer");
                go.name = info.Name;
                Add(info.ObjectId, go);

                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer._positionInfo = info.PosInfo;
                MyPlayer.Stat.MergeFrom(info.StatInfo);
            }
            else
            {
                GameObject go = Managers.Resource.Instantiate("Entity/Player");
                go.name = info.Name;
                Add(info.ObjectId, go);

                PlayerController pc = go.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc._positionInfo = info.PosInfo;
                pc.Stat = info.StatInfo;
            }
        }
        else if (type == GameObjectType.Monster)
        {
            GameObject go = Managers.Resource.Instantiate(info.Name);
            go.name = info.Name;
            Add(info.ObjectId, go);

            MonsterController monster = go.GetComponent<MonsterController>();
            monster.Id = info.ObjectId;
            monster._positionInfo = info.PosInfo;
            monster.Stat = info.StatInfo;
        }
        else if (type == GameObjectType.Projectile)
        {
            GameObject go = Managers.Resource.Instantiate("Entity/Arrow");
            go.name = "Arrow";
            Add(info.ObjectId, go);

            ArrowController arrow = go.GetComponent<ArrowController>();
            arrow.PositionInfo = info.PosInfo;
            arrow.Stat = info.StatInfo;
        }
    }

    bool Contains(int objectId)
    {
        return _objects.ContainsKey(objectId);
    }

    public void Add(int id, GameObject obj)
    {
        _objects.TryAdd(id, obj);
    }

    public void Remove(int id)
    {
        if (MyPlayer != null && MyPlayer.Id == id)
            return;
        if (Contains(id) == false)
            return;

        GameObject go = FindById(id);
        _objects.Remove(id);
        Managers.Resource.Destroy(go);
    }

    public GameObject FindCreature(Vector3Int cellPosition)
    {
        // O(N)
        foreach (GameObject obj in _objects.Values)
        {
            CreatureController creature = obj.GetComponent<CreatureController>();
            if (creature == null)
                continue;

            if (creature.CellPosition == cellPosition)
                return obj;
        }

        return null;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (GameObject obj in _objects.Values)
        {
            if (condition(obj))
                return obj;
        }

        return null;
    }

    internal GameObject FindById(int playerId)
    {
        GameObject go = null;
        _objects.TryGetValue(playerId, out go);

        return go;
    }

    public void Clear()
    {
        foreach (GameObject obj in _objects.Values)
            Managers.Resource.Destroy(obj);
        _objects.Clear();
        MyPlayer = null;
    }
}
