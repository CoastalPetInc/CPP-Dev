<%@ Control Language="VB" AutoEventWireup="false" CodeFile="History.ascx.vb" Inherits="usercontrols_Cart_History" %>
<script language="VBScript">
Sub myAlert(title, content)
MsgBox content, 0, title
End Sub
</script>
<script>
    $(function () {
        $('.previous-orders').each(function(index, element) {
            var container = $(this);
			var orders = $('.order', container);
            //$('.order-line').hide();
			$('.order-detail').hide();


			orders.each(function (index, element) {

			    $(this).find('td:first').prepend('<i class="icon icon-open-close" title="View This Order"></i><i class="icon icon-reorder" title="Start a New Order and Add These Items to Your Cart" label="Start a New Order and Add These Items to Your Cart" ></i>');
            });


			container.on('click', 'i.icon-reorder', function(e){
			    e.preventDefault();
			    var orderNumber = $(this).parents('tr').attr('data-order-number');
			    //console.log('Reorder:' + orderNumber);
			    window.location = window.location.pathname + '?reorder=' + orderNumber;
			});

			container.on('click', 'i.icon-open-close', function (e) {
				e.preventDefault();
				var that = $(this);
				var row = that.parents('tr');
			    var detail = row.nextUntil('.order');
				var isOpen = that.is('.open');
				//console.log(detail, isOpen);
				that.toggleClass('open', !isOpen);
			    detail.toggle(!isOpen);
			});
        });
   });
</script>
<style>
</style>