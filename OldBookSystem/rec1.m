function isbn = rec1( filename );

load char_symbols.mat

x = imread(filename);
x = squeeze( sum( x, 3 ) );
x = double( x < 500 );  % make it a binary image, erase noise

% which correspond to the characters
numbers = '0123456789';

%conv_image = zeros( 10, 22, 104 );

for n = 1:10  % 2d convolve image with each template
   %conv_image = conv2( x, squeeze( characters(n,:,:) ) );

   %conv_data(n,:) = conv_image(15,10:6:end);
   %figure;plot( squeeze( conv_image(n,15,:) ) ) %imagesc(squeeze(conv_image(n,:,:)))
   %plot(conv_data(n,:))
   %hold on

   for m = 1:13
      diff_data(n,m) = sum(sum( abs(squeeze(x(8:15,5+[1:5]+(m-1)*6)) - squeeze(characters(n,:,:))) ));
   end

end


for n = 1:13 

   [X,class] = min( diff_data(:,n) );
   %disp(sprintf('winner=%.2f, 2nd=%.2f, mean=%.2f',X,min( diff_data([1:class-1 class+1:end],n) ),mean(diff_data(:,n))))
   output(n) = class-1;

end

isbn = numbers( output + 1);