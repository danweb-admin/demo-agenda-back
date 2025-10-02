CREATE or alter PROCEDURE GetCalendars
    @startDate DATE,
    @endDate DATE,
    @isAdmin BIT = 0
AS
BEGIN

    if @isAdmin is null
        set @isAdmin = 0
    SELECT 
        starttime AS start,
        endtime AS "end",
        e.name + ' - ' + CONVERT(varchar(15), cli.Name) + ' - ' + CONVERT(varchar(15), ci.Nome) + ' - ' + s.Sigla  AS title,
        e.name AS EquipamentoFull,
        cli.Name + ' - ' + CONVERT(varchar(15), ci.Nome) + ' - ' + s.Sigla AS ClienteFull,
        p1.Name AS MotoristaRecolhe,
        p.Name AS MotoristaEntrega,
        c.status,
        e.Color,
        cli.CellPhone,
        cli.Address + ', ' + cli.Number + ' - ' + cli.Complement AS Endereco,
        CASE WHEN @isAdmin = 1 THEN c.Id ELSE NULL END AS CalendarId,
        c.note
    FROM Calendars AS c 
    INNER JOIN Equipaments AS e ON c.EquipamentId = e.Id 
    INNER JOIN Clients AS cli ON c.ClientId = cli.Id 
    INNER JOIN Cities AS ci ON cli.CityId = ci.Id 
    INNER JOIN States AS s ON ci.StateId = s.Id 
    LEFT JOIN People AS p ON c.DriverId = p.Id
    LEFT JOIN People AS p1 ON c.DriverCollectsId = p1.Id
    WHERE c.Active = 1 
    AND CAST([Date] AS DATE) BETWEEN @startDate AND @endDate
    AND [status] in (1,2)
      
    ORDER BY [Date];
END