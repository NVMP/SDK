using Newtonsoft.Json;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NVMP.BuiltinPlugins
{
    internal class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly JsonSerializer Serializer;

        public NewtonsoftJsonSerializer()
        {
            Serializer = new JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }

        public string ContentType { get; set; } = "application/json";

        public string Serialize(object obj)
        {
            using var stringWriter = new StringWriter();
            using var jsonTextWriter = new JsonTextWriter(stringWriter);

            Serializer.Serialize(jsonTextWriter, obj);

            return stringWriter.ToString();
        }
    }
}
