using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Solitude.Exchange.Model
{
    /// <summary>
    /// 订单信息
    /// </summary>
    public   class OrderInfo
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public int? OrderNum { get; set; }

        /// <summary>
        /// 货币单位
        /// </summary>
        public string CoinUnit { get; set; }

        /// <summary>
        /// 买价
        /// </summary>
        public decimal Buy { get; set; }

        /// <summary>
        /// 卖价
        /// </summary>
        public decimal Sell { get; set; }
    }

    public class User
    {
        public string UserName { get; set; }
        [NotMapped]
        public DateTime CreateTime { get; set; }
        public string Msg { get; set; }

        public string BizCreateTime
        {
            get
            {
                return CreateTime.ToString();
            }
        }
    }

}
