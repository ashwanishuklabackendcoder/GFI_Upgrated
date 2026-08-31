ALTER PROCEDURE [dbo].[W_MasterBomList]
    @BomId BIGINT = 0,
    @BomName NVARCHAR(500) = '',
    @ItemId BIGINT = 0,
    @CurrentPage INT = 1,
    @RecordPerPage INT = 10,
    @TotalRecord INT OUTPUT,
    @SortColumn VARCHAR(50) = 'BomId',
    @SortOrd VARCHAR(20) = 'DESC',
    @ItemTypeId INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartRow INT
    DECLARE @EndRow INT

    SET @StartRow = ((@CurrentPage - 1) * @RecordPerPage) + 1
    SET @EndRow = @CurrentPage * @RecordPerPage

    -- Count total records
    SELECT @TotalRecord = COUNT(1)
    FROM dbo.W_MasterBom b
    LEFT JOIN dbo.W_MasterItem i ON b.ItemId = i.ItemID
    WHERE (@ItemId = 0 OR b.ItemId = @ItemId)
      AND (@ItemTypeId = 0 OR b.ItemTypeId = @ItemTypeId)
      AND (@BomName = '' 
           OR b.BomName LIKE '%' + @BomName + '%'
           OR i.ItemName LIKE '%' + @BomName + '%'
           OR EXISTS (
               SELECT 1 
               FROM dbo.W_MasterBomItems bi
               INNER JOIN dbo.W_MasterItem child_i ON bi.ItemId = child_i.ItemID
               WHERE bi.BomId = b.BomId 
                 AND child_i.ItemName LIKE '%' + @BomName + '%'
           ));

    -- Select paged data
    -- Dynamic SQL is generally used for sorting with variables, but assuming a generic approach here or update as needed
    -- For safety and full dynamic sorting support (like W_MasterBom.BomId):
    DECLARE @SQL NVARCHAR(MAX) = N'
    WITH CTE AS (
        SELECT 
            b.BomId, 
            b.ItemId, 
            COALESCE(i.ItemName, '''') AS ItemName,
            b.BomName, 
            b.Quantity, 
            b.UnitId, 
            COALESCE(u.UnitName, '''') AS UnitName,
            b.ExtraExpensesPerPiece, 
            b.CreatedDate, 
            b.CreatedBy, 
            b.IsActive, 
            b.ItemTypeId,
            ROW_NUMBER() OVER (
                ORDER BY ' + 
                CASE 
                    WHEN @SortColumn LIKE '%BomId%' THEN 'b.BomId' 
                    WHEN @SortColumn LIKE '%BomName%' THEN 'b.BomName'
                    WHEN @SortColumn LIKE '%ItemName%' THEN 'i.ItemName'
                    ELSE 'b.BomId' 
                END + ' ' + @SortOrd + '
            ) AS RowNum
        FROM dbo.W_MasterBom b
        LEFT JOIN dbo.W_MasterUnit u ON b.UnitId = u.UnitId
        LEFT JOIN dbo.W_MasterItem i ON b.ItemId = i.ItemID
        WHERE (@ItemId = 0 OR b.ItemId = @ItemId)
          AND (@ItemTypeId = 0 OR b.ItemTypeId = @ItemTypeId)
          AND (@BomName = '''' 
               OR b.BomName LIKE ''%'' + @BomName + ''%''
               OR i.ItemName LIKE ''%'' + @BomName + ''%''
               OR EXISTS (
                   SELECT 1 
                   FROM dbo.W_MasterBomItems bi
                   INNER JOIN dbo.W_MasterItem child_i ON bi.ItemId = child_i.ItemID
                   WHERE bi.BomId = b.BomId 
                     AND child_i.ItemName LIKE ''%'' + @BomName + ''%''
               ))
    )
    SELECT * 
    FROM CTE 
    WHERE RowNum BETWEEN @StartRow AND @EndRow;
    '

    EXEC sp_executesql @SQL, 
        N'@ItemId BIGINT, @ItemTypeId INT, @BomName NVARCHAR(500), @StartRow INT, @EndRow INT',
        @ItemId, @ItemTypeId, @BomName, @StartRow, @EndRow;

END
GO
