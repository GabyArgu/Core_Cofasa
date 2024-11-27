using System.Data;
using CoreContable.Entities;
using CoreContable.Models;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Services;

public interface ICiasRepository
{
    Task<List<CiaResultSet>> GetCias();

    Task<Cias?> GetCiaById(string cod);

    Task<List<CiaResultSet>> GetUserCias(int userId);

    Task<List<CiaResultSet>> CallGetCias(string? filter);

    Task<List<Select2ResultSet>> CallGetCiasForSelect2(string query);

    Task<bool> CallSaveCia(CiaDto cia);

    Task<bool> CallUpdateCia(CiaDto cia);

    Task<string> CallGenerateCiaCod();

    Task<CiaResultSet?> GetOneCia(string ciaCod);

    Task<int> GetCount();
}

public class CiasRepository(
    DbContext dbContext,
    ILogger<CiasRepository> logger
) : ICiasRepository
{
    public Task<List<CiaResultSet>> GetCias() =>
        dbContext.Cias.Select(cia => new CiaResultSet
        {
            Cod = cia.CodCia,
            RazonSocial = cia.RazonSocial ?? "",
            NomComercial = cia.NomComercial ?? "",
            DirecEmpresa = cia.DirecEmpresa ?? "",
            TelefEmpresa = cia.TelefEmpresa ?? "",
            NitEmpresa = cia.NitEmpresa ?? "",
            NumeroPatronal = cia.NumeroPatronal ?? "",
        }).ToListAsync();

    public Task<Cias?> GetCiaById(string cod) =>
        dbContext.Cias
            .Where(cia => cia.CodCia == cod)
            .FirstOrDefaultAsync();

    public Task<List<CiaResultSet>> GetUserCias(int userId) =>
        dbContext.Cias
            .Include(c => c.UserCia)
            .Where(c => c.UserCia.Any(uc => uc.UserAppId == userId))
            .Select(cia => new CiaResultSet
            {
                Cod = cia.CodCia,
                RazonSocial = cia.RazonSocial ?? "",
                NomComercial = cia.NomComercial ?? "",
                DirecEmpresa = cia.DirecEmpresa ?? "",
                TelefEmpresa = cia.TelefEmpresa ?? "",
                NitEmpresa = cia.NitEmpresa ?? "",
                NumeroPatronal = cia.NumeroPatronal ?? "",
            })
            .ToListAsync();

    public Task<List<CiaResultSet>> CallGetCias(string? filter = "0") => dbContext.Cias
        .FromSqlRaw("SELECT * FROM CATALANA.Obtener_Empresas({0})", filter)
        .Select(cia => new CiaResultSet
        {
            Cod = cia.CodCia,
            RazonSocial = cia.RazonSocial ?? "",
            NomComercial = cia.NomComercial ?? "",
            DirecEmpresa = cia.DirecEmpresa ?? "",
            TelefEmpresa = cia.TelefEmpresa ?? "",
            NitEmpresa = cia.NitEmpresa ?? "",
            NumeroPatronal = cia.NumeroPatronal ?? "",
        })
        .ToListAsync();

    public Task<List<Select2ResultSet>> CallGetCiasForSelect2(string query)
    {
        IQueryable<Cias> efQuery;

        if (query.IsNullOrEmpty())
        {
            efQuery = dbContext.Cias;
        }
        else
        {
            efQuery = dbContext.Cias
                .Where(cia => EF.Functions.Like(cia.NomComercial, $"%{query}%")
                              || EF.Functions.Like(cia.RazonSocial, $"%{query}%"));
        }

        return efQuery
            .Select(cia => new Select2ResultSet
            {
                id = cia.CodCia,
                text = cia.NomComercial ?? ""
            })
            .ToListAsync();
    }

    public async Task<bool> CallSaveCia(CiaDto cia)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.Inserta_Empresa";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@COD_CIA", SqlDbType.VarChar) { Value = cia.COD_CIA });
            command.Parameters.Add(new SqlParameter("@RAZON_SOCIAL", SqlDbType.VarChar) { Value = cia.RAZON_SOCIAL==null ? DBNull.Value : cia.RAZON_SOCIAL });
            command.Parameters.Add(new SqlParameter("@NOM_COMERCIAL", SqlDbType.VarChar) { Value = cia.NOM_COMERCIAL==null ? DBNull.Value : cia.NOM_COMERCIAL });
            command.Parameters.Add(new SqlParameter("@DIREC_EMPRESA", SqlDbType.VarChar) { Value = cia.DIREC_EMPRESA==null ? DBNull.Value : cia.DIREC_EMPRESA });
            command.Parameters.Add(new SqlParameter("@TELEF_EMPRESA", SqlDbType.VarChar) { Value = cia.TELEF_EMPRESA==null ? DBNull.Value : cia.TELEF_EMPRESA });
            command.Parameters.Add(new SqlParameter("@NIT_EMPRESA", SqlDbType.VarChar) { Value = cia.NIT_EMPRESA==null ? DBNull.Value : cia.NIT_EMPRESA });
            command.Parameters.Add(new SqlParameter("@NUMERO_PATRONAL", SqlDbType.VarChar) { Value = cia.NUMERO_PATRONAL==null ? DBNull.Value : cia.NUMERO_PATRONAL });
            command.Parameters.Add(new SqlParameter("@MES_CIERRE", SqlDbType.Int) { Value = cia.MES_CIERRE==null ? DBNull.Value : cia.MES_CIERRE });
            command.Parameters.Add(new SqlParameter("@MES_PROCESO", SqlDbType.Int) { Value = cia.MES_PROCESO==null ? DBNull.Value : cia.MES_PROCESO });
            command.Parameters.Add(new SqlParameter("@PERIODO", SqlDbType.Int) { Value = cia.PERIODO==null ? DBNull.Value : cia.PERIODO });
            command.Parameters.Add(new SqlParameter("@CTA_1RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_1RESUL_ACT==null ? DBNull.Value : cia.CTA_1RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_2RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_2RESUL_ACT==null ? DBNull.Value : cia.CTA_2RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_3RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_3RESUL_ACT==null ? DBNull.Value : cia.CTA_3RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_4RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_4RESUL_ACT==null ? DBNull.Value : cia.CTA_4RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_5RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_5RESUL_ACT==null ? DBNull.Value : cia.CTA_5RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_6RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_6RESUL_ACT==null ? DBNull.Value : cia.CTA_6RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_1RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_1RESUL_ANT==null ? DBNull.Value : cia.CTA_1RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_2RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_2RESUL_ANT==null ? DBNull.Value : cia.CTA_2RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_3RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_3RESUL_ANT==null ? DBNull.Value : cia.CTA_3RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_4RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_4RESUL_ANT==null ? DBNull.Value : cia.CTA_4RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_5RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_5RESUL_ANT==null ? DBNull.Value : cia.CTA_5RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_6RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_6RESUL_ANT==null ? DBNull.Value : cia.CTA_6RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_1PER_GAN", SqlDbType.Int) { Value = cia.CTA_1PER_GAN==null ? DBNull.Value : cia.CTA_1PER_GAN });
            command.Parameters.Add(new SqlParameter("@CTA_2PER_GAN", SqlDbType.Int) { Value = cia.CTA_2PER_GAN==null ? DBNull.Value : cia.CTA_2PER_GAN });
            command.Parameters.Add(new SqlParameter("@CTA_3PER_GAN", SqlDbType.Int) { Value = cia.CTA_3PER_GAN==null ? DBNull.Value : cia.CTA_3PER_GAN });
            command.Parameters.Add(new SqlParameter("@CTA_4PER_GAN", SqlDbType.Int) { Value = cia.CTA_4PER_GAN==null ? DBNull.Value : cia.CTA_4PER_GAN });
            command.Parameters.Add(new SqlParameter("@CTA_5PER_GAN", SqlDbType.Int) { Value = cia.CTA_5PER_GAN==null ? DBNull.Value : cia.CTA_5PER_GAN });
            command.Parameters.Add(new SqlParameter("@CTA_6PER_GAN", SqlDbType.Int) { Value = cia.CTA_6PER_GAN==null ? DBNull.Value : cia.CTA_6PER_GAN });
            command.Parameters.Add(new SqlParameter("@FECH_ULT", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FECH_ULT) ? DBNull.Value : DateTimeUtils.ParseFromString(cia.FECH_ULT) });
            command.Parameters.Add(new SqlParameter("@FEC_ULT_CIE", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FEC_ULT_CIE) ? DBNull.Value : DateTimeUtils.ParseFromString(cia.FEC_ULT_CIE) });
            command.Parameters.Add(new SqlParameter("@TASA_IVA", SqlDbType.Float) { Value = cia.TASA_IVA==null ? DBNull.Value : cia.TASA_IVA });
            command.Parameters.Add(new SqlParameter("@MESES_CHQ", SqlDbType.Int) { Value = cia.MESES_CHQ==null ? DBNull.Value : cia.MESES_CHQ });
            command.Parameters.Add(new SqlParameter("@TASA_CAM", SqlDbType.Float) { Value = cia.TASA_CAM==null ? DBNull.Value : cia.TASA_CAM });
            command.Parameters.Add(new SqlParameter("@IVA_PORC", SqlDbType.Float) { Value = cia.IVA_PORC==null ? DBNull.Value : cia.IVA_PORC });
            command.Parameters.Add(new SqlParameter("@ND_IVA", SqlDbType.VarChar) { Value = cia.ND_IVA==null ? DBNull.Value : cia.ND_IVA });
            command.Parameters.Add(new SqlParameter("@FD_IVA", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FD_IVA) ? DBNull.Value : DateTimeUtils.ParseFromString(cia.FD_IVA) });
            command.Parameters.Add(new SqlParameter("@ISR_PORC", SqlDbType.Float) { Value = cia.ISR_PORC==null ? DBNull.Value : cia.ISR_PORC });
            command.Parameters.Add(new SqlParameter("@ND_ISR", SqlDbType.VarChar) { Value = cia.ND_ISR==null ? DBNull.Value : cia.ND_ISR });
            command.Parameters.Add(new SqlParameter("@FD_ISR", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FD_ISR) ? DBNull.Value : DateTimeUtils.ParseFromString(cia.FD_ISR) });
            command.Parameters.Add(new SqlParameter("@PRB_PORC", SqlDbType.Float) { Value = cia.PRB_PORC==null ? DBNull.Value : cia.PRB_PORC });
            command.Parameters.Add(new SqlParameter("@ND_PRB", SqlDbType.VarChar) { Value = cia.ND_PRB==null ? DBNull.Value : cia.ND_PRB });
            command.Parameters.Add(new SqlParameter("@FD_PRB", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FD_PRB) ? DBNull.Value : DateTimeUtils.ParseFromString(cia.FD_PRB) });
            command.Parameters.Add(new SqlParameter("@PRS_PORC", SqlDbType.Float) { Value = cia.PRS_PORC==null ? DBNull.Value : cia.PRS_PORC });
            command.Parameters.Add(new SqlParameter("@ND_PRS", SqlDbType.VarChar) { Value = cia.ND_PRS==null ? DBNull.Value : cia.ND_PRS });
            command.Parameters.Add(new SqlParameter("@FD_PRS", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FD_PRS) ? DBNull.Value : DateTimeUtils.ParseFromString(cia.FD_PRS) });
            command.Parameters.Add(new SqlParameter("@COD_MONEDA", SqlDbType.VarChar) { Value = cia.COD_MONEDA==null ? DBNull.Value : cia.COD_MONEDA });
            command.Parameters.Add(new SqlParameter("@DUP_DET_PARTIDAD", SqlDbType.VarChar) { Value = cia.DUP_DET_PARTIDAD==null ? DBNull.Value : cia.DUP_DET_PARTIDAD });
            command.Parameters.Add(new SqlParameter("@VAL_MIN_DEPRECIAR", SqlDbType.Float) { Value = cia.VAL_MIN_DEPRECIAR==null ? DBNull.Value : cia.VAL_MIN_DEPRECIAR });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA1", SqlDbType.Int) { Value = cia.INGRESO_CTA1==null ? DBNull.Value : cia.INGRESO_CTA1 });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA2", SqlDbType.Int) { Value = cia.INGRESO_CTA2==null ? DBNull.Value : cia.INGRESO_CTA2 });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA3", SqlDbType.Int) { Value = cia.INGRESO_CTA3==null ? DBNull.Value : cia.INGRESO_CTA3 });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA4", SqlDbType.Int) { Value = cia.INGRESO_CTA4==null ? DBNull.Value : cia.INGRESO_CTA4 });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA5", SqlDbType.Int) { Value = cia.INGRESO_CTA5==null ? DBNull.Value : cia.INGRESO_CTA5 });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA6", SqlDbType.Int) { Value = cia.INGRESO_CTA6==null ? DBNull.Value : cia.INGRESO_CTA6 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA1", SqlDbType.Int) { Value = cia.GASTO_CTA1==null ? DBNull.Value : cia.GASTO_CTA1 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA2", SqlDbType.Int) { Value = cia.GASTO_CTA2==null ? DBNull.Value : cia.GASTO_CTA2 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA3", SqlDbType.Int) { Value = cia.GASTO_CTA3==null ? DBNull.Value : cia.GASTO_CTA3 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA4", SqlDbType.Int) { Value = cia.GASTO_CTA4==null ? DBNull.Value : cia.GASTO_CTA4 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA5", SqlDbType.Int) { Value = cia.GASTO_CTA5==null ? DBNull.Value : cia.GASTO_CTA5 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA6", SqlDbType.Int) { Value = cia.GASTO_CTA6==null ? DBNull.Value : cia.GASTO_CTA6 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA1", SqlDbType.Int) { Value = cia.COSTO_CTA1==null ? DBNull.Value : cia.COSTO_CTA1 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA2", SqlDbType.Int) { Value = cia.COSTO_CTA2==null ? DBNull.Value : cia.COSTO_CTA2 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA3", SqlDbType.Int) { Value = cia.COSTO_CTA3==null ? DBNull.Value : cia.COSTO_CTA3 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA4", SqlDbType.Int) { Value = cia.COSTO_CTA4==null ? DBNull.Value : cia.COSTO_CTA4 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA5", SqlDbType.Int) { Value = cia.COSTO_CTA5==null ? DBNull.Value : cia.COSTO_CTA5 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA6", SqlDbType.Int) { Value = cia.COSTO_CTA6==null ? DBNull.Value : cia.COSTO_CTA6 });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(CallSaveCia));
            return false;
        }
    }

    public async Task<bool> CallUpdateCia(CiaDto cia)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.ActualizarCia";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@CodigoCia", SqlDbType.VarChar) { Value = cia.COD_CIA });
            command.Parameters.Add(new SqlParameter("@RAZON_SOCIAL", SqlDbType.VarChar) { Value = cia.RAZON_SOCIAL==null ? DBNull.Value : cia.RAZON_SOCIAL });
            command.Parameters.Add(new SqlParameter("@NOM_COMERCIAL", SqlDbType.VarChar) { Value = cia.NOM_COMERCIAL==null ? DBNull.Value : cia.NOM_COMERCIAL });
            command.Parameters.Add(new SqlParameter("@DIREC_EMPRESA", SqlDbType.VarChar) { Value = cia.DIREC_EMPRESA==null ? DBNull.Value : cia.DIREC_EMPRESA });
            command.Parameters.Add(new SqlParameter("@TELEF_EMPRESA", SqlDbType.VarChar) { Value = cia.TELEF_EMPRESA==null ? DBNull.Value : cia.TELEF_EMPRESA });
            command.Parameters.Add(new SqlParameter("@NIT_EMPRESA", SqlDbType.VarChar) { Value = cia.NIT_EMPRESA==null ? DBNull.Value : cia.NIT_EMPRESA });
            command.Parameters.Add(new SqlParameter("@NUMERO_PATRONAL", SqlDbType.VarChar) { Value = cia.NUMERO_PATRONAL==null ? DBNull.Value : cia.NUMERO_PATRONAL });
            command.Parameters.Add(new SqlParameter("@MES_CIERRE", SqlDbType.Int) { Value = cia.MES_CIERRE==null ? DBNull.Value : cia.MES_CIERRE });
            command.Parameters.Add(new SqlParameter("@MES_PROCESO", SqlDbType.Int) { Value = cia.MES_PROCESO==null ? DBNull.Value : cia.MES_PROCESO });
            command.Parameters.Add(new SqlParameter("@PERIODO", SqlDbType.Int) { Value = cia.PERIODO==null ? DBNull.Value : cia.PERIODO });
            command.Parameters.Add(new SqlParameter("@CTA_1RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_1RESUL_ACT==null ? DBNull.Value : cia.CTA_1RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_2RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_2RESUL_ACT==null ? DBNull.Value : cia.CTA_2RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_3RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_3RESUL_ACT==null ? DBNull.Value : cia.CTA_3RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_4RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_4RESUL_ACT==null ? DBNull.Value : cia.CTA_4RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_5RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_5RESUL_ACT==null ? DBNull.Value : cia.CTA_5RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_6RESUL_ACT", SqlDbType.Int) { Value = cia.CTA_6RESUL_ACT==null ? DBNull.Value : cia.CTA_6RESUL_ACT });
            command.Parameters.Add(new SqlParameter("@CTA_1RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_1RESUL_ANT==null ? DBNull.Value : cia.CTA_1RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_2RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_2RESUL_ANT==null ? DBNull.Value : cia.CTA_2RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_3RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_3RESUL_ANT==null ? DBNull.Value : cia.CTA_3RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_4RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_4RESUL_ANT==null ? DBNull.Value : cia.CTA_4RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_5RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_5RESUL_ANT==null ? DBNull.Value : cia.CTA_5RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_6RESUL_ANT", SqlDbType.Int) { Value = cia.CTA_6RESUL_ANT==null ? DBNull.Value : cia.CTA_6RESUL_ANT });
            command.Parameters.Add(new SqlParameter("@CTA_1PER_GAN", SqlDbType.Int) { Value = cia.CTA_1PER_GAN==null ? DBNull.Value : cia.CTA_1PER_GAN });
            command.Parameters.Add(new SqlParameter("@CTA_2PER_GAN", SqlDbType.Int) { Value = cia.CTA_2PER_GAN==null ? DBNull.Value : cia.CTA_2PER_GAN });
            command.Parameters.Add(new SqlParameter("@CTA_3PER_GAN", SqlDbType.Int) { Value = cia.CTA_3PER_GAN==null ? DBNull.Value : cia.CTA_3PER_GAN });
            command.Parameters.Add(new SqlParameter("@CTA_4PER_GAN", SqlDbType.Int) { Value = cia.CTA_4PER_GAN==null ? DBNull.Value : cia.CTA_4PER_GAN });
            command.Parameters.Add(new SqlParameter("@CTA_5PER_GAN", SqlDbType.Int) { Value = cia.CTA_5PER_GAN==null ? DBNull.Value : cia.CTA_5PER_GAN });
            command.Parameters.Add(new SqlParameter("@CTA_6PER_GAN", SqlDbType.Int) { Value = cia.CTA_6PER_GAN==null ? DBNull.Value : cia.CTA_6PER_GAN });
            command.Parameters.Add(new SqlParameter("@FECH_ULT", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FECH_ULT) ? DBNull.Value : DateTimeUtils.ParseFromString(cia.FECH_ULT) });
            command.Parameters.Add(new SqlParameter("@FEC_ULT_CIE", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FEC_ULT_CIE) ? DBNull.Value : DateTimeUtils.ParseFromString(cia.FEC_ULT_CIE) });
            command.Parameters.Add(new SqlParameter("@TASA_IVA", SqlDbType.Float) { Value = cia.TASA_IVA==null ? DBNull.Value : cia.TASA_IVA });
            command.Parameters.Add(new SqlParameter("@MESES_CHQ", SqlDbType.Int) { Value = cia.MESES_CHQ==null ? DBNull.Value : cia.MESES_CHQ });
            command.Parameters.Add(new SqlParameter("@TASA_CAM", SqlDbType.Float) { Value = cia.TASA_CAM==null ? DBNull.Value : cia.TASA_CAM });
            command.Parameters.Add(new SqlParameter("@IVA_PORC", SqlDbType.Float) { Value = cia.IVA_PORC==null ? DBNull.Value : cia.IVA_PORC });
            command.Parameters.Add(new SqlParameter("@ND_IVA", SqlDbType.VarChar) { Value = cia.ND_IVA==null ? DBNull.Value : cia.ND_IVA });
            command.Parameters.Add(new SqlParameter("@FD_IVA", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FD_IVA) ? DBNull.Value : DateTimeUtils.ParseFromString(cia.FD_IVA) });
            command.Parameters.Add(new SqlParameter("@ISR_PORC", SqlDbType.Float) { Value = cia.ISR_PORC==null ? DBNull.Value : cia.ISR_PORC });
            command.Parameters.Add(new SqlParameter("@ND_ISR", SqlDbType.VarChar) { Value = cia.ND_ISR==null ? DBNull.Value : cia.ND_ISR });
            command.Parameters.Add(new SqlParameter("@FD_ISR", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FD_ISR) ? DBNull.Value : DateTimeUtils.ParseFromString(cia.FD_ISR) });
            command.Parameters.Add(new SqlParameter("@PRB_PORC", SqlDbType.Float) { Value = cia.PRB_PORC==null ? DBNull.Value : cia.PRB_PORC });
            command.Parameters.Add(new SqlParameter("@ND_PRB", SqlDbType.VarChar) { Value = cia.ND_PRB==null ? DBNull.Value : cia.ND_PRB });
            command.Parameters.Add(new SqlParameter("@FD_PRB", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FD_PRB) ? DBNull.Value : cia.FD_PRB });
            command.Parameters.Add(new SqlParameter("@PRS_PORC", SqlDbType.Float) { Value = cia.PRS_PORC==null ? DBNull.Value : cia.PRS_PORC });
            command.Parameters.Add(new SqlParameter("@ND_PRS", SqlDbType.VarChar) { Value = cia.ND_PRS==null ? DBNull.Value : cia.ND_PRS });
            command.Parameters.Add(new SqlParameter("@FD_PRS", SqlDbType.DateTime) { Value = string.IsNullOrEmpty(cia.FD_PRS) ? DBNull.Value : DateTimeUtils.ParseFromString(cia.FD_PRS) });
            command.Parameters.Add(new SqlParameter("@COD_MONEDA", SqlDbType.VarChar) { Value = cia.COD_MONEDA==null ? DBNull.Value : cia.COD_MONEDA });
            command.Parameters.Add(new SqlParameter("@DUP_DET_PARTIDAD", SqlDbType.VarChar) { Value = cia.DUP_DET_PARTIDAD==null ? DBNull.Value : cia.DUP_DET_PARTIDAD });
            command.Parameters.Add(new SqlParameter("@VAL_MIN_DEPRECIAR", SqlDbType.Float) { Value = cia.VAL_MIN_DEPRECIAR==null ? DBNull.Value : cia.VAL_MIN_DEPRECIAR });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA1", SqlDbType.Int) { Value = cia.INGRESO_CTA1==null ? DBNull.Value : cia.INGRESO_CTA1 });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA2", SqlDbType.Int) { Value = cia.INGRESO_CTA2==null ? DBNull.Value : cia.INGRESO_CTA2 });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA3", SqlDbType.Int) { Value = cia.INGRESO_CTA3==null ? DBNull.Value : cia.INGRESO_CTA3 });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA4", SqlDbType.Int) { Value = cia.INGRESO_CTA4==null ? DBNull.Value : cia.INGRESO_CTA4 });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA5", SqlDbType.Int) { Value = cia.INGRESO_CTA5==null ? DBNull.Value : cia.INGRESO_CTA5 });
            command.Parameters.Add(new SqlParameter("@INGRESO_CTA6", SqlDbType.Int) { Value = cia.INGRESO_CTA6==null ? DBNull.Value : cia.INGRESO_CTA6 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA1", SqlDbType.Int) { Value = cia.GASTO_CTA1==null ? DBNull.Value : cia.GASTO_CTA1 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA2", SqlDbType.Int) { Value = cia.GASTO_CTA2==null ? DBNull.Value : cia.GASTO_CTA2 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA3", SqlDbType.Int) { Value = cia.GASTO_CTA3==null ? DBNull.Value : cia.GASTO_CTA3 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA4", SqlDbType.Int) { Value = cia.GASTO_CTA4==null ? DBNull.Value : cia.GASTO_CTA4 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA5", SqlDbType.Int) { Value = cia.GASTO_CTA5==null ? DBNull.Value : cia.GASTO_CTA5 });
            command.Parameters.Add(new SqlParameter("@GASTO_CTA6", SqlDbType.Int) { Value = cia.GASTO_CTA6==null ? DBNull.Value : cia.GASTO_CTA6 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA1", SqlDbType.Int) { Value = cia.COSTO_CTA1==null ? DBNull.Value : cia.COSTO_CTA1 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA2", SqlDbType.Int) { Value = cia.COSTO_CTA2==null ? DBNull.Value : cia.COSTO_CTA2 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA3", SqlDbType.Int) { Value = cia.COSTO_CTA3==null ? DBNull.Value : cia.COSTO_CTA3 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA4", SqlDbType.Int) { Value = cia.COSTO_CTA4==null ? DBNull.Value : cia.COSTO_CTA4 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA5", SqlDbType.Int) { Value = cia.COSTO_CTA5==null ? DBNull.Value : cia.COSTO_CTA5 });
            command.Parameters.Add(new SqlParameter("@COSTO_CTA6", SqlDbType.Int) { Value = cia.COSTO_CTA6==null ? DBNull.Value : cia.COSTO_CTA6 });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(CallUpdateCia));
            return false;
        }
    }

    public async Task<string> CallGenerateCiaCod()
    {
        try
        {
            var command = dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT [CATALANA].[Obtener_Codigo_Cia] ()";

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            var result = (string)(await command.ExecuteScalarAsync())! ?? "";
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(CallGenerateCiaCod));
            throw;
        }
    }

    public Task<CiaResultSet?> GetOneCia(string ciaCod) {
        try {
            var result = dbContext.Cias
                // .FromSql($"SELECT * FROM CATALANA.ObtenerDatosEmpresa({ciaCod})")
                .Where(cia => cia.CodCia == ciaCod)
                .Select(cia => CiaResultSet.EntityToResultSet(cia))
                .FirstOrDefaultAsync();
            return result;
        } catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetOneCia));
            return Task.FromResult<CiaResultSet?>(null);
        }
    }

    public Task<int> GetCount() => dbContext.Cias.CountAsync();
}