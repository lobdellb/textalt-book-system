# -*- coding: utf-8 -*-
# python script 
# -*- coding: iso-8859-15 -*-


from BeautifulSoup import BeautifulSoup
import re

f = open('input.txt')
html = f.read()
f.close()

whole_file = BeautifulSoup(html)

booktbl = whole_file('div',id="bookTbl")

tbody = booktbl[0]('table',summary="textbook8")

records = tbody[0].findChildren('tr',recursive=False)


# at this point we have the book records all stacked together, we want to load the <tr>
# tags one at a time

f = open('output.txt','w')


for record_i in range(1,len(records),4):

   # we'll loop over these later
   record = records[record_i]

   info = record.findAll('tr')

   title_str = info[0].find('a').contents[0]

   

   # the product id also comes from info[0]
   productid_block = info[0].find('a').attrs[0][1]
   productid_block = productid_block[ productid_block.find('productId'):]
   productid_block = productid_block[ productid_block.find('=')+1:]
   productid_block = productid_block[ :productid_block.find('&') ]


   author_str = info[2].find('span').contents[0]


   edpubisbn_block = info[4]
   edition_block = str( edpubisbn_block.contents[1].contents[0] )
   edsearch = re.search('Edition:', edition_block )
   edition_str = edition_block[ edsearch.end():]
   pub_block = edpubisbn_block.contents[1].contents[2]
   pubsearch = re.search('Publisher:', pub_block )
   pub_str = pub_block[ pubsearch.end():]

   

   isbn_block = edpubisbn_block.contents[1].contents[5]

   # it is possible that there is no idbn number, if so len( isbn_block.contents ) = 1

   if len( isbn_block.contents ) == 3:
     isbn_attrs = isbn_block.contents[1].attrs
     isbn_url = isbn_attrs[1][1]
   else:
     isbn_url = 'none'

   print 'Got here'

   reqd_block = info[6].find('span')
   reqd_bool = re.search( 'REQUIRED',str(reqd_block))

   
   newpr_str = info[10].findAll('span')[1].contents[0].strip()
   usedpr_str = info[13].findAll('span')[2].contents[0].strip()

   f.write( title_str )
   f.write( '\n' )
   f.write( author_str )
   f.write( '\n' )
   f.write( edition_str )
   f.write( '\n' )
   f.write( pub_str )
   f.write( '\n' )
   f.write( isbn_url )
   f.write( '\n' )

   if reqd_bool:
      f.write( 'yes' )
   else:
      f.write( 'no' )


   f.write( '\n' )

   f.write( newpr_str )
   f.write( '\n' )
   f.write( usedpr_str )
   f.write( '\n' )

   f.write( productid_block )
   f.write( '\n' )


f.close()


#f = open('tbody.html','w')
#f.write( str(tbody) )
#f.close()




#records = BeautifulSoup(''.join(str(tbody.findAll('tr'))))

#records = records[ 2:4: ]

#for n in range(0,le