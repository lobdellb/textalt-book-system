% Script to create a useable database from the book database file in ./database/bncollege/{term}/

clear

source_dir = get_source_dir;


out_dir = [ '../cooked_database/registrar/' ];
data_dir = [ '../database/registrar/' ];


term = 'Fall09';


files = dir([ data_dir 'data' '/*.mat']);


% next find the newest file
chosen_file = find_most_recent_file( files );
file_name = chosen_file.name;

disp(sprintf('Cooking most recent file:  %s', file_name ))
load([ data_dir 'data' '/' file_name ]);


% I now have the following items to mess with:

%    course_num       1x2472             163144  cell                
%    department       1x2472             174294  cell                
%    description      1x2472             312456  cell                
%    sections         1x2472            3066456  struct              


k = 1;

for n = 1:length(course_num)
   for m = 1:length(sections(n).sections)

      crdb(k).section_str = sections(n).sections(m).section_number;
      crdb(k).section_num = sscanf( char( sections(n).sections(m).section_number ), '%i' );
      crdb(k).department = department(n);
      crdb(k).course_num = course_num(n);
      crdb(k).description = description(n);
      crdb(k).prof = { sections(n).sections(m).prof };
      crdb(k).max_enrol = sections(n).sections(m).max_enrol;
      crdb(k).spots_available = sections(n).sections(m).spots_available;
      crdb(k).waitlist = sections(n).sections(m).waitlist;

      k = k + 1;

      %if strcmp( char( course_num(n) ), '110' ) & strcmp( char( department(n) ), 'COMM-R' )
      %   keyboard
      %end

   end
end



out_filename = sprintf([ out_dir '%s/cooked_%s.mat'],'data',date);
backup_filename = sprintf([ out_dir '%s/current_registrar_cooked.mat'],'data');

disp(out_filename)
save(out_filename,'crdb')
save(backup_filename,'crdb')







