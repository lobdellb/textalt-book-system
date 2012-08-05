function valid = validate_magic_number_file( file_name );

if exist(file_name,'file')
   fp = fopen(file_name);
   html = char( fread(fp)');
   tmp=fclose(fp);

%   I1 = findstr( html, '<html>');
%   I2 = findstr( html, '<body>');
   I3 = findstr( html, '<select name=');
   I4 = findstr( html, '<option value');
   I5 = findstr( html, '</option>' );
   I6 = findstr( html, '</select>');
%   I6 = findstr( html, '</body>');
%   I7 = findstr( html, '</html>');

   if length( [I3 I4 I5 I6 ] ) >= 4 
      valid = 1;
   else
      valid = 0;
   end

else
   valid = 0;
end