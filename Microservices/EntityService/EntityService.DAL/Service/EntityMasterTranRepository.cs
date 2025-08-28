using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.EnumUtility;
using EntityService.BAL.Models;
using EntityService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EntityService.DAL.Service
{
    public class EntityMasterTranRepository : IEntityMasterTranRepository
    {
        #region Constructor 
        private readonly IDbConnection _dbConnection;
        public EntityMasterTranRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<EntityMasterAssignWage> AssignWageToEntityAsync(string procedureName, EntityMasterAssignWage model)
        {
            var parameters = new DynamicParameters();

            // Add parameters
            parameters.Add("@UpdatedBy", ((dynamic)model).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.WageConfigMaster); // Cast enum to int

            parameters.Add("@Employee_Id", model.Employee_Id, DbType.Int64);
            parameters.Add("@Wage_Id", model.Wage_Id, DbType.Int64);

            try
            {
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                if (result != null)
                {
                    model.StatusMessage = result.ApplicationMessage;
                    model.MessageType = result.ApplicationMessageType;
                }
                return model;

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        #endregion

        #region Test DrowpDown Values With Caching
        public List<SelectListItem> GetWageGradesForDropdown()
        {
            // SQL query to fetch Wage_Id and WageGradeCode
            string sql = "SELECT Wage_Id, WageGradeCode FROM tbl_mst_wagegrade";

            // Execute the query and map the result
            var wageItems = _dbConnection.Query(sql).Select(w =>
                new SelectListItem
                {
                    Value = w.Wage_Id.ToString(),  // Wage_Id as value
                    Text = w.WageGradeCode         // WageGradeCode as text
                }).ToList();

            return wageItems;
        }
        #endregion
    }
}