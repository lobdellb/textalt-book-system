% script to download the booklist


clear

more off

addpath ./bin

status_fn = 'bncollege_good_exit_status.mat';
if exist( status_fn , 'file' )
   delete(status_fn)
end


%  fp = fopen('book_available_terms.txt');
%  
%  i = 1;
%  
%  while ~feof(fp)
%  
%     txt = fgetl(fp);
%     
%     I = min(find( txt == ','));
%     session_names(i) = {txt(1:I-1)};
%     session_magic_numbers(i) = {txt(I+1:end)};
%     
%     i = i + 1;
%  
%  end
%  
%  fclose(fp);
%  
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


% these lines must be manually set every semester
session_name = 'Fall09';
session_magic_number = '37015231';

disp(sprintf('\nSession %s selected, magic number is %s',session_name,session_magic_number))


temp_dir = './temp/';


%dept = get_magic_numbers3( temp_dir , session_magic_number );
%keyboard
%save('saved_mn_db.mat','dept','temp_dir','session_magic_number')
load('saved_mn_db.mat')
%keyboard

dept = download_all_books4( dept, temp_dir, session_magic_number );





% get the source_dir so we can save the data in the proper directory
%source_dir = get_source_dir;
source_dir = '..';

out_filename = sprintf('../database/bncollege/data/book_info_%s_%s.mat',session_name,date);
backup_filename = sprintf('../database/bncollege/data/current_bncollege.mat');

disp(out_filename)
save(out_filename,'dept')
save(backup_filename,'dept')



% now grab all the info and store it in dept

%  
%  for n = 1:length(dept)
%  
%     for m = 1:length(dept(n).classes)
%  
%        for o = 1:length(dept(n).classes(m).section_strs)
%  
%  
%           section_str = char(dept(n).classes(m).section_strs(o));
%           section_str = remove_space(section_str);
%           course_str = char(dept(n).classes(m).str);
%           course_str = remove_space( course_str );
%           dept_str = char(dept(n).str);
%           dept_str = remove_space( dept_str );
%  
%  
%           mn = char(dept(n).classes(m).section_magic_numbers(o));
%  
%           if mn(end) == 'N'
%     
%              html_filename = sprintf('dept_%s_class_%s_section_%s.html',dept_str,course_str,section_str);
%     
%              if exist( [ './database/book_download/classes_html_temp/' html_filename ],'file')
%                 fp = fopen(  [ './database/book_download/classes_html_temp/' html_filename ] );
%                 html = fread(fp);
%                 fclose(fp);
%        
%                 books = parse_book_html_file( html );
%        
%                 for p = 1:length(books)
%                    dept(n).classes(m).section_books(o).books(p) = books(p);
%                 end
%              else
%                 disp(['warning:  ' html_filename ' doesnt exist'])
%              end
%  
%           else
%                 dept(n).classes(m).section_books(o).books.no_book = 'none_req';
%           end
%  
%        end
%  
%     end
%  
%  end




% Now we want to save all the data we just got.

%save('./database/book_download/condensed_book_info.mat','dept')
%save('./database/condensed_book_info.mat','dept')

% next, ZIP the data collected and save the condensed version to a file

%exec = sprintf('zip -9 -r ./db_backups/%s/bncollege/bookdata_%s_%s.zip ./database/book_download/*',char(session_names(i)),char(session_names(i)),date);

%system(exec)


%!rm ./database/book_download/bncollege_temp/*
%!rm -rf ./database/book_download/classes_html_temp
%!mkdir ./database/book_download/classes_html_temp

% erase all files in the temporary directory
%  file = dir([ temp_dir '*.html' ]);
%  for n = 1:length(file)
%    delete([ temp_dir file(n).name ])
%  end

status = 'good';
save(status_fn,'status')


