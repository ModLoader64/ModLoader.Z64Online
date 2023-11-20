
namespace Z64Online
{
    public class OoTOnlineStorageClient : OoTOnlineStorageBase
    {
        public string autoSaveHash = "!";
        public string keySaveHash = "!";
        public string lastPushHash = "!";

        public int world = 0;
        public int lastBeans = 0;
        public int lastKnownSkullCount = -1;

        public bool first_time_sync = false;

    }
}
