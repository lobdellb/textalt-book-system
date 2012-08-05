function book_database = book_database_add_record( book_database, book_record, dept_str, class_str, section_str )

% function to add a record to the book database structure because MATLAB is too much 
% of a fuck to do some very simple pointer arithmatic

%disp('adding record')

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

   new_i = length(book_database)+1;

   book_database( new_i ).title = book_record.title;
   book_database( new_i ).author = book_record.author;
   book_database( new_i ).publisher = book_record.publisher;
   book_database( new_i ).edition = book_record.edition;
   book_database( new_i ).new_price = book_record.new_price;
   book_database( new_i ).used_price = book_record.used_price;
   book_database( new_i ).required = book_record.required;
   book_database( new_i ).classes = { [char(dept_str) '-' remove_space(char(class_str))] };
   book_database( new_i ).sections = section_str;
   book_database( new_i ).productid = book_record.productid;
   book_database( new_i ).isbn1 = book_record.isbn;
