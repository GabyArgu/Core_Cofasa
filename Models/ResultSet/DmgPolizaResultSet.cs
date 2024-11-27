using CoreContable.Entities;
using CoreContable.Entities.FuntionResult;
using CoreContable.Entities.Views;
using CoreContable.Utils;

namespace CoreContable.Models.ResultSet;

public class DmgPolizaResultSet
{
    public long RowNum { get; set; }
    public string COD_CIA { get; set; }
    public string PERIODO { get; set; }
    public string TIPO_DOCTO { get; set; }
    public Select2ResultSet selTIPO_DOCTO { get; set; }
    public int NUM_POLIZA { get; set; }
    public string? NUM_REFERENCIA { get; set; }
    public DateTime? FECHA { get; set; }
    public string ANIO { get; set; }
    public string MES { get; set; }
    public string? CONCEPTO { get; set; }
    public double? TOTAL_POLIZA { get; set; }
    public string STAT_POLIZA { get; set; }
    public Select2ResultSet selSTAT_POLIZA { get; set; }
    public DateTime? FECHA_CAMBIO { get; set; }
    public string? GRABACION_USUARIO { get; set; }
    public DateTime? GRABACION_FECHA { get; set; }
    public string? MODIFICACION_USUARIO { get; set; }
    public DateTime? MODIFICACION_FECHA { get; set; }
    public double? DiferenciaCargoAbono { get; set; }
    public string Asiento_Impreso { get; set; }
    public string NOMBRE_DOCTO { get; set; }

    // public DmgPolizaResultSet EntityToResultSet(DmgPoliza entity)
    // {
    //     return new DmgPolizaResultSet()
    //     {
    //         RowNum = entity.RowNum,
    //         COD_CIA = entity.COD_CIA,
    //         PERIODO = $"{entity.PERIODO}",
    //         TIPO_DOCTO = entity.TIPO_DOCTO,
    //         NUM_POLIZA = entity.NUM_POLIZA,
    //         NUM_REFERENCIA = entity.NUM_REFERENCIA,
    //         FECHA = entity.FECHA,
    //         ANIO = $"{entity.ANIO}",
    //         MES = $"{entity.MES}",
    //         CONCEPTO = entity.CONCEPTO,
    //         TOTAL_POLIZA = MoneyUtils.GetDefaultFormatAsDouble(entity.TOTAL_POLIZA),
    //         STAT_POLIZA = entity.STAT_POLIZA,
    //         FECHA_CAMBIO = entity.FECHA_CAMBIO,
    //         // GRABACION_USUARIO = entity.GRABACION_USUARIO,
    //         GRABACION_FECHA = entity.GRABACION_FECHA,
    //         // MODIFICACION_USUARIO = entity.MODIFICACION_USUARIO,
    //         // MODIFICACION_FECHA = entity.MODIFICACION_FECHA,
    //         DiferenciaCargoAbono = MoneyUtils.GetDefaultFormatAsDouble(entity.DiferenciaCargoAbono),
    //         Asiento_Impreso = entity.Asiento_Impreso
    //     };
    // }
    
    public static DmgPolizaResultSet ViewToResultSet(DmgPolizaView entity)
    {
        return new DmgPolizaResultSet()
        {
            RowNum = entity.RowNum,
            COD_CIA = entity.COD_CIA,
            PERIODO = $"{entity.PERIODO}",
            TIPO_DOCTO = entity.TIPO_DOCTO,
            NUM_POLIZA = entity.NUM_POLIZA,
            NUM_REFERENCIA = entity.NUM_REFERENCIA,
            FECHA = entity.FECHA,
            ANIO = $"{entity.ANIO}",
            MES = $"{entity.MES}",
            CONCEPTO = entity.CONCEPTO,
            TOTAL_POLIZA = MoneyUtils.GetDefaultFormatAsDouble(entity.TOTAL_POLIZA),
            STAT_POLIZA = entity.STAT_POLIZA,
            FECHA_CAMBIO = entity.FECHA_CAMBIO,
            // GRABACION_USUARIO = entity.GRABACION_USUARIO,
            GRABACION_FECHA = entity.GRABACION_FECHA,
            // MODIFICACION_USUARIO = entity.MODIFICACION_USUARIO,
            // MODIFICACION_FECHA = entity.MODIFICACION_FECHA,
            DiferenciaCargoAbono = MoneyUtils.GetDefaultFormatAsDouble(entity.DiferenciaCargoAbono),
            Asiento_Impreso = entity.Asiento_Impreso,
            NOMBRE_DOCTO = entity.NOMBRE_DOCTO
        };
    }

    public static DmgPolizaResultSet FuncToResultSet(ObtenerDatosDmgPolizaFromFunc entity)
    {
        return new DmgPolizaResultSet()
        {
            COD_CIA = entity.COD_CIA,
            PERIODO = $"{entity.PERIODO}",
            TIPO_DOCTO = entity.TIPO_DOCTO,
            NUM_POLIZA = entity.NUM_POLIZA,
            NUM_REFERENCIA = entity.NUM_REFERENCIA,
            FECHA = entity.FECHA,
            ANIO = $"{entity.ANIO}",
            MES = $"{entity.MES}",
            CONCEPTO = entity.CONCEPTO,
            TOTAL_POLIZA = MoneyUtils.GetDefaultFormatAsDouble(entity.TOTAL_POLIZA),
            STAT_POLIZA = entity.STAT_POLIZA,
            FECHA_CAMBIO = entity.FECHA_CAMBIO,
            // GRABACION_USUARIO = repositorio.GRABACION_USUARIO,
            GRABACION_FECHA = entity.GRABACION_FECHA,
            // MODIFICACION_USUARIO = repositorio.MODIFICACION_USUARIO,
            // MODIFICACION_FECHA = repositorio.MODIFICACION_FECHA,
            DiferenciaCargoAbono = MoneyUtils.GetDefaultFormatAsDouble(entity.DiferenciaCargoAbono),
            Asiento_Impreso = entity.Asiento_Impreso,
        };
    }
}