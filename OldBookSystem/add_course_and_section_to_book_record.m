function book_database = add_course_and_section_to_book_record( book_database, I, dept_str, class_str, section_str )


%disp('found double')

new_sec_i = length( book_database(I).sections ) + 1;
book_database( I ).sections( new_sec_i ) = section_str;

% only add class if it isn't already included

new_class_str = [char(dept_str) '-' remove_space(char(class_str))];

class_found = 'no';
for k = 1:length( book_database(I).classes )

   %keyboard
   if strcmp( char( book_database(I).classes(k) ), new_class_str )
      class_found = 'yes';
      break;
   end
end

if strcmp('no',class_found)
   new_class_i = length(book_database(I).classes) + 1;
   book_database(I).classes(new_class_i) = {new_class_str};
end
