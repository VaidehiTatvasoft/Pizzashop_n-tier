﻿using System;
using System.Collections.Generic;

namespace Entity.Data;

public partial class Payment
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public int? PaymentMethod { get; set; }

    public int? Status { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual User? ModifiedByNavigation { get; set; }
}
