﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization
{
    public interface IAdminAction
    {
         void AddUserAccount();
         void DeleteOtherAccount(String userId);
    }
}
