Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[W_MasterItemModify]      
                                                                                                                                                                                                            
@ItemID bigint=0,      
                                                                                                                                                                                                                                      
@ItemName NVarChar(500),
                                                                                                                                                                                                                                     
@ItemCode NVarChar(50),
                                                                                                                                                                                                                                      
@ShortName nvarchar(50),
                                                                                                                                                                                                                                     
@IsActive bit,
                                                                                                                                                                                                                                               
@ItemCatID int,
                                                                                                                                                                                                                                              
@ItemTypeID int,
                                                                                                                                                                                                                                             
@StatusID int,
                                                                                                                                                                                                                                               
@IsEditable bit,
                                                                                                                                                                                                                                             
@Description NVarChar(1000)	
                                                                                                                                                                                                                                 
,
                                                                                                                                                                                                                                                            
@StorageDetails NVarChar(1000),
                                                                                                                                                                                                                              
@CreatedDate DateTime=getdate,
                                                                                                                                                                                                                               
@CreatedBy NVarChar(200),
                                                                                                                                                                                                                                    
@Tags NVarChar(1000)  ,
                                                                                                                                                                                                                                      
@TentativeExpiryDays int =0,
                                                                                                                                                                                                                                 
@PurchaseUnit bigint=0,
                                                                                                                                                                                                                                      
@BrandId bigint =0,
                                                                                                                                                                                                                                          
@MasterItemTypeId bigint=null,
                                                                                                                                                                                                                               
@ReturnVal int=0 output      
                                                                                                                                                                                                                                
as    	
                                                                                                                                                                                                                                                      
  
                                                                                                                                                                                                                                                           
begin      
                                                                                                                                                                                                                                                  
set nocount on      
                                                                                                                                                                                                                                         
set @ReturnVal = 0      
                                                                                                                                                                                                                                     
      
                                                                                                                                                                                                                                                       
      
                                                                                                                                                                                                                                                       
IF @ItemID = 0      
                                                                                                                                                                                                                                         
Begin      
                                                                                                                                                                                                                                                  
if not exists(select 1 from dbo.W_MasterItem where ( ItemName =@ItemName and ItemCatID=@ItemCatID and ItemTypeID=@ItemTypeID ))      
                                                                                                                        
Begin      
                                                                                                                                                                                                                                                  
 	
                                                                                                                                                                                                                                                           
     
                                                                                                                                                                                                                                                        
insert into W_MasterItem(ItemName,ItemCode,ShortName,ItemCatID,ItemTypeID,StatusID,Description,CreatedDate,CreatedBy,Tags,TentativeExpiryDays,PurchaseUnit,BrandId,StorageDetails,MasterItemTypeId)
                                                          
values (@ItemName,@ItemCode,@ShortName,@ItemCatID,@ItemTypeID,@StatusID,@Description,@CreatedDate,@CreatedBy,@Tags,@TentativeExpiryDays,@PurchaseUnit,@BrandId,@StorageDetails,@MasterItemTypeId)    
                                                        
set @ReturnVal=scope_Identity()      
                                                                                                                                                                                                                        
End      
                                                                                                                                                                                                                                                    
else      
                                                                                                                                                                                                                                                   
set @ReturnVal= -1      
                                                                                                                                                                                                                                     
End      
                                                                                                                                                                                                                                                    
Else      
                                                                                                                                                                                                                                                   
Begin      
                                                                                                                                                                                                                                                  
if not exists(select 1 from dbo.W_MasterItem where (ItemName =@ItemName and ItemCatID=@ItemCatID and ItemTypeID=@ItemTypeID) and ItemID <> @ItemID)      
                                                                                                    
Begin     
                                                                                                                                                                                                                                                   
update W_MasterItem set
                                                                                                                                                                                                                                      
ItemName=@ItemName,
                                                                                                                                                                                                                                          
ItemCode=@ItemCode,
                                                                                                                                                                                                                                          
ShortName=@ShortName,
                                                                                                                                                                                                                                        
ItemCatID=@ItemCatID,
                                                                                                                                                                                                                                        
ItemTypeID=@ItemTypeID,
                                                                                                                                                                                                                                      
PurchaseUnit=@PurchaseUnit,
                                                                                                                                                                                                                                  
BrandId=@BrandId,
                                                                                                                                                                                                                                            
StatusID=@StatusID,
                                                                                                                                                                                                                                          
Description=@Description,
                                                                                                                                                                                                                                    
TentativeExpiryDays=@TentativeExpiryDays,
                                                                                                                                                                                                                    
Tags=@Tags,
                                                                                                                                                                                                                                                  
MasterItemTypeId=@MasterItemTypeId where ItemID=@ItemID
                                                                                                                                                                                                      
   
                                                                                                                                                                                                                                                          
      
                                                                                                                                                                                                                                                       
set @ReturnVal =@ItemID      
                                                                                                                                                                                                                                
End      
                                                                                                                                                                                                                                                    
else      
                                                                                                                                                                                                                                                   
set @ReturnVal= -1      
                                                                                                                                                                                                                                     
End      
                                                                                                                                                                                                                                                    
      
                                                                                                                                                                                                                                                       
End                                                                                                                                                                                                                                                            
