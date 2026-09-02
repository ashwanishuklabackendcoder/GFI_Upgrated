ALTER PROCEDURE [dbo].[W_ProductionList]      
                                                                                                                                                                                                              
    @ProductionId int=0,      
                                                                                                                                                                                                                               
    @Remarks nvarchar(200)=null,
                                                                                                                                                                                                                             
	 @SkuId int=0, 
                                                                                                                                                                                                                                             
	  @BomId int=0, 
                                                                                                                                                                                                                                            
    @CurrentPage int=1 output,        
                                                                                                                                                                                                                       
    @RecordPerPage int=10,        
                                                                                                                                                                                                                           
    @TotalRecord  int=0 output,           
                                                                                                                                                                                                                   
    @SortOrd varchar(5)='DESC',          
                                                                                                                                                                                                                    
    @SortColumn varchar(20)='ProductionId'        
                                                                                                                                                                                                           
As      
                                                                                                                                                                                                                                                     
begin      
                                                                                                                                                                                                                                                  
 set nocount on          
                                                                                                                                                                                                                                    
 set @Remarks = dbo.ReplaceSingleQuote(@Remarks)      
                                                                                                                                                                                                       
 declare @Query as varchar(1000)          
                                                                                                                                                                                                                   
    declare @RecQuery as nvarchar(2500)          
                                                                                                                                                                                                            
    set @Query=''       
                                                                                                                                                                                                                                     
     if @ProductionId<>0          
                                                                                                                                                                                                                           
  set @Query=@Query + ' and ProductionId =' + cast(@ProductionId as varchar) 
                                                                                                                                                                                
       if @SkuId<>0          
                                                                                                                                                                                                                                
  set @Query=@Query + ' and SkuId =' + cast(@SkuId as varchar)  
                                                                                                                                                                                             
       if @BomId<>0          
                                                                                                                                                                                                                                
  set @Query=@Query + ' and BomId =' + cast(@BomId as varchar)     
                                                                                                                                                                                          
 if @Remarks<>''          
                                                                                                                                                                                                                                   
  set @Query=@Query + ' and Remarks like ''%' + @Remarks + '%'''   
                                                                                                                                                                                          
declare @MaxPage int          
                                                                                                                                                                                                                               
 set @RecQuery='select @TotalRecord=count(1) from dbo.W_Production where 1=1 ' + @Query             
                                                                                                                                                         
 exec dbo.sp_ExecuteSql @RecQuery,N'@TotalRecord int output',@TotalRecord output      
                                                                                                                                                                       
 set @MaxPage=ceiling(isnull(@TotalRecord,0) / (@RecordPerPage * 1.0))            
                                                                                                                                                                           
 if @MaxPage < @CurrentPage          
                                                                                                                                                                                                                        
 begin          
                                                                                                                                                                                                                                             
  if @MaxPage<=0          
                                                                                                                                                                                                                                   
   set @CurrentPage = 1          
                                                                                                                                                                                                                            
  else          
                                                                                                                                                                                                                                             
   set @CurrentPage = @MaxPage                         
                                                                                                                                                                                                      
 end          
                                                                                                                                                                                                                                               
 declare @Top as int          
                                                                                                                                                                                                                               
 declare @Bottom as int          
                                                                                                                                                                                                                            
 set @Top=((@CurrentPage - 1) * @RecordPerPage + 1)          
                                                                                                                                                                                                
 set @Bottom = (@CurrentPage * @RecordPerPage)           
                                                                                                                                                                                                    
 set @RecQuery='select * from (select row_number() over (order by '+ @SortColumn + ' ' + @SortOrd + ') as RowNumber,ProductionId	,W_Production.BomId,	convert(varchar(12),CookingDate,106) CookingDate	,convert(varchar(12),FilledDate,106) FilledDate,	convert

                                                                                                                                                                                                                                                             
(varchar(12),ExpiryDate,106) ExpiryDate,	
                                                                                                                                                                                                                    
(select ZC.CountryName from Z_CountriesMaster ZC where ZC.CountryID=W_Production.PackedCountry) as PackedCountryName,(select ZC.SkuName from W_MasterSku ZC where ZC.SkuId=W_Production.SkuId) as SkuName,	Colli,	PalletNumber	,ProcessEmployees	, W_Production

                                                                                                                                                                                                                                                             
.KettleId,	KettleRun	,BatchNo,	FillingBottles,	FillingPerBottleUnit,	ExtraBottles	,Remarks,	DocumentUpload,(select WC.WarehouseName from W_MasterWarehouse WC where WC.WarehouseId=W_Production.WarehouseId)	WarehouseName,
                                  
W_Production.BomQty, PackedCountry,W_Production.SkuId,W_Production.WarehouseId, W_MasterBom.BomName,W_Production.IsComplete AS IsComplete '+          
                                                                                                       
 ' from dbo.W_Production inner join W_MasterBom on W_Production.BomId=W_MasterBom.BomId where 1=1 ' + @Query + ')t1 where t1.RowNumber>=' + cast(@Top as varchar) + ' and t1.RowNumber <=' + cast(@Bottom as varchar) + ' order by t1.RowNumber'          
                              
print @RecQuery  
                                                                                                                                                                                                                                            
 exec dbo.sp_ExecuteSql @RecQuery      
                                                                                                                                                                                                                      
end                                                                                                                                                                                                                                                            
