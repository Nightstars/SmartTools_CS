/*
 * Files generated by SmartTools.Net Codeless
 * $GenDate$
 *
 */

using $Rootnamespace$.Data;
using $Rootnamespace$.Models;
using idp_wechat.Models;
using idp_wechat.Repository;
using Longnows.Saas.Framework.Common.Entity;
using Longnows.Saas.Framework.DB;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace $Rootnamespace$.Services
{
    public class $ModelName$Service: CoreBaseRepository<$ModelName$>
    {

        #region initialize
        private readonly IConfiguration _configuration;
        public $ModelName$Service(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region GetAll
        /// <summary>
        /// GetAll By Page
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public CommonResult<List<$ModelName$>> GetAll($ModelName$SearchDto searchDto)
        {
            int total = 0;
            var list=Db.Queryable<$ModelName$>()
                .AS($"{CoreDbTables.Db((DBType)Enum.Parse(typeof(DBType), _configuration["ConnectionConfig:DBType"]),CoreDbTables.$Dadabase$)}{CoreDbTables.$Dbtable$}")
                $SearchFiled$
                .ToPageList(searchDto.PageIndex, searchDto.PageSize, ref total);

            return new CommonResult<List<$ModelName$>>() { Code= list.Any()?"0X00":"0X01",Data = list,Message= list.Any()?$"{total}":null,Success=list.Any() };
        }
        #endregion

        #region GetById
        /// <summary>
        /// Get By Id
        /// </summary>
        /// <param name="currSearchDto"></param>
        /// <returns></returns>
        public CommonResult<List<$ModelName$>> GetById($ModelName$SearchDto searchDto)
        {
            int total = 0;
            var list = Db.Queryable<$ModelName$>()
                .AS($"{CoreDbTables.Db((DBType)Enum.Parse(typeof(DBType), _configuration["ConnectionConfig:DBType"]), CoreDbTables.$Dadabase$)}{CoreDbTables.$Dbtable$}")
                .Where(x=>x.PK_SEQ.Equals(searchDto.PK_SEQ))
                .ToList();

            return new CommonResult<List<$ModelName$>>() { Code = list.Any() ? "0X00" : "0X01", Data = list, Message = list.Any() ? $"{total}" : null, Success = list.Any() };
        }
        #endregion

    }
}
