function data = parse_book_html_file_v6( html )


% v4 - this version grabs the isbn number 

% v2 - this version is made to deal with an addition <div id="errorblock" tag and message



% script to parse the html file and get the exact info I want

%clear

%html_dir = './spring08/classes_html_temp/';

%fn1 = 'dept_PSY_class_698_section_21705.html';
%fn2 = 'dept_ENG_class_W131_section_22370.html';
%fn3 = 'dept_ENG_class_W131_section_20224.html';

%fp = fopen([html_dir fn3]);
%fp = fopen(fn3);

%html = fread(fp);

%retval = fclose(fp);


%start_str = '<stuff>';
%end_str = '</stuff>';


%  start_str = '<div id="bookTbl">';
%  end_str = '</div>';
%  I = findstr(html',start_str);
%  closer = parse2(html, I(1)+length(start_str), start_str,end_str);
%  html2 = html((I+length(start_str)):closer);
%  %disp(char(html2'))




if length(findstr(html','<div id="errorBlock')) > 0

   I1 = findstr(html','<div id="errorBlock');
   I2 = findstr(html(I1:end)','</div>');

   [html2, closer ] = parse_grabber( html([1:I1 (I1+min(I2)+7):end]), '<div id="bookTbl' , '</div>');

else
   [html2, closer ] = parse_grabber( html, '<div id="bookTbl', '</div>' );
end

keyboard

%  start_str = '<table';
%  end_str = '</table>';
%  I = findstr( html2', start_str);I = min(I);
%  closer = parse2(html2, I+length(start_str), start_str,end_str);
%  html3 = html2((I+length(start_str)):closer);
%  %disp(char(html3'))

[html3, closer ] = parse_grabber( html2, '<table', '</table>');



%  
%  start_str = '<tr>';
%  end_str = '</tr>';
%  I = findstr( html3', start_str);I = min(I);
%  closer = parse2(html3, I+length(start_str), start_str,end_str);
%  html4 = html3((I+length(start_str)):closer);
%  %disp(char(html4'))

[html4, closer ] = parse_grabber( html3, '<tr>', '</tr>');



if length(findstr( html4', 'Currently no textbook has been assigned for this course.  You can still reserve now by clicking the')) > 0  
   % then there is no book assigned

   %disp('no book assigned')

   data.author = [];
   data.edition = [];
   data.new_price = [];
   data.no_book = 'none_listed';
   data.publisher = [];
   data.required = [];
   data.title = [];
   data.used_price = [];

else

   next_html = html3;

%   [book_block, closer ] = parse_grabber( next_html, '<tr>', '</tr>');
%   [title, author, edition, publisher, required, new_price, used_price ] = get_book_info(book_block);
%   next_html = next_html( (closer + length('</tr>')):end );

%   disp([ title '-' author '-' edition '-' publisher '-' required '-' new_price '-' used_price ])


   I = findstr( next_html', '<tr>');
   i = 1;

  

   while length(I) > 0
      [book_block, closer ] = parse_grabber( next_html, '<tr>', '</tr>');
      next_html = next_html( (closer + length('</tr>')):end );

      if length(findstr(book_block','BNCB_TextbookDetailView')) > 0 
         [title, author, edition, publisher, required, new_price, used_price, isbn ] = get_book_info_v4(book_block);

         

         data(i).title = title;
         data(i).author = author;
         data(i).edition = edition;
         data(i).publisher = publisher;
         data(i).required = required;
         data(i).new_price = new_price;
         data(i).used_price = used_price;
         data(i).isbn = isbn;
         data(i).no_book = 'book_listed';

         % added after the fact to get the productid
         idx = strfind( char(book_block) , 'productId' );
         [T,R] = strtok( char(book_block(idx(1)+10:end)), '&' );
         data(i).productid = T';

         i = i + 1;

         %disp([ title '-' author '-' edition '-' publisher '-' required '-' sprintf('%.2f',new_price/100) '-' sprintf('%.2f',used_price/100) ])
      end

%      disp('------------------------------------------------------')
%      disp(char(book_block'))

      I = findstr( next_html', '<tr>');

   end


end







