function dept = download_all_books3( deptx, html_temp, term_magic_number )

% dept is the list of magic numbers used to execute the download, and put the files
% in temp_dir
% term_magic_number indicates Fall07, Spring08, etc.

% script to download all the book html files

% script to download all book pages from teh B&N webpage

%clear

% save magic_numbers_file.mat dept

%dir_prefix = './summer08/';

%temp_dir = [ dir_prefix 'bncollege_temp/'];

bad_count = 1;


%html_temp = [ dir_prefix 'classes_html_temp/'];


%load([temp_dir 'magic_numbers_file.mat'])
% term_magic_number = '31453645';  % spring 08
%term_magic_number = '33122779';  % summer08

%exec_str = 'lynx -source -connect_timeout=2 ';
exec_str = 'wget --tries=1 --output-document=- --connect-timeout=2 --read-timeout=2 -q ';
server_str = 'http://iupui.bncollege.com/webapp/wcs/stores/servlet/TBListView';


% load files to be excluded

bad_sections = load_txtfile('bncollege_exclude_sections.txt');
bad_courses = load_txtfile('bncollege_exclude_courses.txt');
bad_depts = load_txtfile('bncollege_exclude_depts.txt');


dept = [];
tic

for n = 1:length(deptx)

   % go through each department, except pool depts which are equiv

    % need to check and see if this is a duplicate department 
   dept_strx = char( deptx(n).str );
   I = strfind( dept_strx, '-');

   if length(I)> 0
      course_prefix = [ dept_strx(I+1:end) ' '];
      dept_strx = dept_strx(1:I-1);
   else
      course_prefix = [];
   end

   dept_index = [];

   % find it in the existing list
   found = 0;
   for idx = 1:length(dept)
      if strcmp( char(dept(idx).str), dept_strx )
         %disp( [ char(dept(idx).str) '---------' dept_str ])
         dept_index = idx;
         found = 1;
         break;
      end
   end

   if ~found
      dept_index = length(dept) + 1;
      %dept(dept_index) = [];
   end

   %keyboard

   dept(dept_index).str = {dept_strx};

   if length(dept) > 0
      %keyboard
      if isfield( dept(dept_index), 'classes' )
         class_index = length( dept(dept_index).classes );
      else
         class_index = 0;
      end
   else
      class_index = 0;
   end

   
   

   % outputs:  dept_index, dept_str, course_prefix

   for m = 1:length(deptx(n).classes)

      % need to find the proper value for the indexes into dept, as hey are different from the indexes n,m,p into deptx
      % n -> dept_index
      % p -> p
      % m -> m + class_index

      class_strx = char(deptx(n).classes(m).str);
      if length(class_strx) > 3
         class_strx = class_strx(1:3);
      end

      dept(dept_index).classes(m + class_index).str = {[ course_prefix class_strx ]};
      dept(dept_index).classes(m + class_index).section_magic_numbers = deptx(n).classes(m).section_magic_numbers;
      dept(dept_index).classes(m + class_index).section_strs = deptx(n).classes(m).section_strs;

      %disp( dept(dept_index).str )

      for p = 1:length(deptx(n).classes(m).section_magic_numbers)

         % /webapp/wcs/stores/servlet/TBListView?storeId=36052&langId=-1&catalogId=10001&savedListAdded=true&clearAll=&viewName=TBWizardView&removeSectionId=&section_1=31495184&numberOfCourseAlready=1&viewTextbooks.x=43&viewTextbooks.y=15&sectionList=newSectionNumber

         magic_number = char(deptx(n).classes(m).section_magic_numbers(p));

%         mn = magic_number(1:end-1);
         mn = magic_number( isdigit( magic_number ) );  % remove Y or N if it exists


         section_str = char(deptx(n).classes(m).section_strs(p));
         section_str = remove_space(section_str);
         course_str = class_strx;  %char(deptx(n).classes(m).str);
         course_str = remove_space( course_str );
         dept_str = dept_strx;  %char(deptx(n).str);
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

            %html_filename = sprintf('dept_%s_class_%s_section_%s.html',dept_str,course_str,section_str);

            html_filename = 'temp.html';
            delete([ html_temp html_filename ]);

            %url_str = sprintf('/webapp/wcs/stores/servlet/TBListView?storeId=36052&langId=-1&catalogId=10001&savedListAdded=true&clearAll=&viewName=TBWizardView&removeSectionId=&section_1=%s&numberOfCourseAlready=1&viewTextbooks.x=43&viewTextbooks.y=15&sectionList=newSectionNumber" ', mn );
 
            %post_data = sprintf('storeId=36052&langId=-1&catalogId=10001&savedListAdded=true&clearAll=&viewName=TBWizardView&removeSectionId=&section_1=%s&sectionList=%s&viewTextbooks.x=&viewTextbooks.y=&sectionList=newSectionNumber',mn,mn);
            
            post_data = sprintf('storeId=36052&langId=-1&catalogId=10001&savedListAdded=false&clearAll=&viewName=TBWizardView&removeSectionId=&mcEnabled=N&section_1=%s&numberOfCourseAlready=1&viewTextbooks.x=43&viewTextbooks.y=11&sectionList=newSectionNumber',mn);

%section_1=%s&sectionList=%s&viewTextbooks.x=&viewTextbooks.y=&sectionList=newSectionNumber',mn,mn);
            

            %disp(sprintf('\n%s\n',post_data))

% storeId=36052&langId=-1&catalogId=10001&savedListAdded=false&clearAll=&viewName=TBWizardView&removeSectionId=&mcEnabled=N&section_1=38127737&numberOfCourseAlready=1&viewTextbooks.x=43&viewTextbooks.y=11&sectionList=newSectionNumber


            %disp( [ '-->' mn '<--' ])
            run_str = sprintf('wget --tries=1 --output-document=- --connect-timeout=2 --read-timeout=2 -q --post-data="%s" http://iupui.bncollege.com/webapp/wcs/stores/servlet/TBListView',post_data);

            output_str = [ ' > ' html_temp html_filename ];

            %disp([ exec_str server_str url_str output_str]);
            %keyboard

            if exist([ html_temp html_filename ])
               delete([ html_temp html_filename ])
            end
            try_no = 1;
            %while ~exist([ html_temp html_filename ],'file')
            while 1   %~validate_book_records( [ html_temp html_filename ] )
               %disp(sprintf('try #%i:  %s, magic# is %s',try_no,html_filename,mn))
               disp(sprintf('try #%i:  %s %s %s, magic# is %s',try_no,dept_str,course_str,section_str,mn))
               %disp(S)
               S = system([ run_str output_str]);

               keyboard

               if validate_book_records( [ html_temp html_filename ] )

                  fp = fopen(  [ html_temp html_filename ] );
                  html = char(fread(fp));
                  tmp=fclose(fp);

                  % this will be replaced by the perl verision, thank goodness
                  %   books = parse_book_html_file_v6( html );  % change to deal with new html format

                  if length( findstr( html, 'Currently no textbook has been assigned for this course.' ) ) > 0



                  end
                  
                  if length(books) > 0
                     break;
                  else
                     books = [];
                  end

               end

               try_no = try_no + 1;
               if try_no > 30
                  disp('too many failures')
                  quit(1);
               end

               pause(3)

            end

            %pause


      

            %keyboard
            %  books = parse_book_html_file( html );
            % version 3 is just like 2, except that it gets the productid

%              if length(books) == 0 % ie something bad happened
%                 fp = fopen( [ html_temp sprintf('bad_file_%i.html',bad_count) ],'w');
%                 fwrite(fp,html);
%                 fclose(fp);
%                 bad_count = bad_count + 1;
%  
%              end



            %

            % this is too slow, need to get it from the book block
            % DO NOT UNCOMMENT THE FOLLOWING LINE, IT'S WRAPPED INTO parse_book_html_file_v3
            %books = get_productId_from_html( books, html );

            if 0 %length(books) == 0
               disp('warning: length(books) = 0')
            end

            %keyboard

            %for o = 1:length(books)
            %   dept(n).classes(m).section_books(p).books(o) = books(o);
            %end
            

            dept(dept_index).classes(m + class_index).section_books(p).books = books;

            %delete([ html_temp html_filename ])


         else  % do this if no book is assigned
         
            dept(dept_index).classes(m + class_index).section_books(p).books.author = [];
            dept(dept_index).classes(m + class_index).section_books(p).books.edition = [];
            dept(dept_index).classes(m + class_index).section_books(p).books.new_price = [];
            dept(dept_index).classes(m + class_index).section_books(p).books.no_book = 'none_req';
            dept(dept_index).classes(m + class_index).section_books(p).books.publisher = [];
            dept(dept_index).classes(m + class_index).section_books(p).books.required = [];
            dept(dept_index).classes(m + class_index).section_books(p).books.title = [];
            dept(dept_index).classes(m + class_index).section_books(p).books.used_price = [];
            dept(dept_index).classes(m + class_index).section_books(p).isbn = [];

         end

      end

   end

end


disp(sprintf('that took %.1f seconds',toc))



