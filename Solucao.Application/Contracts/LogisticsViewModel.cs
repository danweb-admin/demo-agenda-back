using System;

public class LogisticsViewModel
{
    public Guid? Id { get; set; }

    public Guid? CalendarId { get; set; }

    public Guid? DriverId { get; set; }

    public DateTime DataHora { get; set; }

    public int Tipo { get; set; }

    public string Titulo { get; set; }

    public string Descricao { get; set; }

    public string Observacao { get; set; }

    public string Endereco { get; set; }

    public bool Concluido { get; set; }

    public bool Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Dados da locação
    public string Cliente { get; set; }

    public string Equipamento { get; set; }

    public string Cidade { get; set; }

    // Dados do motorista
    public string Motorista { get; set; }
    public string Placa { get; set; }

}