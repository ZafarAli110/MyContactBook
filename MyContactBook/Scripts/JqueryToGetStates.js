// here we will fetch the states for the selected country
$(function () {   //$(document).ready(function () { });

    $("#CountryId").change(function () {
        var countryID = parseInt($(this).val());

        if (!isNaN(countryID)) {

            var $ddStates = $("#StateId");  // grabbing the dropdownList of states 
            $ddStates.empty();   //clear all items from the dropdownList
            $ddStates.append($("<option></option>").val('').html("Please wait...")); //adding empty('') value and the header text in the dropdownList of states

            //Making jquery Ajax call
            $.ajax({

                url: '@Url.Action("GetStates", "Home")',
                type: 'GET',
                dataType: 'json',
                data: { countryID: countryID },

                success: function (d) {

                    $ddStates.empty(); //clear all items including the Please wait text
                    $ddStates.append($("<option></option>").val('').html("Select State")); //now again adding the header,this time with different html text

                    $.each(d, function (index, item) {
                        $ddStates.append($("<option></option>").val(item.StateId).html(item.StateName));
                    });
                },

                error: function () {
                    alert("An error has been occured while gathering the data.please try again");
                }
            });

        }


    });


});