Create   PROCEDURE [dbo].[Z_UsersMenuModify]  
    @LinkID BIGINT = 0,        
    @ModuleID BIGINT = 0,        
    @PageHeading NVARCHAR(200) = NULL,        
    @ParentID BIGINT = 0,        
    @PagePath NVARCHAR(200) = NULL,        
    @ActualName NVARCHAR(200) = NULL,        
    @IsView BIT,        
    @LevelNo INT = 0,        
    @SequenceNo INT = 0,       
    @CreatedDate DATETIME,        
    @CreatedBy NVARCHAR(200) = '',       
    @UpdatedBy NVARCHAR(200) = '',  
    @IsDashboard BIT,  
    @ShowInMenu BIT,  
    @ReturnVal INT OUTPUT        
AS        
BEGIN        
    SET NOCOUNT ON        
    SET @ReturnVal = 0        

    IF @ParentID = 0 SET @ParentID = NULL

    ---------------------------------------------------
    -- ? NEW VALIDATION: LABEL MUST EXIST
    ---------------------------------------------------
    IF NOT EXISTS (
        SELECT 1 
        FROM dbo.Z_MasterLabels 
        WHERE LabelName = @PageHeading
    )
    BEGIN
        SET @ReturnVal = -2   -- custom error for label not found
        RETURN
    END

    ---------------------------------------------------
    -- INSERT
    ---------------------------------------------------
    IF (@LinkID = 0)
    BEGIN
        IF EXISTS (
            SELECT 1 
            FROM dbo.Z_UsersMenu 
            WHERE PageHeading = @PageHeading 
              AND ModuleID = @ModuleID 
              AND ISNULL(ParentID,0) = ISNULL(@ParentID,0)
        )
        BEGIN
            SET @ReturnVal = -1
            RETURN
        END

        INSERT INTO dbo.Z_UsersMenu
        (
            ModuleID, PageHeading, ParentID, PagePath,
            ActualName, IsView, LevelNo, SequenceNo,
            CreatedBy, IsDashboard, ShowInMenu
        )
        VALUES
        (
            @ModuleID, @PageHeading, @ParentID, @PagePath,
            @ActualName, @IsView, @LevelNo, @SequenceNo,
            @CreatedBy, @IsDashboard, @ShowInMenu
        )

        SET @ReturnVal = SCOPE_IDENTITY()
    END

    ---------------------------------------------------
    -- UPDATE
    ---------------------------------------------------
    ELSE
    BEGIN
        IF EXISTS (
            SELECT 1 
            FROM dbo.Z_UsersMenu 
            WHERE PageHeading = @PageHeading 
              AND ModuleID = @ModuleID 
              AND ISNULL(ParentID,0) = ISNULL(@ParentID,0)
              AND LinkID <> @LinkID
        )
        BEGIN
            SET @ReturnVal = -1
            RETURN
        END

        UPDATE dbo.Z_UsersMenu
        SET 
            ModuleID = @ModuleID,
            PageHeading = @PageHeading,
            ParentID = @ParentID,
            PagePath = @PagePath,
            ActualName = @ActualName,
            IsView = @IsView,
            LevelNo = @LevelNo,
            SequenceNo = @SequenceNo,
            IsDashboard = @IsDashboard,
            ShowInMenu = @ShowInMenu
        WHERE LinkID = @LinkID

        SET @ReturnVal = @LinkID
    END
END

