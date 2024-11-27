using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreContable.Entities.Views {

    public class CentroCuentaFormatoView {
        [Key]
        [Column("COD_CIA")]
        [MaxLength(3)]
        public string COD_CIA { get; set; }

        [Column("CENTRO_COSTO")]
        [MaxLength(25)]
        public string CENTRO_COSTO { get; set; }

        [Column("CTA_1")]
        public int CTA_1 { get; set; }

        [Column("CTA_2")]
        public int CTA_2 { get; set; }

        [Column("CTA_3")]
        public int CTA_3 { get; set; }

        [Column("CTA_4")]
        public int CTA_4 { get; set; }

        [Column("CTA_5")]
        public int CTA_5 { get; set; }

        [Column("CTA_6")]
        public int CTA_6 { get; set; }

        [Column("ESTADO")]
        [MaxLength(1)]
        public string ESTADO { get; set; }

        [Column("USUARIO_CREACION")]
        [MaxLength(50)]
        public string? USUARIO_CREACION { get; set; }

        [Column("FECHA_CREACION")]
        public DateTime? FECHA_CREACION { get; set; }

        [Column("USUARIO_MODIFICACION")]
        [MaxLength(50)]
        public string? USUARIO_MODIFICACION { get; set; }

        [Column("FECHA_MODIFICACION")]
        public DateTime? FECHA_MODIFICACION { get; set; }
    }
}
