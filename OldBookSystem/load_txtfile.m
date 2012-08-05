function strs = load_txtfile( filename )

fp = fopen(filename,'r');
i = 1;
while ~feof(fp)
   txt = fgetl(fp);
   strs(i) = {txt};
   i = i + 1;

end

fclose(fp)