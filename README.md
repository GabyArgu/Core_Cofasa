# CoreContable

## Compilacion y ejecucion
```sh
# Generar compilado de la app
dotnet publish -c Release

hostname
getmac
```

## Seguridad
### Llave para deshabilitar la verificacion de dispositivos
```sh
# https://randomkeygen.com/
# DIO
# $2a$11$Xsu7nMBcAS3cF.v1xLDtJewXBqdkYgQlazcg1UDK4jNj.E7OKpFk.
dotnet user-secrets set "AppSettings:DisableDeviceVerification" "true"
```

## Reportes
Para generar reportes en PDF es necesario instalar el paquete `Rotativa.AspNetCore` en el proyecto.
Descargar la dependencia desde: https://wkhtmltopdf.org/downloads.html
Segun la plataforma que se esta usando es necesario comentar/descomentar la siguiente linea en el archivo `Program.cs`:

```csharp
// Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "../Rotativa/Windows");
Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "../Rotativa/macOS");
```

Los binarios necesarios ya estan incluidos en el proyecto, bajo la carpeta `Rotativa`.

## EF Core y Database

### Definir el nivel de compatibilidad de la base de datos
Es necesario definir el nivel de compatibilidad de la base de datos para que EF Core pueda trabajar correctamente.
Esto ayudara a que la integridad de la aplicacion no se vea afectada.

Para esto se puede, modificar el nivel de compatibilidad de la base de datos con el siguiente comando:
```sql
ALTER DATABASE [CORE_CONTABLE] SET COMPATIBILITY_LEVEL = 140
```

O se puede configurar EF para que use un nivel de compatibilidad especifico, para esto primero se obtiene el nivel de compatibilidad actual:
```sql
SELECT compatibility_level
FROM sys.databases
WHERE name = 'CORE_CONTABLE';
```

Y luego se setea el nivel de compatibilidad en el archivo `Program.cs`:
```csharp
builder.Services.AddDbContext<DbContext>(options => {
    options.UseSqlServer(
        "name=DefaultWin",
        o => o.UseCompatibilityLevel(120)
    ).EnableSensitiveDataLogging();
});
```

### Ejemplo de consulta con EF Core
```csharp
public Task<List<Select2ResultSet>> GetForSelect2(string codCia, string codCC, string? query = null,
    int pageNumber = 1, int pageSize = 10)
{
    IQueryable<CentroCuentaView> efQuery = dbContext.CentroCuentaView
        .Where(entity => entity.COD_CIA == codCia && entity.CENTRO_COSTO == codCC);

    if (!query.IsNullOrEmpty())
    {
        efQuery = dbContext.CentroCuentaView
            .Where(entity => entity.COD_CIA == codCia && entity.CENTRO_COSTO == codCC)
            .Where(
                entity => EF.Functions.Like(entity.CuentaContable, $"%{query}%")
                    || EF.Functions.Like(entity.Descripcion_CTA, $"%{query}%")
            );
    }

    var count = efQuery.Count();

    return efQuery
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(entity => new Select2ResultSet
        {
            id = $"{entity.COD_CIA}|{entity.CENTRO_COSTO}|{entity.CTA_1}|{entity.CTA_2}|{entity.CTA_3}|{entity.CTA_4}|{entity.CTA_5}|{entity.CTA_6}",
            text = entity.CuentaContable,
            more = pageNumber * pageSize < count
        })
        .ToListAsync();
}
```

### Ejemplo de consulta mas compleja con EF Core
```csharp
public async Task<DataTableResultSet<List<DmgPolizaResultSet>>?> GetAllBy(DataTabletDto dataTabletDto,
        string codCia, int? periodo = null, string? tipoDocto = null, int? numPoliza = null, string? fechaInicio = null,
        string? fechaFin = null)
{
    try
    {
        IQueryable<DmgPolizaView> efQuery = dbContext.DmgPolizaView
            .Where(entity => entity.COD_CIA == codCia);

        if (periodo != null) efQuery = efQuery.Where(entity => entity.PERIODO == periodo);
        if (tipoDocto != null) efQuery = efQuery.Where(entity => entity.TIPO_DOCTO == tipoDocto);
        if (numPoliza != null) efQuery = efQuery.Where(entity => entity.NUM_POLIZA == numPoliza);
        if (fechaInicio != null) efQuery = efQuery.Where(entity => entity.FECHA >= DateTimeUtils.ParseFromString(fechaInicio));
        if (fechaFin != null) efQuery = efQuery.Where(entity => entity.FECHA <= DateTimeUtils.ParseFromString(fechaFin));

        if (dataTabletDto.orderIndex > 0)
        {
            var column = _columns[dataTabletDto.orderIndex];
            efQuery = dataTabletDto.orderDirection == "asc"
                ? efQuery.OrderBy(entity => EF.Property<object>(entity, column))
                : efQuery.OrderByDescending(entity => EF.Property<object>(entity, column));
        }

        var total = efQuery.Count();

        if (dataTabletDto.search != null && dataTabletDto.search != "")
        {
            efQuery = dataTabletDto.search.ToLower() switch
            {
                "impreso" => efQuery.Where(entity => entity.Asiento_Impreso == "S"),
                "no impreso" => efQuery.Where(entity => entity.Asiento_Impreso == "N"),
                _ => efQuery.Where(entity =>
                    EF.Functions.Like(entity.COD_CIA, $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.PERIODO.ToString(), $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.TIPO_DOCTO, $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.NUM_POLIZA.ToString(), $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.NUM_REFERENCIA, $"%{dataTabletDto.search}%")
                    // FECHA
                    || EF.Functions.Like(entity.ANIO.ToString(), $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.MES.ToString(), $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.CONCEPTO, $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.TOTAL_POLIZA.ToString(), $"%{dataTabletDto.search}%"))
            };
        }

        var totalFiltered = efQuery.Count();

        var data = await efQuery
            .Skip(dataTabletDto.start)
            .Take(dataTabletDto.length)
            .Select(entity => new DmgPolizaResultSet
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
                Asiento_Impreso = entity.Asiento_Impreso
            })
            .ToListAsync();

        return new DataTableResultSet<List<DmgPolizaResultSet>>
        {
            success = true,
            message = "Access data",
            data = data,
            recordsFiltered = totalFiltered,
            draw = dataTabletDto.draw,
            recordsTotal = total
        };
    }
    catch (Exception e)
    {
        return null;
    }
}
```

### Migrations
Inicialmente la app usaba las migraciones de EF Core, pero se cambio a un modelo de base de datos ya existente.