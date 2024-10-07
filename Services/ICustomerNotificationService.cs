using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAD.Services
{
    public interface ICustomerNotificationService
    {
        Task NotifyCustomerActivationAsync(string customerEmail);
    }
}