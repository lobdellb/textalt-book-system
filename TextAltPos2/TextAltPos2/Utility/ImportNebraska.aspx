<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ImportNebraska.aspx.cs" Inherits="TextAltPos.Utility.ImportNebraska" Title="Load Nebraska List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
  function checkForm(form)
  {
  
    valid = true;
    // regular expression to match required date format
    re = /^\d{1,2}\/\d{1,2}\/\d{4}$/;

    if( !form.textalt_startdate.value.match(re)) {
      // alert("Invalid date format: " + form.startdate.value);
      jQuery('#startdate_error').css('display','block');
      valid = false;
    } else {
      jQuery('#startdate_error').css('display','none');
    }
    
    if( !form.textalt_enddate.value.match(re)) {
      // alert("Invalid date format: " + form.enddate.value);
      jQuery('#enddate_error').css('display','block');
      valid = false;
    } else {
      jQuery('#enddate_error').css('display','none');
    }
    
    return valid;
  }

</script>

    <form id="textalt_form">
        Nebraska List Import<br />
        <br />
        <label for="textalt_startdate">Start Date: </label>
        <input type="text" class="textalt datepicker" id="textalt_startdate" name="textalt_startdate" size="10" />
        <div id="startdate_error" style="color:Red;display:none;">Date is not valid, should be dd/mm/yyyy.</div>
        <br />
        <br />
        <label for="textalt_startend">Start Date: </label>
        <input type="text" class="textalt datepicker" id="textalt_enddate" name="textalt_enddate" size="10" />
        <div id="enddate_error" style="color:Red;display:none;">Date is not valid, should be dd/mm/yyyy.</div>
        <br />
        <br />
        <label for="textalt_password">Password: </label>
        <input type="text" class="textalt" id="textalt_password" size="20" />
        <div id="password_error" style="color:Red;display:none;">Password should not be blank.</div>
        <br />
        <br /><button id="textalt_startimport">Start Import</button>
        <br /><button id="textalt_archivedelete">Archive Old Data</button>
        <br />
        <div id="textalt_status"></div>
        <br />
    </form>
        


    <script type="text/javascript">
    
        function resetTimeout() 
        {   
            jQuery('#textalt_status').load('/Utility/ImportNebraska.aspx?function=status');
            setTimeout("resetTimeout()",5000);
        }
 
        jQuery('document').ready( function () { 
 
                   
            jQuery('.datepicker').blur( function() { checkForm( document.getElementById('aspnetForm') ); });
            
            jQuery('#textalt_archivedelete').click( function() {
                
                jQuery.get('/Utility/ImportNebraska.aspx?function=archivedelete', function (data) {
                
                        alert(data);
                        
                        return false;                
                    });    
                
                /*
                if ( result == 'true') {
                    jQuery('#textalt_status').html('Old purchasing data has been archived.');
                } else {
                    jQuery('#textalt_status').html('An error occurred archiving old wholesale data.');
                }*/
                
                return false;
                
            });
            
            
            
            
            jQuery('#textalt_startimport').click( function() {
            
                // alert('stuff');
                
                if ( checkForm( document.getElementById('aspnetForm') ) ) {
                    
                    
                    var StartDate = jQuery('#textalt_startdate').val();
                    var EndDate = jQuery('#textalt_enddate').val();
                    var Password = jQuery('#textalt_password').val();
                    
     

                                    
                    
                    jQuery('#textalt_status').html('Starting Download');
                    
                    result = jQuery.get('/Utility/ImportNebraska.aspx?function=startdownload'
                                            + '&startdate=' + jQuery('#textalt_startdate').val() 
                                            + '&enddate=' + jQuery('#textalt_enddate').val() 
                                            + '&password=' + jQuery('#textalt_password').val() );
                }
                                    
                return false;
            });
       
       
            resetTimeout();
            
        });    
    
    </script>
    
</asp:Content>
