Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[W_ProductionModify]      
                                                                                                                                                                                                            
    @ProductionId bigint,
                                                                                                                                                                                                                                    
    @BomId bigint,
                                                                                                                                                                                                                                           
    @CookingDate Date,
                                                                                                                                                                                                                                       
    @FilledDate Date,
                                                                                                                                                                                                                                        
    @ExpiryDate Date,
                                                                                                                                                                                                                                        
    @PackedCountry NVarChar(200),
                                                                                                                                                                                                                            
    @SkuId NVarChar(100),
                                                                                                                                                                                                                                    
    @Colli NVarChar(100),
                                                                                                                                                                                                                                    
    @PalletNumber NVarChar(100),
                                                                                                                                                                                                                             
    @ProcessEmployees NVarChar(200),
                                                                                                                                                                                                                         
    @KettleId NVarChar(200),
                                                                                                                                                                                                                                 
    @KettleRun Int,
                                                                                                                                                                                                                                          
    @BatchNo NVarChar(200),
                                                                                                                                                                                                                                  
    @FillingBottles Int,
                                                                                                                                                                                                                                     
    @FillingPerBottleUnit bigint,
                                                                                                                                                                                                                            
    @ExtraBottles float,
                                                                                                                                                                                                                                     
    @Remarks NVarChar(1000),
                                                                                                                                                                                                                                 
    @DocumentUpload NVarChar(500),
                                                                                                                                                                                                                           
    @WarehouseId bigint,
                                                                                                                                                                                                                                     
    @BomQty float,
                                                                                                                                                                                                                                           
    @ReturnVal int=0 output      
                                                                                                                                                                                                                            
As      
                                                                                                                                                                                                                                                     
begin      
                                                                                                                                                                                                                                                  
    set nocount on      
                                                                                                                                                                                                                                     
    set @ReturnVal = 0      
                                                                                                                                                                                                                                 
      
                                                                                                                                                                                                                                                       
    IF @ProductionId = 0      
                                                                                                                                                                                                                               
    Begin      
                                                                                                                                                                                                                                              
        if not exists(select 1 from dbo.W_Production where ( ProductionId =0 ))      
                                                                                                                                                                        
        Begin  
                                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
            declare @ShortName nvarchar(10), @KettleNo nvarchar(500)
                                                                                                                                                                                         
            select @ShortName=b.ShortName from W_MasterBom a inner join W_MasterItem b on a.ItemId=b.ItemID where BomId=@BomId
                                                                                                                               
            
                                                                                                                                                                                                                                                 
            set @KettleNo = ''
                                                                                                                                                                                                                               
            if (isnull(@KettleId, '') <> '')
                                                                                                                                                                                                                 
            BEGIN
                                                                                                                                                                                                                                            
                SELECT @KettleNo = @KettleNo + KettleNumber + ', ' 
                                                                                                                                                                                          
                FROM W_MasterKettle 
                                                                                                                                                                                                                         
                WHERE KettleId IN (SELECT CAST(value AS BIGINT) FROM STRING_SPLIT(@KettleId, ','))
                                                                                                                                                           

                                                                                                                                                                                                                                                             
                if (LEN(@KettleNo) > 1)
                                                                                                                                                                                                                      
                    set @KettleNo = SUBSTRING(@KettleNo, 1, LEN(@KettleNo) - 1)
                                                                                                                                                                              
            END
                                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
            declare @Batch nvarchar(100)
                                                                                                                                                                                                                     
            IF (LTRIM(RTRIM(ISNULL(@BatchNo, ''))) != '')
                                                                                                                                                                                                    
                SET @Batch = @BatchNo;
                                                                                                                                                                                                                       
            ELSE
                                                                                                                                                                                                                                             
                SET @Batch = @ShortName +' | ' + FORMAT(@CookingDate, 'yyMMdd') + '.' + CAST(@KettleNo AS NVARCHAR(100)) + '.' + CAST(@KettleRun AS NVARCHAR(10));
                                                                                           

                                                                                                                                                                                                                                                             
            insert into W_Production(BomId,CookingDate,FilledDate,ExpiryDate,PackedCountry,SkuId,Colli,PalletNumber,ProcessEmployees,KettleId,KettleRun,BatchNo,FillingBottles,FillingPerBottleUnit,ExtraBottles,Remarks,DocumentUpload,WarehouseId,BomQty)
  
            values (@BomId,@CookingDate,@FilledDate,@ExpiryDate,@PackedCountry,@SkuId,@Colli,@PalletNumber,@ProcessEmployees,@KettleId,@KettleRun,@Batch,@FillingBottles,@FillingPerBottleUnit,@ExtraBottles,@Remarks,@DocumentUpload,@WarehouseId,@BomQty)
  
            set @ReturnVal=scope_Identity()  
                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
        End      
                                                                                                                                                                                                                                            
        else      
                                                                                                                                                                                                                                           
            set @ReturnVal= -1      
                                                                                                                                                                                                                         
    End      
                                                                                                                                                                                                                                                
    Else      
                                                                                                                                                                                                                                               
    Begin      
                                                                                                                                                                                                                                              
        if exists(select 1 from dbo.W_Production where ProductionId = @ProductionId)      
                                                                                                                                                                   
        Begin     
                                                                                                                                                                                                                                           
            update W_Production set BomId=@BomId,BomQty=@BomQty,CookingDate=@CookingDate,FilledDate=@FilledDate,
                                                                                                                                             
            ExpiryDate=@ExpiryDate,PackedCountry=@PackedCountry,SkuId=@SkuId,Colli=@Colli,PalletNumber=@PalletNumber,
                                                                                                                                        
            ProcessEmployees=@ProcessEmployees,KettleId=@KettleId,KettleRun=@KettleRun,
                                                                                                                                                                      
            BatchNo=@BatchNo,FillingBottles=@FillingBottles,
                                                                                                                                                                                                 
            FillingPerBottleUnit=@FillingPerBottleUnit,ExtraBottles=@ExtraBottles,Remarks=@Remarks,DocumentUpload=@DocumentUpload,WarehouseId=@WarehouseId where ProductionId=@ProductionId
                                                                  
      
                                                                                                                                                                                                                                                       
            set @ReturnVal =@ProductionId      
                                                                                                                                                                                                              
        End      
                                                                                                                                                                                                                                            
        else      
                                                                                                                                                                                                                                           
            set @ReturnVal= -1      
                                                                                                                                                                                                                         
    End      
                                                                                                                                                                                                                                                
end                                                                                                                                                                                                                                                            
