using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_for_ChatApp.Interfaces.RequestInterfaces
{
    internal interface IRequestUserID : IRequest
    {

        byte GetUserID();

    }
}
