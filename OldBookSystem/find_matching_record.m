function I = find_matching_record( book_database, book_record )

% has fields
% .title = ' '
% .author = ' '
% .publisher = ' '
% .edition = ' '
% .new_price = ' '
% .used_price = ' '
% .required = ' '

I = [];

for n = 1:length(book_database)

   title_match =  strcmp( book_database(n).title, book_record.title );
   author_match = strcmp( book_database(n).author, book_record.author );
   publisher_match = strcmp( book_database(n).publisher, book_record.publisher );
   edition_match = strcmp( book_database(n).edition, book_record.edition );
   
   if (title_match + author_match + publisher_match + edition_match ) == 4
      I = n;
      break;
   end

end















