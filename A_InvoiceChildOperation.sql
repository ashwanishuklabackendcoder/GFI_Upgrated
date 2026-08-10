Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE  PROCEDURE [dbo].[A_InvoiceChildOperation]      
                                                                                                                                                                                                      
@ID varchar(2000)='',      
                                                                                                                                                                                                                                  
@OprType smallint=2,      
                                                                                                                                                                                                                                   
@UpdatedBy nvarchar='',      
                                                                                                                                                                                                                                
@Iserror int=0 output      
                                                                                                                                                                                                                                  
as      
                                                                                                                                                                                                                                                     
begin      
                                                                                                                                                                                                                                                  
set nocount on      
                                                                                                                                                                                                                                         
if @OprType=1       
                                                                                                                                                                                                                                         
begin      
                                                                                                                                                                                                                                                  
--if not exists(select 1 from Fees_BillSundryMaster where BillSundryID in (select items from dbo.Fun_SplitStr(@ID,',')))      
                                                                                                                               
--begin      
                                                                                                                                                                                                                                                
set  @Iserror=1      
                                                                                                                                                                                                                                        
--update A_InvoiceMaster set UpdatedBy=@UpdatedBy where BomId in (select items from dbo.Fun_SplitStr(@ID,','))       
                                                                                                                                        
delete from dbo.A_InvoiceChild where InvoiceChildID in (select items from dbo.Fun_SplitStr(@ID,','))      
                                                                                                                                                   
end      
                                                                                                                                                                                                                                                    
 ----else if @OprType=2      
                                                                                                                                                                                                                                
 ----begin      
                                                                                                                                                                                                                                             
 ----set  @Iserror=2      
                                                                                                                                                                                                                                   
 ---- update dbo.A_InvoiceMaster set isActive=1 where BomId in (select items from dbo.Fun_SplitStr(@ID,','))      
                                                                                                                                           
 ---- end      
                                                                                                                                                                                                                                              
 ----else if @OprType=3      
                                                                                                                                                                                                                                
 ----begin      
                                                                                                                                                                                                                                             
 ----set  @Iserror=3      
                                                                                                                                                                                                                                   
 ---- update dbo.A_InvoiceMaster set isActive=0 where BomId in (select items from dbo.Fun_SplitStr(@ID,','))      
                                                                                                                                           
 ---- end      
                                                                                                                                                                                                                                              
end      
                                                                                                                                                                                                                                                    
      
                                                                                                                                                                                                                                                       
--end                                                                                                                                                                                                                                                          
