using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreBusiness.Interface
{
    public interface IServerStatus
    {
        string GetDescription();
        string GetIP();
        string GetPort();
        string GetClientRequirement();
        string GetOnlineUsers();
        string GetTotalMobiles();
        string GetTotalItens();
        DateTime GetServerUptime();
        DateTime GetLastInfoUpdated();
    }
}
