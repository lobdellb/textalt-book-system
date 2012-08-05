function [department, course_num, description, sections ] = parse_registrar_files(html_dir)

% this script gets all information about sections and puts it in a structure

files = dir([html_dir '*.html']);

% Note:  These files seem to be updated ever two days.


for n = 1:length(files)

   disp(sprintf('working on %i of %i - %s',n,length(files),files(n).name))


   [dep, cn, desc, sec ] = glean_info_from_registrar_files([html_dir files(n).name] );

   department(n) = {dep};

   if length(cn) > 3
      cn = cn(1:3);
   end

   course_num(n) = {cn};
   description(n) = {desc};
   sections(n).sections = sec;

   %if strcmp(dep,'COMM-R') & strcmp(cn,'110')
   %   keyboard
   %end

   
end