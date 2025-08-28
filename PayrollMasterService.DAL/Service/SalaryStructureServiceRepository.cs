using Dapper;
using Payroll.Common.EnumUtility;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Service
{
    public class SalaryStructureServiceRepository : ISalaryStructureRepository
    {
        private readonly IDbConnection _dbConnection;
        public SalaryStructureServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region Pay Component Master Crud
        public async Task<IEnumerable<SalaryStructureDTO>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<SalaryStructureDTO>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<SalaryStructureGrid>> GetAllByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<SalaryStructureGrid>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        //public async Task<SalaryStructureDTO> GetByIdAsync(string procedureName, object parameters)
        //{
        //    var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
        //    return await _dbConnection.QuerySingleOrDefaultAsync<SalaryStructureDTO>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        //}

        public async Task<SalaryStructureDTO> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            using (var multi = await _dbConnection.QueryMultipleAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure))
            {
                // Read the first result set (SalaryStructure header)
                var salaryStructure = await multi.ReadFirstOrDefaultAsync<SalaryStructureDTO>();

                if (salaryStructure != null)
                {
                    // Read the second result set (SalaryStructure details)
                    salaryStructure.SalaryStructureDetails = (await multi.ReadAsync<SalaryStructureDetailDTO>()).ToList();
                }

                return salaryStructure;
            }
        }

        public async Task<SalaryStructureDTO> AddAsync(string procedureName, SalaryStructureDTO objSalaryStructure)
        {
            // Executing the stored procedure with the given parameters
            var Parameters = new DynamicParameters();
            Parameters.Add("@SalaryStructure_Hdr_Id", objSalaryStructure.SalaryStructure_Hdr_Id, DbType.Int32);
            Parameters.Add("@FinancialYr_Id", objSalaryStructure.FinancialYr_Id, DbType.Int32);
            Parameters.Add("@Company_Id", objSalaryStructure.Company_Id, DbType.Int32);
            Parameters.Add("@Correspondance_ID", objSalaryStructure.Correspondance_ID, DbType.Int32);
            Parameters.Add("@SalaryStructureName", objSalaryStructure.SalaryStructureName, DbType.String);
            Parameters.Add("@EffectiveFrom", objSalaryStructure.EffectiveFrom, DbType.DateTime);
            Parameters.Add("@Effective_To", objSalaryStructure.Effective_To, DbType.DateTime);
            Parameters.Add("@SalaryFrequency", objSalaryStructure.SalaryFrequency, DbType.Int32);
            Parameters.Add("@PayGradeConfig_Id", objSalaryStructure.PayGradeConfig_Id, DbType.Int32);
            Parameters.Add("@MinSalary", objSalaryStructure.MinSalary, DbType.Decimal);
            Parameters.Add("@MaxSalary", objSalaryStructure.MaxSalary, DbType.Decimal);
            Parameters.Add("@SalaryBasis", objSalaryStructure.SalaryBasis, DbType.Int32);
            Parameters.Add("@IsActive", objSalaryStructure.IsActive, DbType.Boolean);
            Parameters.Add("@CreatedBy", objSalaryStructure.CreatedBy, DbType.Int32);

            // Additional parameters for messages and status using enum values
            Parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            Parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            Parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.SalaryStructureConfiguration); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, Parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                objSalaryStructure.StatusMessage = result.ApplicationMessage;
                objSalaryStructure.MessageType = ((dynamic)result).ApplicationMessageType;
            }
            return objSalaryStructure;
        }
        public async Task<SalaryStructureDTO> UpdateAsync(string procedureName, SalaryStructureDTO objSalaryStructure)
        {
            // Executing the stored procedure with the given parameters
            var Parameters = new DynamicParameters();
            Parameters.Add("@SalaryStructure_Hdr_Id", objSalaryStructure.SalaryStructure_Hdr_Id, DbType.Int32);
            Parameters.Add("@FinancialYr_Id", objSalaryStructure.FinancialYr_Id, DbType.Int32);
            Parameters.Add("@Company_Id", objSalaryStructure.Company_Id, DbType.Int32);
            Parameters.Add("@Correspondance_ID", objSalaryStructure.Correspondance_ID, DbType.Int32);
            Parameters.Add("@SalaryStructureName", objSalaryStructure.SalaryStructureName, DbType.String);
            Parameters.Add("@EffectiveFrom", objSalaryStructure.EffectiveFrom, DbType.DateTime);
            Parameters.Add("@Effective_To", objSalaryStructure.Effective_To, DbType.DateTime);
            Parameters.Add("@SalaryFrequency", objSalaryStructure.SalaryFrequency, DbType.Int32);
            Parameters.Add("@PayGradeConfig_Id", objSalaryStructure.PayGradeConfig_Id, DbType.Int32);
            Parameters.Add("@MinSalary", objSalaryStructure.MinSalary, DbType.Decimal);
            Parameters.Add("@MaxSalary", objSalaryStructure.MaxSalary, DbType.Decimal);
            Parameters.Add("@SalaryBasis", objSalaryStructure.SalaryBasis, DbType.Int32);
            Parameters.Add("@IsActive", objSalaryStructure.IsActive, DbType.Boolean);

            Parameters.Add("@CreatedBy", objSalaryStructure.CreatedBy, DbType.Int32);
            Parameters.Add("@UpdatedBy", objSalaryStructure.UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            Parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            Parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            Parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.SalaryStructureConfiguration); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, Parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                objSalaryStructure.StatusMessage = result.ApplicationMessage;
                objSalaryStructure.MessageType = result.ApplicationMessageType;
            }
            return objSalaryStructure;
        }

        public async Task<SalaryStructureDTO> DeleteAsync(string procedureName, object objSalaryStructure)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SalaryStructure_Hdr_Id", ((dynamic)objSalaryStructure).SalaryStructure_Hdr_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)objSalaryStructure).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.SalaryStructureConfiguration); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)objSalaryStructure).StatusMessage = result.ApplicationMessage;
                ((dynamic)objSalaryStructure).MessageType = result.ApplicationMessageType;
            }
            return (SalaryStructureDTO)objSalaryStructure;
        }

        public async Task<SalaryStructureDTO> AddUpdateSalaryStructure(string procedureName, SalaryStructureDTO objSalaryStructure)
        {
            // Executing the stored procedure with the given parameters
            var Parameters = new DynamicParameters();
            Parameters.Add("@SalaryStructure_Hdr_Id", objSalaryStructure.SalaryStructure_Hdr_Id, DbType.Int32);
            Parameters.Add("@FinancialYr_Id", objSalaryStructure.FinancialYr_Id, DbType.Int32);
            Parameters.Add("@Company_Id", objSalaryStructure.Company_Id, DbType.Int32);
            Parameters.Add("@Correspondance_ID", objSalaryStructure.Correspondance_ID, DbType.Int32);
            Parameters.Add("@SalaryStructureName", objSalaryStructure.SalaryStructureName, DbType.String);
            Parameters.Add("@EffectiveFrom", objSalaryStructure.EffectiveFrom, DbType.DateTime);
            Parameters.Add("@Effective_To", objSalaryStructure.Effective_To, DbType.DateTime);
            Parameters.Add("@SalaryFrequency", objSalaryStructure.SalaryFrequency, DbType.Int32);
            Parameters.Add("@PayGradeConfig_Id", objSalaryStructure.PayGradeConfig_Id, DbType.Int32);
            Parameters.Add("@MinSalary", objSalaryStructure.MinSalary, DbType.Decimal);
            Parameters.Add("@MaxSalary", objSalaryStructure.MaxSalary, DbType.Decimal);
            Parameters.Add("@SalaryBasis", objSalaryStructure.SalaryBasis, DbType.Int32);
            Parameters.Add("@IsActive", objSalaryStructure.IsActive, DbType.Boolean);
            Parameters.Add("@CreatedBy", objSalaryStructure.CreatedBy, DbType.Int32);


            // Add parameter for the detail table
            var detailTable = new DataTable();
            detailTable.Columns.Add("SalaryStructure_Dtl_Id", typeof(int));
            detailTable.Columns.Add("SalaryStructure_Hdr_Id", typeof(int));
            detailTable.Columns.Add("EarningDeductionID", typeof(int));
            detailTable.Columns.Add("SubEarningDeductionID", typeof(int));
            detailTable.Columns.Add("EarningDeductionType", typeof(int));
            detailTable.Columns.Add("CalculationType", typeof(int));
            detailTable.Columns.Add("EarningDeductionValue", typeof(decimal));
            detailTable.Columns.Add("Formula_ID", typeof(int));
            detailTable.Columns.Add("ComponentSequence", typeof(int));
            detailTable.Columns.Add("FormulaContent", typeof(string));
            detailTable.Columns.Add("IStaxable", typeof(bool));
            detailTable.Columns.Add("Remarks", typeof(string));
            detailTable.Columns.Add("IsActive", typeof(bool));
            detailTable.Columns.Add("UpdatedDate", typeof(DateTime));
            detailTable.Columns.Add("IsDeleted", typeof(bool));
           

            foreach (var detail in objSalaryStructure.SalaryStructureDetails)
            {
                detailTable.Rows.Add(
                    detail.SalaryStructure_Dtl_Id,
                    detail.SalaryStructure_Hdr_Id,
                    detail.EarningDeductionID,
                    detail.SubEarningDeductionID,
                     detail.EarningDeductionType,
                    detail.CalculationType,
                    detail.EarningDeductionValue,
                    detail.ComponentSequence,
                    detail.Formula_ID,
                    detail.FormulaContent,                    
                    detail.IStaxable,
                    detail.Remarks ?? "",
                    detail.IsActive,
                    detail.UpdatedDate,
                    detail.IsDeleted
                    
                   
                );
            }

            Parameters.Add("@SalaryStructureDtl", detailTable.AsTableValuedParameter("dbo.udt_salarystructure_dtl"));
            // Additional parameters for messages and status using enum values
            Parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            Parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            Parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.SalaryStructureConfiguration); // Cast enum to int


            try
            {
              var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, Parameters, commandType: CommandType.StoredProcedure);
                if (result != null)
                {
                    objSalaryStructure.StatusMessage = result.ApplicationMessage;
                    objSalaryStructure.MessageType = result.ApplicationMessageType;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception("Failed to save salary structure.", ex);
            }

            return objSalaryStructure;
        }

        //public async Task<SalarySimulatorDTO> CalculateSalaryStructure1(string procedureName, SalarySimulatorDTO objSalaryStructure)
        //{
        //    // Executing the stored procedure with the given parameters
        //    var Parameters = new DynamicParameters();
        //    Parameters.Add("@SalaryStructure_Hdr_Id", objSalaryStructure.SalaryStructure_Hdr_Id, DbType.Int32);
        //    Parameters.Add("@FinancialYr_Id", objSalaryStructure.FinancialYr_Id, DbType.Int32);
        //    Parameters.Add("@Company_Id", objSalaryStructure.Company_Id, DbType.Int32);
        //    Parameters.Add("@Correspondance_ID", objSalaryStructure.Correspondance_ID, DbType.Int32);
        //    Parameters.Add("@SalaryStructureName", objSalaryStructure.SalaryStructureName, DbType.String);
        //    Parameters.Add("@EffectiveFrom", objSalaryStructure.EffectiveFrom, DbType.DateTime);
        //    Parameters.Add("@Effective_To", objSalaryStructure.Effective_To, DbType.DateTime);
        //    Parameters.Add("@SalaryFrequency", objSalaryStructure.SalaryFrequency, DbType.Int32);
        //    Parameters.Add("@PayGradeConfig_Id", objSalaryStructure.PayGradeConfig_Id, DbType.Int32);
        //    Parameters.Add("@MinSalary", objSalaryStructure.MinSalary, DbType.Decimal);
        //    Parameters.Add("@MaxSalary", objSalaryStructure.MaxSalary, DbType.Decimal);
        //    Parameters.Add("@SalaryBasis", objSalaryStructure.SalaryBasis, DbType.Int32);
        //    Parameters.Add("@IsActive", objSalaryStructure.IsActive, DbType.Boolean);
        //    Parameters.Add("@CreatedBy", objSalaryStructure.CreatedBy, DbType.Int32);
        //    Parameters.Add("@SimulatedAmount", objSalaryStructure.SimulatedAmount, DbType.Decimal);

        //    // Add parameter for the detail table
        //    var detailTable = new DataTable();
        //    detailTable.Columns.Add("SalaryStructure_Hdr_Id", typeof(int));
        //    detailTable.Columns.Add("EarningDeductionID", typeof(int));
        //    detailTable.Columns.Add("SubEarningDeductionID", typeof(int));
        //    detailTable.Columns.Add("EarningDeductionType", typeof(int));
        //    detailTable.Columns.Add("CalculationType", typeof(int));
        //    detailTable.Columns.Add("EarningDeductionValue", typeof(decimal));
        //    detailTable.Columns.Add("Formula_ID", typeof(int));
        //    detailTable.Columns.Add("ComponentSequence", typeof(int));
        //    detailTable.Columns.Add("FormulaContent", typeof(string));
        //    detailTable.Columns.Add("IStaxable", typeof(bool));
        //    detailTable.Columns.Add("Remarks", typeof(string));
        //    detailTable.Columns.Add("IsActive", typeof(bool));
        //    detailTable.Columns.Add("UpdatedDate", typeof(DateTime));
        //    detailTable.Columns.Add("IsDeleted", typeof(bool));


        //    foreach (var detail in objSalaryStructure.SalarySimulatorDetails)
        //    {
        //        detailTable.Rows.Add(
        //            detail.SalaryStructure_Hdr_Id,
        //            detail.EarningDeductionID,
        //            detail.SubEarningDeductionID,
        //             detail.EarningDeductionType,
        //            detail.CalculationType,
        //            detail.EarningDeductionValue,
        //            detail.ComponentSequence,
        //            detail.Formula_ID,
        //            detail.FormulaContent,
        //            detail.IStaxable,
        //            detail.Remarks ?? "",
        //            detail.IsActive,
        //            detail.UpdatedDate,
        //            detail.IsDeleted


        //        );
        //    }

        //    Parameters.Add("@SalaryStructureDtl", detailTable.AsTableValuedParameter("dbo.udt_salarystructure_dtl"));
        //    // Additional parameters for messages and status using enum values
        //    Parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
        //    Parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
        //    Parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.SalaryStructureConfiguration); // Cast enum to int


        //    try
        //    {
        //        var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, Parameters, commandType: CommandType.StoredProcedure);
        //        if (result != null)
        //        {
        //            objSalaryStructure.StatusMessage = result.ApplicationMessage;
        //            objSalaryStructure.MessageType = result.ApplicationMessageType;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle the exception
        //        throw new Exception("Failed to save salary structure.", ex);
        //    }

        //    return objSalaryStructure;
        //}
        public async Task<SalarySimulatorDTO> CalculateSalaryStructure(string procedureName, SalarySimulatorDTO simulatorDTO)
        {
            var parameters = new DynamicParameters();

            // Create the detail UDT table matching the new UDT structure
            var simulationTable = new DataTable();
            simulationTable.Columns.Add("SalaryComponentId", typeof(int));
            simulationTable.Columns.Add("PayComponentType", typeof(int));
            simulationTable.Columns.Add("PayComponentName", typeof(string));
            simulationTable.Columns.Add("CalculationType", typeof(int));
            simulationTable.Columns.Add("ComponentSequence", typeof(int));
            simulationTable.Columns.Add("Formula_ID", typeof(int));
            simulationTable.Columns.Add("Formula", typeof(string));

            foreach (var item in simulatorDTO.SalarySimulatorDetails)
            {
                simulationTable.Rows.Add(
                    item.SalaryComponentId,
                    item.PayComponentType,
                    item.PayComponentName,
                    item.CalculationType,
                    item.ComponentSequence,
                    item.Formula_ID,
                    item.Formula
                );
            }

            // Add parameters
            parameters.Add("@SalaryComponents", simulationTable.AsTableValuedParameter("dbo.udt_salarysimulation"));
            parameters.Add("@Ctc", simulatorDTO.ctc, DbType.Decimal);
            parameters.Add("@PayrollMonth", simulatorDTO.payrollMonth, DbType.Int32);

            try
            {
                //var result = await _dbConnection.QueryAsync<dynamic>(
                //    procedureName, parameters, commandType: CommandType.StoredProcedure);

                //// Fill the returned values back into the model (optional)
                //var computedDetails = result.ToList();

                //simulatorDTO.SalarySimulatorDetails = computedDetails.Select(x => new SalarySimulatorDetailDTO
                //{
                //    SalaryComponentId = x.SalaryComponentId,
                //    PayComponentType = x.PayComponentType,
                //    PayComponentName = x.PayComponentName,
                //    ComputedValue = x.ComputedValue,
                //    Employer_Contribution = x.Employer_Contribution,
                //    Employee_Contribution = x.Employee_Contribution
                //}).ToList();

                var dynamicParameters = new DynamicParameters(parameters);

                using (var multi = await _dbConnection.QueryMultipleAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure))
                {
                    // Read the first result set (SalaryStructure header)
                    var salarySimulator = await multi.ReadAsync<SalarySimulatorDetailDTO>();
                    var computedDetails = salarySimulator.ToList();

                    simulatorDTO.SalarySimulatorDetails = computedDetails.Select(x => new SalarySimulatorDetailDTO
                    {
                        SalaryComponentId = x.SalaryComponentId,
                        PayComponentType = x.PayComponentType,
                        PayComponentName = x.PayComponentName,
                        ComputedValue = x.ComputedValue,
                        Employer_Contribution = x.Employer_Contribution,
                        Employee_Contribution = x.Employee_Contribution
                    }).ToList();

                    var salarySimulatorTotal = await multi.ReadAsync<SalarySimulatorTotalDTO>();
                    var computedDetailsTotal = salarySimulatorTotal.ToList();

                    simulatorDTO.SalarySimulatorTotal = computedDetailsTotal.Select(x => new SalarySimulatorTotalDTO
                    {
                        TotalEarnings = x.TotalEarnings,
                        TotalDeduction = x.TotalDeduction,
                        NetSalary = x.NetSalary,
                        EmployerContribution = x.EmployerContribution
                    }).ToList();
                    
                }


            }
            catch (Exception ex)
            {
                throw new Exception("Error while calculating salary simulation.", ex);
            }

            return simulatorDTO;
        }

      

        #endregion
    }
}
