% script to find the issue with books coming back null

%clear

%files = dir('./temp/*.html');

for n = 597:length(files)

   if mod(n,100) == 0
      disp(n)
   end

   fp = fopen(['./temp/' files(n).name ]);
   html = fread(fp);
   fclose(fp);

   book = parse_book_html_file_v3( html );

   if length(book) == 0
      disp(files(n).name)
   end


end