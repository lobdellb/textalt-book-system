% script to download info from the registrar

clear

more off

addpath ./bin

temp_dir1 = './reg_temp1/';
%temp_dir2 = './reg_temp2/';

% make sure the dir is cleared
delete([ temp_dir1 '*.html'])
%delete([ temp_dir2 '*.html'])

status_fn = 'registrar_good_exit_status.mat';
if exist( status_fn , 'file' )
   delete(status_fn)
end

%fp = fopen('registrar_available_terms.txt');

%i = 1;

%  while ~feof(fp)
%  
%     txt = fgetl(fp);
%     
%     I = min(find( txt == ','));
%     session_names(i) = {txt(1:I-1)};
%     session_dir(i) = {txt(I+1:end)};
%     
%     i = i + 1;
%  
%  end
%  
%  fclose(fp);

%  
%  for n = 1:length(session_names)
%  
%     disp(sprintf('%i.) %s',n,char(session_names(n))))
%  
%  end
%  
%  s = input('Select which session to download: ','s');
%  
%  i = sscanf(s,'%i');
%  
%  


% wget -o logfile.txt -N -nd -l 2 -np -r -P prefixdir http://registrar.iupui.edu/enrollment/4082/index.html



% no longer works because of -np
%exec = sprintf('wget -o logfile.txt -N -nd -l 2 -np -r -P %s %sindex.html',temp_dir,char(session_dir(i)));


% These names must be changed every semester
session_name = 'Fall09';
session_dir1 = 'http://registrar.iupui.edu/enrollment/4098/';
%session_dir1 = 'http://registrar.iupui.edu/enrollment/4095/ss1/';
%session_dir2 = 'http://registrar.iupui.edu/enrollment/4095/ss2/';


disp(sprintf('\nSession %s selected, URL is %s',session_name,session_dir1))



%exec = sprintf('wget -o logfile.txt -N -nd -l 2 -np -r -P %s %sindex.html',temp_dir2,session_dir2);

if 1
   delete([ temp_dir1 '*.html'])
   exec = sprintf('wget -o logfile.txt -N -nd -l 2 -np -r -P %s %sindex.html',temp_dir1,session_dir1);
   disp(exec)
   system(exec);

 %  delete([ temp_dir2 '*.html'])
 %  exec = sprintf('wget -o logfile.txt -N -nd -l 2 -np -r -P %s %sindex.html',temp_dir2,session_dir2);
  % disp(exec)
  % system(exec);

end


%  bad_files = {  'index.html',...
%                 'UNDERGRADUATEDENT-INDIANAPOLI.html',...
%                 'GRADUATEDENT-INDIANAPOLIS.html',...
%                 'ECE-COOP.html',...
%                 'SPEA-LINKEDCOURSES.html',...
%                 'ME-COOP.html'};

bad_files = load_txtfile('registrar_exclude_files.txt');




for n = 1:length(bad_files)
   if exist( [temp_dir1 char(bad_files(n))],'file')
      delete( [ temp_dir1 char(bad_files(n))])
   end
   %if exist( [temp_dir2 char(bad_files(n))],'file')
   %   delete( [ temp_dir2 char(bad_files(n))])
   %end
end


[department1, course_num1, description1, sections1 ] = parse_registrar_files(temp_dir1);
%[department2, course_num2, description2, sections2 ] = parse_registrar_files(temp_dir2);

department = [ department1 ];% department2 ];
course_num = [ course_num1 ]; %course_num2 ];
description = [ description1 ]; % description2 ];
sections = [ sections1 ]; %sections2 ];

% get the source_dir so we can save the data in the proper directory

%source_dir = get_source_dir;
source_dir = '..';

% Now we want to save all the data we just got.
out_filename = sprintf('%s/database/registrar/%s/registrar_info_%s_%s.mat',source_dir,'data',session_name,date);
backup_filename = sprintf('%s/database/registrar/%s/current_registrar.mat',source_dir,'data');
disp(out_filename)

%  save(out_filename,'department','course_num','description','sections','department1','department2','course_num1','course_num2','description1','description2','sections1','sections2')
%  save(backup_filename,'department','course_num','description','sections','department1','department2','course_num1','course_num2','description1','description2','sections1','sections2')

save(out_filename,'department','course_num','description','sections')
save(backup_filename,'department','course_num','description','sections')


% next, ZIP the data collected and save the condensed version to a file

%exec = sprintf('zip -9 -r ./db_backups/%s/registrar/registrardata_%s_%s.zip ./database/condensed_registrar_info.mat ./database/registrar_download/*',char(session_names(i)),char(session_names(i)),date);

%disp(exec)
%system(exec)


%!rm ./database/registrar_download/*

%delete([ temp_dir '*.html'])

% delete all temporary files
files = dir([temp_dir1 '*.html']);
for n = 1:length(files)
   delete([temp_dir1 files(n).name])
end

%  
%  files = dir([temp_dir2 '*.html']);
%  for n = 1:length(files)
%     delete([temp_dir2 files(n).name])
%  end

status = 'good';
save(status_fn,'status')


