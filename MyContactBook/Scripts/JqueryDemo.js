
$(document).ready(function () {

    $("#div").click(function () { alert("you clicked !!"); });
    $("<div id='div2'>Dynamically created div via jquery DOM manupolation</div>").appendTo(document.body);
    $("#div2").wrapInner("<h2></h2>");
    $("#content > div > p ").css("background-color", "red");
    $("<div id='mydiv'>Hello from JQ</div>").appendTo(document.body);
    $("#mydiv").wrapInner("<p></p>");
    // $("a").click(function () { alert("clicked"); return false }); //selects all 'a' tag and returned false to prevent browser to open the anchors link
    // $("div a").click(function () { alert("clicked"); return false }); //only applicable to 'a' tag which are inside div element
    $("a[href='www.google.com']").click(function () { alert("clicked"); return false }); //selecting all 'a' tag which has attribute value of www.google.com

    // CSS pseudo class example.....note: css pseudo class always begins with ':'(colon)
    $("a:odd").css("background-color", "yellow");  // only applicable to even 'a' tag
    $("a:even").css("background-color", "lightgreen");  // only applicable to even 'a' tag
    $("a:contains('facebook')").css("background-color", "grey");  // only applicable to 'a' tag that contains text 'facebook'

    //Working with Attributes

    $("a:even").addClass("evenrows"); //adding the class attribute
    $("a:odd").addClass("oddrows");  //adding the class attribute
    $("a:contains('somewhere 4')").attr("href", "http://www.somewhere4.com"); //setting the 'href' attribute of 'a' tag that contains text 'somewhere4'
    //note: when we call attr() method without any parameter then we are reading the attribute but when called attr() method with parameter
    // then we are setting the attribute...


    $("a").mouseover(function () { $(this).addClass("highlight"); });
    $("a").mouseout(function () { $(this).removeClass("highlight"); });
    $("a:contains('somewhere 3')").attr("href", "http://www.somewhere3.com");

    //we can acheive same thing as above via chaining method...jquery uses the builder design pattern
    $("a").mouseover(function () { $(this).addClass("highlight"); })
        .mouseout(function () { $(this).removeClass("highlight") })
        .filter(":even").addClass("evenrow").end()
        .filter(":odd").addClass("oddrow").end()
        .filter(":contains('somewhere 2')").attr("href", "http://www.somewhere2.com");
    // where end() function revert last destructive filter means we can continue to work with our first selection instead
    // of working with the selection based on the filter e.g a:even or a:odd we destroy them and continue to work with previously selected all "a" tag.

    //Basic Effect
    $(".section").hide();
    $("#menu > p").click(function () {
        $(this).next().slideToggle("slow");
    });
});