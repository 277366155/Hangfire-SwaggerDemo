using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Solitude.Exchange.Core;
using Solitude.Exchange.Model;
using Solitude.Exchange.Service;
using System;
using System.ComponentModel;

namespace Solitude.Exchange.Hangfire.Controllers
{
    /// <summary>
    /// 默认控制器
    /// </summary>
    [Route("api/[controller]")]
    public class DefaultController:Controller
    {
        /// <summary>
        /// 间隔执行分钟数
        /// </summary>
        private static int RunTimeSpan => Convert.ToInt32(BaseCore.Configuration["AppSettings:TimeSet:RunTimeSpan"]);

        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("update")]
        public ActionResult UpdateData(C2CParam param)
        {
            RecurringJob.AddOrUpdate("C2CUpdateOrder", () => new C2CService().C2CUpdate(param), Cron.MinuteInterval(RunTimeSpan));
            RecurringJob.Trigger("C2CUpdateOrder");
            return Json("OK");
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetInfo")]
        [ProducesResponseType(typeof(ResultModel), 200)]
        public ActionResult GetData(C2CParam param)
        {
            return Json(new C2CService().C2CUpdate(param));
        }
    }
}
