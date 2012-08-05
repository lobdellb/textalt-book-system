function out = remove_space( in )

I = ~isspace(in);

out = in(I);