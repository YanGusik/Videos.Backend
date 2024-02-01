using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videos.Domain
{
    public enum VideoStatus
    {
        Uploaded,
        Processing,
        Processed,
        Terminated,
        Rejected,
        Failed,
        Deleted,
    }
}
