Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE Proc [dbo].[A_InvoicesItemByInvoicesID]
                                                                                                                                                                                                               
@InvoiceID int =0
                                                                                                                                                                                                                                            
as
                                                                                                                                                                                                                                                           
if(@InvoiceID>0)
                                                                                                                                                                                                                                             
select * from A_InvoiceChild where InvoiceID=@InvoiceID
                                                                                                                                                                                                      
else
                                                                                                                                                                                                                                                         
select * from A_InvoiceChild                                                                                                                                                                                                                                   
