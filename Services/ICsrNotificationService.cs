using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAD.Services
{
    public interface ICsrNotificationService
    {
        Task NotifyCsrsAboutNewCustomerAsync(string customerEmail, List<string> csrEmails);

    }
}