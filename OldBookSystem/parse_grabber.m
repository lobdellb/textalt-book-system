function [html_out, closer ] = parse_grabber(html_in, open_tag, close_tag )

I = findstr( html_in', open_tag );I = min(I);

closer = parse2(html_in, I + length(open_tag), open_tag, close_tag );

html_out = html_in( (I+length(open_tag)):closer );
