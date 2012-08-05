function dept = get_magic_numbers2( temp_dir, term_magic_number )

% temp_dir is the directory where we put the stuff we download, term_magic_number is what it 
% sounds like, and dept is a structure which has all the info needed to complete the download


% this script will download the magic numbers used to query the bookstore webpage for book information

%clear


%dir_prefix = './summer08/';

%mkdir(dir_prefix)

%temp_dir = [ dir_prefix 'bncollege_temp/'];

%mkdir(temp_dir)

% this item should probably be passed by command line

%term_magic_number = '31453645';  this one goes with spring08
%term_magic_number = '33122779';  % summer08


% get list of departments

% This code gets the data about magic session numbers, etc.
%lynx -source "http://iupui.bncollege.com/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=31453645&deptId=31498509&courseId=&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809 HTTP/1.1" > pg3.html

%exec_str = 'lynx -source -connect_timeout=2 ';
exec_str = 'wget --tries=1 --output-document=- --connect-timeout=2 --read-timeout=2 -q ';

server_str = '"http://iupui.bncollege.com';
url_str = sprintf('/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=%s&deptId=&courseId=&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809" ',term_magic_number);
output_str = [ '> ' temp_dir 'dep_list.html' ];

try_no = 1;

if exist([temp_dir 'dep_list.html' ],'file')
   delete([temp_dir 'dep_list.html' ])
end

while ~validate_magic_number_file( [temp_dir 'dep_list.html' ])
   disp(sprintf('try #%i for dept list',try_no))
   S = system([ exec_str server_str url_str output_str]);
   try_no = try_no + 1;
   if try_no > 30  % looks like the internet went down or something
      quit(1);
   end
end


%disp([temp_dir output_str]);
[dept_magic_number, dept_strs] = parse_select_box_code([ temp_dir 'dep_list.html']);



tic

for n = 14  %1:length(dept_strs)

   dept(n).str = dept_strs(n);
   dept(n).magic_number = dept_magic_number(n);

   % now the list of classes
   
   url_str = sprintf('/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=%s&deptId=%s&courseId=&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809" ',term_magic_number, char(dept_magic_number(n)));

   output_str = [ '> ' temp_dir 'class_list.html' ];

   if exist([ temp_dir 'class_list.html'],'file')
      delete([ temp_dir 'class_list.html'])
   end
   try_no = 1;
   while ~validate_magic_number_file([ temp_dir 'class_list.html'])
      disp(sprintf('try #%i for class list for dept #%i',try_no,n))
      S = system([ exec_str server_str url_str output_str]);
      try_no = try_no + 1;
      if try_no > 30
         disp('too many failures')
         quit(1);
      end
   end


   [course_magic_number, course_strs] = parse_select_box_code([ temp_dir 'class_list.html']);



   for m = 1  %:length(course_strs)

      dept(n).classes(m).str = course_strs(m);
      dept(n).classes(m).magic_number = course_magic_number(m);


      % now the list of sections

      url_str = sprintf('/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=%s&deptId=%s&courseId=%s&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809" ',term_magic_number, char(dept_magic_number(n)), char(course_magic_number(m)) );

      output_str = [ '> ' temp_dir 'section_list.html' ];

      if 0 %exist([temp_dir 'section_list.html'],'file')
         delete([temp_dir 'section_list.html'])
      end
      try_no = 1;
      while ~validate_section_file( [temp_dir 'section_list.html'])
         disp(sprintf('try #%i for sections for %s-%s',try_no,char( dept(n).str ), char(dept(n).classes(m).str)))
         %disp(sprintf('try #%i for dept list',try_no))
         S = system([ exec_str server_str url_str output_str]);
         try_no = try_no + 1;


         if try_no > 30
            disp('too many failures')
         quit(1);
      end

      end

      disp('entered')
      [section_magic_number, section_strs] = parse_section_select_box_code([temp_dir 'section_list.html']);

      disp('exited')
      %keyboard

      

      dept(n).classes(m).section_strs = section_strs;
      dept(n).classes(m).section_magic_numbers = section_magic_number;

   end


end

disp(sprintf('took %.1f seconds',toc))


%delete([temp_dir 'section_list.html'])
%delete([temp_dir 'class_list.html'])
%delete([temp_dir 'dep_list.html'])


%save([temp_dir 'magic_numbers_file.mat'],'dept')



