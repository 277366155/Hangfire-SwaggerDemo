using System;

namespace Solitude.Exchange.Model
{
    /// <summary>
    /// c2c方法参数模型
    /// </summary>
    public class C2CParam
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public int? OrderNum { get; set; }

        /// <summary>
        /// 货币单位
        /// </summary>
        public string CoinUnit { get; set; }
    }
}
