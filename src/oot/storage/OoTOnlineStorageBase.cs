using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z64Online.OoTOnline
{
    public class OoTOnlineStorageBase
    {
        public OoTOSaveData saveManager;
        public OoTOnlineStorageBase(OoTOSaveData saveManger) {
            this.saveManager = saveManger;
        }
    }
}
