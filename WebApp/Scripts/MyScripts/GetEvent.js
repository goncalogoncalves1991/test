
$(document).ready(function () {
    $("[id=popHover]").popover({
        placement: 'bottom', //placement of the popover. also can use top, bottom, left or right
        title: '<div style="text-align:center; text-decoration:underline; font-size:20px;"> More</div>', //this is the top title bar of the popover. add some basic css
        html: 'true', //needed to show html of course
        content: function () {
            return $('#popover-content').html();
        }
    });
});


function completeModel(user) {
    $('#profileName').text(user.name + " " + user.lastName);
    $('#local').text(user.local)
}


function getUser(userId) {
    $.ajax({
        url: 'https://exampleapp.com/api/users/' + userId,
        type: 'Get',
        success: function (data) {
            completeModel(data);
        },
        error: function (error) {
            alert("Error" + JSON.stringify(error))
        }
    });

}