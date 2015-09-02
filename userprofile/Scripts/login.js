$(".login").click(function (event) {
    event.preventDefault();
    document.forms["sp2"].submit();
    $('form').fadeOut(500);
    $('.wrapper').addClass('form-success');
    
});
