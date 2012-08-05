// /////////////////////////////////////////////////////////////////////////////////
// JavaScript Code for the payment screen
//
///////////////////////////////////////////////////////////////////////////////////


function onclick_credit_radio_key()
{

	card_num = prompt("Card Number","0000111122223333");
	exp_date = prompt("Expire Date","MMYY");

	if ( card_num.length == 16 && exp_date.length == 4 )
	{

		document.getElementById("ccnumber").value = card_num;
		document.getElementById("expdate").value = exp_date;
		document.getElementById("swipetype").value = "manual";
		document.getElementById("last4").value = card_num.substring(12,16);
	}
	else
	{
		document.GetElementByType("swipetype").value = "invalidmanual";
	}

}




function onclick_credit_radiox()
{

	swipe_data=prompt("Swipe Card","");
	
	var track_data = new SwipeParserObj( swipe_data );
	
	document.getElementById('swipetype').value = 'swipe';
		prompt("Got here","");
	
	document.getElementById("track1").value = track_data.track1;
	document.getElementById("track2").value = track_data.track2;

    document.getElementById("swipetype").value = swipe_data;


			
}








function onclick_credit_radio()
{


	fault = false;

	// Was the swipe valid?
	
	// Do the last 4 digits match?



   swipe_data=prompt("Swipe Card","");

   var track_data = new SwipeParserObj( swipe_data );

   if ( ( track_data.account == null ) | ( track_data.exp_month == null ) | ( track_data.exp_year == null ) )
   {
		fault = true;
   }
   else
   {

      // validate the number
      if ( track_data.account.length == 16 )
      {
         fault = false;
         for ( n = 0; n <= 15; n++ )
            if ( parseInt( track_data.account.charAt(n) ).toString() != track_data.account.charAt(n) )
               fault = true;

	  }

	  
      last4_str = prompt("Enter Last 4 Digits","");

      //balh = prompt( last4_str, "ksd" );

      if ( ! ( last4_str == track_data.account.substring(12,16) ) )
         fault = true;
	  else
	     document.getElementById("last4").value = last4_str;

   }

   
   if ( !fault )
   {
      
	  document.getElementById("custname").value = track_data.account_name;	  
	  
      if  ( track_data.hasTrack1 )
      {
        // trl_temp = track_data.track1;
         // Fix the stray "?" on the back or ";" on the stripe
		 
         if ( track_data.track1.charAt(track_data.track1.length-1) == "?".charAt(0) )
         {
            document.getElementById("track1").value = track_data.track1.substring(0,track_data.track1.length-1);
            
         }         
		 else
			document.getElementById("track1").value = track_data.track1;

         //str += '<input type="hidden" name="x_track_1" value="' + trl_temp + '">';
		 
		 
      }

      if  ( track_data.hasTrack2 )
      {
         //trl_temp = track_data.track2;

       // Fix the stray "?" on the back or ";" on the stripe
       //  if ( trl_temp.charAt(trl_temp.length-1) == "?".charAt(0) )
       /*  {
            temp = trl_temp.substring(0,trl_temp.length-1);
            trl_temp = temp;
         } */     

		 document.getElementById("track2").value = track_data.track2;
		 
         //str += '<input type="hidden" name="x_track_2" value="' + trl_temp + '">';
      }

	  document.getElementById("swipetype").value = "swipe";
	  
   }
   else
   {
	  document.getElementById("swipetype").value = "swipeinvalid";
   }

}












