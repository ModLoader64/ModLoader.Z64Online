
namespace ModLoader.Z64Online
{
    internal class OoTOnlineStorageClient
    {
        string autoSaveHash = "!";
        string keySaveHash = "!";
        string lastPushHash = "!";

        int world = 0;
        int lastBeans = 0;
        int lastKnownSkullCount = -1;

        bool first_time_sync = false;

    }
}
