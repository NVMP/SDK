using System;
using System.Runtime.Serialization;

namespace NVMP.BuiltinServices
{
    [DataContract]
    internal class NativeGameServer
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "ip")]
        public string IP { get; set; }

        [DataMember(Name = "port")]
        public ushort Port { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "tag")]
        public string Tag { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "mods_download_url")]
        public string ModsDownloadURL { get; set; }

        [DataMember(Name = "max_players")]
        public int MaxPlayers { get; set; }

        [DataMember(Name = "num_players")]
        public int NumPlayers { get; set; }

        [DataMember(Name = "last_ping")]
        public DateTimeOffset LastPing { get; set; }

        [DataMember(Name = "region")]
        public string Region { get; set; } = null;

        // This is a private token used to authenticate updates
        [DataMember(Name = "secure_token")]
        public string SecureToken { get; set; }

        [DataMember(Name = "reserved_slots")]
        public uint ReservedSlots { get; set; }

        [DataMember(Name = "mods")]
        public ServerReporterServiceImpl.ServerModInfo[] Mods { get; set; }
    }

}
