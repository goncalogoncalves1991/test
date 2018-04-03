
var socialButtons = {
    facebook: '<a id="user-facebook" target="_blank" class="userSocialButton btn btn-social-icon btn-facebook" style="margin-left:2px"><i class="fa fa-facebook"></i></a>',
    twitter: '<a id="user-twitter" target="_blank" class="userSocialButton btn btn-social-icon btn-twitter" style="margin-left:2px"><i class="fa fa-twitter"></i></a>',
    github: '<a id="user-github" target="_blank" class="userSocialButton btn btn-social-icon btn-github" style="margin-left:2px"><i class="fa fa-github"></i></a>',
    email: '<a id="user-email" target="_blank" class="userSocialButton btn btn-sm btn-primary" style="margin-left:2px"><i class="fa fa-envelope"></i></a>',
    linkedin: '<a id="user-linkedin" target="_blank" class="userSocialButton btn btn-social-icon btn-linkedin" style="margin-left:2px"><i class="fa fa-linkedin"></i></a>'
}


function completeModel(user) {

    $("#WaitingUser").addClass("hidden");
    $('#profileName').text(user.name + " " + user.lastName);
    $('#local').text(user.local)
    $("#userPhoto").attr("src", user.picture);
    $("#regDate").text(user.registerDate.split('T')[0])

    if (user.biography != null)
        $("#userBiography").text(user.biography)

    completeUserSocialNetwork(user)
    $("#userInfo").removeClass('hidden')
}



function getUser(userId) {
    
    Request('none', 'Get', {}, 'https://exampleapp.com/api/users/' + userId, (data, error) => {
        if (data) {
            completeModel(data);
        }
        else
            alert(JSON.stringify(error))
    })
}

function completeUserSocialNetwork(user) {
    let socialNode = $("#user-social")
  
    if (user.facebook != undefined) {
        socialNode.append(socialButtons.facebook);
        $("#user-facebook").attr('href', user.facebook)
    }
    if (user.gitHub != undefined) {
        socialNode.append(socialButtons.github);
        $("#user-github").attr('href', user.gitHub)
    }
    if (user.email != undefined) {
        socialNode.append(socialButtons.email);
        $("#user-email").attr('href', user.email)
    }
    if (user.twitter != undefined) {
        socialNode.append(socialButtons.twitter);
        $("#user-twitter").attr('href', user.twitter)
    }
    if (user.linkedin != undefined) {
        socialNode.append(socialButtons.linkedin);
        $("#user-linkedin").attr('href', user.linkedin)
    }
}

function closeUserModel() {
    $(".userSocialButton").remove();

    $('#profileName').text("");
    $('#local').text("")
    $("#userPhoto").attr("src", "");
    $("#regDate").text("")
    $("#userBiography").text('This user doesnt have any biography.')


    $("#WaitingUser").removeClass("hidden");
    $("#userInfo").addClass("hidden");
}