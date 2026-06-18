  
Create    PROCEDURE [dbo].[W_CustomerOrderReturnOperation]          
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
--update W_OrderCustomerItems set UpdatedBy=@UpdatedBy where BomItemsId in (select items from dbo.Fun_SplitStr(@ID,','))           
delete from dbo.W_OrderCustomerReturn where OrderReturnID in (select items from dbo.Fun_SplitStr(@ID,','))          
end          
 --else if @OprType=2          
 --begin          
 --set  @Iserror=2          
 -- update dbo.W_OrderCustomerItems set isActive=1 where BomItemsId in (select items from dbo.Fun_SplitStr(@ID,','))          
 -- end          
 --else if @OprType=3          
 --begin          
 --set  @Iserror=3          
 -- update dbo.W_OrderCustomerItems set isActive=0 where BomItemsId in (select items from dbo.Fun_SplitStr(@ID,','))          
 -- end          
end          
          
--end  

