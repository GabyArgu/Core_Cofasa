using CoreContable.Models.ResultSet;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IDmgDetalleRepository
{
    Task<List<DmgDetalleResultSet>> GetAllBy(
        string? codCia = null,
        int? periodo = null,
        string? tipoDocto = null,
        int? numPoliza = null
    );
}

public class DmgDetalleRepository(
    DbContext dbContext
) : IDmgDetalleRepository
{
    public Task<List<DmgDetalleResultSet>> GetAllBy(
        string? codCia = null,
        int? periodo = null,
        string? tipoDocto = null,
        int? numPoliza = null
    )
    {
        return dbContext.DmgDetalle
            .FromSqlRaw(
                "SELECT * FROM CONTABLE.ObtenerDetalleMayorizado({0}, {1}, {2}, {3})",
                codCia!=null ? codCia : DBNull.Value,
                periodo!=null ? periodo : DBNull.Value,
                tipoDocto!=null ? tipoDocto : DBNull.Value,
                numPoliza!=null ? numPoliza : DBNull.Value
            )
            .Select(detRepo => new DmgDetalleResultSet
            {
                COD_CIA = detRepo.COD_CIA,
                PERIODO = detRepo.PERIODO,
                TIPO_DOCTO = detRepo.TIPO_DOCTO,
                NUM_POLIZA = detRepo.NUM_POLIZA,
                Desc_CCosto = detRepo.Desc_CCosto,
                selCentroCosto = new Select2ResultSet
                {
                    id = detRepo.CENTRO_COSTO,
                    text = detRepo.Desc_CCosto
                },
                selCentroCuenta = new Select2ResultSet
                {
                    id = $"{detRepo.COD_CIA}|{detRepo.CENTRO_COSTO}|{detRepo.CTA_1}|{detRepo.CTA_2}|{detRepo.CTA_3}|{detRepo.CTA_4}|{detRepo.CTA_5}|{detRepo.CTA_6}",
                    text = $"{detRepo.CTA_1}{detRepo.CTA_2}{detRepo.CTA_3}{detRepo.CTA_4}{detRepo.CTA_5}{detRepo.CTA_6}"
                },
                CORRELAT = detRepo.CORRELAT,
                CTA_1 = detRepo.CTA_1,
                CTA_2 = detRepo.CTA_2,
                CTA_3 = detRepo.CTA_3,
                CTA_4 = detRepo.CTA_4,
                CTA_5 = detRepo.CTA_5,
                CTA_6 = detRepo.CTA_6,
                Desc_CContable = detRepo.Desc_CContable,
                CONCEPTO = detRepo.CONCEPTO,
                CARGO = detRepo.CARGO,
                ABONO = detRepo.ABONO
            })
            .ToListAsync();
    }
}