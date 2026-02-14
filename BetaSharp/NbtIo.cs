using BetaSharp.NBT;
using java.io;
using java.util.zip;
using Console = System.Console;

namespace BetaSharp;

public static class NbtIo
{
    public static void Write(NBTTagCompound tag, DataOutput output)
    {
        NBTBase.WriteTag(tag, output);
    }

    public static void WriteCompressed(NBTTagCompound tag, OutputStream output)
    {
        var stream = new DataOutputStream(new GZIPOutputStream(output));
        Write(tag, stream);
    }

    public static NBTTagCompound ReadCompressed(InputStream input)
    {
        var stream = new DataInputStream(new GZIPInputStream(input));
        return Read(stream);
    }

    public static NBTTagCompound Read(DataInput input)
    {
        var tag = NBTBase.ReadTag(input);

        if (tag is NBTTagCompound compound)
        {
            return compound;
        }

        throw new InvalidOperationException("Root tag must be a named compound tag");
    }
}