using Dapper;
using Payroll.Common.EnumUtility;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Service
{
    public class GLGroupRepository : IGLGroupRepository
    {
        private readonly IDbConnection _dbConnection;
        public GLGroupRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<GlGroupRequest>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<GlGroupRequest>(procedureName, commandType: CommandType.StoredProcedure);
        }

        public async Task<GlGroupRequest> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QuerySingleOrDefaultAsync<GlGroupRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<GlGroupRequest>> GetParentOrSubGLGroupsAsync(string procedureName, int parentGLGroupId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Parent_GL_Group_Id", parentGLGroupId);
            parameters.Add("@IsActive", true); // Always fetch active records per SP logic

            return await _dbConnection.QueryAsync<GlGroupRequest>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<GlGroupRequest> AddAsync(string procedureName, GlGroupRequest model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@GL_Group_Id", 0); // For Insert
            parameters.Add("@Group_Name", model.Group_Name);
            parameters.Add("@Parent_GL_Group_Id", model.Parent_GL_Group_Id);
            parameters.Add("@Level", model.Level);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@UpdatedBy", 0); // Required by SP
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.GLGroup);

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }
            return model;
        }

        public async Task<GlGroupRequest> UpdateAsync(string procedureName, GlGroupRequest model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@GL_Group_Id", model.GL_Group_Id);
            parameters.Add("@Group_Name", model.Group_Name);
            parameters.Add("@Parent_GL_Group_Id", model.Parent_GL_Group_Id);
            parameters.Add("@Level", model.Level);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@CreatedBy", model.CreatedBy); // Optional, included based on your previous structure
            parameters.Add("@UpdatedBy", model.UpdatedBy ?? 0);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.GLGroup);

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }

            return model;
        }
        public async Task<GlGroupRequest> DeleteAsync(string procedureName, object parametersObj)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@GL_Group_Id", ((dynamic)parametersObj).GL_Group_Id);
            parameters.Add("@UpdatedBy", ((dynamic)parametersObj).UpdatedBy);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.GLGroup);

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                ((dynamic)parametersObj).StatusMessage = result.ApplicationMessage;
                ((dynamic)parametersObj).MessageType = result.ApplicationMessageType;
            }

            return (GlGroupRequest)parametersObj;
        }

    }

}
