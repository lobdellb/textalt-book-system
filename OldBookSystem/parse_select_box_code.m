function [ magic_numbers, item_str] = parse_select_box_code( filename )

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
while length(strfind(s,'>Select ')) == 0
   s = fgetl(fp);
end

s = fgetl(fp);

index = 1;

while ~feof(fp)

   %disp(s)

   %keyboard

%   if strcmp(s(1:min(length('<option value'),length(s))),'<option value')
   if length(strfind( s, '<option value' ) ) > 0

      I = find(s == '''');
   
      magic_numbers(index) = { s(I(1)+1:I(2)-1) };

      %s = s(I(2)+2:end);
      s = s(I(2):end);
      I1 = find( s == '>' );
      I2 = find(s == '<');
   
      item_str(index) = { s( (I1(1)+1):(I2(1)-1) ) };

      index = index + 1;
   end

   s = fgetl(fp);
end

fclose(fp);





