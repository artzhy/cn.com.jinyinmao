﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Commands
{
    public class CommandExcuteResult
    {
        public string Message { get; set; }

        public bool Result { get; set; }
    }
}