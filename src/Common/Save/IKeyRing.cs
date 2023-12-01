using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z64Online
{
    public interface IKeyRing
    {
        byte[] keys { get; set; }
    }
}
