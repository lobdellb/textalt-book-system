function [ magic_numbers, item_str] = parse_section_select_box_code( filename )

% Format of the file:

%  <html>
%  <body>
%  <select name='s3' class='sSelect'  onkeydown='readKeyboard(event)' onclick='javascript:processDropdown();' onblur='javascript:processDropdown();'>
%  <option value =''>Select Department</option>
%  <option value ='31498511'>ACE</option>
%  <option value ='31498494'>AERO</option>
%  ...
%  <option value ='31498602'>UCOL</option>
%  <option value ='31498603'>WOST</option>
%  </select>
%  </body>
%  </html>

fp = fopen( filename );

if ~fp
   disp(['warning didnt open ' filename])
end

s = fgetl(fp);

%<option value
while ~length(strfind(s,'<option value')) %~strcmp(s(1:min(length('<option value'),length(s))),'<option value')
   s = fgetl(fp);
end

s = fgetl(fp);

%keyboard

index = 1;

while ~length(strfind(s,'</select>')) & ~feof(fp)


   if length(strfind(s,'<option value'))
   
      I = find(s == '''');
      
      magic_numbers(index) = { s(I(1)+1:I(2)-1) };
   
      %disp( char( magic_numbers(index)))

      s = s(I(2)+2:end);
      I = find(s == '<');
      
      section_str = s(1:I-1);
      section_str = section_str( isdigit( section_str ) );

      item_str(index) = { section_str };
   
      index = index + 1;
   
      
   end

   s = fgetl(fp);

end

fclose(fp);





