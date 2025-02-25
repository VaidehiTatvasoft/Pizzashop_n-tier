﻿using System;
using System.Collections.Generic;

namespace Entity.Data;

public partial class City
{
    public int Id { get; set; }

    public int StateId { get; set; }

    public string Name { get; set; } = null!;

    public virtual State State { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
