function [title, author, edition, publisher, required, new_price, used_price ] = get_book_info(html4);


   keyboard

   %disp( char( html4'))

   [html5, tbl1_closer] = parse_grabber( html4, '<table','</table>');
   
   %html5 = html4((closer+length('</td>')):end);

   %[ tape_block, closer ] = parse_grabber( html5, '<td>','</td>');

   [title_block, closer ] = parse_grabber( html4, '<td>','</td>');
   next_html = html4(closer+length('</td>'):end);

   [trash, closer] = parse_grabber( next_html, '<td>','</td>');
   next_html = next_html(closer+length('</td>'):end);
   
   [ author_block, closer] = parse_grabber( next_html, '<td>','</td>');
   next_html = next_html(closer+length('</td>'):end);

   [trash, closer] = parse_grabber( next_html, '<td>','</td>');
   next_html = next_html(closer+length('</td>'):end);

   [ edpub_block, closer] = parse_grabber( next_html, '<td>','</td>');
   next_html = next_html(closer+length('</td>'):end);

   title_start = min(find( title_block == '>'))+1;
   title_end = max(find( title_block == '<'))-1;

   title = char( title_block(title_start:title_end)');

   author_start = min(find( author_block == '>'))+1;
   author_end = max(find( author_block == '<' ))-1;

   author = char( author_block(author_start:author_end)');


   edition_start = findstr(edpub_block','Edition:') + length('Edition:');
   edition_end = min( find( edpub_block == '<'))-1;

   pub_start = findstr(edpub_block','Publisher:') + length('Publisher:');
   pub_end = max( find( edpub_block == '<'))-1;

   edition = char( edpub_block( edition_start:edition_end )');
   publisher = char( edpub_block( pub_start:pub_end )');


   html6 = html4((tbl1_closer+length('</table>')):end );

   [html7, closer] = parse_grabber( html6, '<table','</table>');

   %disp( char( html7'))

   % find out if it's required
   [html8, closer] = parse_grabber( html7, '<tr>','</tr>');
   [html9, closer] = parse_grabber( html8, '<td','</td>');
   [html10, closer] = parse_grabber( html9, '<span','</span>');

   I = findstr(html10','REQUIRED');
   if length(I) > 0
      required = 'yes';
   else
      required = 'no';
   end

   % now get prices

   I = findstr( html7','<span>');

   keyboard

   newprice_block = parse_grabber( html7((I(2)+length('<span>')):end), '<span>','</span>');
   usedprice_block = parse_grabber( html7((I(4)+length('<span>')):end), '<span>','</span>');

   price_start = find( newprice_block == '$') + 1;
   price_end = find( newprice_block == '.') + 2;

   new_price = sscanf( char( newprice_block( price_start:price_end )'), '%f');
   new_price = round(new_price*100);  % the price in cents

   price_start = find( usedprice_block == '$') + 1;
   price_end = find( usedprice_block == '.') + 2;

   used_price = sscanf( char( usedprice_block( price_start:price_end )'), '%f');
   used_price = round(used_price*100);  % the price in cents
