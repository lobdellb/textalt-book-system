% Script to create a useable database from the book database file in ./database/bncollege/{term}/

clear

more off

addpath ./bin

%source_dir = get_source_dir;
source_dir = '..';

out_dir = [ source_dir '/cooked_database/bncollege/' ];
data_dir = [ source_dir '/database/bncollege/' ];

term = 'Fall09';


files = dir([ data_dir 'data' '/*.mat']);

% next find the newest file
chosen_file = find_most_recent_file( files );
file_name = chosen_file.name;



disp(sprintf('\nfile %s selected',file_name))


load([ data_dir 'data' '/' file_name ]);



% steps:
% 1) traverse through every title, author, etc
% 2) for each item see if the title already exists
%    -> if it exists, add the section to the existing record
%    -> if not, add the item to the list

% this will be our book database

% has fields
% .title = ' '
% .author = ' '
% .publisher = ' '
% .edition = ' '
% .new_price = ' '
% .used_price = ' '
% .required = ' '
%
% fields which are added by this program:
% .classes = {}
% .sections = {};



for n = 1:length(dept)  % once for each department

   dep = dept(n);

   for m = 1:length(dep.classes)

      disp(sprintf('working on %s - %s',char(dept(n).str),char(dept(n).classes(m).str)))

      class = dep.classes(m);


      % note: the next line maybe should be class.section_strs, not sure why they're not the same
      for o = 1:length(class.section_books)

         % note:  this should be removed once the preceding script is rerun
         %if length(dept(n).classes(m).section_books) == 0
         %   dept(n).classes(m).section_books = dept(n).classes(m).sections_books;
         %end


         for p = 1:length(dept(n).classes(m).section_books(o).books)
            
            if strcmp( class.section_books(o).books(p).no_book, 'book_listed')

               if exist('book_database','var')
                  I = find_matching_record( book_database, class.section_books(o).books(p) );

                  if length(I) > 0  % then book already exists and we add to that record
                     book_database = add_course_and_section_to_book_record( book_database, I, dep.str, class.str, class.section_strs(o) );
                  else  % then book is new and we add a new record
                     book_database = book_database_add_record( book_database, class.section_books(o).books(p), dep.str, class.str, class.section_strs(o) );
                  end

               else

                  book_database = book_database_add_record( [], class.section_books(o).books(p), dep.str, class.str, class.section_strs(o) );

               end

            end

         end

      end

   end

end



out_filename = sprintf([ out_dir '%s/cooked_%s.mat'],'data',date);
backup_filename = sprintf([ out_dir '%s/current_bncollege_cooked.mat'],'data');

%keyboard

disp(out_filename)
save(out_filename,'book_database')
save(backup_filename,'book_database')














