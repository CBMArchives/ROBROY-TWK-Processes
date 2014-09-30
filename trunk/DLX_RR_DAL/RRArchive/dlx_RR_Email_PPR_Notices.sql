USE [DocumentManager]
GO
/****** Object:  StoredProcedure [dbo].[dlx_RR_Email_PPR_Notices]    Script Date: 08/29/2012 13:51:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[dlx_RR_Email_PPR_Notices]
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
	IF (@remaining_approvers >= @approvals_needed) SET @MustWait = 0 ELSE SET @MustWait = 1



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



	FETCH NEXT FROM JobsToProcess INTO @pprheaderid
END
CLOSE JobsToProcess
DEALLOCATE JobsToProcess

