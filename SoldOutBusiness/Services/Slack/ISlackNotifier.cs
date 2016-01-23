using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoldOutBusiness.Services.Slack
{
    public interface ISlackNotifier
    {
        void PostMessage(string message);
    }
}
