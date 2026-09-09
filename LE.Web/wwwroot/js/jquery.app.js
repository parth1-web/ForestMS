(function ($) {

    'use strict';

    function initSlimscroll() {
        $('.slimscroll').slimscroll({
            height: 'auto',
            position: 'right',
            size: "5px",
            color: '#9ea5ab'
        });
    }

    function initNavbar () {
        $('.navbar-toggle').on('click', function(event) {
          $(this).toggleClass('open');
          $('#navigation').slideToggle(400);
        });
    
        $('.navigation-menu>li').slice(-1).addClass('last-elements');
    
        $('.navigation-menu li.has-submenu a[href="#"]').on('click', function(e) {
          if ($(window).width() < 992) {
            e.preventDefault();
            $(this).parent('li').toggleClass('open').find('.submenu:first').toggleClass('open');
          }
        });
      }

      function initMenuItem () {
        
        $("#navigation a").each(function() {
            var pageUrl = window.location.href.split(/[?#]/)[0];
            if (this.href == pageUrl) {
                $(this).parent().addClass("active"); // add active to li of the current link
                $(this).parent().parent().parent().addClass("active"); // add active class to an anchor
                $(this).parent().parent().parent().parent().parent().addClass("active"); // add active class to an anchor
            }
        });
      }

    function init() {
        initSlimscroll();
        initNavbar();
        initMenuItem ();
    }

    init();

})(jQuery)

