CREATE Procedure [dbo].[W_MasterItemTypeSelectMain]    
@ItemTypeId int=0    
as    
if(@ItemTypeId>0)    
select * from W_MasterItemType where ItemTypeId=@ItemTypeId    
else    
select * from W_MasterItemType where IsActive=1 and IsMainType=1
order by ItemTypeName
