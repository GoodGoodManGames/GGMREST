using System.Threading.Tasks;
using GGMREST.Attribute;
using UnitTest.Bithumb.Entity;

namespace UnitTest.Bithumb
{
    public interface IBithumbService
    {
        [GET("ticker/{currency}")]
        Task<RequestData<TickerData>> GetTicker([Path("currency")]CurrencyType currency = CurrencyType.BTC);
    }
}