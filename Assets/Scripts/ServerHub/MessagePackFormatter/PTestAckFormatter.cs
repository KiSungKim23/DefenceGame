using MessagePack;
using MessagePack.Formatters;
using Vals;
using GameVals;

public sealed class PTestAckFormatter : IMessagePackFormatter<PTestAck>
{
    public void Serialize(ref MessagePackWriter writer, PTestAck value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
            return;
        }
        var formatterResolver = options.Resolver;
        writer.WriteArrayHeader(4);
        writer.Write(value.msgData);
        writer.Write(value.testmsg);
        writer.Write(value.testmsg2);
        writer.Write(value.testmsg3);
    }

    public PTestAck Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }
        options.Security.DepthStep(ref reader);
        var formatterResolver = options.Resolver;
        var length = reader.ReadArrayHeader();
        var result = new PTestAck();
        for (int i = 0; i < length; i++)
        {
            switch(i)
            {
                case 0:
                    result.msgData = reader.ReadString();
                    break;
                case 1:
                    result.testmsg = reader.ReadString();
                    break;
                case 2:
                    result.testmsg2 = reader.ReadString();
                    break;
                case 3:
                    result.testmsg3 = reader.ReadString();
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