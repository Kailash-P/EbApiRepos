﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class EmailParameters
    {
        public int ID { get; set; }
        public string consumerName { get; set; }
        public string fromEmailAddress { get; set; }
        public string toEmailAddress { get; set; }
        public string resolvedMessage { get; set; }
    }
}
