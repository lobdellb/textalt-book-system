function close_loc = parse2( in_html, start_loc, open_tag, close_tag )

% finds all "outside" instances of a particular tag and returns them individually
% out_html is an ar
%
% Finds the location of the close_tag found from the beginning of the file in_html

current_loc = start_loc;


tmp = findstr( in_html( current_loc:end )', open_tag ) + current_loc - 1;
next_open_tag = min( tmp );

tmp = findstr( in_html( current_loc:end )', close_tag ) + current_loc - 1;
next_close_tag = min( tmp );


% we're not going to trap this, assume html is valid
%if length(next_close_tag) == 0
%   % then the html is not valid
%end

while 1

   
   if length(next_open_tag) == 0
      % then we are at the end and we can find matching tag

      %disp( char( in_html( current_loc:end )'))

      %close_loc = findstr( in_html( current_loc:end)', close_tag) + current_loc - 1;

     % keyboard

      close_loc = next_close_tag - 1;

      break;
   
   else


      if next_close_tag < next_open_tag
         % then we need to return

         close_loc = next_close_tag - 1;  %findstr( in_html( current_loc:end)', close_tag) + current_loc - 1;
         %close_loc = close_loc(1);


         break;

      else   % then we need to recurse

%         disp( char( in_html( current_loc:end )') )

         current_loc = parse2( in_html, next_open_tag+length(open_tag), open_tag, close_tag ) + length(close_tag);
   
         tmp = findstr( in_html( current_loc:end )', open_tag ) + current_loc - 1;
         next_open_tag = min( tmp );
         
         tmp = findstr( in_html( current_loc:end )', close_tag ) + current_loc - 1;
         next_close_tag = min( tmp );

      end

   end

end










