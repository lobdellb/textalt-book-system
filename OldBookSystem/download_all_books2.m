function dept = download_all_books2( dept, html_temp, term_magic_number )

% dept is the list of magic numbers used to execute the download, and put the files
% in temp_dir
% term_magic_number indicates Fall07, Spring08, etc.

% script to download all the book html files

% script to download all book pages from teh B&N webpage

%clear

% save magic_numbers_file.mat dept

%dir_prefix = './summer08/';

%temp_dir = [ dir_prefix 'bncollege_temp/'];



%html_temp = [ dir_prefix 'classes_html_temp/'];


%load([temp_dir 'magic_numbers_file.mat'])
% term_magic_number = '31453645';  % spring 08
%term_magic_number = '33122779';  % summer08

%exec_str = 'lynx -source -connect_timeout=2 ';
exec_str = 'wget --tries=1 --output-document=- --connect-timeout=2 --read-timeout=2 -q ';
server_str = '"http://iupui.bncollege.com';


% load files to be excluded

bad_sections = load_txtfile('bncollege_exclude_sections.txt');
bad_courses = load_txtfile('bncollege_exclude_courses.txt');
bad_depts = load_txtfile('bncollege_exclude_depts.txt');





tic

for n = 1:length(dept)

   % go through each department

   for m = 1:length(dept(n).classes)

      for p = 1:length(dept(n).classes(m).section_magic_numbers)

         % /webapp/wcs/stores/servlet/TBListView?storeId=36052&langId=-1&catalogId=10001&savedListAdded=true&clearAll=&viewName=TBWizardView&removeSectionId=&section_1=31495184&numberOfCourseAlready=1&viewTextbooks.x=43&viewTextbooks.y=15&sectionList=newSectionNumber

         magic_number = char(dept(n).classes(m).section_magic_numbers(p));

         mn = magic_number(1:end-1);

         section_str = char(dept(n).classes(m).section_strs(p));
         section_str = remove_space(section_str);
         course_str = char(dept(n).classes(m).str);
         course_str = remove_space( course_str );
         dept_str = char(dept(n).str);
         dept_str = remove_space( dept_str );

         % I made a special function to hand this problem because the list got too long.

         good = 1;
         for q = 1:length(bad_depts)
            if strcmp( dept_str, char(bad_depts(q)))
               good = 0;
               break;
            end
         end

         for q = 1:length(bad_courses)
            if strcmp( course_str, char(bad_courses(q)))
               good = 0;
               break;
            end
         end

         for q = 1:length(bad_sections)
            if strcmp( section_str, char(bad_sections(q)))
               good = 0;
               break;
            end
         end

         if magic_number(end) ~= 'Y' & good == 1

            html_filename = sprintf('dept_%s_class_%s_section_%s.html',dept_str,course_str,section_str);

            url_str = sprintf('/webapp/wcs/stores/servlet/TBListView?storeId=36052&langId=-1&catalogId=10001&savedListAdded=true&clearAll=&viewName=TBWizardView&removeSectionId=&section_1=%s&numberOfCourseAlready=1&viewTextbooks.x=43&viewTextbooks.y=15&sectionList=newSectionNumber" ', mn );
 
            output_str = [ '> ' html_temp html_filename ];

            %disp([ exec_str server_str url_str output_str]);
            %keyboard

            if exist([ html_temp html_filename ])
               delete([ html_temp html_filename ])
            end
            try_no = 1;
            %while ~exist([ html_temp html_filename ],'file')
            while ~validate_book_records( [ html_temp html_filename ] )
               disp(sprintf('try #%i:  %s, magic# is %s',try_no,html_filename,mn))
               S = system([ exec_str server_str url_str output_str]);
               try_no = try_no + 1;
               if try_no > 30
                  disp('too many failures')
                  quit(1);
               end
            end

            fp = fopen(  [ html_temp html_filename ] );
            html = fread(fp);
            tmp=fclose(fp);
      
            %  books = parse_book_html_file( html );
            % version 3 is just like 2, except that it gets the productid
            books = parse_book_html_file_v3( html );  % change to deal with new html format

            % this is too slow, need to get it from the book block
            % books = get_productId_from_html( books, html );

            if length(books) == 0
               disp('warning: length(books) = 0')
            end

            %keyboard

            %for o = 1:length(books)
            %   dept(n).classes(m).section_books(p).books(o) = books(o);
            %end
            dept(n).classes(m).section_books(p).books = books;

            %delete([ html_temp html_filename ])


         else  % do this if no book is assigned
         
            dept(n).classes(m).section_books(p).books.author = [];
            dept(n).classes(m).section_books(p).books.edition = [];
            dept(n).classes(m).section_books(p).books.new_price = [];
            dept(n).classes(m).section_books(p).books.no_book = 'none_req';
            dept(n).classes(m).section_books(p).books.publisher = [];
            dept(n).classes(m).section_books(p).books.required = [];
            dept(n).classes(m).section_books(p).books.title = [];
            dept(n).classes(m).section_books(p).books.used_price = [];

         end

      end

   end

end


disp(sprintf('that took %.1f seconds',toc))



