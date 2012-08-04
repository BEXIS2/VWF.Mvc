﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vaiona.Model.Common
{
    public class EntityStateInfo
    {
        public virtual string State { get; set; }
        public virtual DateTime Timestamp { get; set; }
        public virtual string Comment { get; set; }
    }
}
