﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.DAO.Models;

public partial class EmpresaLogo
{
    [Key]
    public int IdEmpresa { get; set; }

    [Required]
    [StringLength(255)]
    public string Nome { get; set; }

    [Required]
    [StringLength(10)]
    public string Extensao { get; set; }

    public int Tamanho { get; set; }

    [Required]
    [StringLength(255)]
    public string Tipo { get; set; }

    [Required]
    public string Base64 { get; set; }

    [ForeignKey("IdEmpresa")]
    [InverseProperty("EmpresaLogo")]
    public virtual Empresa IdEmpresaNavigation { get; set; }
}