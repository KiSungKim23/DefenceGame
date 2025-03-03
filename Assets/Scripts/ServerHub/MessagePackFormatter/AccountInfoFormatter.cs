using GameVals;
using MessagePack;
using MessagePack.Formatters;


public sealed class AccountInfoFormatter : IMessagePackFormatter<AccountInfo>
{
    public void Serialize(ref MessagePackWriter writer, AccountInfo value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
            return;
        }
        var formatterResolver = options.Resolver;
        writer.WriteArrayHeader(4);
        writer.Write(value.AccountID);
        writer.Write(value.NickName);
        FormatterResolverExtensions.GetFormatterWithVerify<System.DateTime>(formatterResolver).Serialize(ref writer, value.LogoutTime, options);
        writer.Write(value.clearCount);
    }

    public AccountInfo Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }
        options.Security.DepthStep(ref reader);
        var formatterResolver = options.Resolver;
        var length = reader.ReadArrayHeader();
        var result = new AccountInfo();
        for (int i = 0; i < length; i++)
        {
            switch (i)
            {
                case 0:
                    result.AccountID = reader.ReadInt64();
                    break;
                case 1:
                    result.NickName = reader.ReadString();
                    break;
                case 2:
                    result.LogoutTime = FormatterResolverExtensions.GetFormatterWithVerify<System.DateTime>(formatterResolver).Deserialize(ref reader, options);
                    break;
                case 3:
                    result.clearCount = reader.ReadInt32();
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