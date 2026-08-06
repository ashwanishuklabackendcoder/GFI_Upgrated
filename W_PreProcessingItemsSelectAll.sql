Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE Proc [dbo].[W_PreProcessingItemsSelectAll]
                                                                                                                                                                                                            
@PreProcessingId int =0
                                                                                                                                                                                                                                      
as
                                                                                                                                                                                                                                                           
if(@PreProcessingId>0)
                                                                                                                                                                                                                                       
select * from W_PreProcessingItems where PreProcessingId=@PreProcessingId
                                                                                                                                                                                    
else
                                                                                                                                                                                                                                                         
select * from W_PreProcessingItems -- order by BomName                                                                                                                                                                                                         
