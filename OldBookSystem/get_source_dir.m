function source_dir = get_source_dir;

% function to load the source dir from a file on disk

fp = fopen('source_dir','r');
source_dir = fgetl(fp);
fclose(fp);
