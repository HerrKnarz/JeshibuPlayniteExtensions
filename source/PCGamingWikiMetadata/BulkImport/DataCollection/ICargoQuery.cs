using System.Collections.Generic;

namespace PCGamingWikiBulkImport.DataCollection;

public interface ICargoQuery
{
    CargoResultRoot<CargoResultGame> GetGamesByExactValues(string table, string field, IEnumerable<string> values, int offset);
    CargoResultRoot<CargoResultGame> GetGamesByHolds(string table, string field, string holds, int offset);
    CargoResultRoot<CargoResultGame> GetGamesByHoldsLike(string table, string field, string holds, int offset);
    IEnumerable<ItemCount> GetValueCounts(string table, string field, string filter = null);
}
