                                                                                                                                                                                                                                                                
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
ALTER PROCEDURE [dbo].[ItemBatchNumberList]
    @ItemID int=0,
    @CurrentPage INT = 1 OUTPUT,
    @RecordPerPage INT = 10,
    @TotalRecord INT = 0 OUTPUT,
    @SortOrd VARCHAR(5) = 'DESC'
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Query AS VARC
