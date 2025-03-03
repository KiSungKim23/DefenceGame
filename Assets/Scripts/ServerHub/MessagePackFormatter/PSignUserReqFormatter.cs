using MessagePack;
using MessagePack.Formatters;
using Vals;
using GameVals;

public sealed class PSignUserReqFormatter : IMessagePackFormatter<PSignUserReq>
{
    public void Serialize(ref MessagePackWriter writer, PSignUserReq value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
            return;
        }
        var formatterResolver = options.Resolver;
        writer.WriteArrayHeader(2);
        FormatterResolverExtensions.GetFormatterWithVerify<Define.AuthType>(formatterResolver).Serialize(ref writer, value.authType, options);
        writer.Write(value.userGUID);
    }

    public PSignUserReq Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }
        options.Security.DepthStep(ref reader);
        var formatterResolver = options.Resolver;
        var length = reader.ReadArrayHeader();
        var result = new PSignUserReq();
        for (int i = 0; i < length; i++)
        {
            switch(i)
            {
                case 0:
                    result.authType = FormatterResolverExtensions.GetFormatterWithVerify<Define.AuthType>(formatterResolver).Deserialize(ref reader, options);
                    break;
                case 1:
                    result.userGUID = reader.ReadString();
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