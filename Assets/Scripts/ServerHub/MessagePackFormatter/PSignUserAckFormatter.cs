using MessagePack;
using MessagePack.Formatters;
using Vals;
using GameVals;

public sealed class PSignUserAckFormatter : IMessagePackFormatter<PSignUserAck>
{
    public void Serialize(ref MessagePackWriter writer, PSignUserAck value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
            return;
        }
        var formatterResolver = options.Resolver;
        writer.WriteArrayHeader(1);
        FormatterResolverExtensions.GetFormatterWithVerify<AccountInfo>(formatterResolver).Serialize(ref writer, value.accountInfo, options);
    }

    public PSignUserAck Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }
        options.Security.DepthStep(ref reader);
        var formatterResolver = options.Resolver;
        var length = reader.ReadArrayHeader();
        var result = new PSignUserAck();
        for (int i = 0; i < length; i++)
        {
            switch(i)
            {
                case 0:
                    result.accountInfo = FormatterResolverExtensions.GetFormatterWithVerify<AccountInfo>(formatterResolver).Deserialize(ref reader, options);
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