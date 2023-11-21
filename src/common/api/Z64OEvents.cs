using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z64Online
{
    public class ClientPlayerChangedScenes : IEvent
    {
        public string Id { get; set; } = "ClientRemotePlayerChangedScenes";
        public NetworkPlayer player;
        public u16 scene;

        public ClientPlayerChangedScenes(NetworkPlayer player, ushort scene)
        {
            this.player = player;
            this.scene = scene;
        }
    }

    public class ServerPlayerChangedScenes : IEvent
    {
        public string Id { get; set; } = "ServerPlayerChangedScenes";
        public NetworkPlayer player;
        public u16 scene;

        public ServerPlayerChangedScenes(NetworkPlayer player, ushort scene)
        {
            this.player = player;
            this.scene = scene;
        }
    }
}
