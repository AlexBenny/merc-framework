using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using it.mintlab.desktopnet.tupleengine;

namespace it.mintlab.desktopnet.mercframework
{
    public static class JsonLib
    {
        public static Message Json2MercMessage(string JsonString)
        {
            string sender = null;
            string recipient = null;
            string content = null;

            JsonReader reader = new JsonTextReader(new StringReader(JsonString));
            while (reader.Read())
            {
                if (reader.TokenType.ToString().Equals("PropertyName") && reader.Value.Equals("sender"))
                {
                    reader.Read();
                    sender = reader.Value.ToString();
                }
                if (reader.TokenType.ToString().Equals("PropertyName") && reader.Value.Equals("recipient"))
                {
                    reader.Read();
                    recipient = reader.Value.ToString();
                }
                if (reader.TokenType.ToString().Equals("PropertyName") && reader.Value.Equals("content"))
                {
                    reader.Read();
                    content = reader.Value.ToString();
                }
            }

            return new Message(sender, recipient, new MessageContent(MessageContent.Category.COMMAND, TupleEngine.parse(content)));
        }

        public static string MercMessage2Json(Message mercMessage)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            JsonWriter jsonWriter = new JsonTextWriter(sw);
            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("sender");
            jsonWriter.WriteValue(mercMessage.getSender());
            jsonWriter.WritePropertyName("recipient");
            jsonWriter.WriteValue(mercMessage.getRecipient());
            jsonWriter.WritePropertyName("content");
            jsonWriter.WriteValue(mercMessage.getContent().getTuple().ToString());
            jsonWriter.WriteEndObject();
            jsonWriter.Flush();

            return sw.GetStringBuilder().ToString();
        }

        public static string JsonOkResponse() 
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            JsonWriter jsonWriter = new JsonTextWriter(sw);
            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("sendingResult");
            jsonWriter.WriteValue(true);
            jsonWriter.WriteEndObject();
            jsonWriter.Flush();

            return sw.GetStringBuilder().ToString();
        }

        public static string JsonKoResponse()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            JsonWriter jsonWriter = new JsonTextWriter(sw);
            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("sendingResult");
            jsonWriter.WriteValue(false);
            jsonWriter.WriteEndObject();
            jsonWriter.Flush();

            return sw.GetStringBuilder().ToString();
        }

        public static bool wasDelivered(String jsonResult)
        {
            if (jsonResult.Contains("sendingResult"))
                if (jsonResult.Contains("true"))
                    return true;
            return false;
        }
    }
}
