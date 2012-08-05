function books = get_productId_from_html( books, html )

% this function sifts through the html file from B&N and grabs the product ID
% which can be used to direct the browser to the page about a particular book

html = char(html);

idx = strfind( html , 'productId' );

%  if (length(idx)/2) ~= length(books)
%     disp('Warning:  length(books) and number of "productId" are not equal')
%  end

i = 1;

for n = 1:length(books)
   % the books will only have the "productid" if a book is listed

   if ~strcmp( books(n).no_book, 'none_listed' )
      % then there is a book and there will be a productId

      [T,R] = strtok( html(idx(i)+9:end), '&' );
      books(n).productid = T;
      i = i + 2;

   end

end