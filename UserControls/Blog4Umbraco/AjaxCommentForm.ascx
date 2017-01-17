<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AjaxCommentForm.ascx.cs" Inherits="Umlaut.Umb.Blog.usercontrols.AjaxCommentForm" %>

<div id="commentform" class="post-comment">
	<div id="gravatar" style="display: none; height: 80px; width:80px;"></div>
	<div class="cols">
        <div class="col col1">
            <div>
                <input type="text" id="author" name="author" class="input-text required" required="required" placeholder="*Name" />
            </div>
            <div>
                <input type="text" id="email" name="email" class="input-text required email" required="required" placeholder="*Email" />
            </div>
            <div>
                <input type="text" id="url" name="website"  class="input-text url" placeholder="Website" />
            </div>
        </div>
        <div class="col col2">
            <div>
                <textarea id="comment" name="comment" class="required" required placeholder="*Comment"></textarea>
            </div>
        </div> 
    </div>
    <div class="cols">
        <div class="col col1 colspan">
            <input type="submit" id="submit" class="submit button right" value="Submit" />
        </div>
    </div>
</div>

<div id="commentLoading" style="display: none"><div class="msg-info">Your comment is being submitted, please wait</div></div>
<div id="commentPosted" style="display: none"><div class="msg-ok">Your comment has been posted, thank you very much</div></div>

<script type="text/javascript">
    jQuery(document).ready(function(){
          
          /**/
          jQuery("#commentform #email").blur(function(){
                var email = jQuery("#commentform #email").val();
                                
                if(email != ""){
                    var url = "/base/Blog4Umbraco/GetGravatarImage/" + email + "/80.aspx";
                    jQuery.get(url, function(data){
                        if(data != ""){
                             jQuery("#gravatar").css( "background-image","url(" + data + ")" ).show();
                        }else{
                            jQuery("#gravatar").hide();
                        }
                    });
                }
          });
            
		jQuery('#content form').validate({
			debug: true,
			errorClass: 'formError',
			errorPlacement: function(error, element) {
				error.insertBefore(element);
	  	    },
			invalidHandler: function(){ },
          	submitHandler: function(form) {
				jQuery("#commentform").hide();
			    jQuery("#commentLoading").show();
			    jQuery("#commentform #submit").attr("enabled", false);
			    
			    var url = "/base/Blog4Umbraco/CreateComment/<umbraco:Item field="pageID" runat="server"/>.aspx";
			    
				jQuery.post(url, 
					{ author: jQuery("#commentform #author").val(), email: jQuery("#commentform #email").val(), url: jQuery("#commentform #url").val(), comment: jQuery("#commentform #comment").val() },
                	function(data){
						jQuery("#commentLoading").hide();
						jQuery("#commentPosted").show().removeClass("error");
						if(data == 0){
							  jQuery("#commentPosted").addClass("error").html("Your comment could not be posted, we're very sorry for the inconvenience");
							  jQuery("#commentform").show();
							  jQuery("#commentform #submit").attr("enabled", true);
						}
                   });
			}
		});
    });
</script>