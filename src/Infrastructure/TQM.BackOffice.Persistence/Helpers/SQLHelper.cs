
   

namespace TQM.BackOffice.Persistence.Helpers
{
    public static class SQLHelper
    {
        public static async Task<string> GetBookName(int PeriodId, string ReceiveBookCode, int Sequence)
        {
            string Result = ReceiveBookCode + "-" + PeriodId.ToString().XSubstring(3,4) + ":" + Sequence.ToString().PadLeft(5,'0');
            System.Threading.Thread.Sleep(1000);
            return await Task.FromResult(Result);
        }

        /// <summary>
        /// This Function will do DELETE FROM XININSURE.SYSCALLPROCEDURE By 3 Parameters <br />
        /// 1. ProcedureName : must assign. If not assign or Empty, throw error. <br />
        /// 2. DBLOGIN : (if not assign, System will automatic get DBUser of Login user like ACXXXXXX) <br />
        /// 3. HostName : (if not assign, System will automatic get Hostname by System.Environment.MachineName) <br />
        /// </summary>
        //public static async Task<Boolean> InsertSysCallProcedure(Oracle.ManagedDataAccess.Client.OracleConnection connection, 
        //                                                   IDBAdapter _dbAdapter,
        //                                                   SysCallProcedure sysCallObj, string DBLogin = "", string HostName = "")
        //{
        //    try
        //    {
        //        string query = "";
        //        string _DBLogin = (DBLogin == "" ? (_dbAdapter.GetStaffDBLoginCode() ?? throw new ArgumentNullException("DB Login Code")) : DBLogin);
        //        string _HostName = (HostName == "" ? System.Environment.MachineName : HostName);
        //        sysCallObj.ProcedureName = (sysCallObj.ProcedureName == null ? "" : sysCallObj.ProcedureName);

        //        if (sysCallObj.DBLogin == "")
        //            sysCallObj.DBLogin = _DBLogin;
        //        if (sysCallObj.HostName == "")
        //            sysCallObj.HostName = _HostName;

        //        if (sysCallObj.ProcedureName.Trim() != "")
        //        {
        //            var props = PropertiesHelper.GetNotNullProps(sysCallObj);
        //            DynamicParameters dynamicParameters = new();
        //            query = await _dbAdapter.GetInsertQueryStringFromProp(props,"XININSURE.SYSCALLPROCEDURE", dynamicParameters);
        //            await connection.ExecuteAsync(query, dynamicParameters);
        //            // query = @"
        //            //             INSERT INTO XININSURE.SYSCALLPROCEDURE
        //            //             (DBLOGIN, HOSTNAME, PROCEDURENAME, parameter1, parameter2, parameter3)
        //            //             VALUES
        //            //             (:DBLOGIN, :HOSTNAME, :PROCEDURENAME, :parameter1, :parameter2, :parameter3)
        //            //         ";
        //            // await connection.ExecuteAsync(query, new{
        //            //     DBLOGIN = DBSYSLOGIN,
        //            //     HOSTNAME = System.Environment.MachineName,
        //            //     PROCEDURENAME = "GENNEWSALEACTION", 
        //            //     parameter1 = SALEID.ToString(), 
        //            //     parameter2 = "381", 
        //            //     parameter3 = REMARK
        //            // });

        //            return true;
        //        }
        //        else
        //        {
        //            throw new Exception("InsertSysCallProcedure must assign ProcedureName");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public static async Task<bool> SYSUSERTEMPCLEAR(string wordSQL, Oracle.ManagedDataAccess.Client.OracleConnection connection,
                                                        IDBAdapter _dbAdapter,
                                                        string DBLogin = "", string HostName = "")
        {
            //using var connection = _dbAdapter.GetConn(DataAccessTypeEnum.ESM);
            try
            {
                string query = "";

                // query = @" SELECT SUBSTR(USERENV('TERMINAL'),1,32) FROM DUAL";
                // string Hostname = await connection.ExecuteScalarAsync<string>(query);

                // query = @" SELECT SUBSTR(USER,1, 16) FROM DUAL";
                // string DBName = await connection.ExecuteScalarAsync<string>(query);

                string _DBLogin = (DBLogin == "" ? (_dbAdapter.GetStaffDBLoginCode() ?? throw new ArgumentNullException("DB Login Code")) : DBLogin);
                string _HostName = (HostName == "" ? System.Environment.MachineName : HostName);


                query = @$" DELETE FROM XININSURE.SYSUSERTEMP 
                            WHERE DBLOGIN = :DBLOGIN 
                            AND HOSTNAME = :HOSTNAME 
                            AND " + wordSQL;
                var result = (await connection.ExecuteAsync(query, new
                {
                    DBLOGIN = _DBLogin,
                    HOSTNAME = _HostName
                }));

                return true;

            }
            catch (System.Exception)
            {
                throw;
            }       
        }
    
        //public static async Task<Boolean> InsertPayfile(Oracle.ManagedDataAccess.Client.OracleConnection connection, 
        //                                             IDBAdapter _dbAdapter,   
        //                                             PayFile classObj, string DBLogin = "")
        //{
        //    try
        //    {
        //        string query = "";
        //        string _DBLogin = (DBLogin == "" ? (_dbAdapter.GetStaffDBLoginCode() ?? throw new ArgumentNullException("DB Login Code")) : DBLogin);

        //        if (classObj.CreateUserId == "")
        //            classObj.CreateUserId = _DBLogin;

        //        if (classObj.FileCode.Trim() != "")
        //        {
        //            var props = PropertiesHelper.GetNotNullProps(classObj);
        //            DynamicParameters dynamicParameters = new();
        //            query = await _dbAdapter.GetInsertQueryStringFromProp(props,"XININSURE.PAYFILE", dynamicParameters);
        //            await connection.ExecuteAsync(query, dynamicParameters);
                    
        //            return true;
        //        }
        //        else
        //        {
        //            throw new Exception("InsertPayfile must assign FileCode");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public static async Task<Boolean> InsertPayfile_Suspense(Oracle.ManagedDataAccess.Client.OracleConnection connection, 
        //                                             IDBAdapter _dbAdapter,   
        //                                             Payfile_Suspense classObj, string DBLogin = "")
        //{
        //    try
        //    {
        //        string query = "";
        //        string _DBLogin = (DBLogin == "" ? (_dbAdapter.GetStaffDBLoginCode() ?? throw new ArgumentNullException("DB Login Code")) : DBLogin);

        //        if (classObj.CreateUserId == "")
        //            classObj.CreateUserId = _DBLogin;
        //        // if (classObj.CreateDateTime == null)
        //        //     classObj.CreateDateTime = DateTime.Now;
        //        if (classObj.LastUpdateUserId == "")
        //            classObj.LastUpdateUserId = _DBLogin;
        //        // if (classObj.LastUpdateDateTime == null)
        //        //     classObj.LastUpdateDateTime = DateTime.Now;

        //        if (classObj.FileCode.Trim() != "")
        //        {
        //            var props = PropertiesHelper.GetNotNullProps(classObj);
        //            DynamicParameters dynamicParameters = new();
        //            query = await _dbAdapter.GetInsertQueryStringFromProp(props,"XININSURE.PAYFILE_SUSPENSE", dynamicParameters);
        //            //await connection.ExecuteAsync(query, dynamicParameters);
                    
        //            return true;
        //        }
        //        else
        //        {
        //            throw new Exception("InsertPayfile_Suspense must assign FileCode");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    
        //public static async Task<Boolean> UpdateReceiveItem(Oracle.ManagedDataAccess.Client.OracleConnection connection, 
        //                                             IDBAdapter _dbAdapter,   
        //                                             ReceiveItemSubmitRequest classObjUpdatePart,
        //                                             ReceiveItemSubmitRequest classObjWherePart,
        //                                             string DBLogin = "")
        //{
        //    try
        //    {
        //        string query = "";
        //        string _DBLogin = (DBLogin == "" ? (_dbAdapter.GetStaffDBLoginCode() ?? throw new ArgumentNullException("DB Login Code")) : DBLogin);

        //        // if (classObj.CreateUserId == "")
        //        //     classObj.CreateUserId = _DBLogin;
        //        // if (classObj.CreateDateTime == null)
        //        //     classObj.CreateDateTime = DateTime.Now;
        //        if (classObjUpdatePart.LastUpdateUserId == "")
        //            classObjUpdatePart.LastUpdateUserId = _DBLogin;
        //        if (classObjUpdatePart.LastUpdateDateTime == null)
        //            classObjUpdatePart.LastUpdateDateTime = DateTime.Now;

        //        // Get Value of Current 
        //        query = @"SELECT *
        //                  FROM XININSURE.RECEIVEITEM
        //                  WHERE RECEIVEID = :RECEIVEID
        //                  AND RECEIVEITEM = :RECEIVEITEM
        //                ";
        //        var tempReceiveItem = await connection.QueryFirstOrDefaultAsync<ReceiveItemSubmitRequest>(query, new {
        //            RECEIVEID = classObjWherePart.ReceiveId,
        //            RECEIVEITEM = classObjWherePart.ReceiveItem
        //        });


        //        //var propsUpdate = PropertiesHelper.GetNotNullProps_ForUpdate(classObjUpdatePart, new ReceiveItemSubmitRequest());
        //        var propsUpdate = PropertiesHelper.GetNotNullProps_ForUpdate(classObjUpdatePart, tempReceiveItem);
        //        var propsWhere = PropertiesHelper.GetNotNullProps_ForUpdate(classObjWherePart, new ReceiveItemSubmitRequest());

        //        List<string> noUpdateParameter = (new string[]{"ReceiveId","ReceiveItem","CreateDateTime", "CreateUserId", "LastUpdateDateTime", "LastUpdateUserId"}).ToList();
        //        propsUpdate.RemoveAll( x => noUpdateParameter.Contains(x.Item1));

        //        DynamicParameters dynamicParameters = new();
        //        query = await _dbAdapter.GetUpdateQueryStringFromProp(propsUpdate, propsWhere, "XININSURE.RECEIVEITEM", dynamicParameters);
        //        await connection.ExecuteAsync(query, dynamicParameters);
                
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    
        public static async Task<string> GetDBStaffFromStaffCode(Oracle.ManagedDataAccess.Client.OracleConnection connection, 
                                                     IDBAdapter _dbAdapter, string StaffCode)
        {
            try
            {
                string query = "";
                query = @"
                            SELECT DBLOGIN 
                            FROM
                            (   
                                SELECT SU.DBLOGIN
                                FROM XININSURE.STAFF ST
                                JOIN XININSURE.SYSUSER SU
                                ON ST.STAFFID = SU.STAFFID
                                WHERE SU.USERSTATUS = 'Y'
                                AND ST.STAFFCODE = :STAFFCODE
                                ORDER BY SU.LASTCHANGE DESC, SU.LASTUPDATEDATETIME DESC, SU.CREATEDATETIME DESC
                            )
                            WHERE ROWNUM <= 1
                        ";
                string _DBLogin = await connection.ExecuteScalarAsync<string>(query, new{
                    STAFFCODE = StaffCode
                });

                if (_DBLogin == "")
                {
                    throw new Exception($"ไม่พบ DB Name ของผู้ใช้ {StaffCode} ที่สถานะ Y");
                }


                return _DBLogin;
            }
            catch (Exception)
            {
                throw;
            }
        }
    

        public static async Task<int> GetStaffLoginCompanyId(Oracle.ManagedDataAccess.Client.OracleConnection connection, 
                                                     IDBAdapter _dbAdapter, string StaffCode = "")
        {
            try
            {
                string query = "";
                string _StaffCode = _dbAdapter.GetStaffCode() ?? "";
                string _DBLOGIN = _dbAdapter.GetStaffDBLoginCode() ?? throw new ArgumentNullException("DB Login Code");
                string _HOSTNAME = System.Environment.MachineName;

                if (StaffCode != "")
                {
                    // Find Staff code based on input
                    _DBLOGIN = await GetDBStaffFromStaffCode(connection, _dbAdapter, StaffCode);
                }

                query = @"
                            SELECT TEMPNUMBER
                            FROM XININSURE.SYSUSERTEMP
                            WHERE TEMPNAME = 'COMPANYID'
                            AND DBLOGIN = :DBLOGIN
                            AND HOSTNAME = :HOSTNAME
                        ";
                int companyId = await connection.ExecuteScalarAsync<int>(query, new{
                    DBLOGIN = _DBLOGIN,
                    HOSTNAME = _HOSTNAME
                });

                return companyId;
            }
            catch (Exception)
            {
                throw;
            }
        }
    
        public static async Task<Boolean> InsertTableDynamic<T>(Oracle.ManagedDataAccess.Client.OracleConnection connection, 
                                                     IDBAdapter _dbAdapter,   
                                                     T classObj, 
                                                     T classOrig,
                                                     string TableNameAndSchema)
        {
            try
            {
                string query = "";
                //string _DBLogin = (DBLogin == "" ? (_dbAdapter.GetStaffDBLoginCode() ?? throw new ArgumentNullException("DB Login Code")) : DBLogin);

                // if (classObj.CreateUserId == "")
                //     classObj.CreateUserId = _DBLogin;
                
                var propsOrig = PropertiesHelper.GetNotNullProps_ForUpdate(classObj, classOrig);

                DynamicParameters dynamicParameters = new();
                query = await _dbAdapter.GetInsertQueryStringFromProp2(propsOrig ,TableNameAndSchema, dynamicParameters);
                await connection.ExecuteAsync(query, dynamicParameters);
                
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> sourceList, int nSize=30)  
        {        
            for (int i = 0; i < sourceList.Count; i += nSize) 
            { 
                yield return sourceList.GetRange(i, Math.Min(nSize, sourceList.Count - i)); 
            }  
        }

        public static string GetRawQueries(string PathSQL, string ProjectName, string Queries)
        {
            try
            {



                string projectDirectory = Environment.CurrentDirectory;

                string Querie = File.ReadAllText(Path.Combine(projectDirectory, PathSQL, ProjectName, Queries));

                return Querie;
            }
            catch
            {
                throw new Exception("ไม่เจอไฟล์อ่าน SQL");
            }
        }


    }
}
