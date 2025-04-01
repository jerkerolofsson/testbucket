using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using TestBucket.Formats.Ctrf.Models;

namespace TestBucket.Formats.Ctrf.Xunit
{
    internal class AttachmentsConverter : JsonConverter<XunitAttachments>
    {
        public override XunitAttachments? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var attachments = new XunitAttachments();

            if(reader.TokenType == JsonTokenType.StartObject)
            {
                reader.Read();
                while (reader.TokenType != JsonTokenType.EndObject)
                {
                    if(reader.TokenType == JsonTokenType.PropertyName)
                    {
                        var name = reader.GetString();
                        if(name is not null)
                        {
                            reader.Read();
                            if (reader.TokenType == JsonTokenType.String)
                            {
                                reader = ReadTrait(reader, attachments, name);
                            }
                            else if(reader.TokenType == JsonTokenType.StartObject)
                            {
                                var attachment = new AttachmentDto() {  Name =  name };
                                attachments.Attachments.Add(attachment);
                                reader.Read();
                                while (reader.TokenType != JsonTokenType.EndObject)
                                {
                                    if (reader.TokenType == JsonTokenType.PropertyName)
                                    {
                                        var attachmentPropertyName = reader.GetString();
                                        reader.Read();
                                        if(reader.TokenType != JsonTokenType.String)
                                        {
                                            continue;
                                        }
                                        switch(attachmentPropertyName)
                                        {
                                            case "media-type":
                                            case "mediaType":
                                                attachment.ContentType = reader.GetString();
                                                break;
                                            case "value":
                                                attachment.Data = reader.GetBytesFromBase64();
                                                break;
                                        }

                                    }
                                    reader.Read();
                                }
                            }

                        }
                    }
                    reader.Read();
                }
            }

            return attachments;
        }

        private static Utf8JsonReader ReadTrait(Utf8JsonReader reader, XunitAttachments attachments, string name)
        {
            var value = reader.GetString();
            if (value is not null)
            {
                attachments.Traits.Add(new TestTrait(name, value));
            }

            return reader;
        }

        public override void Write(Utf8JsonWriter writer, XunitAttachments value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var trait in value.Traits)
            {
                writer.WritePropertyName(trait.Name ?? Guid.NewGuid().ToString());
                writer.WriteStringValue(trait.Value);
                writer.WriteEndObject();
            }
            foreach (var attachment in value.Attachments)
            {
                writer.WritePropertyName(attachment.Name ?? Guid.NewGuid().ToString());

                writer.WriteStartObject();
                writer.WritePropertyName("media-type");
                writer.WriteStringValue(attachment.ContentType);

                writer.WritePropertyName("value");
                writer.WriteBase64StringValue(attachment.Data);
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }
    }
}
