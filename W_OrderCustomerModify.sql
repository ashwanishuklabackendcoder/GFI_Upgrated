Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE   PROCEDURE [dbo].[W_OrderCustomerModify]              
                                                                                                                                                                                               
@OrderID bigint=0,              
                                                                                                                                                                                                                             
@OrderNo NVarChar(50),  
                                                                                                                                                                                                                                     
@CreatedDate DateTime,  
                                                                                                                                                                                                                                     
@CreatedBy NVarChar(200),
                                                                                                                                                                                                                                    
@OrderDate Date,  
                                                                                                                                                                                                                                           
@SentDate Date,  
                                                                                                                                                                                                                                            
@Remarks NVarChar(1000), 
                                                                                                                                                                                                                                    
@CustomerID bigint=0,  
                                                                                                                                                                                                                                      
@UploadedFile NVarChar(2000),
                                                                                                                                                                                                                                
@ReturnVal int=0 output              
                                                                                                                                                                                                                        
as              
                                                                                                                                                                                                                                             
begin              
                                                                                                                                                                                                                                          
set nocount on              
                                                                                                                                                                                                                                 
set @ReturnVal = 0              
                                                                                                                                                                                                                             
              
                                                                                                                                                                                                                                               
              
                                                                                                                                                                                                                                               
IF @OrderID = 0              
                                                                                                                                                                                                                                
Begin              
                                                                                                                                                                                                                                          
if not exists(select 1 from dbo.W_OrderCustomer where ( OrderNo =@OrderNo ))              
                                                                                                                                                                   
Begin              
                                                                                                                                                                                                                                          
insert into W_OrderCustomer(CustomerID,OrderNo,OrderDate,SentDate,CreatedDate,CreatedBy,Remarks,DocumentUpload)   
                                                                                                                                           
values (@CustomerID,@OrderNo,@OrderDate,@SentDate,getdate(),@CreatedBy,@Remarks,@UploadedFile)  
                                                                                                                                                             
      
                                                                                                                                                                                                                                                       
  
                                                                                                                                                                                                                                                           
set @ReturnVal=scope_Identity()              
                                                                                                                                                                                                                
End              
                                                                                                                                                                                                                                            
else              
                                                                                                                                                                                                                                           
set @ReturnVal= -1              
                                                                                                                                                                                                                             
End              
                                                                                                                                                                                                                                            
Else              
                                                                                                                                                                                                                                           
Begin              
                                                                                                                                                                                                                                          
if not exists(select 1 from dbo.W_OrderCustomer where (OrderNo =@OrderNo) and OrderID <> @OrderID)              
                                                                                                                                             
Begin                  
                                                                                                                                                                                                                                      
	update W_OrderCustomer  
                                                                                                                                                                                                                                    
	set CustomerID=@CustomerID,OrderNo=@OrderNo,OrderDate=@OrderDate,SentDate=@SentDate,
                                                                                                                                                                        
	Remarks=@Remarks,DocumentUpload=@UploadedFile where OrderID=@OrderID              
                                                                                                                                                                          
	set @ReturnVal =@OrderID              
                                                                                                                                                                                                                      
End              
                                                                                                                                                                                                                                            
else              
                                                                                                                                                                                                                                           
set @ReturnVal= @OrderID             
                                                                                                                                                                                                                        
End              
                                                                                                                                                                                                                                            
              
                                                                                                                                                                                                                                               
End                                                                                                                                                                                                                                                            
