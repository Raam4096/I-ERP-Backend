using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Catalog.Domain;

public sealed class UnitOfMeasureConversion : AuditableEntity
{

    public Guid FromUomId { get; set; }
    public Guid ToUomId { get; set; }
    public decimal Factor { get; set; }

}
