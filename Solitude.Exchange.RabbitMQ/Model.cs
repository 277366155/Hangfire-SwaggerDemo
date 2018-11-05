using System;
using System.Collections.Generic;

namespace Solitude.Exchange.RabbitMQ
{
    public interface IExchangeRequest<T>
    {
        /// <summary>
        /// When the request was created
        /// </summary>
        Guid CorrelationId { get; set; }

        T Data { get; set; }

    }
    public class ExchangeRequest<T> : IExchangeRequest<T>
    {
        public Guid CorrelationId { get; set; }

        public T Data { get; set; }
    }
    public class MatchingNotifyInfo
    {
        public MatchingResponse Response { get; set; }
        public TradeMarketInfo Info { get; set; }
    }

    public class MatchingResponse
    {
        /// <summary>
        /// 撮合完成的订单
        /// </summary>
        public List<SimpleOrderInfo> Orders { get; set; }

        /// <summary>
        /// 返回撮合过程中的资产变更记录
        /// </summary>
        public Dictionary<int, List<UserAssetRecord>> AssetLogs { get; set; }


        public List<SimpleUserAssetInfo> UserAsset { get; set; }

        /// <summary>
        /// 是否发生错误
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string[] ErrMsgs { get; set; }
    }
    public class SimpleUserAssetInfo
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int CurrencyID { get; set; }
        public decimal Available { get; set; }
        public decimal Balance { get; set; }
        public decimal Frozen { get; set; }

        /// <summary>
        /// 买卖单Id
        /// </summary>
        public int EntrustId { get; set; }
    }
    public class UserAssetRecord
    {
        /// <summary>
        ///     记录ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        ///     流水号
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        ///     用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        ///     币种ID
        /// </summary>
        public int CurrencyID { get; set; }

        /// <summary>
        ///     币种符号
        /// </summary>
        public string CurrencyUnit { get; set; }

        /// <summary>
        ///     数量
        /// </summary>
        public decimal ChangeValue { get; set; }

        /// <summary>
        ///     用户余额
        /// </summary>
        public decimal AssetBalance { get; set; }

        /// <summary>
        ///     资金方向 买入卖出
        /// </summary>
        public int InOutType { get; set; }

        /// <summary>
        ///     记录类型ID
        /// </summary>
        public int RecordTypeID { set; get; }

        /// <summary>
        ///     记录类型名称
        /// </summary>
        public string RecordTypeName { get; set; }

        /// <summary>
        ///     记录类型名称(英文)
        /// </summary>
        public string RecordTypeName_EN { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     记录状态
        /// </summary>
        public int RecordStatus { get; set; }

        /// <summary>
        /// 买卖单Id
        /// </summary>
        public int EntrustId { get; set; }
    }


    public class TradeMarketInfo
    {
        /// <summary>
        /// 买盘
        /// </summary>
        public List<DishModel> Ask { get; set; }
        /// <summary>
        /// 卖盘
        /// </summary>
        public List<DishModel> Bid { get; set; }

        public List<RecentOrderModel> Orders { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public long TimeStamp { get; set; }

        /// <summary>
        /// 涨跌幅
        /// </summary>
        //public decimal DailyChange { get; set; }
        /// <summary>
        /// 涨跌幅
        /// </summary>
        public decimal DailyChangePerc { get; set; }
        /// <summary>
        /// 24小时最高
        /// </summary>
        public decimal High { get; set; }
        /// <summary>
        /// 24小时最低
        /// </summary>
        public decimal Low { get; set; }
        /// <summary>
        /// 市场ID
        /// </summary>
        public int MarketId { get; set; }
        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal Open { get; set; }
        /// <summary>
        /// 24小时成交量
        /// </summary>
        public decimal Volume { get; set; }
        /// <summary>
        /// 日成交额
        /// </summary>

        public decimal DayAmount { get; set; }

        /// <summary>
        /// 最新价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 货币图标
        /// </summary>
        public string CurrencyIcon { get; set; }
        /// <summary>
        /// 货币符号
        /// </summary>
        public string CurrencyUnit { get; set; }
        /// <summary>
        /// 市场货币符号
        /// </summary>
        public string ExchangeCurrencyUnit { get; set; }
        /// <summary>
        /// USDT价格
        /// </summary>
        public decimal USDTPrice { get; set; }

        /// <summary>
        /// 受影响的用户ID
        /// </summary>
        public int[] AffectUserIds { get; set; }
        /// <summary>
        /// 受影响的买卖单ID
        /// </summary>
        public int[] AffectEntrustIds { get; set; }
        /// <summary>
        /// 受影响的用户订单ID
        /// </summary>
        public int[] AffectOrderIds { get; set; }
    }
    public class DishModel
    {
        public decimal price { get; set; }
        public decimal NoComplete { get; set; }
        public decimal amount { get { return price * NoComplete; } }
        public long TimeSpan { get; set; }
    }
    public class RecentOrderModel
    {/// <summary>
     /// 订单id
     /// </summary>
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public decimal price { get; set; }
        public decimal amount { get; set; }
        public long TimeSpan { get; set; }
        /// <summary>
        /// 买卖方向，是否以买单成交
        /// </summary>
        public int IsBuy { get; set; }
    }

    public class SimpleOrderInfo
    {
        public int CEID { get; set; }
        public int BuyUserID { get; set; }
        public int SellUserID { get; set; }
        public int BuyEntrustID { get; set; }
        public int SellEntrustID { get; set; }
        public decimal TradePrice { get; set; }
        public decimal TradeVolume { get; set; }
        public decimal BuyTradeFees { get; set; }
        public decimal SellTradeFees { get; set; }
        public bool IsBuy { get; set; }
        public SimpleEntrustInfo SellEntrust { get; set; }
        public SimpleEntrustInfo BuyEntrust { get; set; }

        public DateTime CreateTime { get; set; }
        public long TimeSpan { get; set; }

        public int OrderId { get; set; }
    }

    public class SimpleEntrustInfo
    {
        public string EntrustStatusName_EN { get; set; }
        public decimal EntrustPrice { get; set; }
        public int IsBuy { get; set; }
        public DateTime CreateTime { get; set; }
        public int UserID { get; set; }
        public decimal NoCompletedVolume { get; set; }
        public int EntrustStatus { get; set; }
        public decimal CompletedTotalAmount { get; set; }
        public decimal CompletedVolume { get; set; }
        public decimal AveragePrice { get; set; }
        public string EntrustStatusName { get; set; }
        public decimal TradeFees { get; set; }
        public decimal FeesRate { get; set; }
        public string SerialNumber { get; set; }
        public int ID { get; set; }
        public decimal EntrustVolume { get; set; }
        public int CEID { get; set; }
    }
}
