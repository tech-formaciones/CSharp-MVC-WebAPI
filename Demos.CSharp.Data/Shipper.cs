﻿using System;
using System.Collections.Generic;

namespace Demos.CSharp.Data;

public partial class Shipper
{
    public int ShipperID { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? Phone { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
