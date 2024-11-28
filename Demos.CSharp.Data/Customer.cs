using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Demos.CSharp.Data;

public partial class Customer
{
    [Display(Name = "Identificador")]
    [Required(ErrorMessage = "El campo identificador es requerido.")]
    public string CustomerID { get; set; } = null!;

    [Required(ErrorMessage = "El campo Empresa es requerido.")]
    [Display(Name = "Empresa")]
    public string CompanyName { get; set; } = null!;

    [Display(Name = "Contacto")]
    public string? ContactName { get; set; }

    [Display(Name = "Cargo")]
    public string? ContactTitle { get; set; }

    [Display(Name = "Dirección")]
    public string? Address { get; set; }

    [Display(Name = "Ciudad")]
    public string? City { get; set; }

    public string? Region { get; set; }

    [Display(Name = "Código Postal")]
    public string? PostalCode { get; set; }

    [Display(Name = "País")]
    public string? Country { get; set; }
    
    [Display(Name = "Teléfono")]
    public string? Phone { get; set; }

    public string? Fax { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<CustomerDemographic> CustomerTypes { get; set; } = new List<CustomerDemographic>();
}
