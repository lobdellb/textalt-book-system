% script to test parse_book_html_file_v3.m

clear

fp = fopen('source_test_2.html','r');
html = fread(fp);
fclose(fp);


data = parse_book_html_file_v3( html );

