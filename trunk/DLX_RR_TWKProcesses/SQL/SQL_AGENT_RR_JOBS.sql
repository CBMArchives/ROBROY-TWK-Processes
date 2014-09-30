USE [msdb]
GO

/****** Object:  Job [RR_DM_PROCESSES]    Script Date: 10/14/2012 08:47:55 ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]]    Script Date: 10/14/2012 08:47:55 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'RR_DM_PROCESSES', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'No description available.', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'dmclient', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [VENDOR GL MASTER FILE]    Script Date: 10/14/2012 08:47:56 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'VENDOR GL MASTER FILE', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'declare @xdoc as xml
set @xdoc = N''
<procedures initialstatus="pending" status_error="error_aborted" status_onretry="error_willretry" status_onsuccess="all_completed">
  <procedure type="DLX_IMPORTS" runorder="1" status="pending">
    <actions>
	<action type="process_Vendor_GL_Master" description="process_Vendor_GL_Master" runorder="1" status="pending">
		  <connectionstrings>
			<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
		  </connectionstrings>
		  <parameters>
			<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
			<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
			<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
			<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
			<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
			<parameter name="drawername" value="Accounts Payable" />
			<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
			<parameter name="documenttype" value="PurchaseRequest" />
			<parameter name="workflowstatus" value="NEW" />
			<parameter name="workflowapprover" value="DM" />
			<parameter name="tokuser_username" value="wfprocessing" />
			<parameter name="tokuser_password" value="wfprocessing" />
			<parameter name="process_file_name" value="vnglacct.idx" />
			<parameter name="process_sp_name" value="" />
			<parameter name="file_startswith" value="vnglacct" />
			<parameter name="file_regex" value="^([Vv][Nn][Gg][Ll][Aa][Cc][Cc][Tt])(.[Ii][Dd][Xx])$" />
			<parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
		  </parameters>
		</action>
 </actions>
  </procedure>
</procedures>''

insert into TWK_RunProcedures
(name,source, priority, currentstatus, procsXML, maxretrys, IsComplete, AddedBy, AddedOn)
values (''dlx_rr_chess_procs_VendorGLMasterFile'',''auto'',1,''pending'',@xdoc, 10, 0,  ''DLX-SYSTEM'', GETDATE())', 
		@database_name=N'DocumentManager', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [EMAIL PPR Notices]    Script Date: 10/14/2012 08:47:56 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'EMAIL PPR Notices', 
		@step_id=2, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'declare @xdoc as xml
set @xdoc = N''
<procedures initialstatus="pending" status_error="error_aborted" status_onretry="error_willretry" status_onsuccess="all_completed">
   <procedure type="DLX_IMPORTS" runorder="1" status="pending">
      <actions>
	<action type="process_Email_PPR_Notices" description="process_Email_PPR_Notices" runorder="1" status="pending">
		<connectionstrings>
          			<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
          			<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
        		</connectionstrings>
        		<parameters>
			<parameter name="expire_minutes" value="120" />
			<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
         			<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
          			<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
          			<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
          			<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
          			<parameter name="drawername" value="Accounts Payable" />
          			<parameter name="tokopen_realm" value="documentmanager" />
          			<parameter name="tokopen_archivename" value="documentmanager" />
          			<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
          			<parameter name="documenttype" value="" />
          			<parameter name="workflowstatus" value="" />
          			<parameter name="workflowapprover" value="" />
          			<parameter name="tokuser_username" value="" />
          			<parameter name="tokuser_password" value="" />
          			<parameter name="process_sp_name" value="dlx_RR_Email_PPR_Notices" />
          			<parameter name="file_startswith" value="" />
          			<parameter name="file_regex" value="" />
			<parameter name="smtp_host" value="veronaexchange.robroy.net" />
          			<parameter name="smtp_userid" value="chessimp" />
          			<parameter name="smtp_password" value="nc3kw96a" />
			<parameter name="smtp_port" value="25" />
			<parameter name="smtp_fromaddress" value="APWorkflow@robroy.com" />
			<parameter name="smtp_toaddress_override" value="" />
		</parameters>
	</action>
      </actions>
   </procedure>
</procedures>''

insert into TWK_RunProcedures
(name,source, priority, currentstatus, procsXML, maxretrys, IsComplete, AddedBy, AddedOn)
values (''process_Email_PPR_Notices'',''auto'',1,''pending'',@xdoc, 10, 0,  ''DLX-SYSTEM'', GETDATE())', 
		@database_name=N'DocumentManager', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [EMAIL PO Supplier Notices]    Script Date: 10/14/2012 08:47:56 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'EMAIL PO Supplier Notices', 
		@step_id=3, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'declare @xdoc as xml
set @xdoc = N''
<procedures initialstatus="pending" status_error="error_aborted" status_onretry="error_willretry" status_onsuccess="all_completed">
   <procedure type="DLX_IMPORTS" runorder="1" status="pending">
      <actions>
	<action type="process_Email_PO_Notices" description="process_Email_PO_Notices" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="" />
				<parameter name="workflowstatus" value="" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="" />
				<parameter name="tokuser_password" value="" />
				<parameter name="process_sp_name" value="dlx_RR_EmailRequests_POToSupplier" />
				<parameter name="file_startswith" value="" />
				<parameter name="file_regex" value="" />
          				<parameter name="smtp_host" value="veronaexchange.robroy.net" />
          				<parameter name="smtp_userid" value="chessimp" />
          				<parameter name="smtp_password" value="nc3kw96a" />
				<parameter name="smtp_port" value="25" />
				<parameter name="smtp_fromaddress" value="APWorkflow@robroy.com" />
				<parameter name="smtp_toaddress_override" value="" />
			</parameters>
		</action>
      </actions>
   </procedure>
</procedures>''

insert into TWK_RunProcedures
(name,source, priority, currentstatus, procsXML, maxretrys, IsComplete, AddedBy, AddedOn)
values (''dlx_rr_chess_procs_VendorGLMasterFile'',''auto'',1,''pending'',@xdoc, 10, 0,  ''DLX-SYSTEM'', GETDATE())
', 
		@database_name=N'DocumentManager', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [NON-PO PDF INV Imports]    Script Date: 10/14/2012 08:47:56 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'NON-PO PDF INV Imports', 
		@step_id=4, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'declare @xdoc as xml
set @xdoc = N''
<procedures initialstatus="pending" status_error="error_aborted" status_onretry="error_willretry" status_onsuccess="all_completed">
   <procedure type="DLX_IMPORTS" runorder="1" status="pending">
      <actions>
	<action type="process_Invoices_PDF" description="process_Invoices_PDF" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office" />
				
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="documenttype" value="AP Inv" />
				<parameter name="workflowstatus" value="scanned" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_filename" value="" />
				<parameter name="process_sp_name" value="" />
			          	<parameter name="file_startswith" value="InvBkUp_" />
				<parameter name="file_regex" value="^(([Ii][Nn][Vv][Bb][Kk][Uu][Pp])|([Ii][Nn][Vv]))(_{1})([A-Za-z0-9_-]{0,20})(.[Ii][Dd][Xx])$" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
			</parameters>
		</action>	
      </actions>
   </procedure>
</procedures>''

insert into TWK_RunProcedures
(name,source, priority, currentstatus, procsXML, maxretrys, IsComplete, AddedBy, AddedOn)
values (''dlx_rr_chess_procs_nonpoXLSImports'',''auto'',1,''pending'',@xdoc, 10, 0,  ''DLX-SYSTEM'', GETDATE())

', 
		@database_name=N'DocumentManager', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [NON-PO XLS Imports]    Script Date: 10/14/2012 08:47:56 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'NON-PO XLS Imports', 
		@step_id=5, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'declare @xdoc as xml
set @xdoc = N''
<procedures initialstatus="pending" status_error="error_aborted" status_onretry="error_willretry" status_onsuccess="all_completed">
   <procedure type="DLX_IMPORTS" runorder="1" status="pending">
      <actions>
	<action type="process_Invoices_XLS" description="process_Invoices_XLS" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office\XLSImports" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office" />
				
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="documenttype" value="AP Inv" />
				<parameter name="workflowstatus" value="scanned" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_filename" value="" />
				<parameter name="process_sp_name" value="" />
				<parameter name="file_startswith" value="" />
				<parameter name="file_regex" value="^(([A-Za-z0-9_-]{1,40})[.][Xx][Ll][Ss])$" />
			<parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
			</parameters>
		</action>	
      </actions>
   </procedure>
</procedures>''

insert into TWK_RunProcedures
(name,source, priority, currentstatus, procsXML, maxretrys, IsComplete, AddedBy, AddedOn)
values (''dlx_rr_chess_procs_nonpoXLSImports'',''auto'',1,''pending'',@xdoc, 10, 0,  ''DLX-SYSTEM'', GETDATE())

', 
		@database_name=N'DocumentManager', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [SalesInv Commission]    Script Date: 10/14/2012 08:47:56 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'SalesInv Commission', 
		@step_id=6, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'declare @xdoc as xml
set @xdoc = N''
<procedures initialstatus="pending" status_error="error_aborted" status_onretry="error_willretry" status_onsuccess="all_completed">
   <procedure type="DLX_IMPORTS" runorder="1" status="pending">
      <actions>
	<action type="process_SalesInv_Commision" description="process_SalesInv_Commision" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office" />
				
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="documenttype" value="Sales Comm Invoice" />
				<parameter name="workflowstatus" value="scanned" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_filename" value="" />
				<parameter name="process_sp_name" value="" />
			          	<parameter name="file_startswith" value="SalesComm_" />
				<parameter name="file_regex" value="^([Ss][Aa][Ll][Ee][Ss][Cc][Oo][Mm][Mm])(_){1}([A-Za-z0-9_-]{0,20})(.[Ii][Dd][Xx])$" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
			</parameters>
		</action>	
      </actions>
   </procedure>
</procedures>''

insert into TWK_RunProcedures
(name,source, priority, currentstatus, procsXML, maxretrys, IsComplete, AddedBy, AddedOn)
values (''process_SalesInv_Commision'',''auto'',1,''pending'',@xdoc, 10, 0,  ''DLX-SYSTEM'', GETDATE())

', 
		@database_name=N'DocumentManager', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Commission File By Rep]    Script Date: 10/14/2012 08:47:56 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Commission File By Rep', 
		@step_id=7, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'declare @xdoc as xml
set @xdoc = N''
<procedures initialstatus="pending" status_error="error_aborted" status_onretry="error_willretry" status_onsuccess="all_completed">
   <procedure type="DLX_IMPORTS" runorder="1" status="pending">
      <actions>
		<action type="process_CommissionFileByRep" description="process_CommissionFileByRep" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
          				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="Sales Comm Rpt" />
				<parameter name="workflowstatus" value="" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_sp_name" value="" />
				<parameter name="file_startswith" value="" />
				<parameter name="file_regex" value="^[AaZz0-9]{4}(_)[0-9]{2}(_)[0-9]{6}(.[Ii][Dd][Xx])$" />
			</parameters>
		</action>
      </actions>
   </procedure>
</procedures>''

insert into TWK_RunProcedures
(name,source, priority, currentstatus, procsXML, maxretrys, IsComplete, AddedBy, AddedOn)
values (''process_CommissionFileByRep'',''auto'',1,''pending'',@xdoc, 10, 0,  ''DLX-SYSTEM'', GETDATE())


', 
		@database_name=N'DocumentManager', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [SalesOrders-Out_Packlists-SalesAck-Inv]    Script Date: 10/14/2012 08:47:56 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'SalesOrders-Out_Packlists-SalesAck-Inv', 
		@step_id=8, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'declare @xdoc as xml
set @xdoc = N''
<procedures initialstatus="pending" status_error="error_aborted" status_onretry="error_willretry" status_onsuccess="all_completed">
   <procedure type="DLX_IMPORTS" runorder="1" status="pending">



      <actions>
	<action type="process_SalesInvoice" description="process_SalesInvoice" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office" />
				
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="documenttype" value="Sales Invoice" />
				<parameter name="workflowstatus" value="scanned" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_filename" value="" />
				<parameter name="process_sp_name" value="" />
			          	<parameter name="file_startswith" value="SalesInv_" />
				
				<parameter name="file_regex" value="^([Ss][Aa][Ll][Ee][Ss][Ii][Nn][Vv])(_){1}([A-Za-z0-9_-]{0,20})(.[Ii][Dd][Xx])$" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
			</parameters>
		</action>	
	<action type="process_SalesOrderAck" description="process_SalesOrderAck" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office" />
				
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="documenttype" value="Sales Ord Ack" />
				<parameter name="workflowstatus" value="scanned" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_filename" value="" />
				<parameter name="process_sp_name" value="" />
			          	<parameter name="file_startswith" value="SalesOrdAck_" />
				
				<parameter name="file_regex" value="^([Ss][Aa][Ll][Ee][Ss][Oo][Rr][Dd])(_){1}([A-Za-z0-9_-]{0,20})(.[Ii][Dd][Xx])$" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
			</parameters>
		</action>	
	<action type="process_SalesOrder" description="process_SalesOrder" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office" />
				
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="documenttype" value="Sales Ord Ack" />
				<parameter name="workflowstatus" value="scanned" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_filename" value="" />
				<parameter name="process_sp_name" value="" />
			          	<parameter name="file_startswith" value="SalesOrd_" />
				<parameter name="file_regex" value="^([Ss][Aa][Ll][Ee][Ss][Oo][Rr][Dd])(_){1}([A-Za-z0-9_-]{0,20})(.[Ii][Dd][Xx])$" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
			</parameters>
		</action>	
	<action type="process_OutgoingPacklists" description="process_OutgoingPacklists" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office" />
				
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="documenttype" value="AR Packlist" />
				<parameter name="workflowstatus" value="scanned" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_filename" value="" />
				<parameter name="process_sp_name" value="" />
			          	<parameter name="file_startswith" value="OutPacklist_" />
				<parameter name="file_regex" value="^([Oo][Uu][Tt][Pp][Aa][Cc][Kk][Ll][Ii][Ss][Tt])(_){1}([A-Za-z0-9_-]{0,20})(.[Ii][Dd][Xx])$" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
			</parameters>
		</action>	
      </actions>
   </procedure>
</procedures>''

insert into TWK_RunProcedures
(name,source, priority, currentstatus, procsXML, maxretrys, IsComplete, AddedBy, AddedOn)
values (''Sales stuff and outgoing packlists'',''auto'',1,''pending'',@xdoc, 10, 0,  ''DLX-SYSTEM'', GETDATE())

', 
		@database_name=N'DocumentManager', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Insert runproc job]    Script Date: 10/14/2012 08:47:56 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Insert runproc job', 
		@step_id=9, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'
declare @xdoc as xml
set @xdoc = N''
<procedures initialstatus="pending" status_error="error_aborted" status_onretry="error_willretry" status_onsuccess="all_completed">
  <procedure type="DLX_IMPORTS" runorder="1" status="pending">
    <actions>
		
      <action type="process_PurchaseRequests" description="process_PurchaseRequests" runorder="1" status="pending">
        <connectionstrings>
			<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
        </connectionstrings>
		<parameters>
			<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
			<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
			<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
			<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
			<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
			<parameter name="drawername" value="Accounts Payable" />
			<parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
			<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
			<parameter name="documenttype" value="AP Pur Req" />
			<parameter name="workflowstatus" value="Order Raised" />
			<parameter name="workflowapprover" value="DM" />
			<parameter name="tokuser_username" value="wfprocessing" />
			<parameter name="tokuser_password" value="wfprocessing" />
			<parameter name="file_startswith" value="APPurReq_" />
			<parameter name="file_regex" value="^([Aa][Pp][Pp][Uu][Rr][Rr][Ee][Qq]_{1})([A-Za-z0-9_]{1,20})(.[Ii][Dd][Xx])$" />
		</parameters>
		<items>
			<item type="purchaserequests">
				<parameters>
					<parameter name="name1" value="value1" />
				</parameters>
				<indexes>
				</indexes>
				<files>
				</files>
			</item>
		</items>
      </action>
      
      <action type="process_Vendor" description="process_Vendor" runorder="1" status="pending">
      <connectionstrings>
        <connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
        <connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
      </connectionstrings>
      <parameters>
        <parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
        <parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
        <parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
        <parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
        <parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
        <parameter name="drawername" value="RRArchive" />
        <parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
        <parameter name="documenttype" value="PurchaseRequest" />
        <parameter name="workflowstatus" value="NEW" />
        <parameter name="workflowapprover" value="DM" />
        <parameter name="tokuser_username" value="wfprocessing" />
        <parameter name="tokuser_password" value="wfprocessing" />
        <parameter name="process_file_name" value="vendor.idx" />
        <parameter name="process_sp_name" value="dlx_RR_AP_Supplier_Update" />
		<parameter name="file_startswith" value="Vendor" />
		<parameter name="file_regex" value="^([Vv][Ee][Nn][Dd][Oo][Rr])(.[Ii][Dd][Xx])$" />
		<parameter name="tokopen_realm" value="documentmanager" />
		<parameter name="tokopen_archivename" value="documentmanager" />
		
      </parameters>
      <items>
        <item type="purchaserequests">
          <parameters>
            <parameter name="name1" value="value1" />
          </parameters>
          <indexes>
          </indexes>
          <files>
          </files>
        </item>
      </items>
    </action>

      
      <action type="process_PurchaseRequests_Approved" description="process_PurchaseRequests_Approved" runorder="1" status="pending">
      <connectionstrings>
        <connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
        <connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
      </connectionstrings>
      <parameters>
        <parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
        <parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
        <parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
        <parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
        <parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
        <parameter name="drawername" value="RRArchive" />
        <parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
        <parameter name="documenttype" value="PurchaseRequest" />
        <parameter name="workflowstatus" value="NEW" />
        <parameter name="workflowapprover" value="DM" />
        <parameter name="tokuser_username" value="wfprocessing" />
        <parameter name="tokuser_password" value="wfprocessing" />
        <parameter name="process_filename" value="vendor.idx" />
        <parameter name="process_sp_name" value="dlx_RR_PurchaseRequests_GetApproved" />
        <parameter name="tokopen_realm" value="documentmanager" />
		<parameter name="tokopen_archivename" value="documentmanager" />
	  	<parameter name="file_startswith" value="ICN_" />  	<parameter name="file_startswith" value="PR" />
		<parameter name="file_regex" value="^([Pp][Rr])([A-Za-z0-9_-]{1,20})(.[Tt][Xx][Tt])$" />
      </parameters>
      <items>
        <item type="purchaserequests">
          <parameters>
            <parameter name="name1" value="value1" />
          </parameters>
          <indexes>
          </indexes>
          <files>
          </files>
        </item>
      </items>
    </action>

    </actions>
  </procedure>
</procedures>''

insert into TWK_RunProcedures
(name,source, priority, currentstatus, procsXML, maxretrys, IsComplete, AddedBy, AddedOn)
values (''dlx_rr_chess_procs_grp1'',''auto'',1,''pending'',@xdoc, 10, 0,  ''rmcnett'', GETDATE())

---------------------------------------------------------------------------------------------------
set @xdoc = N''
<procedures initialstatus="pending" status_error="error_aborted" status_onretry="error_willretry" status_onsuccess="all_completed">
  <procedure type="DLX_IMPORTS" runorder="1" status="pending">
    <actions>
      <action type="process_ImportICNs" description="process_ImportICNs" runorder="1" status="pending">
        <connectionstrings>
          <connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
          <connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
        </connectionstrings>
        <parameters>
          <parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
          <parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
          <parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
          <parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
          <parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
          <parameter name="drawername" value="Accounts Payable" />
          <parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
          <parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
          <parameter name="documenttype" value="ICN" />
          <parameter name="workflowstatus" value="Un-Matched" />
          <parameter name="workflowapprover" value="DM" />
          <parameter name="tokuser_username" value="wfprocessing" />
          <parameter name="tokuser_password" value="wfprocessing" />
          	<parameter name="file_startswith" value="ICN_" />
			<parameter name="file_regex" value="^([Ii][Cc][Nn]_{1})([A-Za-z0-9_-]{1,20})(.[Ii][Dd][Xx])$" />
        </parameters>
      </action>
      
		<action type="process_PurchaseOrders" description="process_PurchaseOrders" runorder="1" status="pending">
        <connectionstrings>
          <connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
          <connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
        </connectionstrings>
        <parameters>
          <parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
          <parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
          <parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
          <parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
          <parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
          <parameter name="drawername" value="Accounts Payable" />
          <parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
          <parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
          <parameter name="documenttype" value="AP Pur Ord" />
          <parameter name="workflowstatus" value="Order Raised" />
          <parameter name="workflowapprover" value="" />
          <parameter name="tokuser_username" value="wfprocessing" />
          <parameter name="tokuser_password" value="wfprocessing" />
          <parameter name="process_sp_name" value="dlx_RR_APUpdates_FromPO" />
    		<parameter name="file_startswith" value="APPurOrdD_" />
			<parameter name="file_regex" value="^([Aa][Pp][Pp][Uu][Rr][Oo][Rr][Dd][Dd]_{1})([A-Za-z0-9_-]{1,20})(.[Ii][Dd][Xx])$" />

        </parameters>
      </action>

		<action type="process_Receivers" description="process_Receivers" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="AP Recvr" />
				<parameter name="workflowstatus" value="Un-Matched" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_sp_name" value="dlx_RR_Receiver_Create" />
				<parameter name="file_startswith" value="APRecvrD_" />
				<parameter name="file_regex" value="^([Aa][Pp][Rr][Ee][Cc][Vv][Rr][Dd]_{1})([A-Za-z0-9_-]{1,20})(.[Ii][Dd][Xx])$" />
			</parameters>
			<filefields type="Receivers" format="idx">
				<filefield name="AP_Recvr_LineNum" size="4" start="1" end="4" linenumber="2" cell="" range=""></filefield>
				<filefield name="AP_Recvr_ItemCode" size="30" start="5" end="35" linenumber="2" cell="" range=""></filefield>
				<filefield name="AP_Recvr_Quantity" size="14" start="35" end="49" linenumber="2" cell="" range=""></filefield>
				<filefield name="AP__Recvr_POLineNum" size="4" start="49" end="53" linenumber="2" cell="" range=""></filefield>
				<filefield name="Vendor_Number" size="10" start="1" end="9" linenumber="1" cell="" range=""></filefield>
				<filefield name="Vend_Loc" size="6" start="11" end="16" linenumber="1" cell="" range=""></filefield>
				<filefield name="Vend_Name" size="35" start="17" end="51" linenumber="1" cell="" range=""></filefield>
				<filefield name="Pur_Req_Num" size="20" start="52" end="71" linenumber="1" cell="" range=""></filefield>
				<filefield name="Pur_Req_Date" size="10" start="72" end="81" linenumber="1" cell="" range=""></filefield>
				<filefield name="Pur_Ord_Num" size="20" start="82" end="101" linenumber="1" cell="" range=""></filefield>
				<filefield name="Pur_Ord_Date" size="10" start="102" end="111" linenumber="1" cell="" range=""></filefield>
				<filefield name="AP_Recvr_Number" size="8" start="112" end="119" linenumber="1" cell="" range=""></filefield>
				<filefield name="Master_Location" size="3" start="120" end="122" linenumber="1" cell="" range=""></filefield>
			</filefields>
		</action>
		<action type="process_ReceiverUpdates" description="process_ReceiverUpdates" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="AP Recvr" />
				<parameter name="workflowstatus" value="Archived" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_sp_name" value="dlx_RR_Receiver_Create" />
				<parameter name="file_startswith" value="APRecvrD_" />
				<parameter name="file_regex" value="^([Aa][Pp][Rr][Ee][Cc][Vv][Rr][Dd][Uu]_{1})([A-Za-z0-9_-]{1,20})(.[Ii][Dd][Xx])$" />
			</parameters>
			<filefields type="Receivers" format="idx">
				<filefield name="AP_Recvr_LineNum" size="4" start="1" end="4" linenumber="2" cell="" range=""></filefield>
				<filefield name="AP_Recvr_ItemCode" size="30" start="5" end="35" linenumber="2" cell="" range=""></filefield>
				<filefield name="AP_Recvr_Quantity" size="14" start="35" end="49" linenumber="2" cell="" range=""></filefield>
				<filefield name="AP__Recvr_POLineNum" size="4" start="49" end="53" linenumber="2" cell="" range=""></filefield>
				<filefield name="Vendor_Number" size="10" start="1" end="9" linenumber="1" cell="" range=""></filefield>
				<filefield name="Vend_Loc" size="6" start="11" end="16" linenumber="1" cell="" range=""></filefield>
				<filefield name="Vend_Name" size="35" start="17" end="51" linenumber="1" cell="" range=""></filefield>
				<filefield name="Pur_Req_Num" size="20" start="52" end="71" linenumber="1" cell="" range=""></filefield>
				<filefield name="Pur_Req_Date" size="10" start="72" end="81" linenumber="1" cell="" range=""></filefield>
				<filefield name="Pur_Ord_Num" size="20" start="82" end="101" linenumber="1" cell="" range=""></filefield>
				<filefield name="Pur_Ord_Date" size="10" start="102" end="111" linenumber="1" cell="" range=""></filefield>
				<filefield name="AP_Recvr_Number" size="8" start="112" end="119" linenumber="1" cell="" range=""></filefield>
				<filefield name="Master_Location" size="3" start="120" end="122" linenumber="1" cell="" range=""></filefield>
			</filefields>
		</action>
		<action type="process_ReOpenReceivers" description="process_ReOpenReceivers" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
          <parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="AP Recvr Reopen" />
				<parameter name="workflowstatus" value="Un-Matched" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_sp_name" value="" />
				<parameter name="file_startswith" value="APRecvrI_" />
				<parameter name="file_regex" value="^([Aa][Pp][Rr][Ee][Cc][Vv][Rr][Ii]_{1})([A-Za-z0-9_-]{1,20})(.[Ii][Dd][Xx])$" />
			</parameters>
		</action>

		<action type="process_Invoices" description="process_Invoices" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="AP Invoice" />
				<parameter name="workflowstatus" value="Un-Matched" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_filename" value="vendor.xls" />
				<parameter name="process_sp_name" value="dlx_RR_Invoices_Create" />
				<parameter name="file_startswith" value="vendor" />
				<parameter name="file_regex" value="^([Vv][Ee][Nn][Dd][Oo][Rr][.][Xx][Ll][Ss])$" />
			<parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
			</parameters>
		</action>
		
		<action type="process_InvoiceERPNonPO" description="process_InvoiceERPNonPO" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
				<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="AP Invoice" />
				<parameter name="workflowstatus" value="Awaiting Approval" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_sp_name" value="dlx_RR_Invoices_ERP_Create" />
				<parameter name="file_startswith" value="IN_" />
				<parameter name="file_regex" value="^([Ii][Nn]_{1})[0-9]{1,10}(-)([A-Za-z0-9]{1,10})(.[Ii][Dd][Xx])$" />
			</parameters>
		</action>

		<action type="process_InvoiceText" description="process_InvoiceText" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
          <parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="AP Invoice" />
				<parameter name="workflowstatus" value="Awaiting Approval" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_sp_name" value="dlx_RR_Invoices_Create" />
				<parameter name="file_startswith" value="IN_" />
				<parameter name="file_regex" value="^([Ii][Nn]_{1})[0-9]{1,10}(.[Tt][Xx][Tt])$" />
			</parameters>
		</action>
		
		<action type="process_InvoiceBackup" description="process_InvoiceBackup" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
          <parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="AP Inv BkUp" />
				<parameter name="workflowstatus" value="Awaiting Approval" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_sp_name" value="" />
				<parameter name="file_startswith" value="IN_" />
				<parameter name="file_regex" value="^([Ii][Nn][Vv][Bb][Kk][Uu][Pp]_{1})[0-9]{1,10}(-)([A-Za-z0-9]{1,10})(.[Ii][Dd][Xx])$" />
			</parameters>
		</action>
		
		<action type="process_Check" description="process_Check" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
          <parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="" />
				<parameter name="workflowstatus" value="" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_sp_name" value="dlx_RR_Checks_Update" />
				<parameter name="file_startswith" value="APChk_" />
				<parameter name="file_regex" value="^([Aa][Pp][Cc][Hh][Kk])(_)([A-Za-z0-9]{1,20})(.[Ii][Dd][Xx])$" />
			</parameters>
		</action>
		<action type="process_LimitsOfAuthority" description="process_LimitsOfAuthority" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
				<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
          <parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="" />
				<parameter name="workflowstatus" value="" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_sp_name" value="" />
				<parameter name="file_startswith" value="RobroyLOA" />
				<parameter name="file_regex" value="^([Rr][Oo][Bb][Rr][Oo][Yy][Ll][Oo][Aa][2][.][Ii][Dd][Xx])$" />
			</parameters>
		</action>
		
		<action type="process_PrePaymentRegister" description="process_PrePaymentRegister" runorder="1" status="pending">
			<connectionstrings>
				<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
				<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			</connectionstrings>
			<parameters>
			<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
				<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
				<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
				<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
				<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
				<parameter name="drawername" value="Accounts Payable" />
				<parameter name="tokopen_realm" value="documentmanager" />
				<parameter name="tokopen_archivename" value="documentmanager" />
				<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
				<parameter name="documenttype" value="PPR" />
				<parameter name="workflowstatus" value="" />
				<parameter name="workflowapprover" value="" />
				<parameter name="tokuser_username" value="wfprocessing" />
				<parameter name="tokuser_password" value="wfprocessing" />
				<parameter name="process_sp_name" value="dlx_RR_PPR_CreateApprovalList" />
				<parameter name="file_startswith" value="PPR" />
				<parameter name="file_regex" value="^([Pp][Pp][Rr])(_)([A-Za-z0-9_]{1,20})(.[Ii][Dd][Xx])$" />
				<parameter name="pprlinkurl" value="http&#58;&#47;&#47;doclog&#47;default.aspx?search=&amp;DATABASE=CLS&amp;DRAWER=AP&#32;Payroll&amp;DOCUMENTID=8831&amp;OPENSINGLEDOC=true" />
			</parameters>
		</action>
		<action type="process_GL_Master" description="process_GL_Master" runorder="1" status="pending">
		  <connectionstrings>
			<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
		  </connectionstrings>
		  <parameters>
			<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
			<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
			<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
			<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
			<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
			<parameter name="drawername" value="Accounts Payable" />
			<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
			<parameter name="documenttype" value="PurchaseRequest" />
			<parameter name="workflowstatus" value="NEW" />
			<parameter name="workflowapprover" value="DM" />
			<parameter name="tokuser_username" value="wfprocessing" />
			<parameter name="tokuser_password" value="wfprocessing" />
			<parameter name="process_file_name" value="msglacct.idx" />
			<parameter name="process_sp_name" value="" />
			<parameter name="file_startswith" value="msglacct" />
			<parameter name="file_regex" value="^([Mm][Ss][Gg][Ll][Aa][Cc][Cc][Tt])(.[Ii][Dd][Xx])$" />
			<parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
			
		  </parameters>
		  <items>
			<item type="purchaserequests">
			  <parameters>
				<parameter name="name1" value="value1" />
			  </parameters>
			  <indexes>
			  </indexes>
			  <files>
			  </files>
			</item>
		  </items>
		</action>
		<action type="process_Skeleton_GL" description="process_Skeleton_GL" runorder="1" status="pending">
		  <connectionstrings>
			<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
		  </connectionstrings>
		  <parameters>
			<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
			<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
			<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
			<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
			<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
			<parameter name="drawername" value="Accounts Payable" />
			<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
			<parameter name="documenttype" value="PurchaseRequest" />
			<parameter name="workflowstatus" value="NEW" />
			<parameter name="workflowapprover" value="DM" />
			<parameter name="tokuser_username" value="wfprocessing" />
			<parameter name="tokuser_password" value="wfprocessing" />
			<parameter name="process_file_name" value="skglacct.idx" />
			<parameter name="process_sp_name" value="" />
			<parameter name="file_startswith" value="msglacct" />
			<parameter name="file_regex" value="^([Ss][Kk][Gg][Ll][Aa][Cc][Cc][Tt])(.[Ii][Dd][Xx])$" />
			<parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
		  </parameters>
		</action>
		<action type="process_PPR_CreateApprovals" description="process_PPR_CreateApprovals" runorder="1" status="pending">
		  <connectionstrings>
			<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
		  </connectionstrings>
		  <parameters>
			<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
			<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
			<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
			<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
			<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export" />
			<parameter name="drawername" value="Accounts Payable" />
			<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
			<parameter name="documenttype" value="" />
			<parameter name="workflowstatus" value="" />
			<parameter name="workflowapprover" value="" />
			<parameter name="tokuser_username" value="wfprocessing" />
			<parameter name="tokuser_password" value="wfprocessing" />
			<parameter name="process_file_name" value="" />
			<parameter name="process_sp_name" value="" />
			<parameter name="file_startswith" value="" />
			<parameter name="file_regex" value="" />
			<parameter name="tokopen_realm" value="documentmanager" />
			<parameter name="tokopen_archivename" value="documentmanager" />
		  </parameters>
		</action>
		
    </actions>
  </procedure>
</procedures>''

insert into TWK_RunProcedures
(name,source, priority, currentstatus, procsXML, maxretrys, IsComplete, AddedBy, AddedOn)
values (''dlx_rr_chess_procs_grp2'',''auto'',1,''pending'',@xdoc, 10, 0,  ''DLX-SYSTEM'', GETDATE())




', 
		@database_name=N'DocumentManager', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'EveryMin', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=4, 
		@freq_subday_interval=1, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20120710, 
		@active_end_date=99991231, 
		@active_start_time=0, 
		@active_end_time=235959, 
		@schedule_uid=N'0364b25e-275f-4082-a73c-084ab10f666b'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO

/****** Object:  Job [RR_DM_REPORTS]    Script Date: 10/14/2012 08:47:56 ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]]    Script Date: 10/14/2012 08:47:56 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'RR_DM_REPORTS', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'No description available.', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'ROBROY\administrator', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [reports_reconciliaiton]    Script Date: 10/14/2012 08:47:56 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'reports_reconciliaiton', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'declare @xdoc as xml
set @xdoc = 
N''<procedures initialstatus="pending" status_error="error_aborted" status_onretry="error_willretry" status_onsuccess="all_completed">
  <procedure type="DLX_IMPORTS" runorder="1" status="pending">
    <actions>
	<action type="process_Reports_Reconciliation" description="process_Reports_Reconciliation" runorder="1" status="pending">
		  <connectionstrings>
			<connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
			<connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;" />
		  </connectionstrings>
		  <parameters>
			<parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office" />
			<parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed" />
			<parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess" />
			<parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs" />
			<parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export\Reports" />
			<parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp" />
			<parameter name="process_file_name" value="" />
			<parameter name="process_sp_name" value="dlx_RR_ReconciliationItems_Get" />
		  </parameters>
	</action>
    </actions>
  </procedure>
</procedures>''

insert into TWK_RunProcedures
(name,source, priority, currentstatus, procsXML, maxretrys, IsComplete, AddedBy, AddedOn)
values (''dlx_rr_chess_procs_reports_reconciliation'',''auto'',1,''pending'',@xdoc, 10, 0,  ''DLX-SYSTEM'', GETDATE())
', 
		@database_name=N'DocumentManager', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'rpt_at_5pm', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=1, 
		@freq_subday_interval=2, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20120710, 
		@active_end_date=99991231, 
		@active_start_time=170000, 
		@active_end_time=235959, 
		@schedule_uid=N'eccb2483-b619-4ee3-b3cd-a7930901e9a6'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO

