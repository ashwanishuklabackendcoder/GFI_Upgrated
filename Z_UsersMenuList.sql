Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
                                                                                                                                                                                                                                                         
CREATE PROCEDURE [dbo].[Z_UsersMenuList]        
                                                                                                                                                                                                             
@LinkID int = 0,      
                                                                                                                                                                                                                                       
@ModuleID int=0,      
                                                                                                                                                                                                                                       
@PageHeading varchar(200)='',    
                                                                                                                                                                                                                            
@MenuName varchar(200)='',     
                                                                                                                                                                                                                              
@PagePath varchar(200)='',     
                                                                                                                                                                                                                              
@ActualName varchar(200)='',     
                                                                                                                                                                                                                            
@LevelNo int=0,     
                                                                                                                                                                                                                                         
@SequenceNo int=0,         
                                                                                                                                                                                                                                  
@CurrentPage int=1 output,      
                                                                                                                                                                                                                             
@RecordPerPage int=50,      
                                                                                                                                                                                                                                 
@TotalRecord  int=0 output,         
                                                                                                                                                                                                                         
@SortOrd varchar(5)='DESC',        
                                                                                                                                                                                                                          
@SortColumn varchar(200)='LinkID'        
                                                                                                                                                                                                                    
as        
                                                                                                                                                                                                                                                   
begin        
                                                                                                                                                                                                                                                
 set nocount on        
                                                                                                                                                                                                                                      
 set @PageHeading = dbo.ReplaceSingleQuote(@PageHeading)        
                                                                                                                                                                                             
 declare @Query as varchar(max)        
                                                                                                                                                                                                                      
declare @RecQuery as nvarchar(max)        
                                                                                                                                                                                                                   
 set @Query=''        
                                                                                                                                                                                                                                       
 if @LinkID<>0        
                                                                                                                                                                                                                                       
  set @Query=@Query + ' and Z_UsersMenu.LinkID =' + cast(@LinkID as varchar)       
                                                                                                                                                                          
   if @ModuleID<>0        
                                                                                                                                                                                                                                   
  set @Query=@Query + ' and Z_UsersMenu.ModuleID =' + cast(@ModuleID as varchar)      
                                                                                                                                                                       
 if @PageHeading<>''        
                                                                                                                                                                                                                                 
  set @Query=@Query + ' and (Z_UsersMenu.PageHeading like ''%' + @PageHeading + '%'''     
                                                                                                                                                                   
  if @PageHeading<>''        
                                                                                                                                                                                                                                
  set @Query=@Query + ' or Z_UsersModule.ModuleName like ''%' + @PageHeading + '%'''     
                                                                                                                                                                    
  if @PageHeading<>''        
                                                                                                                                                                                                                                
  set @Query=@Query + ' or Z_UsersMenu.PagePath like ''%' + @PageHeading + '%'''     
                                                                                                                                                                        
  if @PageHeading<>''        
                                                                                                                                                                                                                                
  set @Query=@Query + ' or Z_UsersMenu.ActualName like ''%' + @PageHeading + '%'''     
                                                                                                                                                                      
   if @PageHeading<>''        
                                                                                                                                                                                                                               
  set @Query=@Query + ' or English like ''%' + @PageHeading + '%'''     
                                                                                                                                                                                     
  if @PageHeading<>''        
                                                                                                                                                                                                                                
   set @Query=@Query + ' or Z_UsersMenu.LevelNo like ''' + @PageHeading + ''''     
                                                                                                                                                                          
     if @PageHeading<>''        
                                                                                                                                                                                                                             
  set @Query=@Query + ' or Z_UsersMenu.SequenceNo like ''' + @PageHeading + ''')'   
                                                                                                                                                                         
    if @PagePath<>''        
                                                                                                                                                                                                                                 
  set @Query=@Query + ' and Z_UsersMenu.PagePath like ''%' + @PagePath + '%'''     
                                                                                                                                                                          
  if @ActualName<>''        
                                                                                                                                                                                                                                 
  set @Query=@Query + ' and Z_UsersMenu.ActualName like ''%' + @ActualName + '%'''     
                                                                                                                                                                      
   if @MenuName<>''        
                                                                                                                                                                                                                                  
  set @Query=@Query + ' and English like ''%' + @MenuName + '%'''     
                                                                                                                                                                                       
  if @LevelNo<>0        
                                                                                                                                                                                                                                     
   set @Query=@Query + ' and Z_UsersMenu.LevelNo ='+cast(@LevelNo as varchar)     
                                                                                                                                                                           
     if @SequenceNo<>0        
                                                                                                                                                                                                                               
  set @Query=@Query + ' and Z_UsersMenu.SequenceNo ='+cast(@SequenceNo as varchar)     
                                                                                                                                                                      
 --if @PageUrl <> ''        
                                                                                                                                                                                                                                 
  --set @Query=@Query + ' and PageUrl like ''' + @PageUrl + '%'''        
                                                                                                                                                                                    
 declare @MaxPage int        
                                                                                                                                                                                                                                
 set @RecQuery='select @TotalRecord=count(1) from  Z_UsersMenu inner join Z_UsersModule on Z_UsersModule.ModuleID=Z_UsersMenu.ModuleID    
                                                                                                                   
  left join Z_MasterLabels on Z_UsersMenu.PageHeading=Z_MasterLabels.LabelName    
                                                                                                                                                                           
  where 1=1 and Z_UsersModule.IsActive=1 ' + @Query           
                                                                                                                                                                                               
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
                                                                                                                                                                                                      
 set @RecQuery='select * from (select row_number() over (order by '+ @SortColumn + ' ' + @SortOrd + ') as RowNumber,Z_UsersModule.ModuleName,ShowInMenu,LinkID,Z_UsersMenu.ModuleID,PageHeading,ParentID,PagePath,    
                                       
 ActualName,IsView,LevelNo,SequenceNo,IsDashboard,Z_MasterLabels.English as MainMenuName '+        
                                                                                                                                                          
 ' from Z_UsersMenu inner join Z_UsersModule on Z_UsersModule.ModuleID=Z_UsersMenu.ModuleID    
                                                                                                                                                              
  left join Z_MasterLabels on Z_UsersMenu.PageHeading=Z_MasterLabels.LabelName    
                                                                                                                                                                           
  where 1=1 and Z_UsersModule.IsActive=1 ' + @Query + ')t1 where t1.RowNumber>=' + cast(@Top as varchar) + ' and t1.RowNumber <=' + cast(@Bottom as varchar)        
                                                                                         
 exec dbo.sp_ExecuteSql @RecQuery        
                                                                                                                                                                                                                    
 --print @RecQuery    
                                                                                                                                                                                                                                       
end    
                                                                                                                                                                                                                                                      
  
                                                                                                                                                                                                                                                           
    
                                                                                                                                                                                                                                                         
