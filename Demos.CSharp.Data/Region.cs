using System;
using System.Collections.Generic;

namespace Demos.CSharp.Data;

public partial class Region
{
    public int RegionID { get; set; }

    public string RegionDescription { get; set; } = null!;

    public virtual ICollection<Territory> Territories { get; set; } = new List<Territory>();
}
