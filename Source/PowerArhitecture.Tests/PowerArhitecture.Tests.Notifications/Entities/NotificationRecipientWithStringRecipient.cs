﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Notifications.Entities;

namespace PowerArhitecture.Tests.Notifications.Entities
{
    public class NotificationRecipientWithStringRecipient
        : NotificationRecipient<string, NotificationWithStringRecipient, NotificationRecipientWithStringRecipient>
    {
    }
}
