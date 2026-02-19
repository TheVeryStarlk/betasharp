using BetaSharp.Items;
using BetaSharp.Network.Packets;
using BetaSharp.Util.Maths;
using java.io;
using java.lang;
using java.util;

namespace BetaSharp;

public class DataWatcher
{
    private static readonly Dictionary<Type, int> dataTypes = [];

    private readonly Dictionary<int, WatchableObject> watchedObjects = new();
    public bool dirty { get; private set; }

    public void addObject(int id, java.lang.Object value)
    {
        if (!dataTypes.TryGetValue(value.GetType(), out int typeId))
        {
            throw new ArgumentException("Unknown data type: " + value.GetType());
        }

        if (id > 31)
        {
            throw new ArgumentException("Data value id is too big with " + id + "! (Max is " + 31 + ")");
        }

        if (watchedObjects.ContainsKey(id))
        {
            throw new ArgumentException("Duplicate id value for " + id + "!");
        }

        watchedObjects[id] = new WatchableObject(typeId, id, value);
    }

    public List getDirtyEntries()
    {
        List res = null;
        if (dirty)
        {
            foreach (var obj in watchedObjects.Values)
            {
                if (obj.dirty)
                {
                    if (res == null) res = new ArrayList();

                    obj.dirty = false;
                    res.add(obj);
                }
            }
        }

        dirty = false;
        return res;
    }

    public sbyte getWatchableObjectByte(int id)
    {
        return (sbyte)((java.lang.Byte)watchedObjects[id].watchedObject).byteValue();
    }

    public int getWatchableObjectInt(int id)
    {
        return ((Integer)watchedObjects[id].watchedObject).intValue();
    }

    public string getWatchableObjectString(int id)
    {
        return ((JString)watchedObjects[id].watchedObject).value;
    }

    public void updateObject(int id, java.lang.Object value)
    {
        WatchableObject obj = watchedObjects[id];
        if (!value.Equals(obj.watchedObject))
        {
            obj.watchedObject = value;
            obj.dirty = true;
            dirty = true;
        }
    }

    public static void writeObjectsInListToStream(List list, Stream stream)
    {
        if (list != null)
        {
            Iterator it = list.iterator();
            while (it.hasNext())
            {
                var obj = (WatchableObject)it.next();
                writeWatchableObject(stream, obj);
            }
        }

        stream.WriteByte(127);
    }

    public void writeWatchableObjects(Stream stream)
    {
        foreach (var obj in watchedObjects.Values)
        {
            writeWatchableObject(stream, obj);
        }

        stream.WriteByte(127);
    }

    private static void writeWatchableObject(Stream stream, WatchableObject obj)
    {
        int header = (obj.objectType << 5 | obj.dataValueId & 31) & 255;
        stream.WriteByte((byte)header);
        switch (obj.objectType)
        {
            case 0:
                stream.WriteByte(((java.lang.Byte)obj.watchedObject).byteValue());
                break;
            case 1:
                stream.WriteShort(((Short)obj.watchedObject).shortValue());
                break;
            case 2:
                stream.WriteInt(((Integer)obj.watchedObject).intValue());
                break;
            case 3:
                stream.WriteFloat(((Float)obj.watchedObject).floatValue());
                break;
            case 4:
                stream.WriteString(((JString)obj.watchedObject).value);
                break;
            case 5:
                ItemStack item = (ItemStack)obj.watchedObject;
                stream.WriteShort((short)item.getItem().id);
                stream.WriteByte((byte)item.count);
                stream.WriteShort((short)item.getDamage());
                break;
            case 6:
                Vec3i vec = (Vec3i)obj.watchedObject;
                stream.WriteInt(vec.x);
                stream.WriteInt(vec.y);
                stream.WriteInt(vec.z);
                break;
        }
    }

    public static List readWatchableObjects(Stream stream)
    {
        ArrayList res = null;

        for (sbyte b = (sbyte)stream.ReadByte(); b != 127; b = (sbyte)stream.ReadByte())
        {
            res ??= [];

            int objectType = (b & 224) >> 5;
            int dataValueId = b & 31;
            WatchableObject obj = null;
            switch (objectType)
            {
                case 0:
                    obj = new WatchableObject(objectType, dataValueId, java.lang.Byte.valueOf((byte)stream.ReadByte()));
                    break;
                case 1:
                    obj = new WatchableObject(objectType, dataValueId, Short.valueOf(stream.ReadShort()));
                    break;
                case 2:
                    obj = new WatchableObject(objectType, dataValueId, Integer.valueOf(stream.ReadInt()));
                    break;
                case 3:
                    obj = new WatchableObject(objectType, dataValueId, Float.valueOf(stream.ReadFloat()));
                    break;
                case 4:
                    obj = new WatchableObject(objectType, dataValueId, new JString(stream.ReadString(64)));
                    break;
                case 5:
                    short id = stream.ReadShort();
                    sbyte count = (sbyte)stream.ReadByte();
                    short damage = stream.ReadShort();
                    obj = new WatchableObject(objectType, dataValueId, new ItemStack(id, count, damage));
                    break;
                case 6:
                    int x = stream.ReadInt();
                    int y = stream.ReadInt();
                    int z = stream.ReadInt();
                    obj = new WatchableObject(objectType, dataValueId, new Vec3i(x, y, z));
                    break;
            }

            res.add(obj);
        }

        return res;
    }

    public void updateWatchedObjectsFromList(List list)
    {
        Iterator it = list.iterator();

        while (it.hasNext())
        {
            WatchableObject obj = (WatchableObject)it.next();
            if (watchedObjects.TryGetValue(obj.dataValueId, out var obj2))
            {
                obj2.watchedObject = obj.watchedObject;
            }
        }
    }

    static DataWatcher()
    {
        dataTypes[typeof(java.lang.Byte)] =  0;
        dataTypes[typeof(Short)] =  1;
        dataTypes[typeof(Integer)] =  2;
        dataTypes[typeof(Float)] =  3;
        dataTypes[typeof(JString)] =  4;
        dataTypes[typeof(ItemStack)] =  5;
        dataTypes[typeof(Vec3i)] =  6;
    }
}
