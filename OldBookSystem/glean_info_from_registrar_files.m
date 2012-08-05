function [department, course_num, description, sections ] = glean_info_from_registrar_files( filename )


fp = fopen( filename );

if ~fp
   disp(['failed to open file ' filename])
end

txt = fgetl(fp);

while ~strcmp(txt(1:min(5,length(txt))),'<pre>')
   txt = fgetl(fp);
end


txt = fgetl(fp);
txt = fgetl(fp);


% [T,R] = STRTOK(S,D) 

I = min(find( txt == '>' ) );
txt = txt(I+1:end);

[department,R] = strtok(txt,' ');

[course_num,R] = strtok(R,' ');

description = strtok(R,'<');

txt = fgetl(fp);

section_index = 1;

while ~strcmp(txt,'</pre>')

   if length(txt) == 104 | length(txt) == 105
   
      % disp(['xx' txt 'xx'])
   
      %fields = return_fields( txt );
   
      %num_fields = length(fields);
   
      max_enrol_str = txt(90:94);
      spots_available_str = txt(95:99);
      waitlist_str = txt(100:104);
      prof_str = txt(63:83);

      % [A,COUNT,ERRMSG,NEXTINDEX] = SSCANF(S,FORMAT,SIZE)
   
      sections(section_index).section_number = txt(15:19);
      sections(section_index).max_enrol = sscanf( max_enrol_str,'%i');
      sections(section_index).spots_available = sscanf( spots_available_str,'%i');
      sections(section_index).waitlist = sscanf( waitlist_str, '%i');
      sections(section_index).prof = prof_str;

      section_index = section_index + 1;

   end

   txt = fgetl(fp);
end




fclose(fp);


