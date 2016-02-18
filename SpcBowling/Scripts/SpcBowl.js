$(function () {

    // this code gets rid of '_=_' sign that attached to https://spcbowl.azurewebsite.net/#_=_
    // when the user gets redirected usign Facebook Login API...
    // but it causes page refresh..which i don't want...so I disabled it for now.
    // also I need a method to remove '#' when user gets redirected from Google API+.
    //if (window.location.hash && window.location.hash == '#_=_') {
    //    window.location.href = window.location.href.split('#')[0];
    //}

    var ajaxFormSubmit = function () {
        var $form = $(this);
        var options = {
            url: $form.attr("action"),
            type: $form.attr("method"),
            data: $form.serialize()
        };
        $.ajax(options).done(function (data) {
            var $target = $($form.attr("data-spcbowl-target"));
            var $newHtml = $(data);
            $target.replaceWith($newHtml);
            $newHtml.effect("highlight");
        });
        return false;
    };

    var createAutocomplete = function () {
        var $input = $(this);
        var options = {
            source: $input.attr("data-spcbowl-autocomplete"),
            select: submitAutocompleteForm
        };
        $input.autocomplete(options);
    };

    var submitAutocompleteForm = function (event, ui) {
        var $input = $(this);
        $input.val(ui.item.label);
        var $form = $input.parents("form:first");
        $form.submit();
    };

    var getPage = function () {
        var $a = $(this);
        var options = {
            url: $a.attr("href"),
            data: $("form").serialize(),
            type: "get"
        };
        $.ajax(options).done(function (data) {
            var target = $a.parents("div.pagedList").attr("data-spcbowl-target");
            $(target).replaceWith(data);
        });
        return false;
    };

    $("form[data-spcbowl-ajax='true']").submit(ajaxFormSubmit);
    $("input[data-spcbowl-autocomplete]").each(createAutocomplete);

    $(".main-content").on("click", ".pagedList a", getPage);

    // test script to see if I can set the default Score under Player/Create View
    //$('input[name*=BowlingScore]').val(100);
    //e.preventDefault();
});
