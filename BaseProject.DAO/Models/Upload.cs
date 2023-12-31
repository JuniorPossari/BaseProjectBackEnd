﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.DAO.Models
{
    [Index("IdEmpresa", Name = "IX_Upload_IdEmpresa")]
    [Index("IdUsuario", Name = "IX_Upload_IdUsuario")]
    public partial class Upload
    {
        [Key]
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        [Required]
        [StringLength(32)]
        [Unicode(false)]
        public string MD5 { get; set; }
        public byte Tipo { get; set; }
        public byte Status { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int IdEmpresa { get; set; }

        [ForeignKey("IdEmpresa")]
        [InverseProperty("Upload")]
        public virtual Empresa IdEmpresaNavigation { get; set; }
        [ForeignKey("IdUsuario")]
        [InverseProperty("Upload")]
        public virtual Usuario IdUsuarioNavigation { get; set; }
        [InverseProperty("IdUploadNavigation")]
        public virtual UploadArquivo UploadArquivo { get; set; }
    }
}