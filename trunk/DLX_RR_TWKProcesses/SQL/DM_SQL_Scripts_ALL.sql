USE [UATDocumentManager]
GO

/****** Object:  StoredProcedure [dbo].[API_Clear_User_Session]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


	CREATE procedure [dbo].[API_Clear_User_Session]
	@username AS VARCHAR(50)
	as
	DECLARE @userid int
	SET @userid = (SELECT TOP 1 userid FROM directry WHERE name = @username) DELETE FROM toksessions24 WHERE userid = @userid
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_AP_Supplier_CreateDummy]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--WARNING THIS WILL ONLY WORK FOR ONE DUMMY RECORD IN THE AP_SUPPLIER_DUMMY TABLE
create procedure [dbo].[dlx_RR_AP_Supplier_CreateDummy]
as
if not  exists(select * from AP_SUPPLIER where supplierid = '9999') 
BEGIN
	print 'do insert'
	INSERT INTO AP_SUPPLIER
	SELECT [ERPSystem]
		  ,[CompanyCode]
		  ,[ClientCode]      ,[SupplierId]
		  ,[SupplierName]      ,[Street]
		  ,[Town]      ,[Country]
		  ,[PostCode]      ,[PhoneNum]
		  ,[VATExempt]      ,[VATRegNo]
		  ,[DefaultVATCode]
		  ,[Currency]
		  ,[DefaultDepartment]
		  ,[SupplierLocation]
		  ,[Email]
		  ,[CCN]
	  FROM [DocumentManager].[dbo].[AP_SUPPLIER_DUMMY]
END
else 	print 'already exists'
	
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_AP_Supplier_Update]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[dlx_RR_AP_Supplier_Update]
     @SupplierID as nvarchar(20)
     ,@SupplierName as nvarchar(200)
     ,@PhoneNum  as nvarchar(25)
as 
IF (SELECT COUNT(SupplierId) FROM AP_SUPPLIER  WHERE SupplierId = @SupplierID ) =0 
     BEGIN
          INSERT INTO AP_SUPPLIER( ERPSystem, CompanyCode, ClientCode, SupplierId, SupplierName, PhoneNum, VATExempt) 
          VALUES ('CHESS',100,100,@SupplierID,@SupplierName,@PhoneNum,0)
     END
ELSE
     BEGIN
          UPDATE AP_SUPPLIER SET SupplierName = @SupplierName, PhoneNum = @PhoneNum
          WHERE SupplierId = @SupplierID    
     END
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_APUpdates_FromPO]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--dlx_RR_APUpdates_FromPO '05-65658'

CREATE procedure [dbo].[dlx_RR_APUpdates_FromPO] 
@PONumber varchar(50)
as

select distinct erpsystem, companycode, clientcode, dept, supplierid, costcentre CCN, GLCODE into #dept  from AP_PO_LINE P where ponumber = @ponumber

insert into DEPARTMENT
select D.erpsystem, D.companycode, D.clientcode, D.dept from #dept D full join DEPARTMENT A
on D.ERPSystem = A.ERPSystem and D.CompanyCode = A.CompanyCode and D.ClientCode = A.ClientCode and D.dept = A.department
where A.department is null

insert into AP_SUPPLIER
(erpsystem,companycode,clientcode,supplierid,vatexempt)
select D.erpsystem, D.companycode, D.clientcode, D.supplierid, 0 from #dept D full join AP_SUPPLIER A
on D.ERPSystem = A.ERPSystem and D.CompanyCode = A.CompanyCode and D.ClientCode = A.ClientCode and D.supplierid = A.supplierid
where A.supplierid is null

insert into COST_CENTRE
(erpsystem,companycode,clientcode,department,CostCentre)
select D.erpsystem, D.companycode, D.clientcode, D.dept, D.ccn from #dept D full join COST_CENTRE A
on D.ERPSystem = A.ERPSystem and D.CompanyCode = A.CompanyCode and D.ClientCode = A.ClientCode 
and D.dept = A.department and D.CCN = A.CostCentre
where A.CostCentre is null

insert into GL_CODE
(erpsystem,companycode,clientcode,department,CostCentre, GLCode)
select D.erpsystem, D.companycode, D.clientcode, D.dept, D.ccn, D.GLCode from #dept D full join GL_CODE A
on D.ERPSystem = A.ERPSystem and D.CompanyCode = A.CompanyCode and D.ClientCode = A.ClientCode 
and D.dept = A.department and D.CCN = A.CostCentre and D.GLCode = A.GLCode
where A.GLCode is null

drop table #dept


/*
select * from AP_PO_LINE 
select * from DEPARTMENT
select * from COST_CENTRE
select * from #dept
*/
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_CHESS_getjobs]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[dlx_RR_CHESS_getjobs]
@ownername as varchar(50)
, @initStr as varchar(500)
, @return_maxtasks int
as 

SELECT top 20  procedureID, procsXML  FROM  TWK_RunProcedures where currentstatus = 'pending' 
order by AddedOn


GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_Email_PPR_Notices]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[dlx_RR_Email_PPR_Notices]
AS

DECLARE @pprheaderid INT
DECLARE  JobsToProcess CURSOR FOR 
	SELECT pprheaderid FROM AP_PPR_HEADER WHERE PPRStatusID = 0
OPEN JobsToProcess
FETCH NEXT FROM JobsToProcess INTO @pprheaderid

WHILE @@FETCH_STATUS = 0
BEGIN

	print 'WORKING ON PPRDEADER [' + cast(@pprheaderid as  varchar(3)) + ']'
	declare @approvals_needed int
	
	set @approvals_needed = (SELECT  top 1 numapprovers from  [AP_PPR_APPROVALS] where pprHeaderID = @pprheaderid)

	declare @queued_approvers int
	set @queued_approvers = (SELECT  COUNT(*) from  [AP_PPR_APPROVALS] where pprHeaderID = @pprheaderid 
								and LOAStatus = 11) 

	declare @expired_approvers int
	set @expired_approvers = (SELECT  COUNT(*) from  [AP_PPR_APPROVALS] where pprHeaderID = @pprheaderid 
								and LOAStatus = 15) 

	declare @remaining_approvers int
	set @remaining_approvers = (SELECT  COUNT(*) from  [AP_PPR_APPROVALS] where pprHeaderID = @pprheaderid 
								and LOAStatus = 0) 
								
	declare @complete_approvals int
	set @complete_approvals = (SELECT  COUNT(*) from  [AP_PPR_APPROVALS] where pprHeaderID = @pprheaderid 
								and LOAStatus = 1) 

	DECLARE @rejected_approvals INT
	SET @rejected_approvals = (SELECT  COUNT(*) FROM  [AP_PPR_APPROVALS] WHERE pprHeaderID = @pprheaderid 
								and LOAStatus = 2) 
	/*********************/
	DECLARE @HasExpired as bit
	SET @HasExpired = 0
	-- check if expired
	--print '  CHECKING CURRENT APPROVER FOR [' + cast(@pprheaderid as  varchar(3)) + ']'
	DECLARE @DiffMins int					
	SET @DiffMins = (
		SELECT top 1 datediff(minute, expireson,GETDATE())  
		FROM [AP_PPR_APPROVALS] 
		WHERE pprHeaderID = @pprheaderid AND LOAStatus=11 ORDER BY ApprovalOrder)
	PRINT 'DiffMins: ' + CAST( @DiffMins AS VARCHAR(20)) 
	IF (@DiffMins >= 0)
		BEGIN
			SET @HasExpired = 1 
			PRINT 'HAS EXPIRED'
		END
	ELSE
		BEGIN
			SET @HasExpired = 0
			PRINT 'NOT EXPIRED'
		END
	/***************/
	DECLARE @MustWait AS BIT
	IF ( (@approvals_needed - @complete_approvals) < (@remaining_approvers + @queued_approvers) ) SET @MustWait = 0 ELSE SET @MustWait = 1
	
	--2 < 1+1 



	IF (@queued_approvers = 0) AND (@complete_approvals <> @approvals_needed) AND (@MustWait = 0)
		BEGIN
			print 'RETURNING FIRST LOA IN QUEUE'
			SELECT top 1 *, @MustWait [MustWait]  FROM [AP_PPR_APPROVALS] 
			where pprHeaderID = @pprheaderid and LOAStatus = 0 order by ApprovalOrder
		END
	IF (@queued_approvers = 1) AND (@complete_approvals <> @approvals_needed) AND (@MustWait = 0)
		BEGIN
			IF @HasExpired = 1
				BEGIN
					print 'EXPIRE CURRENT LOA - RETURN NEXT'--
					update [AP_PPR_APPROVALS] set LOAStatus = 15 where pprHeaderID = @pprheaderid and LOAStatus = 11 
					--return next LOA
					SELECT top 1 *, @MustWait [MustWait]  FROM [AP_PPR_APPROVALS] 
					where pprHeaderID = @pprheaderid and LOAStatus = 0 order by ApprovalOrder
				END
			BEGIN
				print 'WAIT FOR PENDING LOA'
			END
		END
	IF (@queued_approvers = 0) AND (@complete_approvals <> @approvals_needed) AND (@MustWait = 1) 
		BEGIN
			PRINT 'NONE QUEUED - STILL NEED APPROVALS - RETURN NEXT IN QUEUE'
			SELECT top 1 *, @MustWait [MustWait]  FROM [AP_PPR_APPROVALS] 
			where pprHeaderID = @pprheaderid and LOAStatus = 0 order by ApprovalOrder
		END
	IF (@complete_approvals = @approvals_needed) 
		BEGIN
			UPDATE AP_PPR_HEADER SET PPRStatusID = 10, StatusChangedOn = getdate(), ChangedBy = 'dlx_RR_Email_PPR_Notices' WHERE pprHeaderID = @pprheaderid
			PRINT 'DONE - UPDATE APPROVAL'
		END

	PRINT '@mustwait '		+  cast( @mustwait as varchar(20))
	PRINT '@approvals_needed '		+  cast( @approvals_needed as varchar(20))
	PRINT '@queued_approvers '		+  cast( @queued_approvers as varchar(20))
	PRINT '@remaining_approvers '	+  cast( @remaining_approvers as varchar(20))
	PRINT '@complete_approvals '		+  cast( @complete_approvals as varchar(20))
	PRINT '@rejected_approvals '		+  cast( @rejected_approvals as varchar(20))
	PRINT '@expired_approvers '		+  cast( @expired_approvers as varchar(20))


	FETCH NEXT FROM JobsToProcess INTO @pprheaderid
END
CLOSE JobsToProcess
DEALLOCATE JobsToProcess


GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_EmailRequests_POToSupplier]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--update ap_po_header set docid=0 , emailstatus=0
--01-03945
--01-03949

CREATE procedure [dbo].[dlx_RR_EmailRequests_POToSupplier]
as

select '' [Link],
PO.PONumber, PO.SupplierID, PO.TransDate, PO.NetAmount
, coalesce( dbo.Archive_GetFirstImagePath(PO.Docid),'') [FilePath], S.SupplierName
,case when S.email = '' then 
	(select RecipientAddresses from AP_NOTIFY where NotificationId 
	= (SELECT TOP 1 [NotificationId]  FROM [AP_NOTIFY_CONDITION] where value 
	=  (select top 1 Literal from AP_CCN_TRANSLATION where  CCNID = LEFT(PO.PONumber,2) )  ))
		
	
	ELSE S.email
END [email]

, coalesce( S.street,'')[street]
, coalesce( S.Town, '')[Town]
, coalesce( S.Country, '')[Country]
, coalesce( S.PostCode, '')[PostCode]
, S.PhoneNum 
,DocID
,LEFT(PO.PONumber,2) [CCNCODE]
,(select top 1 Literal from AP_CCN_TRANSLATION where  CCNID = LEFT(PO.PONumber,2) )[CCN]
 from ap_po_header PO, AP_SUPPLIER S  
 
where ( emailstatus  =0 ) or (EmailStatus is null)
and PO.supplierid = S.SupplierID
--and Email <> ''
--and Email = 'akewalra@robroy.com'

-- if email is blank  translate 1st 2 digits of PO and translate it to cost centre, then lookup in the 
-- ap_notify_contion table -> then get email from  record from ap_notify table

--select --'http://rr-core/WebRequests/RequestApproval.aspx?approvalid=' + cast( R.approvalid as varchar(40)) [Link]
--'' [Link]
--, R.ApprovalID, R.LOAID, R.docid, L.email1, l.email2
--, L.DLX_Login, L.ApproverName, L.SignOffLimit

--from APPROVALS_RQ R, APPROVAL_LOA L where 
--r.loaid = L.loaid
--AND R.statusid = 1
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_ExceptionItems_Get]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- select * from domains where domainname = 'AP WF Status'
-- select * from AP_HEADER

CREATE procedure [dbo].[dlx_RR_ExceptionItems_Get]
as

select *,  case when dateadd(day,1, createdate) < getdate() then 'true' else 'false' end [Over24Hours] into #extbl from
(
	select 'Non Approved' [Status],  H.SupplierName, H.InvoiceNo, H.TaxDate, H.GrossAmount, H.TransType  
	, U.field03 [ICN], U.field04[PONumber] , U.field05[Request Number ] 
	, U.field09 [Location] , U.field10 [Check Date]
	, U.field11 [Approver], U.field12 [Last Workflow] , U.field13 [Check Number], U.docid
	, U.createdate [tokcreatedate]
	, cast(substring(U.createdate, 5,2) + '/' + substring(U.createdate, 7,2) + '/' + substring(U.createdate, 1,4) 
	+ ' ' + substring(U.createdate, 9,2) + ':' + substring(U.createdate, 11,2) + ':'+ substring(U.createdate, 13,2)   as datetime) [createdate]
	, case U.field03 when null then 'NOICN'
		when '' then 'NOICN'
		else 'ICN' END [HASICN]
	from AP_HEADER H , udf4 U where wfstatus <> 30 and U.FIELD11 <> ''
	and H.docid = U.docid
	--union
	--select 'Approved' [Status],  H.SupplierName, H.InvoiceNo, H.TaxDate, H.GrossAmount, H.TransType  
	--, U.field03 [ICN], U.field04[PONumber] , U.field05[Request Number ] 
	--, U.field09 [Location] , U.field10 [Check Date]
	--, U.field11 [Approver], U.field12 [Last Workflow] , U.field13 [Check Number], U.docid
	--, U.createdate [tokcreatedate]
	--, cast(substring(U.createdate, 5,2) + '/' + substring(U.createdate, 7,2) + '/' + substring(U.createdate, 1,4) 
	--+ ' ' + substring(U.createdate, 9,2) + ':' + substring(U.createdate, 11,2) + ':'+ substring(U.createdate, 13,2)   as datetime) [createdate]
	--, case U.field03 when null then 'NOICN'
	--	when '' then 'NOICN'
	--	else 'ICN' END [HASICN]
	--from AP_HEADER H , udf4 U where wfstatus in( 30,125)
	--and H.docid = U.docid
) Z

select * from #extbl

drop table #extbl
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_LOA_ClearTable]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[dlx_RR_LOA_ClearTable]
as
truncate table APPROVAL_LOA
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_PendingEmailRequest_Get]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[dlx_RR_PendingEmailRequest_Get]
as

select 'http://rr-core/WebRequests/RequestApproval.aspx?approvalid=' + cast( R.approvalid as varchar(40)) [Link]
, R.ApprovalID, R.LOAID, R.docid, L.email1, l.email2
, L.DLX_Login, L.ApproverName, L.SignOffLimit

from APPROVALS_RQ R, APPROVAL_LOA L where 
r.loaid = L.loaid
AND R.statusid = 1
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_PPR_CreateApprovalList]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


 CREATE procedure [dbo].[dlx_RR_PPR_CreateApprovalList]
 @pprHeaderID int
 as
 declare @department as varchar(20)
 declare @CCN as varchar(20)
 declare @CCNID as varchar(10)
 declare @docid as int
 declare @filepath as varchar(500)
 
 select @department = PPR, @CCN = CCN, @docid = docid from AP_PPR_HEADER where pprHeaderID = @pprHeaderID
 set @CCNID = (select CCNID from AP_CCN_TRANSLATION where literal = @CCN)
 set @filepath = dbo.Archive_GetFirstImagePath (@docid)
 
INSERT INTO AP_PPR_APPROVALS
select PPR.pprHeaderID, 0 [LOAStatus], PPR.CCN, PPR.DocDate, PPR.FileName
, LOA.LOAID,		LOA.CostCentre,		LOA.Department,		LOA.ApproverName
, LOA.SignOffLimit, LOA.DLX_Login,		LOA.Email1,			LOA.Email2
, LOA.NumApprovers,	LOA.AutoEscalate,	LOA.ApprovalOrder
,1 [GroupID] ,1 [GroupOrder]

from
	(
		 select @pprHeaderID [pprHeaderID],* from APPROVAL_LOA LOA 
		 WHERE  department = @department and costcentre = @CCN
	) LOA, AP_PPR_HEADER PPR
where  LOA.pprheaderid = PPR.pprheaderid
order by approvalorder
 
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_PPR_Get_Detail]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[dlx_RR_PPR_Get_Detail]
--  dlx_RR_PPR_Get_Detail 4
@pprheaderid int
as
select * from 
(
	SELECT 'datarow' [CSS], pprDetailID, pprHeaderID, VendorNumber, VendLoc,VendName,CCN,ICNNumber   
	,AP_Inv_Number, AP_Doc_Date, Net_Due_date, Total_ICN_Amount
	, Amount_To_Pay, Disc_Amount, ACH_Check, PPRStatusID, StatusChangedOn, ChangedBy
	FROM [AP_PPR_DETAIL] 
	where pprheaderid=@pprheaderid
	union
	SELECT 'subtotalrow',0,pprHeaderID, vendornumber ,null,null,null,null
	,null,null,null,null
	, sum(cast( amount_to_pay as decimal(18,2)) )  ,null,null,null,null,null
	FROM [AP_PPR_DETAIL] 
	where pprheaderid=@pprheaderid  
	group by vendornumber ,pprHeaderID

) Z
order by  vendornumber,amount_to_pay,  pprdetailid desc

select sum( cast(amount_to_pay as decimal(18,2)) ) [GrandTotal] from [AP_PPR_DETAIL] where pprheaderid = @pprheaderid 
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_PurchaseOrders_Create]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[dlx_RR_PurchaseOrders_Create]

           @PONumber  as nvarchar(50) ,
           @SupplierId as nvarchar(20) ,
           @POLineNum as int ,
           @ItemCode as nvarchar(30) ,
           @Descr as nvarchar(255) ,
           @Qty as float ,
           @UnitPrice as float ,
           @UoM as nvarchar(20) ,
           @NetTotal as float ,
           @TaxTotal as float ,
           @VatCode as nvarchar(5) ,
           @CostCentre as nvarchar(20) ,
           @Dept as nvarchar(20) ,
           @GLCode as nvarchar(30) ,
           @Requisitioner as nvarchar(100) ,
           @Planner as nvarchar(100) 
as 

if (select COUNT(PONumber) from [AP_PO_LINE] 
          where PONumber = @PONumber and SupplierId = @SupplierId
          and ItemCode = @ItemCode and Requisitioner = @Requisitioner ) =0
BEGIN
     INSERT INTO [AP_PO_LINE]
           ([ERPSystem]
           ,[CompanyCode]
           ,[ClientCode]
           ,[PONumber]
           ,[SupplierId]
           ,[POLineNum]
           ,[ItemCode]
           ,[Descr]
           ,[Qty]
           ,[UnitPrice]
           ,[UoM]
           ,[NetTotal]
           ,[TaxTotal]
           ,[VatCode]
           ,[CostCentre]
           ,[Dept]
           ,[GLCode]
           ,[Requisitioner]
           ,[Planner])
     VALUES
           ('CHESS'
           ,'01'
           ,'100'
           ,@PONumber
           ,@SupplierId
           ,@POLineNum
           ,@ItemCode
           ,@Descr
           ,@Qty
           ,@UnitPrice
           ,@UoM
           ,@NetTotal
           ,@TaxTotal
           ,@VatCode
           ,@CostCentre
           ,@Dept
           ,@GLCode
           ,@Requisitioner
           ,@Planner )
END

GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_PurchaseRequest_Info]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[dlx_RR_PurchaseRequest_Info] 
@ApprovalID uniqueidentifier
as
declare @docid int
declare @HeaderID uniqueidentifier

set @docid = (select docid from APPROVALS_RQ where approvalid = @ApprovalID)
set @HeaderId = (select HeaderId from AP_HEADER where docid = 2202)

select  SupplierId, SupplierName,  InvoiceNo, TaxAmount,TaxDate, GrossAmount from AP_HEADER where HeaderId =@HeaderID
select  InvLine, Qty, UnitPrice, NetTotal from AP_DETAIL where HeaderId =@HeaderID


GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_PurchaseRequests_Detail_Create]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[dlx_RR_PurchaseRequests_Detail_Create]
              @HeaderId as uniqueidentifier
              ,@InvLine as int
              ,@PONum as nvarchar(20)
              ,@POLine as nvarchar(10)
              ,@DelNote as nvarchar(20)
              ,@ItemCode as nvarchar(30)
              ,@Memo as nvarchar(50)
              ,@Qty as float
              ,@UnitPrice as float
              ,@NetTotal as float
              ,@VatTotal as float
              ,@VatExempt as bit
              ,@VatCode as nvarchar(5)
              ,@CostCentre as nvarchar(20)
              ,@Dept as nvarchar(20)
              ,@GLCode as nvarchar(20)
              ,@Matched as int
              ,@ExcludeFromTotals as bit
              ,@Notes as nvarchar(39)
     as

declare @DetailId as uniqueidentifier
set @DetailId = NEWID()

INSERT INTO [RRDM].[dbo].[AP_DETAIL]
           ([DetailId]
           ,[HeaderId]
           ,[InvLine]
           ,[PONum]
           ,[POLine]
           ,[DelNote]
           ,[ItemCode]
           ,[Memo]
           ,[Qty]
           ,[UnitPrice]
           ,[NetTotal]
           ,[VatTotal]
           ,[VatExempt]
           ,[VatCode]
           ,[CostCentre]
           ,[Dept]
           ,[GLCode]
           ,[Matched]
           ,[ExcludeFromTotals]
           ,[Notes])
     VALUES
           (@DetailId
           ,@HeaderId
           ,1                     --@InvLine
           ,null                  --@PONum
           ,null                  --@POLine
           ,null                  --@DelNote
           ,null                  --@ItemCode
           ,'PR'                  --@Memo       
           ,1                     --@Qty
           ,@UnitPrice
           ,@NetTotal
           ,0                     --@VatTotal
           ,0                     --@VatExempt
           ,null                  --@VatCode
           ,left (@CostCentre,2)--@CostCentre
           ,@Dept
           ,null                  --@GLCode
           ,0                     --@Matched
           ,0                     --@ExcludeFromTotals
           ,@Notes)

select @@identity


GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_PurchaseRequests_GetApproved]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[dlx_RR_PurchaseRequests_GetApproved]
as
select DOCID, FIELD05 [RequestNumber] from  UDF4 
where  FIELD08 = (select valueid from domains where DomainName = 'AP WF Status' and Description ='Approved for PO')

GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_PurchaseRequests_Header_Create]    Script Date: 10/14/2012 08:26:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[dlx_RR_PurchaseRequests_Header_Create]
     @DrawerId as int
     ,@DocId as int
     ,@ERPSystem as nvarchar(20)
     ,@CompanyCode as nvarchar(20)
     ,@ClientCode as nvarchar(20)
     ,@SupplierId as nvarchar(20)
     ,@SupplierName as nvarchar(255)
     ,@APDocType as varchar(4)
     ,@InvoiceNo as nvarchar(30)
     ,@TaxDate as datetime
     ,@GrossAmount as float
     ,@TaxAmount as float
     ,@Currency as varchar(5)
     ,@TransType as varchar(2)
     ,@ERP_Ref as nvarchar(30)
     ,@Matched as int
     ,@Posted as bit
     ,@WFStatus as int
     ,@ExportStatus as int
     ,@ExportResult as nvarchar(255)
     ,@ValidationOverridden as bit
     ,@SignedOff as float
     ,@ReasonId as int
     ,@ReasonMessage as nvarchar(1000)
     ,@LoadedFromSkeleton as bit
as 

declare @HeaderId as UNIQUEIDENTIFIER
set @HeaderId = NEWID()


INSERT INTO [AP_HEADER]
           ([HeaderId]
           ,[DrawerId]
           ,[DocId]
           ,[ERPSystem]
           ,[CompanyCode]
           ,[ClientCode]
           ,[SupplierId]
           ,[SupplierName]
           ,[APDocType]
           ,[InvoiceNo]
           ,[TaxDate]
           ,[GrossAmount]
           ,[TaxAmount]
           ,[Currency]
           ,[TransType]
           ,[ERP_Ref]
           ,[Matched]
           ,[Posted]
           ,[WFStatus]
           ,[ExportStatus]
           ,[ExportResult]
           ,[ValidationOverridden]
           ,[SignedOff]
           ,[ReasonId]
           ,[ReasonMessage]
           ,[LoadedFromSkeleton])
     VALUES
           ( @HeaderId
           ,@DrawerId
           ,@DocId
           ,'CHESS' --@ERPSystem
           ,@CompanyCode
           ,@ClientCode
           ,@SupplierId
           ,@SupplierName
           ,@APDocType
           ,@InvoiceNo
           ,@TaxDate
           ,@GrossAmount
           ,@TaxAmount
           ,@Currency
           ,@TransType
           ,@ERP_Ref
           ,@Matched
           ,@Posted
           ,@WFStatus
           ,@ExportStatus
           ,@ExportResult
           ,@ValidationOverridden
           ,@SignedOff
           ,@ReasonId
           ,@ReasonMessage
           ,@LoadedFromSkeleton)



select @@identity
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_Receiver_Create]    Script Date: 10/14/2012 08:26:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[dlx_RR_Receiver_Create]
           @Location as nvarchar(20)
           ,@Receiver as nvarchar(20)
           ,@DeliveryNoteNum as nvarchar(20)
           ,@DeliveryNoteLineNum as int
           ,@ItemCode as nvarchar(30)
           ,@SupplierId as nvarchar(20)
           ,@PONumber as nvarchar(50)
           ,@POLineNum as int
           ,@Outstanding as  float
           ,@RemovedFromSource as bit
as

if (select COUNT(*) from  [AP_GOODS_RECEIVED] where Itemcode =@ItemCode and POLineNum = @POLineNum and Receiver=@Receiver) =0
     BEGIN
          INSERT INTO [AP_GOODS_RECEIVED]
              ([ERPSystem]
              ,[CompanyCode]
              ,[ClientCode]
              ,[Location]
              ,[Receiver]
              ,[DeliveryNoteNum]
              ,[DeliveryNoteLineNum]
              ,[ItemCode]
              ,[SupplierId]
              ,[PONumber]
              ,[POLineNum]
              ,[Outstanding]
              ,[RemovedFromSource])
          VALUES
              ('CHESS'
              ,'01'
              ,'100'
              ,@Location
              ,@Receiver
              ,@DeliveryNoteNum
              ,@DeliveryNoteLineNum
              ,@ItemCode
              ,@SupplierId
              ,@PONumber
              ,@POLineNum
              ,@Outstanding
              ,@RemovedFromSource)
     END

GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_ReconciliationItems_Get]    Script Date: 10/14/2012 08:26:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [dbo].[dlx_RR_ReconciliationItems_Get]
as
SELECT ICN  FROM dlx_RR_View_OpenItems where [document type] = 'Open ICNs' and PONumber <> '' and CheckCount=0 and foldercount <> 0 --OPENAP
SELECT PONumber,*  FROM dlx_RR_View_OpenItems where [document type] = 'Open Purchase Orders' and PONumber <> '' and foldercount <> 0 --OPENPO
SELECT [Request Number]  FROM dlx_RR_View_OpenItems where [document type] = 'Open Receivers' and PONumber <> '' AND ICN <> ''and foldercount <> 0 --OPENPR
SELECT distinct [Request Number]  FROM dlx_RR_View_OpenItems where [document type] = 'Open Purchase Request' and POCount = 0 and foldercount <> 0  --OPENRNI

GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_Skeleton_CreateDummies]    Script Date: 10/14/2012 08:26:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[dlx_RR_Skeleton_CreateDummies]
as

-- Delete existing AP_SUPPLIER record
if exists (SELECT  supplierid FROM   AP_SUPPLIER_DUMMY )
	begin
		delete from AP_SUPPLIER where supplierid = (SELECT  supplierid FROM   AP_SUPPLIER_DUMMY )  --SUPPLIERID IS 9999
	end
insert into AP_SUPPLIER SELECT  * FROM   AP_SUPPLIER_DUMMY --ME LAZY


if exists(select * from AP_SKELETON_CONDITION where skeletonid in (SELECT skeletonid  FROM   AP_SKELETON_HEADER where  supplierid = 'CHESS'))
	BEGIN
		print 'delete conditions'
		delete from AP_SKELETON_CONDITION where skeletonid in (SELECT skeletonid  FROM   AP_SKELETON_HEADER where  supplierid = 'CHESS')
	END
if exists(select * from AP_SKELETON_DETAIL where skeletonid in (SELECT skeletonid  FROM   AP_SKELETON_HEADER where  supplierid = 'CHESS'))
	BEGIN
		print 'delete detail'
		delete from AP_SKELETON_DETAIL where skeletonid in (SELECT skeletonid  FROM   AP_SKELETON_HEADER where  supplierid = 'CHESS')
	END
	
if exists (SELECT *  FROM   AP_SKELETON_HEADER where  supplierid = 'CHESS')
	begin
		PRINT 'DELETE HEADERS'
		delete FROM  AP_SKELETON_HEADER where supplierid = 'CHESS'
	end
	
	
DECLARE @SKID_CHESSPDF as uniqueidentifier
set @SKID_CHESSPDF = newid()
insert into AP_SKELETON_HEADER		values (@SKID_CHESSPDF,'CHESS','01','100','CHESS','CHESS PDF',	'Pre-Coded',1)
insert into AP_SKELETON_DETAIL		values (@SKID_CHESSPDF, 1,NULL,'CHESS PDF',1,NULL,'RRBL',NULL,'41189710000FE06')
insert into AP_SKELETON_CONDITION	values (@SKID_CHESSPDF,'AP_HEADER','TransType',1,'FI')
insert into AP_SKELETON_CONDITION	values (@SKID_CHESSPDF,'UDF4','Note',9,'Inv')

DECLARE @SKID_SalesCommission as uniqueidentifier
set @SKID_SalesCommission = newid()
insert into AP_SKELETON_HEADER		values (@SKID_SalesCommission,'CHESS','01','100','CHESS','Salse Commission',	'Pre-Coded',1)
insert into AP_SKELETON_DETAIL		values (@SKID_SalesCommission, 1,NULL,'CHESS COMMISSION',1,NULL,'RRBL',NULL,'41189710000FE06')
insert into AP_SKELETON_CONDITION	values (@SKID_SalesCommission,'AP_HEADER','TransType',1,'FI')
insert into AP_SKELETON_CONDITION	values (@SKID_SalesCommission,'UDF4','Note',9,'Sales')


/*

'check

select * from AP_SUPPLIER where supplierid = (SELECT  supplierid FROM   AP_SUPPLIER_DUMMY )
SELECT *  FROM   AP_SKELETON_HEADER where skeletonid in (SELECT skeletonid  FROM   AP_SKELETON_HEADER where  supplierid = 'CHESS')
SELECT *  FROM   AP_SKELETON_DETAIL where skeletonid in (SELECT skeletonid  FROM   AP_SKELETON_HEADER where  supplierid = 'CHESS')
SELECT *  FROM   AP_SKELETON_CONDITION where skeletonid in (SELECT skeletonid  FROM   AP_SKELETON_HEADER where  supplierid = 'CHESS')

SELECT  * FROM   AP_SUPPLIER_DUMMY 
SELECT *  FROM   AP_SKELETON_HEADER_DUMMY 
SELECT *  FROM   AP_SKELETON_DETAIL_DUMMY 
SELECT *  FROM   AP_SKELETON_CONDITION_DUMMY

*/
GO

/****** Object:  StoredProcedure [dbo].[dlx_RR_Skeleton_Remove]    Script Date: 10/14/2012 08:26:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[dlx_RR_Skeleton_Remove]
@ID as varchar(50)
as

select skeletonid into #SK from AP_SKELETON_HEADER where name = @ID

delete from AP_SKELETON_CONDITION where skeletonid in (select skeletonid from #SK) -- = @id
delete from AP_SKELETON_DETAIL    where skeletonid in (select skeletonid from #SK) -- = @id
delete from AP_SKELETON_HEADER    where skeletonid in (select skeletonid from #SK) -- = @id



--  dlx_RR_Skeleton_Remove '0E6EB358-6E1A-4320-A9F2-009024C31FD1'
GO

/****** Object:  StoredProcedure [dbo].[taskworker_GetTypeInfo]    Script Date: 10/14/2012 08:26:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


create procedure [dbo].[taskworker_GetTypeInfo] 
@typename as varchar(50)
as 
SELECT * from twk_Assemblies where typename = @typename

GO

/****** Object:  StoredProcedure [dbo].[taskworker_Job_Complete]    Script Date: 10/14/2012 08:26:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[taskworker_Job_Complete] 
@ProcedureID int
,@currentStatus as varchar(50)
, @xAction as xml
, @xData as xml
, @comments as varchar(255)
as 
begin transaction SetJob_Complete
update TWK_RunProcedures set IsComplete = 1, currentstatus = @currentStatus, CompletedOn = GETDATE(), Comments = @comments
,ProcessingBy=''
where procedureID = @ProcedureID
commit transaction SetJob_Complete
GO

