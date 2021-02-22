import $ from 'jquery';
    
function init() {
    //Any input which redirects a user
    $('.js-shortcut').change(function() {
        var url = $(this).val();
        if (url !== 'default') {
            window.location.href = url;
        }
    });

    //Toggle element visibility
    $('.js-toggle').click(function() {
        var toggleClass = $(this).data('target');
        $(this).toggleClass('toggled');
        $(target).toggle();
        return false;
    });

    //Toggle class
    $('.js-toggle-class').click(function() {
        var toggleClass = $(this).data('class');
        if ($(this).attr('data-target')) {
            var target = $(this).data('target');
            $(target).toggleClass(toggleClass);
            $(this).toggleClass('toggled');
        } else {
            $(this).toggleClass(toggleClass);
        }
        return false;
    });

    //Focus
    $('.js-focus').click(function() {
        var target = $(this).data('focus');
        var $target = $(target);
        $target.focus();
        $target.val($target.val());
    });

    //Prevent disabled links/buttons
    $('.btn, a, button, input[type=submit]').click(function(e) {
        if ($(this).hasClass('btn-disabled') || $(this).hasClass('disabled')) {
            e.preventDefault();
            return false;
        }
    });
    
    //Responsive video wrapper
    $('.iframe[src*=\'vimeo.com\'], .iframe[src*=\'youtube.com\']').not('.js-no-maintain-ratio').each(function () {
        var width = parseInt($(this).attr('width'));
        var height = parseInt($(this).attr('height'));
        var aspectRatio = height / width * 100;
        var wrapper = $('<div class=\'iframe-container iframe-container--maintain-ratio\'></div>').css('padding-bottom', aspectRatio + '%');
        $(this).wrap(wrapper);
    });

    //Tables - Responsive, activate!
    // apply the .table--responsive class to apply the appropriate styling responsively
    $('.js-add-responsive-table-labels').each(function() {
        var currentTable = $(this);
        if ($(this).find('th').length) {
            $(this).find('th').each(function(i) {
                var label = $(this).text();
                $(this).addClass('table-label');
                $(currentTable).find('tr').each(function() {
                    $(this).find('td').eq(i).attr('data-label', label);
                });
            });
        }
    });

};

module.exports = { init };