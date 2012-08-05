function valid = validate_book_records( file_name )

if exist(file_name,'file')
   fp = fopen(file_name);
   html = char( fread(fp)');
   tmp=fclose(fp);

   % make sure the file is complete
   I = findstr( html,'</select>');

   if length(I) == 1
      valid = 1;
   else
      valid = 0;
   end

else
   valid = 0;
end












%  
%     I = findstr( html, '<div id="bookTbl">' );
%  
%     if length(I == 1)
%        
%        I1 = length( findstr( html, 'Edition:' ) );
%        I2 = length( findstr( html, 'Publisher:' ) );
%  
%        I3 = length( findstr( html, 'Currently no textbook has been assigned for this course.  You can still reserve now by clicking the') );
%  
%        if I3 > 0 | ( I1 > 0 & I2 > 0 )
%           valid = 1;
%        else
%           valid = 0;
%        end
%  
%     else
%        valid = 0;
%     end
%  
