using GameVals;
using MessagePack;
using MessagePack.Formatters;


public sealed class MonsterCheckDataFormatter : IMessagePackFormatter<MonsterCheckData>
{
    public void Serialize(ref MessagePackWriter writer, MonsterCheckData value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
            return;
        }
        var formatterResolver = options.Resolver;
        writer.WriteArrayHeader(4);
        writer.Write(value.objectID);
        writer.Write(value.nowSectionIndexX);
        writer.Write(value.nowSectionIndexY);
        writer.Write(value.currentHP);
    }

    public MonsterCheckData Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }
        options.Security.DepthStep(ref reader);
        var formatterResolver = options.Resolver;
        var length = reader.ReadArrayHeader();
        var result = new MonsterCheckData();
        for (int i = 0; i < length; i++)
        {
            switch (i)
            {
                case 0:
                    result.objectID = reader.ReadInt32();
                    break;
                case 1:
                    result.nowSectionIndexX = reader.ReadInt32();
                    break;
                case 2:
                    result.nowSectionIndexY = reader.ReadInt32();
                    break;
                case 3:
                    result.currentHP = reader.ReadInt64();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }
        reader.Depth--;
        return result;
    }
}