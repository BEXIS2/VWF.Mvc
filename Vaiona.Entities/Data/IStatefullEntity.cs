﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vaiona.Entities.Data
{
    public interface IStatefullEntity
    {
        EntityStateInfo StateInfo { get; set; }
    }
}
