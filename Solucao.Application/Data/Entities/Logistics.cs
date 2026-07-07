using System;
using Solucao.Application.Utils.Enum;

namespace Solucao.Application.Data.Entities
{
  public class Logistics : BaseEntity
  {
    public DateTime DataHora { get; set; }
    public TipoEventoLogistico Tipo { get; set; }
    public Guid? CalendarId { get; set; }
    public Guid? DriverId { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public string Endereco { get; set; }
    public string Observacao { get; set; }
    public bool Concluido { get; set; }
    public Calendar Calendar { get; set; }
    public Person Driver { get; set; }
  }
}

