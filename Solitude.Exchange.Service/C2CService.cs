using Solitude.Exchange.Model;
using System;

namespace Solitude.Exchange.Service
{
    public class C2CService
    {
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ResultModel C2CUpdate(C2CParam param)
        {
            var result = new ResultModel() { CoinUnit=param.CoinUnit, OrderNum=param.OrderNum };
            return result;
        }
    }
}
